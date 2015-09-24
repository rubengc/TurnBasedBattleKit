using UnityEngine;
using System.Collections;

[ExecuteInEditMode]

public class MonsterGUIManager : MonoBehaviour {

	public Monster monster;

	public Camera previewCamera;
	public Transform previewPostion;

	public GUIStyle textStyle;

	private Vector2 skillsScrollPosition = Vector2.zero;

	void OnEnable() {
		GameObject previewClone = (GameObject)GameObject.Find("PreviewClone");
		if(previewClone != null)
			GameObject.Destroy(previewClone);

		previewClone = Instantiate(monster.monsterGameObject, previewPostion.position, previewPostion.rotation) as GameObject;
		previewClone.name = "PreviewClone";

        if(previewClone.GetComponent<Animation>())
            previewClone.GetComponent<Animation>().Play("Idle");

		previewCamera.enabled = true;
	}

	void Start () {
	
	}

	void Update () {

	}

	void OnGUI() {
		GUI.Window (0, new Rect(10, 10, Screen.width/2 + 70, Screen.height - 20), MonsterWindow, "Monster information");

		// Back button
		if (GUI.Button(new Rect(Screen.width - 100, Screen.height - 45,	80, 25), "Back")) {
			previewCamera.enabled = false;
			this.enabled = false;
		}
	}

	void MonsterWindow(int windowID) {
		// Back button
		/*if (GUI.Button(new Rect(Screen.width - 120, Screen.height - 55,	80, 25), "Back")) {
			previewCamera.enabled = false;
			this.enabled = false;
		}*/

		textStyle.normal.textColor = Color.white;
		textStyle.fontSize = 24;
		
		GUI.Label(new Rect(10, 20, 100, 100), monster.name, textStyle);

		// Stats
		GUI.Box(
			new Rect(
				10,
				50,
				(Screen.width/2) + 50,
				(Screen.height/2) - 50
			),
			"Stats"
		);
		
		textStyle.fontSize = 20;
		
		GUI.Label(new Rect(Screen.width/2 - 5, 50, 100, 100), "Lv: "+monster.GetLevel(), textStyle);
		
		textStyle.fontSize = 16;

		GUI.BeginGroup(new Rect(10, 60, (Screen.width/2) + 50, (Screen.height/2) - 60));

			// HP
			DrawStatLine(
				new Rect(10, 25, (Screen.width/2) + 30, 20),
				"HP:",
				monster.GetCurrentHealth()+"/"+monster.GetMaximumHealth(),
				textStyle
			);

			// MP
			DrawStatLine(
				new Rect(10, 50, (Screen.width/2) + 30, 20),
				"MP:",
				monster.GetCurrentMana()+"/"+monster.GetMaximumMana(),
				textStyle
			);

			// Attack
			DrawStatLine(
				new Rect(10, 75, (Screen.width/2) + 30, 20),
				"Attack:",
				""+monster.GetAttack(),
				textStyle
			);

			// Defense
			DrawStatLine(
				new Rect(10, 100, (Screen.width/2) + 30, 20),
				"Defense:",
				""+monster.GetDefense(),
				textStyle
			);

			// Special Attack
			DrawStatLine(
				new Rect(10, 125, (Screen.width/2) + 30, 20),
				"Special Attack:",
				""+monster.GetSpecialAttack(),
				textStyle
			);
		
			// Special Defense
			DrawStatLine(
				new Rect(10, 150, (Screen.width/2) + 30, 20),
				"Special Defense:",
				""+monster.GetSpecialDefense(),
				textStyle
			);

			// Speed
			DrawStatLine(
				new Rect(10, 175, (Screen.width/2) + 30, 20),
				"Speed:",
				""+monster.GetSpeed(),
				textStyle
			);

		GUI.EndGroup();


		// Preview
		GUI.Box(
			new Rect(
				Screen.width/2 + 70,
				20,
				(Screen.width/2) - 100,
				Screen.height - 80
			),
			"Preview"
		);

		// Skills
		GUI.Box(new Rect(10, Screen.height/2 + 10, (Screen.width/2) + 50, Screen.height/2 - 70), "Skills");

		GUILayout.BeginArea(new Rect(10, Screen.height/2 + 25, (Screen.width/2) + 50, Screen.height/2 - 70));
		skillsScrollPosition = GUILayout.BeginScrollView(skillsScrollPosition, GUILayout.Width((Screen.width/2) + 50), GUILayout.Height(Screen.height/2 - 95));
				for(int i=0; i < monster.skills.Length; i++) {
					GUILayout.Label(monster.skills[i].name);
				}
			GUILayout.EndScrollView();
		GUILayout.EndArea();
	}

	void DrawStatLine(Rect rect, string leftText, string rightText, GUIStyle style) {
		style.alignment = TextAnchor.UpperLeft;
		
		GUI.Label(rect, leftText, style);
		
		style.alignment = TextAnchor.UpperRight;
		
		GUI.Label(rect, rightText, style);

		style.alignment = TextAnchor.UpperLeft;
	}
}
