using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour {

	public string name;
	public string description;
	public Texture2D icon;

	public Global.Effect effect;
	public Global.Attribute targetAttribute;

	public int value;
	public Global.Unit unit;
	
	public Global.NumberOfTargets numberOfTargets;

	public bool overDead;

	public GameObject effectObject;
	public Global.Position effectObjectPosition;
	public float effectObjectDuration;

	private bool used = false;

	void Start () {

	}
	

	void Update () {
	
	}

	public bool IsUsed() {
		return used;
	}

	public void SetUsed(bool used) {
		this.used = used;
	}
}
