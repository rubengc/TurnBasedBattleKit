using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {
	public float speed;
	public GameObject onHitObject;
	public AudioClip onHitSFX;
	public CharacterController controller;

	public Transform target;

	void Start () {
		
	}
	
	void Update () {
		if(target != null) {
			Vector3 targetPosition = target.position;
			Renderer targetRenderer = target.gameObject.GetComponentInChildren<Renderer>();
			targetPosition.y += targetRenderer.bounds.size.y/2;

			if(Vector3.Distance(targetPosition, transform.position) > 3) {
				Vector3 dir = targetPosition - transform.position;
				float dist = dir.magnitude;       
				float move = speed * Time.deltaTime;
				
				
				if(dist > move) {
					controller.Move(dir.normalized * move);
				} 
				else {
					controller.Move(dir);
				}
				
				// Rotate to target
				dir.y = 0;
				transform.forward = Vector3.Slerp(transform.forward, dir, Time.deltaTime * 2);      
			} else {
				Instantiate(onHitObject, transform.position, transform.rotation);

				if(onHitSFX != null)
					AudioSource.PlayClipAtPoint(onHitSFX, transform.position, 1.0F);

				Object.Destroy(this.gameObject);
			}
		}
	}
}
