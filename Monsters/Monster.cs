using UnityEngine;
using System.Collections;

public class Monster : MonoBehaviour {

	public MonsterGUIManager monsterGUIManager;

	public GameObject monsterGameObject;
	private float idleSwitchDelay;

	public string name = "Monster";

	public Texture2D avatar;

	public int level = 5;

	public int health = 100;
	public int mana = 100;
	public int attack = 20;
	public int defense = 10;
	public int specialAttack = 15;
	public int specialDefense = 10;
	public int speed = 10;
	[Range(0,100)] public int criticalChance = 5;

	private int currentHealth;
	private int currentMana;
	private int currentAttack;
	private int currentDefense;
	private int currentSpecialAttack;
	private int currentSpecialDefense;
	private int currentSpeed;
	private int currentCriticalChance;

    private MonsterAnimator monsterAnimator;

	private AudioClip currentAudioClip;
	public AudioClip attackSFX;

	public Global.State state;

	public Skill[] skills;

	public Global.AttackType attackType;
	public GameObject projectile;
	private Monster currentTarget;
	private bool isMovingToTarget = false;

	private bool isAttacking = false;
	private float attackDelay;

	private Skill currentSkill;
	private bool isUsingSkill = false;
	private float specialAttackDelay;

	private Item currentItem;
	private bool isUsingItem = false;
	private float itemDelay;
	private float itemEffectObjectDelay;

	private CharacterController controller;

	private Vector3 initialPosition;
	private Quaternion initialRotation;
	public float moveToInitialPositionSpeed = 15.0F;
	private bool isMovingToInitialPosition = false;
	private float movingToInitialPositionDelay;

	public BattleManager battleManager;
	public BattleGUIManager battleGUIManager;

	void Start () {
        controller = transform.GetComponent<CharacterController>();
        monsterAnimator = GetComponent<MonsterAnimator>();

        currentHealth = calculateAttribute(health);
		currentMana = calculateAttribute(mana);
		currentAttack = calculateAttribute(attack);
   		currentDefense = calculateAttribute(defense);
    	currentSpecialAttack = calculateAttribute(specialAttack);
		currentSpecialDefense = calculateAttribute(specialDefense);
		currentSpeed = calculateAttribute(speed);
		currentCriticalChance = criticalChance;

		initialPosition = transform.position;
		initialRotation = transform.rotation;
	}
	
