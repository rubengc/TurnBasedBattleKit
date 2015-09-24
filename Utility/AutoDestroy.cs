using UnityEngine;
using System.Collections;

public class AutoDestroy : MonoBehaviour {

	public float time;

	// Use this for initialization
	void Start () {
		Invoke("Destroy", time);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void Destroy() {
		Object.Destroy(this.gameObject);
	}
}
