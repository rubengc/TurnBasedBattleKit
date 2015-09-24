using UnityEngine;
using System.Collections;

[ExecuteInEditMode]

public class TeamGUIManager : MonoBehaviour {

	public Player player;

	public Texture2D hpBarTexture;
	public Texture2D mpBarTexture;
	public Texture2D backgroundBarTexture;

	public GUIStyle textStyle;

	private bool displayActionBox;
	private int displayActionBoxNumber = 0;

	private bool fromInventory; // True if script has been enabled from inventory
	private int itemPosition;
	
	void Start () {
	
	}

	void Update () {
	
	}

	void OnEnable() {
		fromInventory = false;
	}

	void OnGUI() {
		if(!IsDisplayingMonsterInformation())
			GUI.Window (0, new Rect(10, 10, Screen.width - 20, Screen.height - 20), TeamWindow, "Team");
	}

	void TeamWindow(int windowID) {
		if(displayActionBox)
			GUI.enabled = false;

		// Back button
		if (GUI.Button(new Rect(Screen.width - 120, Screen.height - 55,	80, 25), "Back")) {
			if(fromInventory)
				player.inventoryGUIManager.enabled = true;

			this.enabled = false;
		}


		GUI.enabled = true;

		for(int i=0; i < 6; i++) {
			if(i >= player.monsters.Length || displayActionBox) {
				GUI.enabled = false;
			}

			if(GUI.Button(
				new Rect(
					((i > 2)?((Screen.width/2) - 40)+40:20), 
					((i > 2)?20 + (((Screen.height/3) - 40)*(i-3)) + (15*(i-3)):20 + (((Screen.height/3) - 40)*i) + (15*i)),
					(Screen.width/2) - 40,
					(Screen.height/3) - 40
				), 
				""
			)) {
				displayActionBox = true;
				displayActionBoxNumber = i;
			}

			if(i < player.monsters.Length) {
				GUI.BeginGroup(
					new Rect(
						((i > 2)?((Screen.width/2) - 40)+40:20), 
						((i > 2)?20 + (((Screen.height/3) - 40)*(i-3)) + (15*(i-3)):20 + (((Screen.height/3) - 40)*i) + (15*i)),
						(Screen.width/2) - 40,
						(Screen.height/3) - 40
					)
				);
				
				GUI.DrawTexture(new Rect(10, 10, 100, 100), player.monsters[i].avatar);

				textStyle.fontSize = 18;
				textStyle.normal.textColor = Color.white;

				GUI.Label(new Rect(120, 10, 100, 100), player.monsters[i].name, textStyle);

				textStyle.fontSize = 14;
				GUI.Label(new Rect(120, 30, 100, 100), "Lv: "+player.monsters[i].GetLevel(), textStyle);

				// Background health bar
				GUI.DrawTexture(
					new Rect(
						(Screen.width/2) - 250,
						10,
						200, 
						22
					),
					backgroundBarTexture
				);

				// Current health bar
				GUI.DrawTexture(
					new Rect(
						(Screen.width/2) - 249,
						11,
						(198*player.monsters[i].GetCurrentHealth())/player.monsters[i].GetMaximumHealth(),
						20
					),
					hpBarTexture
				);

				textStyle.fontSize = 18;

				GUI.Label(
					new Rect(
						(Screen.width/2) - ((player.monsters[i].GetMaximumHealth() > 100)?120:100),
						10,
						100, 
						100
					),
		          	player.monsters[i].GetCurrentHealth()+"/"+player.monsters[i].GetMaximumHealth(),
		          	textStyle
	          	);

				// Background mana bar
				GUI.DrawTexture(
						new Rect(
						(Screen.width/2) - 230,
						32,
						180, 
						16
					),
					backgroundBarTexture
				);
				
				// Current mana bar
				GUI.DrawTexture(
					new Rect(
						(Screen.width/2) - 229,
						33,
						(178*player.monsters[i].GetCurrentMana())/player.monsters[i].GetMaximumMana(),
						14
					),
					mpBarTexture
				);
				
				textStyle.fontSize = 13;
				
				GUI.Label(
					new Rect(
						(Screen.width/2) - ((player.monsters[i].GetMaximumMana() > 100)?103:90),
						32,
						100, 
						100
					),
					player.monsters[i].GetCurrentMana()+"/"+player.monsters[i].GetMaximumMana(),
					textStyle
				);
				
				GUI.EndGroup();
			}
		}

		GUI.enabled = true;

		// Action boxes
		for(int i=0; i < 6; i++) {

			if(displayActionBox && displayActionBoxNumber == i) {
				int x = ((i > 2)?Screen.width-145:(Screen.width/2)-125);
				int y = ((i > 2)?(((Screen.height/3) - 40)*(i-3)) + (15*(i-3)):(((Screen.height/3) - 40)*i) + (15*i));
				int width = 90;
				int height = 25;

				GUI.Box (
					//new Rect(x - 5, y + 110, 100, ((fromInventory)?75:100)), // Use this if you want to implement the change action
                    new Rect(x - 5, y + 110, 100, 75),
                    "Actions"
				);

				if(fromInventory) {
					if(GUI.Button(
						new Rect(x, y + 130, width, height),
						"Use"
					)) {
						displayActionBox = false;

						GameObject battleManagerObject = GameObject.Find("BattleManager");
						BattleManager battleManager = (BattleManager)battleManagerObject.GetComponent("BattleManager");
						battleManager.SetMonsterChoice(4, itemPosition, i, false);

						// TODO: Remove the item if you want, currently only marks the item as used
						player.items[itemPosition].SetUsed(true);

						this.enabled = false;
					}
				} else {
					if(GUI.Button(
						new Rect(x, y + 130, width, height),
						"See"
					)) {
						player.monsters[i].monsterGUIManager.enabled = true;
						displayActionBox = false;
					}

                    // TODO: On click, change monster by another
					/*GUI.Button(
						new Rect(x, y + 155, width, height),
						"Change"
					);*/
				}

				if(GUI.Button(
                    //new Rect(x, y + ((fromInventory)?155:180), width, height), // Use this if you want to implement the change action
                    new Rect(x, y + 155, width, height),
                    "Cancel"
				)) {
					displayActionBox = false;
				}
			}
		}
	}

	bool IsDisplayingMonsterInformation() {
		for(int i=0; i < player.monsters.Length; i++) {
			if(player.monsters[i].monsterGUIManager.enabled)
				return true;
		}

		return false;
	}

	public void SetFromInventory(bool fromInventory) {
		this.fromInventory = fromInventory;
	}

	public void SetItemPosition(int itemPosition) {
		this.itemPosition = itemPosition;
	}
}