	void Update () {
		if(state == Global.State.Dead) return;

		if(isMovingToTarget) {
			if(Vector3.Distance(currentTarget.transform.position, transform.position) < 3 * transform.localScale.x) {
				isMovingToTarget = false;

				if(isAttacking) {
					attackDelay = monsterAnimator.Length("PhysicalAttack") - 0.6F + Time.time;

                    if(attackSFX != null) {
						currentAudioClip = attackSFX;
						Invoke("PlayAudioClip", monsterAnimator.Length("PhysicalAttack") - 0.6F);
					}

                    monsterAnimator.PlayQueued("PhysicalAttack", new string[] { "MoveBackward", "Idle" });
				} else {
					specialAttackDelay = monsterAnimator.Length("SpecialAttack") - 0.6F + Time.time;

                    monsterAnimator.PlayQueued("SpecialAttack", new string[] { "MoveBackward", "Idle" });
                }
			} else {
				MoveTo(currentTarget.transform.position);
			}
		}

		if(isAttacking && attackDelay < Time.time && !isMovingToTarget) {
			currentTarget.ReceiveDamage(currentAttack, 30);

			isAttacking = false;

			movingToInitialPositionDelay = Time.time + 1;
			isMovingToInitialPosition = true;
		}

		if(isUsingSkill && specialAttackDelay < Time.time && !isMovingToTarget) {
			if(currentSkill.effectObject != null) {
				InstantiateParticlesOver(currentSkill.numberOfTargets, currentSkill.effectObject, currentSkill.effectObjectPosition);
			}

			if(currentSkill.numberOfTargets == Global.NumberOfTargets.OneAlly || currentSkill.numberOfTargets == Global.NumberOfTargets.OneEnemy || currentSkill.numberOfTargets == Global.NumberOfTargets.Himself) {
				ApplySkillEffect(currentSkill, currentTarget);
			} else if(currentSkill.numberOfTargets == Global.NumberOfTargets.AllAlly) {
				foreach(Monster ally in battleManager.teamA) {
					ApplySkillEffect(currentSkill, ally);
				}
			} else if(currentSkill.numberOfTargets == Global.NumberOfTargets.AllEnemy) {
				foreach(Monster enemy in battleManager.teamB) {
					ApplySkillEffect(currentSkill, enemy);
				}
			} else if(currentSkill.numberOfTargets == Global.NumberOfTargets.All) {
				foreach(Monster ally in battleManager.teamA) {
					ApplySkillEffect(currentSkill, ally);
				}

				foreach(Monster enemy in battleManager.teamB) {
					ApplySkillEffect(currentSkill, enemy);
				}
			}

			currentMana -= currentSkill.manaCost;

			isUsingSkill = false;
			
			movingToInitialPositionDelay = Time.time + 1;
			isMovingToInitialPosition = true;
		}

        // Instantiate item effect particles
		if(isUsingItem && itemDelay < Time.time && itemEffectObjectDelay == 0) {
			if(currentItem.numberOfTargets == Global.NumberOfTargets.OneAlly || currentItem.numberOfTargets == Global.NumberOfTargets.Himself) {
				GameObject itemEffectObjectClone = Instantiate(currentItem.effectObject, currentTarget.GetPosition(currentItem.effectObjectPosition), currentTarget.transform.rotation) as GameObject;
			} else if(currentItem.numberOfTargets == Global.NumberOfTargets.AllAlly) {
				foreach(Monster ally in battleManager.teamA) {
					if(ally.state == Global.State.Dead) {
						if(currentItem.overDead) {
							GameObject itemEffectObjectClone = Instantiate(currentItem.effectObject, ally.GetPosition(currentItem.effectObjectPosition), ally.transform.rotation) as GameObject;
						}
					} else {
						GameObject itemEffectObjectClone = Instantiate(currentItem.effectObject, ally.GetPosition(currentItem.effectObjectPosition), ally.transform.rotation) as GameObject;
					}
				}
			}

			itemEffectObjectDelay = currentItem.effectObjectDuration-1f + Time.time;
		}

        // Apply item effects
		if(isUsingItem && itemDelay < Time.time && itemEffectObjectDelay < Time.time) {
			if(currentItem.numberOfTargets == Global.NumberOfTargets.OneAlly || currentItem.numberOfTargets == Global.NumberOfTargets.Himself) {
				currentTarget.ApplyItemEffect(currentItem);
			} else if(currentItem.numberOfTargets == Global.NumberOfTargets.AllAlly) {
				foreach(Monster ally in battleManager.teamA) {
					if(ally.state == Global.State.Dead) {
						if(currentItem.overDead) {
							ally.ApplyItemEffect(currentItem);
						}
					} else {
						ally.ApplyItemEffect(currentItem);
					}
				}
			}

			isUsingItem = false;
            // TODO: Remove item from player inventory, currently only marks the item as used
        }

        if(isMovingToInitialPosition && movingToInitialPositionDelay < Time.time) {
			if(Vector3.Distance(initialPosition, transform.position) < 0.5) {
				isMovingToInitialPosition = false;
			} else {
				MoveToInitialPosition();
			}
		}
	}

    // Attack action method
	public void Attack(Monster target) {
		// Rotate to target
		transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(target.transform.position - transform.position), 4 * Time.deltaTime);
		transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

		if(attackType == Global.AttackType.Melee) {
			isMovingToTarget = true;
        } else {
			isMovingToTarget = false;

			if(projectile != null) {
				GameObject projectileClone = Instantiate(projectile, transform.position, transform.rotation) as GameObject;
				Projectile projectileCloneScript = (Projectile)projectileClone.GetComponent("Projectile");
				projectileCloneScript.target = target.transform;

				attackDelay = Vector3.Distance(target.transform.position, transform.position)/projectileCloneScript.speed + Time.time;
			} else {
				attackDelay = monsterAnimator.Length("SpecialAttack") -0.6F + Time.time;
			}

            monsterAnimator.PlayQueued("SpecialAttack");
		}

