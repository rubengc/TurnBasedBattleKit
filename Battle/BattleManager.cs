using UnityEngine;
using System.Collections;

public class BattleManager : MonoBehaviour {

	private Global.BattlePhase battlePhase = Global.BattlePhase.BattleStart;

	public BattleGUIManager battleGUIManager;

	public Player teamAPlayer;

	public Monster[] teamA;
	private int currentMonster = 0;
	private int[] teamAChoices;
	private int[] teamASkillChoices;
	private int[] teamAItemChoices;
	private int[] teamAAllyTargets;
	private int[] teamAEnemyTargets;

	public Monster[] teamB;
	private int currentEnemy = 0;
	private int[] teamBChoices;
	private int[] teamBSkillChoices;
	private int[] teamBItemChoices;
	private int[] teamBAllyTargets;
	private int[] teamBEnemyTargets;

	private bool waitingToNextExecution = false;

	private Monster[] monsters;
	private string[] teams;
	private int[] choices;
	private int[] skillChoices;
	private int[] itemChoices;
	private int[] allyTargets;
	private int[] enemyTargets;

	void Start () {
		teamAChoices = new int[teamA.Length];
		teamASkillChoices = new int[teamA.Length];
		teamAItemChoices = new int[teamA.Length];
		teamAAllyTargets = new int[teamA.Length];
		teamAEnemyTargets = new int[teamA.Length];

		teamBChoices = new int[teamB.Length];
		teamBSkillChoices = new int[teamB.Length];
		teamBItemChoices = new int[teamB.Length];
		teamBAllyTargets = new int[teamB.Length];
		teamBEnemyTargets = new int[teamB.Length];

		InitChoices();

		print("Battle Start!");
		//battleGUIManager.DisplayMessage("Battle start!");

		battlePhase = Global.BattlePhase.TeamASelection;
	}

