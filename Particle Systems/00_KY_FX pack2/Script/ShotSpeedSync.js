#pragma strict
public var offset:int = 10;
private var parentParticle:ParticleSystem;

function Start () {
	parentParticle = transform.parent.GetComponent.<ParticleSystem>();
	var parentSpeed:float = parentParticle.startSpeed;
	var thisSpeed:float = GetComponent.<ParticleSystem>().startSpeed;
	
	//Debug.Log(parentSpeed);
	if( parentSpeed >= thisSpeed){
		var sum:float = parentSpeed - thisSpeed;
		if( sum - offset > 0){
			while( sum - offset > 0){
				thisSpeed++;
				sum = parentSpeed - thisSpeed;
			}
		}
		GetComponent.<ParticleSystem>().startSpeed = thisSpeed;
	}
	
	//Debug.Log(particleSystem.startSpeed);
	//this.enabled = false;
}