		isAttacking = true;
		currentTarget = target;
	}

	public void UseSkill(int skillNumber, Monster target) {
		currentSkill = skills[skillNumber];

		// Rotate to target
		transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(target.transform.position - transform.position), 4 * Time.deltaTime);
		transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

		if(currentSkill.attackType == Global.AttackType.Melee) {
			isMovingToTarget = true;
        } else {
			isMovingToTarget = false;

			if(currentSkill.projectile != null) {
				GameObject projectileClone = Instantiate(currentSkill.projectile, transform.position, transform.rotation) as GameObject;
				Projectile projectileCloneScript = (Projectile)projectileClone.GetComponent("Projectile");
				projectileCloneScript.target = target.transform;

				specialAttackDelay = Vector3.Distance(target.transform.position, transform.position)/projectileCloneScript.speed + Time.time;
			} else {
				specialAttackDelay = monsterAnimator.Length("SpecialAttack") - 0.6F + Time.time;
			}

            monsterAnimator.PlayQueued("SpecialAttack");
        }

		isUsingSkill = true;
		currentTarget = target;
	}

	public void UseItem(string team, int itemNumber, Monster target = null) {
		Player player = battleManager.teamAPlayer;
		currentItem = player.items[itemNumber];

        monsterAnimator.PlayQueued("SpecialAttack");

		isUsingItem = true;
		itemDelay = monsterAnimator.Length("SpecialAttack") - 0.6F + Time.time;
		itemEffectObjectDelay = 0;

		if(target != null)
			currentTarget = target;
	}
	
	public void ReceiveDamage(int attackValue, int bonification) {
		int damage = calculateDamage(attackValue, bonification, currentDefense);

        ApplyDamage(damage);
    }

	public void ReceiveSpecialDamage(int attackValue, int bonification) {
		int damage = calculateDamage(attackValue, bonification, currentSpecialDefense);

        ApplyDamage(damage);
    }

    private void ApplyDamage(int damage) {
        if(currentHealth - damage > 0) {
            currentHealth -= damage;

            monsterAnimator.PlayQueued("TakeDamage");
        } else {
            currentHealth = 0;
            state = Global.State.Dead;

            monsterAnimator.PlayDead();
        }

        battleGUIManager.DisplayDamage(damage, GetTopPosition(), Color.white);
    }

	public void IncreaseAttribute(int value, Global.Attribute attribute) {
		switch(attribute) {
			case Global.Attribute.Health: 
					if((currentHealth+value < GetMaximumHealth()))
						currentHealth+=value;
					else
						currentHealth = GetMaximumHealth();
					if(state == Global.State.Dead)
						state = Global.State.None;
				break;
			case Global.Attribute.Mana:
				if((currentMana+value < GetMaximumMana()))
						currentMana+=value;
					else
						currentMana = GetMaximumMana();
				break;
			case Global.Attribute.Attack: currentAttack+=value;
				break;
			case Global.Attribute.Defense: currentDefense+=value;
				break;
			case Global.Attribute.SpecialAttack: currentSpecialAttack+=value;
				break;
			case Global.Attribute.SpecialDefense: currentSpecialDefense+=value;
				break;
			case Global.Attribute.Speed: currentSpeed+=value;
				break;
			case Global.Attribute.CriticalChance: currentCriticalChance+=value;
				break;
		}

		battleGUIManager.DisplayDamage(value, GetTopPosition(), Color.green);
	}

	public void DecreaseAttribute(int value, Global.Attribute attribute) {
		switch(attribute) {
		case Global.Attribute.Health: currentHealth-=value;
			break;
		case Global.Attribute.Mana: currentMana-=value;
			break;
		case Global.Attribute.Attack: currentAttack-=value;
			break;
		case Global.Attribute.Defense: currentDefense-=value;
			break;
		case Global.Attribute.SpecialAttack: currentSpecialAttack-=value;
			break;
		case Global.Attribute.SpecialDefense: currentSpecialDefense-=value;
			break;
		case Global.Attribute.Speed: currentSpeed-=value;
			break;
		case Global.Attribute.CriticalChance: currentCriticalChance-=value;
			break;
		}
	}

	public void ApplySkillEffect(Skill skill, Monster target) {
		if(skill.effect == Global.Effect.Damage) {
			if(skill.bonificationAttribute == Global.Attribute.Attack)
				target.ReceiveDamage(currentAttack, skill.bonification);
			else if(skill.bonificationAttribute == Global.Attribute.SpecialAttack)
				target.ReceiveSpecialDamage(currentSpecialAttack, skill.bonification);
		} else if(skill.effect == Global.Effect.Increase) {
			if(skill.bonificationAttribute == Global.Attribute.Attack)
				target.IncreaseAttribute((int)((currentAttack+skill.bonification)*0.4), skill.targetAttribute);
			else if(skill.bonificationAttribute == Global.Attribute.SpecialAttack)
				target.IncreaseAttribute((int)((currentSpecialAttack+skill.bonification)*0.4), skill.targetAttribute);
		}
	}

	public void ApplyItemEffect(Item item) {
		if(item.effect == Global.Effect.Increase)
			this.IncreaseAttribute(item.value, item.targetAttribute);
		if(item.effect == Global.Effect.Decrease)
			this.DecreaseAttribute(item.value, item.targetAttribute);
	}

	public int calculateAttribute(int baseAttributeValue) {
		return (int) Mathf.Ceil(
			(((baseAttributeValue*2)*level)/100) + (level/1) + 10
		);
	}

	public int calculateDamage(int attackValue, int bonificationValue, int defenseValue) {
		float variation = Random.Range(85.0F, 100.0F);

		return (int) Mathf.Ceil(
			((((0.2F*level+1) * attackValue * bonificationValue)/ (25 * defenseValue)) + 2) * variation * 0.01F
		);
	}

	public int GetLevel() {
		return level;
	}

	public int GetCurrentHealth() {
		return currentHealth;
	}
	
	public int GetMaximumHealth() {
		return calculateAttribute(health);
	}
	
	public int GetCurrentMana() {
		return currentMana;
	}
	
	public int GetMaximumMana() {
		return calculateAttribute(mana);
	}

	public int GetAttack() {
		return currentAttack;
	}

	public int GetDefense() {
		return currentDefense;
	}

	public int GetSpecialAttack() {
		return currentSpecialAttack;
	}
	
	public int GetSpecialDefense() {
		return currentSpecialDefense;
	}

	public int GetSpeed() {
		return currentSpeed;
	}

	public bool IsAttacking() {
		return isAttacking;
	}
	
	public bool IsUsingSkill() {
		return isUsingSkill;
	}

	public bool IsUsingItem() {
		return isUsingItem;
	}

    public bool IsMovingToTarget() {
        return isMovingToTarget;
    }

    public bool IsMovingToInitialPosition() {
		return isMovingToInitialPosition;
	}

	public void MoveTo(Vector3 targetPosition) {
		if(transform.position != targetPosition) {
			Vector3 dir = targetPosition - transform.position;
			float dist = dir.magnitude;       
			float move = 5 * transform.localScale.x * Time.deltaTime;
			
			
			if(dist > move) {
				controller.Move(dir.normalized * move);
			} 
			else {
				controller.Move(dir);
			}
			
			// Rotate to target
			dir.y = 0;
			transform.forward = Vector3.Slerp(transform.forward, dir, Time.deltaTime * 2 * transform.localScale.x);      
		}    
	}

	public void MoveToInitialPosition() {
		Vector3 dir = initialPosition - transform.position;
		float dist = dir.magnitude;       
		float move = moveToInitialPositionSpeed * transform.localScale.x * Time.deltaTime;
		
		
		if(dist > move) {
			controller.Move(dir.normalized * move);
		} 
		else {
			controller.Move(dir);
		}       

		transform.rotation = Quaternion.Slerp(transform.rotation, initialRotation, Time.time * 0.008F * transform.localScale.x);
	}

	public Vector3 GetTopPosition() {
		Vector3 monsterTopPosition = transform.position;
		Renderer monsterRenderer = GetComponentInChildren<Renderer>();
		monsterTopPosition.y += monsterRenderer.bounds.size.y;

		return monsterTopPosition;
	}

	public Vector3 GetCenterPosition() {
		Vector3 monsterTopPosition = transform.position;
		Renderer monsterRenderer = GetComponentInChildren<Renderer>();
		monsterTopPosition.y += monsterRenderer.bounds.size.y/2;

		return monsterTopPosition;
	}

	public Vector3 GetBottomPosition() {
		return transform.position;
	}

	public Vector3 GetPosition(Global.Position position) {
		if(position == Global.Position.Top) {
			return GetTopPosition();
		} else if(position == Global.Position.Center) {
			return GetCenterPosition();
		} else if(position == Global.Position.Bottom) {
			return GetBottomPosition();
		}

		return transform.position;
	}

	public void InstantiateParticlesOver(Global.NumberOfTargets numberOfTargets, GameObject particleObject, Global.Position position) {
		if(numberOfTargets == Global.NumberOfTargets.OneAlly || numberOfTargets == Global.NumberOfTargets.OneEnemy || numberOfTargets == Global.NumberOfTargets.Himself) {
			GameObject particleObjectClone = Instantiate(particleObject, currentTarget.GetPosition(position), currentTarget.transform.rotation) as GameObject;
		} else if(numberOfTargets == Global.NumberOfTargets.AllAlly) {
			foreach(Monster ally in battleManager.teamA) {
				GameObject particleObjectClone = Instantiate(particleObject, ally.GetPosition(position), ally.transform.rotation) as GameObject;
			}
		} else if(numberOfTargets == Global.NumberOfTargets.AllEnemy) {
			foreach(Monster enemy in battleManager.teamB) {
				GameObject particleObjectClone = Instantiate(particleObject, enemy.GetPosition(position), enemy.transform.rotation) as GameObject;
			}
		} else if(numberOfTargets == Global.NumberOfTargets.All) {
			foreach(Monster ally in battleManager.teamA) {
				GameObject particleObjectClone = Instantiate(particleObject, ally.GetPosition(position), ally.transform.rotation) as GameObject;
			}

			foreach(Monster enemy in battleManager.teamB) {
				GameObject particleObjectClone = Instantiate(particleObject, enemy.GetPosition(position), enemy.transform.rotation) as GameObject;
			}
		}
	}

	public void PlayAudioClip() {
		GetComponent<AudioSource>().PlayOneShot(currentAudioClip, 1f);
	}
}
