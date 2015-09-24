using UnityEngine;
using System;
using System.Collections;

public class MonsterAnimator : MonoBehaviour {

    [Serializable]
    public class AnimationClips {
        public AnimationClip idle;
        public AnimationClip randomIdle; // If is set, will be played randomly on actions selection
        public AnimationClip moveForward;
        public AnimationClip moveBackward;
        public AnimationClip physicalAttack;
        public AnimationClip specialAttack;
        public AnimationClip takeDamage;
        public AnimationClip dead;
    }

    public AnimationClips animationClips = new AnimationClips();

    private Monster monster;
    private Animation animation;

    private float idleSwitchDelay; // A random time to play RandomIdle animation

    void Start () {
        monster = GetComponent<Monster>();
        
        animation = monster.monsterGameObject.GetComponent<Animation>();

        // Initialize monster animator
        if(animationClips.idle != null)
            animation.AddClip(animationClips.idle, "Idle");

        if(animationClips.randomIdle != null) {
            animation.AddClip(animationClips.randomIdle, "RandomIdle");

            idleSwitchDelay = (Length("Idle") * UnityEngine.Random.Range(4, 8)) + Time.time;
        }

        if(animationClips.moveForward != null)
            animation.AddClip(animationClips.moveForward, "MoveForward");
        if(animationClips.moveBackward != null)
            animation.AddClip(animationClips.moveBackward, "MoveBackward");
        if(animationClips.physicalAttack != null)
            animation.AddClip(animationClips.physicalAttack, "PhysicalAttack");
        if(animationClips.specialAttack != null)
            animation.AddClip(animationClips.specialAttack, "SpecialAttack");
        if(animationClips.takeDamage != null)
            animation.AddClip(animationClips.takeDamage, "TakeDamage");
        if(animationClips.dead != null)
            animation.AddClip(animationClips.dead, "Dead");

        Play("Idle");
    }
	
	void Update () {
        if(monster.state == Global.State.Dead) return;

        // Plays RandomIdle animation
        if(idleSwitchDelay + 0.5 < Time.time && !monster.IsAttacking() && animationClips.randomIdle != null) {
            if(monster.battleManager.GetPhase() == Global.BattlePhase.TeamASelection || monster.battleManager.GetPhase() == Global.BattlePhase.TeamBSelection) {
                PlayQueued("RandomIdle");
            }

            idleSwitchDelay = (Length("Idle") * UnityEngine.Random.Range(4, 8)) + Time.time;
        }

        if(monster.IsMovingToTarget())
            Play("MoveForward");
    }

    public void Play(string animation) {
        if(this.animation[animation])
            this.animation.Play(animation);
    }

    public void PlayQueued(string animation, string queuedAnimation = "Idle") {
        Play(animation);

        if(this.animation[queuedAnimation])
            this.animation.PlayQueued(queuedAnimation, QueueMode.CompleteOthers);
    }

    public void PlayQueued(string animation, string[] queuedAnimations) {
        Play(animation);

        foreach(string queuedAnimation in queuedAnimations) {
            if(this.animation[queuedAnimation])
                this.animation.PlayQueued(queuedAnimation, QueueMode.CompleteOthers);
        }
    }

    public void PlayDead() {
        if(animation["Dead"]) {
            animation.Play("Dead", PlayMode.StopAll);
            animation.wrapMode = WrapMode.ClampForever;
        }
    }

    public float Length(string animation) {
        if(this.animation[animation])
            return this.animation[animation].length;
        else
            return 0f;
    }
}
