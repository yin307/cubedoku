using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

	public CubeController target;
	public bool move;
	float t = 0;

	void OnEnable(){
		Vector3 difference = target.transform.position - transform.position;
		float rotationZ = Mathf.Atan2 (difference.y, difference.x) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.Euler (0.0f, 0.0f, rotationZ);
	}
	// Update is called once per frame
	void Update () {		
		if (move) { 
			t += Time.deltaTime / 0.5f;
			transform.position = Vector3.Lerp(transform.position, target.gameObject.transform.position, t); 
			if(transform.position == target.gameObject.transform.position){
				move = false;
				target.onDamaged ();
			}
		}


	}
}
