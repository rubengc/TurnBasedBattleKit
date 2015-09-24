#pragma strict

public var onceFlg:boolean = false;
public var offsetAnimeYFlg:boolean = false;
public var thisMaterial:Material;
public var offsetSpd:float = 1;
public var animeSpeed:float = 30.0f;
public var uvX:int = 0;  
public var uvY:int = 0; 
public var initOffset:Vector2;
public var delay:float = 0;
public var delayBck:float = 0;

private var timer:float;

function Start(){
	thisMaterial = this.GetComponent.<Renderer>().material;
	delayBck = delay;
}

function Update () { 
	if(delay <= 0){
		timer += Time.deltaTime;
	}else{
		delay -= Time.deltaTime;
	}
	
	if( offsetAnimeYFlg ){
		thisMaterial.mainTextureOffset.y += Time.deltaTime * offsetSpd;
		if( thisMaterial.mainTextureOffset.y > 1)thisMaterial.mainTextureOffset.y = 0;
	}else{
		if( !this.GetComponent.<ParticleSystem>().isPlaying ){
			thisMaterial.SetTextureOffset ("_MainTex", Vector2(0 ,0) );
			thisMaterial.SetTextureScale ("_MainTex", Vector2(0 ,0));
			
			timer = 0;
			initOffset = Vector2.zero;
			delay = delayBck;
		}else{
			var index : int = timer * animeSpeed;
		
			index = index % (uvX * uvY);
		
			var size = Vector2 (1.0 / uvX, 1.0 / uvY);
		
			var uIndex = index % uvX;
			var vIndex = index / uvX;
		 	var offset = Vector2 (uIndex * size.x, 1.0 - size.y - vIndex * size.y);
		 	
		 	if( onceFlg ){
			 	if( initOffset.magnitude == 0 && index == (uvX * uvY)-1){
			 		initOffset = offset;
			 		//Debug.Log("index " + index);
			 	}else if(initOffset.magnitude != 0){
			 		offset = initOffset;
			 	}
		 	}
		 	
			thisMaterial.SetTextureOffset ("_MainTex", offset);
			thisMaterial.SetTextureScale ("_MainTex", size);
		}
	}
}