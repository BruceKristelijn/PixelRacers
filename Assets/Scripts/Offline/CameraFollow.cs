using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

	public GameObject player;
	private Vector3 offset;

	void Start () {
		offset = transform.position - player.transform.position;
	}

	void LateUpdate () {
		transform.position = player.transform.position + offset;
		Vector3 temp = transform.rotation.eulerAngles;
		temp.y = player.transform.rotation.eulerAngles.y;
		transform.rotation = Quaternion.Euler(temp);
	}
}