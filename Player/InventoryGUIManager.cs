using UnityEngine;
using System.Collections;

[ExecuteInEditMode]

public class InventoryGUIManager : MonoBehaviour {

	public Player player;

	private Item[] items;
	private int[] quantities;

	private string currentName;
	private string currentDescription;
	private Texture2D currentIcon;
	private bool activeUseButton;

	private Vector2 itemsScrollPosition = Vector2.zero;

	void OnEnable() {
		items = new Item[0];
		quantities = new int[0];

		currentName = "";
		currentDescription = "";
		currentIcon = null;
		activeUseButton = false;

		for(int i=0; i < player.items.Length; i++) {
			if(!player.items[i].IsUsed()) {
				if(!InItemList(player.items[i]))
					AddToItemList(player.items[i]);
				else
					IncrementItemOfList(player.items[i]);
			}
		}
	}

	void Start () {
	
	}

	void Update () {
	
	}

	void OnGUI() {
			GUI.Window (0, new Rect(10, 10, Screen.width - 20, Screen.height - 20), InventoryWindow, "Inventory");
	}

	void InventoryWindow(int windowID) {
		// Back button
		if (GUI.Button(new Rect(Screen.width - 120, Screen.height - 55,	80, 25), "Back")) {
			this.enabled = false;
		}

		// Use button
		if(!activeUseButton)
			GUI.enabled = false;

		if (GUI.Button(new Rect(Screen.width - 210, Screen.height - 55,	80, 25), "Use")) {
			Item item = this.GetItemByName(currentName);

			if(item.numberOfTargets == Global.NumberOfTargets.OneAlly) {
				player.teamGUIManager.enabled = true;

				player.teamGUIManager.SetFromInventory(true);
				player.teamGUIManager.SetItemPosition(this.GetItemPositionByName(currentName));
			} else {
				GameObject battleManagerObject = GameObject.Find("BattleManager");
				BattleManager battleManager = (BattleManager)battleManagerObject.GetComponent("BattleManager");
				battleManager.SetMonsterChoice(4, this.GetItemPositionByName(currentName), 0, false);

				// TODO: Remove the item if you want, currently only marks the item as used
				player.items[this.GetItemPositionByName(currentName)].SetUsed(true);
			}

			this.enabled = false;
		}

		GUI.enabled = true;

		// Icon
		GUI.Box(new Rect(10, 20, (Screen.width/2) - 100, Screen.height/2 - 20), "");

		if(currentIcon != null)
			GUI.DrawTexture(
				new Rect(50, 30, currentIcon.width, ((currentIcon.height < Screen.height/2 - 40)?currentIcon.height:Screen.height/2 - 40)),
				currentIcon, 
				ScaleMode.ScaleToFit,
				true, 
				1.0F);

		// Item list
		GUI.Box(new Rect((Screen.width/2) - 80, 20, (Screen.width/2) + 40, Screen.height/2 - 20), "");
		
		GUILayout.BeginArea(new Rect((Screen.width/2) - 80, 25, (Screen.width/2) + 40, Screen.height/2 - 20));
		itemsScrollPosition = GUILayout.BeginScrollView(itemsScrollPosition, GUILayout.Width((Screen.width/2) + 35), GUILayout.Height(Screen.height/2 - 40));
		for(int i=0; i < items.Length; i++) {
			if(GUILayout.Button(items[i].name+" x"+quantities[i])) {
				currentName = items[i].name;
				currentDescription = items[i].description;
				currentIcon = items[i].icon;
				activeUseButton = true;
			}
		}

		GUILayout.EndScrollView();
		GUILayout.EndArea();

		// Description
		GUI.Box(new Rect(10, Screen.height/2 + 10, Screen.width - 50, Screen.height/2 - 70), "Description");
		GUI.Label(new Rect(20, Screen.height/2 + 20, Screen.width - 50, Screen.height/2 - 70), currentDescription);
	}

	bool InItemList(Item item) {
		for(int i=0; i < items.Length; i++) {
			if(items[i].name == item.name)
				return true;
		}

		return false;
	}

	void AddToItemList(Item item) {
		Item[] auxItems = new Item[items.Length+1];
		int[] auxQuantities = new int[quantities.Length+1];

		for(int i=0; i < items.Length; i++) {
			auxItems[i] = items[i];
			auxQuantities[i] = quantities[i];
		}

		auxItems[auxItems.Length-1] = item;
		auxQuantities[auxQuantities.Length-1] = 1;

		items = auxItems;
		quantities = auxQuantities;
	}

	void IncrementItemOfList(Item item) {
		for(int i=0; i < items.Length; i++) {
			if(items[i].name == item.name) {
				quantities[i]++;
				return;
			}
		}
	}

	Item GetItemByName(string itemName) {
		for(int i=0; i < player.items.Length; i++) {
			if(player.items[i].name == itemName && !player.items[i].IsUsed()) {
				return player.items[i];
			}
		}

		return null;
	}

	int GetItemPositionByName(string itemName) {
		for(int i=0; i < player.items.Length; i++) {
			if(player.items[i].name == itemName && !player.items[i].IsUsed()) {
				return i;
			}
		}

		return -1;
	}
}
