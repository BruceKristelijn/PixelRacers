using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Startsystem : MonoBehaviour {

	public GameObject Player;
	public GameObject[] MainUI;
	public GameObject Timetrialmanager;

	public GameObject[] CamSpots;

	public bool Playerready = false;
	public bool readytostart = false;
	public bool started = false; 

	public float speed;

	public AudioSource CountDownSound;
	public AudioSource Backgroundtrack;
	public Text CountDownText;

	public bool Offline = true;

	public float currentTime;
	private Vector3 StartPosition;
	private Quaternion StartRotation;

	private int x = 0;
	private bool moving = true;

	public Slider Volume;

	void Start () {
		AudioListener.volume = PlayerPrefs.GetFloat("SavedVolume", 1);
		Volume.value = AudioListener.volume;

		Player.transform.parent.gameObject.GetComponentInChildren<CameraFollow> ().enabled = false;
		StartPosition = this.transform.position;
		StartRotation = this.transform.rotation;
		this.transform.position = CamSpots [0].transform.position;
		StartCoroutine (CountDown ());
		BeginFade (-1);
		for (int i = 0; i < MainUI.Length; i++) {
			MainUI[i].SetActive (false);
		}
		if(Offline){
			Player.GetComponent<OfflineCarController> ().enabled = false;
		}
	}
	void Update ()
	{
		float step = speed * Time.deltaTime;
		float rotate = 1 * Time.deltaTime;

		int max = CamSpots.Length;

		if (moving) {
			transform.position = Vector3.MoveTowards (this.transform.position, CamSpots [(x + 1)].transform.position, step);
			transform.rotation = Quaternion.Lerp (this.transform.rotation, CamSpots [(x + 1)].transform.rotation, rotate);
		}
	}

	IEnumerator CountDown(){
		yield return new WaitForSeconds(3f);
		BeginFade (1);
		yield return new WaitForSeconds(0.5f);
		moving = false;
		x++;
		x++;
		this.transform.position = CamSpots [x].transform.position;
		this.transform.rotation = CamSpots [x].transform.rotation;
		yield return new WaitForSeconds(0.5f);
		moving = true;
		BeginFade (-1);
		yield return new WaitForSeconds(3f);
		BeginFade (1);
		yield return new WaitForSeconds (0.5f);
		moving = false;
		x++;
		x++;
		this.transform.position = CamSpots [x].transform.position;
		this.transform.rotation = CamSpots [x].transform.rotation;
		yield return new WaitForSeconds(0.5f);
		moving = true;
		BeginFade (-1);
		yield return new WaitForSeconds(5f);
		moving = false;
		this.transform.position = StartPosition;
		this.transform.rotation = StartRotation;

		this.GetComponentInParent<CameraFollow>().enabled = true;

		Backgroundtrack.volume = 0.3f;
		for (int i = 0; i < MainUI.Length; i++) {
			MainUI[i].SetActive (true);
		}
		Playerready = true;
		if (!Offline) {
			while (readytostart) {

			}
		}
		yield return new WaitForSeconds(2f);
		CountDownSound.Play ();
		CountDownText.text = "3";
		CountDownText.GetComponent<Animator>().SetBool("countdown", true);
		yield return new WaitForSeconds(1f);
		CountDownText.text = "2";
		yield return new WaitForSeconds(1f);
		CountDownText.text = "1";
		yield return new WaitForSeconds(1f);
		if(Offline){
			Player.GetComponent<OfflineCarController> ().enabled = true;
		}
		started = true;
		CountDownText.text = "Go!";
		yield return new WaitForSeconds(1f);
		CountDownText.text = "";
		CountDownText.GetComponent<Animator>().SetBool("countdown", false);
	}

	public Texture2D fadeOutTexture;	// the texture that will overlay the screen. This can be a black image or a loading graphic
	public float fadeSpeed = 1f;		// the fading speed

	private int drawDepth = -1000;		// the texture's order in the draw hierarchy: a low number means it renders on top
	private float alpha = 1.0f;			// the texture's alpha value between 0 and 1
	private int fadeDir = -1;			// the direction to fade: in = -1 or out = 1

	void OnGUI()
	{
		// fade out/in the alpha value using a direction, a speed and Time.deltaTime to convert the operation to seconds
		alpha += fadeDir * fadeSpeed * Time.deltaTime;
		// force (clamp) the number to be between 0 and 1 because GUI.color uses Alpha values between 0 and 1
		alpha = Mathf.Clamp01(alpha);

		// set color of our GUI (in this case our texture). All color values remain the same & the Alpha is set to the alpha variable
		GUI.color = new Color (GUI.color.r, GUI.color.g, GUI.color.b, alpha);
		GUI.depth = drawDepth;																// make the black texture render on top (drawn last)
		GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeOutTexture);		// draw the texture to fit the entire screen area
	}

	// sets fadeDir to the direction parameter making the scene fade in if -1 and out if 1
	public float BeginFade (int direction)
	{
		fadeDir = direction;
		return (fadeSpeed);
	}
}
	
