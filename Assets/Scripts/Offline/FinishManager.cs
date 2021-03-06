using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

public class FinishManager : MonoBehaviour {

	public GameObject MainCamera;
	public GameObject FinishedCamera;
	public GameObject EndUI;
	public GameObject[] UI;
	public GameObject[] FinishObjects;

	public int SceneID;
	public int TrackID;
	public Text EndTime;
	public Text BestTime;

	public Text MsgBest;

	public bool Ismultiplayer;
	public GameObject TimeTrialManager;
	public GameObject StartSystem;


	void OnDrawGizmos(){
		Gizmos.color = new Color(1, 01, 01, 0.5F);
		Gizmos.DrawCube(transform.position, transform.localScale);
	}
	public void MainMenu(){
		StartCoroutine (mainmenu ());
	}
	public void Endgame(){
		StartCoroutine (EndGame ());
	}
	public void Replay(){
		StartCoroutine (replay ());
	}
	IEnumerator mainmenu(){
		StartSystem.GetComponent<Startsystem> ().BeginFade (-1);
		yield return new WaitForSeconds (3f);		
		SceneManager.LoadScene (1, LoadSceneMode.Single);
	}
	IEnumerator replay(){
		StartSystem.GetComponent<Startsystem> ().BeginFade (-1);
		yield return new WaitForSeconds (3f);
		SceneManager.LoadScene (SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
	}
	IEnumerator EndGame(){
		StartSystem.GetComponent<Startsystem> ().BeginFade (1);
		for (int i = 0; i < UI.Length; i++) {
			UI[i].SetActive (false);
		}
		yield return new WaitForSeconds (2f);
		//MainCamera.SetActive(false);
		FinishedCamera.SetActive (true);
		EndUI.SetActive (true);
		for (int i = 0; i < FinishObjects.Length; i++) {
			if (!FinishObjects[i].activeSelf) {
				FinishObjects [i].SetActive (true);	
			}
		}

		EndTime.text = TimeTrialManager.GetComponent<TimeTrailManager> ().endTime.ToString("0.000");
		var TrackID = TimeTrialManager.GetComponent<TimeTrailManager> ().trackId;

		BestTime.text = PlayerPrefs.GetFloat ("Ghost-Time-"+TrackID, 1000000).ToString("0.000");

		if (EndTime.text == BestTime.text && TimeTrialManager.GetComponent<TimeTrailManager> ().bestTime != 1000000) {
				MsgBest.text = "You set a new personal record! Improvement: " + (TimeTrialManager.GetComponent<TimeTrailManager> ().bestTime - TimeTrialManager.GetComponent<TimeTrailManager> ().endTime).ToString ("0.000") + " Seconds!";
		}

		StartSystem.GetComponent<Startsystem> ().BeginFade (-1);

	}

	//------------------------------------------------------------------------------------------------------------
	//Online timetrial saving mechanics.
	//Uploading Timetrial times to the internet.
	//------------------------------------------------------
	//Ui Elements
	//------------------------------------------------------

	public GameObject PostOnlineUI;
	public Text Times;
	public Text yourtime;
	public Button postbutton;

	//------------------------------------------------------

	public void open_postonline(){
		PostOnlineUI.SetActive (true);
		EndUI.SetActive (false);
		StartCoroutine(GetScores ());
		yourtime.text = TimeTrialManager.GetComponent<TimeTrailManager> ().endTime.ToString ("0.000");
	}
	public void close_postonline(){
		PostOnlineUI.SetActive (false);
		EndUI.SetActive (true);
	}

	//------------------------------------------------------
	//Connections 
	//------------------------------------------------------
	public string username;

	//private string secretKey = "aloVMA7SQKkdCgO50CscOdTm48fdzsmX"; // Edit this value and make sure it's the same as the one stored on the server
	public string addScoreURL = "http://brucefj218.218.axc.nl/pixelracers/addscore.php?"; //be sure to add a ? to your url
	public string highscoreURL = "http://brucefj218.218.axc.nl/pixelracers/displayscore.php";

	public void SaveOnline()
	{
		StartCoroutine(PostScores());
		StartCoroutine(GetScores ());
	}

	// remember to use StartCoroutine when calling this function!
	IEnumerator PostScores()
	{
		string name = PlayerPrefs.GetString ("USERNAME");
		string post_url = "http://brucefj218.218.axc.nl/pixelracers/addscore.php?name="+name+"&time="+TimeTrialManager.GetComponent<TimeTrailManager> ().endTime.ToString ("0.000")+"&track="+TrackID+"&key=aloVMA7SQKkdCgO50CscOdTm48fdzsmX&token="+PlayerPrefs.GetString("TOKEN");

		WWW hs_post = new WWW(post_url);
		yield return hs_post;

		if (hs_post.error != null)
		{
			print("There was an error posting the high score: " + hs_post.error);
			Debug.Log (post_url);
		}
	}

	IEnumerator GetScores()
	{
		Times.text = "Loading Scores";
		WWW hs_get = new WWW("http://brucefj218.218.axc.nl/pixelracers/displayscore.php?track="+TrackID);
		yield return hs_get;

		if (hs_get.error != null)
		{
			print("There was an error getting the high score: " + hs_get.error);
		}
		else
		{
			Times.text = hs_get.text.Replace("<br>", "\n").Replace("               ", "");
		}
	}
}
