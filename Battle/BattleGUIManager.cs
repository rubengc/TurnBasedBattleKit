using UnityEngine;
using System.Collections;

[ExecuteInEditMode]

public class BattleGUIManager : MonoBehaviour {

	public BattleManager battleManager;

	private bool displaySkillsWindow = false;
	
	private bool isSelectingTarget = false;
	private bool isSelectingEnemyTarget = false;

	private int cacheChoice = 0;
	private int cacheSkillChoice = 0;

	public Texture2D hpBarTexture;
	public Texture2D mpBarTexture;
	public Texture2D backgroundBarTexture;

	private bool isDisplayingDamage;
	private ArrayList displayedDamage;
	private ArrayList displayedDamagePosition;
	private ArrayList displayedDamageColor;
	private ArrayList timeToDisplay;

	private string displayedMessage;
	private bool isDisplayingMessage;
	private float displayMessageTime;
	private bool displayMessageFullScreenWidth;

	public GUIStyle textStyle;
	
	void Start () {
		displayedDamage = new ArrayList();
		displayedDamagePosition = new ArrayList();
		displayedDamageColor = new ArrayList();
		timeToDisplay = new ArrayList();
	}

	void Update () {
	
	}

	void OnGUI() {
		if(isDisplayingMessage)
	   		if(displayMessageTime < Time.time && displayMessageTime > 0.0f)
	   			isDisplayingMessage = false;
   			else
				GUI.Window (0, new Rect(20, Screen.height - 210, ((displayMessageFullScreenWidth)?Screen.width - 40:700), 200), MessageWindow, "Message");

		if(battleManager.GetPhase() != Global.BattlePhase.BattleEnds 
		   && !battleManager.teamAPlayer.teamGUIManager.enabled 
		   && !battleManager.teamAPlayer.inventoryGUIManager.enabled) {
			if(battleManager.GetPhase() == Global.BattlePhase.TeamASelection && !isDisplayingMessage)
				GUI.Window (1, new Rect(20, Screen.height - 210, 200, 200), ActionsWindow, battleManager.GetCurrentMonster().name);	
			
			if(displaySkillsWindow && !isDisplayingMessage)
				GUI.Window (2, new Rect(230, Screen.height - 210, 200, 200), SkillsWindow, battleManager.GetCurrentMonster().name + " skills");
			
			if(isSelectingTarget && !isDisplayingMessage)
				GUI.Window (3, new Rect(((displaySkillsWindow)?440:230), Screen.height - 210, 200, 200), SelectTargetWindow, "Select the target");

			// Your team information
			for(int i=0; i < battleManager.teamA.Length; i++) {
				textStyle.fontSize = 16;
				// Monster name
				DrawOutlineLabel(
					new Rect(Screen.width - 210, (Screen.height - 210) + (48*i), 100, 20),
					battleManager.teamA[i].name,
					Color.black,
					Color.white,
					textStyle
				);

				textStyle.fontSize = 13;

				// HP
				DrawOutlineLabel(
					new Rect(Screen.width - 210, (Screen.height - 223) + (48*i), 100, 20),
					"Lv: " + battleManager.teamA[i].GetLevel(),
					Color.black,
					Color.white,
					textStyle
				);

				// HP
				DrawOutlineLabel(
					new Rect(Screen.width - 120, (Screen.height - 207) + (48*i), 100, 20),
					"HP: " + battleManager.teamA[i].GetCurrentHealth(),
					Color.black,
					Color.white,
					textStyle
				);

				// MP
				DrawOutlineLabel(
					new Rect(Screen.width - 60, (Screen.height - 207) + (48*i), 100, 20),
					"MP: " + battleManager.teamA[i].GetCurrentMana(),
					Color.black,
					Color.white,
					textStyle
				);

				// Background health bar
				GUI.DrawTexture(
					new Rect(Screen.width - 210, (Screen.height - 190) + (48*i), 200, 8),
					backgroundBarTexture
				);
				
				// Current health bar
				GUI.DrawTexture(
					new Rect(Screen.width - 209, (Screen.height - 189) + (48*i), (198*battleManager.teamA[i].GetCurrentHealth())/battleManager.teamA[i].GetMaximumHealth(), 6),
					hpBarTexture
				);

				// Background mana bar
				GUI.DrawTexture(
					new Rect(Screen.width - 180, (Screen.height - 182) + (48*i), 170, 8),
					backgroundBarTexture
				);
				
				// Current mana bar
				GUI.DrawTexture(
					new Rect(Screen.width - 179, (Screen.height - 181) + (48*i), (168*battleManager.teamA[i].GetCurrentMana())/battleManager.teamA[i].GetMaximumMana(), 6),
					mpBarTexture
				);
			}

			// Enemy health information
			for(int i=0; i < battleManager.teamB.Length; i++) {
				GUI.Window (i+4, new Rect(Screen.width/2 - (80*battleManager.teamB.Length )+ (160*i), 5, 150, 140), EnemyWindow, battleManager.teamB[i].name);
			}
		}

		if(isDisplayingDamage) {
			for(int i=0; i < displayedDamage.Count; i++) {
				if((float)timeToDisplay[i] < Time.time) {
					displayedDamage.RemoveAt(i);
					displayedDamagePosition.RemoveAt(i);
					displayedDamageColor.RemoveAt(i);
					timeToDisplay.RemoveAt(i);
				} else {
					Vector3 screenPosition = Camera.main.WorldToScreenPoint((Vector3)displayedDamagePosition[i]);

					textStyle.fontSize = 20;
					textStyle.fontStyle = FontStyle.Bold;

					DrawOutlineLabel(
						new Rect(screenPosition.x - 25, Screen.height - screenPosition.y - 50 + (((float)timeToDisplay[i]-Time.time)*30), 100, 100), 
						""+(int)displayedDamage[i],
						Color.black,
						(Color)displayedDamageColor[i],
						textStyle
					);

					textStyle.fontStyle = FontStyle.Normal;
				}
			}

			if(displayedDamage.Count == 0)		
				isDisplayingDamage = false;	
		}
	}
	
