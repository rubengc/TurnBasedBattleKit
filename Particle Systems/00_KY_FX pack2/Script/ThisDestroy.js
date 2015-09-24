#pragma strict
public var time:float = 1;
function Start () {
	Destroy(this.gameObject ,time);
	this.enabled = false;
}

function Update () {

}