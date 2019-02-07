using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class startup_system : MonoBehaviour {

	public GameObject LoadingScreen;
	public GameObject Error;
	public Text 	  ErrorText;
	public GameObject DownloadButton;

	public Button ui_post_button;

	public GameObject ui_setname;
	public GameObject ui_error;

	[Header("Loading")]
	public GameObject loadingScreen;
	public Slider slider;
	public Text loadingText;
	public Text loadingInfo;

	void Start () {
		StartCoroutine (Start_Info ());
	
	}

	public void Retry() {
		SceneManager.LoadScene (SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
	}
	public void Download() {
		Application.OpenURL ("http://www.savannadevelopments.com/pixelracers");
	}
	public void start_setname() {
		StartCoroutine (set_player ());
	}

	IEnumerator Start_Info(){
		string url = "http://brucefj218.218.axc.nl/pixelracers/allowed.php";

		WWW s_url = new WWW(url);
		yield return s_url;

		string[] feedback;
		char[] splitters = { '-' };
		feedback = s_url.text.Split (splitters, System.StringSplitOptions.RemoveEmptyEntries);

		if (feedback [0] == "true") {
			loadingInfo.text = "Checking Connection.";
			Debug.Log ("True");
			if (feedback [1] == Application.version.ToString ()) {
				Debug.Log ("Versie Klopt");
				loadingInfo.text = "Checking Version.";
				if (PlayerPrefs.GetString ("USERNAME") == "") {
					ui_setname.SetActive (true);
				} else {
					loadingInfo.text = "Checking Player information.";
					if (PlayerPrefs.GetInt ("NAMESET") == 0) {
						Debug.Log ("Firts Start");
						PlayerPrefs.DeleteAll ();
						ui_setname.SetActive (true);
					} else {
						loadingInfo.text = "Loading game.";
						StartCoroutine (LoadAsync (1));
					}
				}
			} else {
				Debug.Log ("Versie Klopt Niet");
				Error.SetActive (true);
				ErrorText.text = "(ERR Code = 001) Your version number doesnt match up with the newest version. Please make sure to download the newest version down here: ";
				DownloadButton.SetActive (true);
				LoadingScreen.SetActive (false);
			}
		} else {
			Debug.Log ("False");
			Error.SetActive (true);
			ErrorText.text = "(ERR Code = 002) Beta versions arent allowed for now. Please make sure to play when betas are enabled. ";
			LoadingScreen.SetActive (false);
		}
	}
	public void SetName(string _name){
		PlayerPrefs.SetString ("USERNAME", _name);
		if (_name == "") {
			ui_post_button.interactable = false;
			ui_error.GetComponent<Text> ().text = "The username may not be empty.";
		} else if (_name.Contains ("-")) {
			ui_post_button.interactable = false;
			ui_error.GetComponent<Text>().text = "The username may not contains a '-'.";
		} else {
			ui_post_button.interactable = true;
			ui_error.GetComponent<Text> ().text = "";
		}
	}

	public IEnumerator set_player() {
		string name = PlayerPrefs.GetString ("USERNAME");
		string post_url = "http://brucefj218.218.axc.nl/pixelracers/addplayer.php?name="+name+"";

		WWW hs_player = new WWW(post_url);
		yield return hs_player;
	

		string[] plr_info = hs_player.text.Split ("-"[0]);

		if (plr_info [0].Replace("               ", "") == "err5") {
			ui_error.GetComponent<Text>().text = plr_info [1];
		} else {
			plr_info [0].Replace("               ", "");
			PlayerPrefs.SetString ("TOKEN", plr_info [1]);
			PlayerPrefs.SetInt ("PIXELCOUNT", int.Parse(plr_info [2]));
			PlayerPrefs.GetInt ("WINCOUNT", int.Parse(plr_info [3]));

			PlayerPrefs.SetInt ("NAMESET", 1);


		}

	}
	//Loading function wich also invokes the loading screen.
	IEnumerator LoadAsync (int sceneId) {
		AsyncOperation operation = SceneManager.LoadSceneAsync (sceneId, LoadSceneMode.Single);

		loadingScreen.SetActive (true);

		while (!operation.isDone) {
			float progress = Mathf.Clamp01 (operation.progress / 0.9f);

			slider.value = progress;
			loadingText.text = (progress * 100).ToString("0") + "%";

			yield return null;
		}
	}
}