	void ActionsWindow(int windowID) {
		if (GUI.Button(new Rect(10, 20, 180, 30), "Attack")) {
			isSelectingTarget = true;
			isSelectingEnemyTarget = true;
			displaySkillsWindow = false;
			
			cacheChoice = 1;
		}
		
		if (GUI.Button(new Rect(10, 55, 180, 30), "Skills")) {
			displaySkillsWindow = true;
			isSelectingTarget = false;
		}
		
		if (GUI.Button(new Rect(10, 90, 180, 30), "Team")) {
			battleManager.teamAPlayer.teamGUIManager.enabled = true;
			displaySkillsWindow = false;
			isSelectingTarget = false;
		}
		
		if (GUI.Button(new Rect(10, 125, 180, 30), "Inventory")) {
			battleManager.teamAPlayer.inventoryGUIManager.enabled = true;
			displaySkillsWindow = false;
			isSelectingTarget = false;
		}
		
		if (GUI.Button(new Rect(10, 160, 180, 30), "Run")) {
			Application.LoadLevel(0);
		}
	}
	
	void SkillsWindow(int windowID) {
		for(int i=0; i < battleManager.GetCurrentMonster().skills.Length; i++) {
			Skill skill = battleManager.GetCurrentMonster().skills[i];

			if(battleManager.GetCurrentMonster().GetCurrentMana() < skill.manaCost)
				GUI.enabled = false;

			if (GUI.Button(new Rect(10, 20 + (35*i), 180, 30), skill.name)) {

				if(skill.numberOfTargets == Global.NumberOfTargets.OneAlly || skill.numberOfTargets == Global.NumberOfTargets.OneEnemy) {
					isSelectingTarget = true;
					cacheChoice = 2;
					cacheSkillChoice = i;

					if(skill.numberOfTargets == Global.NumberOfTargets.OneEnemy) {
						isSelectingEnemyTarget = true;
					} else if(skill.numberOfTargets == Global.NumberOfTargets.OneAlly) {
						isSelectingEnemyTarget = false;
					}
				} else if(skill.numberOfTargets == Global.NumberOfTargets.Himself) {
					isSelectingTarget = false;
					displaySkillsWindow = false;

					battleManager.SetMonsterChoice(2, i, battleManager.GetCurrentMonsterInArrayPosition(), false);
				} else {
					isSelectingTarget = false;
					displaySkillsWindow = false;

					battleManager.SetMonsterChoice(2, i, 0, false);
				}
			}

			GUI.enabled = true;
		}
	}
	