	void Update () {
		// Battle Ends
		if(battlePhase == Global.BattlePhase.BattleEnds) {
			print("Battle Ends!");
			return;
		}

		// Team A Selection
		if(battlePhase == Global.BattlePhase.TeamASelection) {
			print("Select your action!");

			if(teamAChoices[currentMonster] != 0 || teamA[currentMonster].state == Global.State.Dead) {
				if(teamA.Length > (currentMonster+1)) {
					currentMonster++;
				} else {
					battlePhase = Global.BattlePhase.TeamBSelection;

					currentMonster = 0;
					currentEnemy = 0;
				}
			}
		}

		// Team B Selection
		if(battlePhase == Global.BattlePhase.TeamBSelection) {
			print("The enemy is selecting his action!");

			AIEnemySelection();

			currentMonster = 0;
			currentEnemy = 0;
			battlePhase = Global.BattlePhase.ExecuteActions;

			OrderMonstersBySpeed();
		}



		// Execute Actions
		if(battlePhase == Global.BattlePhase.ExecuteActions) {
			print("Executing actions!");

			if(!waitingToNextExecution) {
				if(monsters[currentMonster].state != Global.State.Dead) {
					if(choices[currentMonster] == 1) { 
                        // Attack
						Monster currentMonsterTarget = ((teams[currentMonster] != "A")?teamA[enemyTargets[currentMonster]]:teamB[enemyTargets[currentMonster]]);

						if(currentMonsterTarget.state == Global.State.Dead)
							currentMonsterTarget = GetNextMonsterOfTeam( ((teams[currentMonster] != "A")?"A":"B") );

						if(battlePhase != Global.BattlePhase.BattleEnds)
							monsters[currentMonster].Attack(
							currentMonsterTarget
						);

						waitingToNextExecution = true;

						battleGUIManager.DisplayMessage(monsters[currentMonster].name + " attacks!");
					} else if(choices[currentMonster] == 2) { 
                        // Use skill
						if(monsters[currentMonster].skills[skillChoices[currentMonster]].numberOfTargets == Global.NumberOfTargets.OneEnemy) {
							Monster currentMonsterTarget = ((teams[currentMonster] != "A")?teamA[enemyTargets[currentMonster]]:teamB[enemyTargets[currentMonster]]);
							
							if(currentMonsterTarget.state == Global.State.Dead)
								currentMonsterTarget = GetNextMonsterOfTeam( ((teams[currentMonster] != "A")?"A":"B") );

							monsters[currentMonster].UseSkill(skillChoices[currentMonster], currentMonsterTarget);
						} else {
							monsters[currentMonster].UseSkill(skillChoices[currentMonster], teamA[allyTargets[currentMonster]]);
						}

						waitingToNextExecution = true;

						battleGUIManager.DisplayMessage(monsters[currentMonster].name + " use " + monsters[currentMonster].skills[skillChoices[currentMonster]].name + "!");
					} else if(choices[currentMonster] == 4) { 
                        // Use item
						//if(teams[currentMonster] == "A") {
							Item item = teamAPlayer.items[itemChoices[currentMonster]];
						//} else {
							// TODO: add this action to the team B
						//}
						
						if(item.numberOfTargets == Global.NumberOfTargets.OneAlly) {
							monsters[currentMonster].UseItem(teams[currentMonster], itemChoices[currentMonster], teamA[allyTargets[currentMonster]]);
						} else if(item.numberOfTargets == Global.NumberOfTargets.Himself) {
							monsters[currentMonster].UseItem(teams[currentMonster], itemChoices[currentMonster], monsters[currentMonster]);
						} else if(item.numberOfTargets == Global.NumberOfTargets.AllAlly) {
							monsters[currentMonster].UseItem(teams[currentMonster], itemChoices[currentMonster]);
						}

						waitingToNextExecution = true;

						battleGUIManager.DisplayMessage(monsters[currentMonster].name + " use a " + item.name + "!");
					}
				}
			}

			if((!monsters[currentMonster].IsAttacking() && !monsters[currentMonster].IsUsingSkill() && !monsters[currentMonster].IsUsingItem()) && !monsters[currentMonster].IsMovingToInitialPosition()) {
				waitingToNextExecution = false;
				
				if(haveSurvivors("A") && haveSurvivors ("B")) {
					if(monsters.Length > (currentMonster+1)) {
						currentMonster++;
					} else {
						InitChoices();
						battlePhase = Global.BattlePhase.TeamASelection;
					}
				} else {
					battleGUIManager.DisplayMessage(((haveSurvivors("A"))?"Victory":"Defeated"), -1.0f, true);
					battlePhase = Global.BattlePhase.BattleEnds;					
				}
			}
		}
	}

	void InitChoices() {
		currentMonster = 0;

		for(int i=0; i < teamAChoices.Length; i++) {
			teamAChoices[i] = 0;
			teamASkillChoices[i] = 0;
		}

		for(int i=0; i < teamBChoices.Length; i++) {
			teamBChoices[i] = 0;
			teamBSkillChoices[i] = 0;
		}
	}

	void AIEnemySelection() {
		for(int i=0; i < teamB.Length; i++) {
			teamBChoices[i] = Random.Range(1, 3);

			if(teamBChoices[i] == 2) {
				if(teamB[i].skills.Length > 0) {
					teamBSkillChoices[i] = Random.Range(0, teamB[i].skills.Length);
				} else {
					teamBChoices[i] = 1;
				}
			}

			teamBEnemyTargets[i] = Random.Range(0, teamA.Length);

			if(teamA[teamBEnemyTargets[i]].state == Global.State.Dead) {
				for(int j=0; j < teamA.Length; j++) {
					if(teamA[j].state != Global.State.Dead) {
						teamBEnemyTargets[i] = j;
						break;
					}
				}
			}
		}
	}

