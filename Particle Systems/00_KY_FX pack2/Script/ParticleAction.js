#pragma strict
private var thisTrs:Transform;
public var setRot:Vector3;
public var randomFlg:boolean = false;
public var rotOffFlg:boolean = false;

public var moveOnFlg:boolean = false;

public var moveRndFlg:boolean = false;
public var spdVec3:Vector3;

public var speed:float = 5;

//public var stopTime:float = 0;

function Start () {
	thisTrs = this.transform;
	if( randomFlg ){
		setRot.x = Random.Range(-setRot.x, setRot.x );
		setRot.y = Random.Range(-setRot.y, setRot.y );
		setRot.z = Random.Range(-setRot.z, setRot.z);
	}
	/*if( stopTime > 0){
		yield WaitForSeconds( stopTime );
		this.enabled = false;
	}*/
}

function Update () {
	if( !rotOffFlg )thisTrs.Rotate( Vector3(setRot.x * Time.deltaTime ,setRot.y * Time.deltaTime ,setRot.z * Time.deltaTime) );
	
	if( moveRndFlg ){
		thisTrs.Translate( Vector3(spdVec3.x * Time.deltaTime ,spdVec3.y * Time.deltaTime,spdVec3.z * Time.deltaTime) );
		//transform.position = new Vector3( Mathf.Abs( Mathf.Sin( Time.time * spdVec3.x ))  , Mathf.Abs( Mathf.Sin( Time.time * spdVec3.y )) , Mathf.Abs( Mathf.Sin( Time.time * spdVec3.z ))  );
	}else if(moveOnFlg ){
		thisTrs.Translate( Vector3(0,0,Time.deltaTime * speed) );
	}
}