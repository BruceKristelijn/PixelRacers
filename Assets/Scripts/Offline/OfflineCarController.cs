using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class OfflineCarController : MonoBehaviour {

	[Header("Movement")]
	public WheelCollider frontLeftWheel;
	public WheelCollider frontRightWheel;
	public WheelCollider backLeftWheel;
	public WheelCollider backRightWheel;

	public float maxMotorPower = 100;
	public float maxSteerAngle = 30;

	[Header("Debug Settings")]
	public VirtualJoystick joystick;
	public GameObject buttons;

	private float gassSpeed;
	private float steer;

	[Header("Respawn Settings")]
	public Vector3 RespawnPosition;
	public Vector3 RespawnRotation;

	public int OnTriggerID;
	public bool isFinished;
	public int CurrentLap;
	public int LapAmount;
	public GameObject[] Trigger;

	public Text LapText;

	[Header("Audio")]
	public AudioSource Engine;
	public AudioSource LapSound;
	public AudioSource FinishSound;

	// Use this for initialization
	void Start () {
		//Get lap amount
		//----------------------------------------
		Trigger = GameObject.FindGameObjectsWithTag("RespawnTrigger");
		for (int i = 0; i < Trigger.Length; i++){
			if (Trigger [i].GetComponent<Trigger> ().isFinish == true) {
				LapAmount = Trigger [i].GetComponent<Trigger> ().Racelaps;
				break;
			}
		}
	}

	void FixedUpdate() {
		if (PlayerPrefs.GetInt ("Sr_Type") > 2) {
			PlayerPrefs.SetInt ("Sr_Type", 0);
		}
		float motor = maxMotorPower * gassSpeed;

		if (PlayerPrefs.GetInt ("Sr_Type", 0) == 0) {
			float steering = maxSteerAngle * joystick.Horizontal ();
			joystick.gameObject.SetActive (true);
			buttons.gameObject.SetActive (false);
			frontLeftWheel.steerAngle = steering;
			frontRightWheel.steerAngle = steering;
		}

		if (PlayerPrefs.GetInt ("Sr_Type") == 1) {
			joystick.gameObject.SetActive (false);
			buttons.gameObject.SetActive (false);
			float steering = maxSteerAngle * Input.acceleration.x;
			frontLeftWheel.steerAngle = steering;
			frontRightWheel.steerAngle = steering;
		}

		if (PlayerPrefs.GetInt ("Sr_Type") == 2) {
			joystick.gameObject.SetActive (false);
			buttons.gameObject.SetActive (true);
			float steering = maxSteerAngle * steer;
			frontLeftWheel.steerAngle = steering;
			frontRightWheel.steerAngle = steering;
		}

		backLeftWheel.motorTorque = motor;
		backRightWheel.motorTorque = motor;

		//Wheel Turning
		TurnWheel(frontLeftWheel);
		TurnWheel(frontRightWheel);
		TurnWheel(backLeftWheel);
		TurnWheel(backRightWheel);

		var rotationVector = transform.rotation.eulerAngles;
		rotationVector.z = 0;
		transform.rotation = Quaternion.Euler(rotationVector);

		LapText.text = CurrentLap.ToString() + "/" + LapAmount.ToString();
	}

	// Update is called once per frame
	void Update () {

		//Movement
		//----------------------------------------
		Vector3 dir = Vector3.zero;
		dir.x = joystick.Horizontal ();

	}

	public void gasPress (){
		gassSpeed = -1;
	}

	public void brakePress (){
		gassSpeed = 0.5f;
	}

	public void gasRelease (){
		gassSpeed = 0;
	}

	public void leftPress (){
		steer = -1;
	}

	public void rightPress  (){
		steer = 1;
	}

	public void buttonRelase  (){
		steer = 0;
	}


	void TurnWheel(WheelCollider collider) {
		Transform wheel = collider.transform.GetChild (0);
		Vector3 position;
		Quaternion rotation;
		collider.GetWorldPose (out position, out rotation);

		wheel.transform.position = position;
		wheel.transform.rotation = rotation;
	}

	void OnTriggerEnter(Collider other){
		if (other.tag == "Itembox") {
			StartCoroutine (ItemboxManager (other.gameObject));
		}
		if (other.tag == "Void") {
			StartCoroutine (Outofmap ());;
		}if (other.tag == "Void") {
			StartCoroutine (Outofmap ());
		}
		if (other.tag == "RespawnTrigger") {
			RespawnPosition = other.GetComponent<Trigger> ().Position;
			RespawnRotation = other.GetComponent<Trigger> ().Rotation;

			if (other.GetComponent<Trigger> ().isFinish) {
				if (OnTriggerID == other.GetComponent<Trigger> ().TriggerAmount) {
					if (CurrentLap == other.GetComponent<Trigger> ().Racelaps) {
						Debug.Log ("Finish");
						LapText.text = "Finished!";
						FinishSound.Play ();
						isFinished = true;
					} else {
						CurrentLap += 1;
						OnTriggerID = 0;
						Debug.Log ("On Lap" + CurrentLap);
						LapText.text = CurrentLap.ToString() + "/" + LapAmount.ToString();
						LapSound.Play ();
					}

				}
			} else {
				if (OnTriggerID == (other.GetComponent<Trigger> ().TriggerID - 1)) {
					OnTriggerID += 1;
					Debug.Log (OnTriggerID);
				}
			}
		}
	}

	private IEnumerator Outofmap (){
		Debug.Log ("Gevallen");
		GameObject.Find ("Player Cam Pivot").GetComponent<CameraFollow> ().enabled = false;
		yield return new WaitForSeconds (1);
		transform.position = RespawnPosition;
		transform.eulerAngles = RespawnRotation;
		Debug.Log ("Terug");
		GameObject.Find ("Player Cam Pivot").GetComponent<CameraFollow> ().enabled = true;
		yield return new WaitForSeconds (1);
	}

	public IEnumerator ItemboxManager (GameObject other){
		other.gameObject.SetActive (false);
		yield return new WaitForSeconds (5f);
		other.gameObject.SetActive (true);
	}
}