	void SelectTargetWindow(int windowID) {
		if(isSelectingEnemyTarget) {
			for(int i=0; i < battleManager.teamB.Length; i++) {
				if(battleManager.teamB[i].state == Global.State.Dead)
					GUI.enabled = false;

				if (GUI.Button(new Rect(10, 20 + (35*i), 180, 30), battleManager.teamB[i].name)) {
					isSelectingTarget = false;
					displaySkillsWindow = false;
					
					battleManager.SetMonsterChoice(cacheChoice, cacheSkillChoice, i, true);
				}

				GUI.enabled = true;
			}
		} else {
			for(int i=0; i < battleManager.teamA.Length; i++) {
				if(battleManager.teamA[i].state == Global.State.Dead) {
					if(!battleManager.GetCurrentMonster().skills[cacheSkillChoice].overDead)
						GUI.enabled = false;
				}

				if (GUI.Button(new Rect(10, 20 + (35*i), 180, 30), battleManager.teamA[i].name)) {
					isSelectingTarget = false;
					displaySkillsWindow = false;
					
					battleManager.SetMonsterChoice(cacheChoice, cacheSkillChoice, i, false);
				}

				GUI.enabled = true;
			}
		}
	}

	void EnemyWindow(int windowID) {
		// Enemy icon
		GUI.DrawTexture(
			new Rect(10, 25, 90, 90),
			battleManager.teamB[windowID-4].avatar
		);

		// Enemy level
		GUI.Label(
			new Rect(110, 100, 100, 20),
			"Lv: " + battleManager.teamB[windowID-4].GetLevel()
		);

		// Background health bar
		GUI.DrawTexture(
			new Rect(10, 120, 130, 8),
			backgroundBarTexture
		);
		
		// Current health bar
		GUI.DrawTexture(
			new Rect(11, 121, (128*battleManager.teamB[windowID-4].GetCurrentHealth())/battleManager.teamB[windowID-4].GetMaximumHealth(), 6),
			hpBarTexture
		);
	}

	void MessageWindow(int windowID) {
		textStyle.fontSize = 13;
		textStyle.fontStyle = FontStyle.Normal;

		DrawOutlineLabel(
			new Rect(10, 20, 700, 200), 
			displayedMessage,
			Color.black,
			Color.white,
			textStyle
		);

		if(battleManager.GetPhase() == Global.BattlePhase.BattleEnds) {
			if (GUI.Button(new Rect(820, 165, 90, 25), "Main menu"))
				Application.LoadLevel(0);
		}
	}

	public void DisplayDamage(int damage, Vector3 position, Color color) {
		isDisplayingDamage = true;
		displayedDamage.Add(damage);
		displayedDamagePosition.Add(position);
		displayedDamageColor.Add(color);
		timeToDisplay.Add(Time.time + 2);
	}

	public void DisplayMessage(string message, float time = 2.0f, bool fullScreenWidth = false) {
		displayedMessage = message;
		isDisplayingMessage = true;

		if(time > 0.0f)
			displayMessageTime = time + Time.time;
		else
			displayMessageTime = -1.0f; // Infinite message time

		displayMessageFullScreenWidth = fullScreenWidth;
	}

	public void DrawOutlineLabel(Rect position, string text, Color outColor, Color inColor, GUIStyle style){
		GUIStyle backupStyle = style;
		style.normal.textColor = outColor;

		position.x--;

		GUI.Label(position, text, style);
		position.x += 2;

		GUI.Label(position, text, style);
		position.x--;
		position.y--;

		GUI.Label(position, text, style);
		position.y += 2;

		GUI.Label(position, text, style);
		position.y--;

		style.normal.textColor = inColor;

		GUI.Label(position, text, style);

		style = backupStyle;
	}
}