	void OrderMonstersBySpeed() {
		monsters = new Monster[(teamA.Length+teamB.Length)];
		teams = new string[(teamA.Length+teamB.Length)];
		choices = new int[(teamA.Length+teamB.Length)];
		skillChoices = new int[(teamA.Length+teamB.Length)];
		itemChoices = new int[(teamA.Length+teamB.Length)];
		allyTargets = new int[(teamA.Length+teamB.Length)];
		enemyTargets = new int[(teamA.Length+teamB.Length)];

		int[] usedPositions = new int[(teamA.Length+teamB.Length)];

		for(int i=0; i < monsters.Length; i++) {
			Monster currentFasterMonster = null;
			string currentFasterMonsterTeam = null;
			int currentFasterMonsterPosition = 0;

			for(int j=0; j < teamA.Length; j++) {
				if(!InArray(usedPositions, teams, j, "A")) {
					if(currentFasterMonster != null) {
						if(teamA[j].speed > currentFasterMonster.speed) {
							currentFasterMonster = teamA[j];
							currentFasterMonsterTeam = "A";
							currentFasterMonsterPosition = j;
						}
					} else {
						currentFasterMonster = teamA[j];
						currentFasterMonsterTeam = "A";
						currentFasterMonsterPosition = j;
					}
				}
			}

			for(int j=0; j < teamB.Length; j++) {
				if(!InArray(usedPositions, teams, j, "B")) {
					if(currentFasterMonster != null) {
						if(teamB[j].speed > currentFasterMonster.speed) {
							currentFasterMonster = teamB[j];
							currentFasterMonsterTeam = "B";
							currentFasterMonsterPosition = j;
						}
					} else {
						currentFasterMonster = teamB[j];
						currentFasterMonsterTeam = "B";
						currentFasterMonsterPosition = j;
					}
				}
			}

			monsters[i] = currentFasterMonster;
			teams[i] = currentFasterMonsterTeam;
			choices[i] = ((currentFasterMonsterTeam == "A")?teamAChoices[currentFasterMonsterPosition]:teamBChoices[currentFasterMonsterPosition]);
			skillChoices[i] = ((currentFasterMonsterTeam == "A")?teamASkillChoices[currentFasterMonsterPosition]:teamBSkillChoices[currentFasterMonsterPosition]);
			itemChoices[i] = ((currentFasterMonsterTeam == "A")?teamAItemChoices[currentFasterMonsterPosition]:teamBItemChoices[currentFasterMonsterPosition]);
			enemyTargets[i] = ((currentFasterMonsterTeam == "A")?teamAEnemyTargets[currentFasterMonsterPosition]:teamBEnemyTargets[currentFasterMonsterPosition]);
			allyTargets[i] = ((currentFasterMonsterTeam == "A")?teamAAllyTargets[currentFasterMonsterPosition]:teamBAllyTargets[currentFasterMonsterPosition]);

			usedPositions[i] = currentFasterMonsterPosition;

		}
	}

	bool InArray(int[] positionsArray, string[] teamsArray, int position, string team) {
		for(int i=0; i < positionsArray.Length; i++) {
			if(teamsArray[i] == team) {
				if(positionsArray[i] == position)
					return true;
			}
		}

		return false;
	}

	public Global.BattlePhase GetPhase() {
		return battlePhase;
	}

	public Monster GetCurrentMonster() {
		return teamA[currentMonster];
	}

	public int GetCurrentMonsterInArrayPosition() {
		return currentMonster;
	}

	public void SetMonsterChoice(int choice, int skillOrItem, int target, bool isEnemyTarget) {
		teamAChoices[currentMonster] = choice;

		if(choice == 2)
			teamASkillChoices[currentMonster] = skillOrItem;

		if(choice == 4)
			teamAItemChoices[currentMonster] = skillOrItem;

		if(isEnemyTarget)
			teamAEnemyTargets[currentMonster] = target;
		else
			teamAAllyTargets[currentMonster] = target;
	}

	Monster GetNextMonsterOfTeam(string team) {
		Monster[] currentTeam = ((team == "A")?teamA:teamB);

		for(int i=0; i < currentTeam.Length; i++) {
			if(currentTeam[i].state != Global.State.Dead) {
				return currentTeam[i];
			}
		}

		battlePhase = Global.BattlePhase.BattleEnds;

		return null;
	}

	bool haveSurvivors(string team) {
		Monster[] currentTeam = ((team == "A")?teamA:teamB);
		
		for(int i=0; i < currentTeam.Length; i++) {
			if(currentTeam[i].state != Global.State.Dead) {
				return true;
			}
		}

		return false;
	}
}
