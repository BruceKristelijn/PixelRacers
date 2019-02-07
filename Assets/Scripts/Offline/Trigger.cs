using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour {

	public Vector3 Position;
	public Vector3 Rotation;
	public int TriggerID;
	public bool isFinish;
	public int TriggerAmount;
	public int Racelaps;
	public AudioClip BackgroundMusic;

	void Start(){
		Position = this.transform.position;
	}

	void OnDrawGizmos(){
		Gizmos.color = new Color(1, 0, 0, 0.5F);
		if (isFinish) {
			Gizmos.color = new Color(0, 0, 1, 0.5F);
		}
		Gizmos.DrawCube(transform.position, transform.localScale);
	}
}