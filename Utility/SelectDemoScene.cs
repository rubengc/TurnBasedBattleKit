using UnityEngine;
using System.Collections;

[ExecuteInEditMode]

public class SelectDemoScene : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI() {
		int width = 100;
		int height = 60;
		string[] levels = new string[4];

		levels[0] = "Battle 1v1"; 
		levels[1] = "Battle 2v2"; 
		levels[2] = "Battle 3v3";
		levels[3] = "Battle 3v5";

		for(int i=0; i < levels.Length; i++) {
			if (GUI.Button(new Rect(20 + ((20 + width) * i), 75, width, height), levels[i]))
				Application.LoadLevel(i+1);
		}
	}
}
