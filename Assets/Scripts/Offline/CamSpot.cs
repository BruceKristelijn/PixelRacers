using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamSpot : MonoBehaviour {

	void Start () {
		
	}
	
	void OnDrawGizmos(){
		Gizmos.color = new Color(5, 0, 0, 0.5F);
		Gizmos.DrawCube(transform.position, transform.localScale);
	}
}
