using UnityEngine;
using System.Collections;

public class Skill : MonoBehaviour {

	public string name;

	public int manaCost;

	public Global.AttackType attackType;

	public int bonification;
	public Global.Attribute bonificationAttribute;

	public Global.Effect effect;
	public Global.Attribute targetAttribute;
	
	public Global.NumberOfTargets numberOfTargets;

	[Range(0,100)] public int precision;

	public bool overDead;

	public GameObject projectile;

	public GameObject effectObject;
	public Global.Position effectObjectPosition;
	public float effectObjectDuration;

	void Start () {
	
	}
	

	void Update () {
	
	}
}