using UnityEngine;
using System.Collections;

public class Global : MonoBehaviour {

	public enum Type {
		Electric,
		Fire,
		Plant,
		Water
	}

	public enum AttackType {
		Melee,
		Ranged
	}

	public enum NumberOfTargets {
		Himself,
		OneAlly,
		AllAlly,
		OneEnemy,
		AllEnemy,
		All
	}

	public enum Effect {
		None,
		Damage,
		Increase,
		Decrease,
		Drain
	}

	public enum Attribute {
		None,
		Health,
		Mana,
		Attack,
		Defense,
		SpecialAttack,
		SpecialDefense,
		Speed,
		CriticalChance
	}

	public enum State {
		None,
		Asleep,
		Burned,
		Paralyzed,
		Poisoned,
		Dead
	}

	public enum BattlePhase {
		BattleStart,
		TeamASelection,
		TeamBSelection,
		ExecuteActions,
		BattleEnds
	}

	public enum Unit {
		Plane,
		Percent
	}

	public enum Position {
		Top,
		Center,
		Bottom
	}
}
