using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;

public class TimeTrailManager : MonoBehaviour {

	List<Vector3> Positions = new List<Vector3>();
	List<Vector3> Rotations = new List<Vector3>();

	Vector3[]    loadedPositions;
	Vector3[]    loadedRotations;

    [Header("Information about different tracks")]
    public trackInfo[] tracks;

    [Header("Information about current track")]
    public int          trackId;
	public string       trackName;

    [Header("Player and Ghost")]
    public GameObject player;
	public GameObject Ghost;
	public MeshFilter GhostCar;
	public MeshFilter GhostChar;

    [Header("Timing")]
    public float currentTime;
	public float bestTime;
	public float endTime;

    [Header("UI")]
    public Text lapTime;
	public GameObject settingsCanvas;
	public GameObject gameplayCanvas;

    [Header("Loading UI")]
    public GameObject loadingCanvas;
    public Slider loadingSlider;
    public Text loadingText;

    bool TrialisFinished;
    bool TimeSet;
    bool settingstrue;

    //Do something while loading.
    void Awake()
    {
        trackId = PlayerPrefs.GetInt("tt-chosentrack");
        var track = tracks[trackId];

        trackName = track.name;
        while (!loadTrack())
        {
            Debug.Log("Spawning Track");
        }

        loadingCanvas.SetActive(false);
    }
    //Function to load track info and Spawn the track.
    bool loadTrack()
    {
        var track = tracks[trackId];

        track.trackObject.SetActive(true);

        player.transform.position = track.startPos;
        var startRotation = Quaternion.Euler(0, track.startRotation, 0);
        player.transform.rotation = startRotation;

        Camera.main.GetComponent<Skybox>().material = track.skyBox;

        return true;                                                                //If all goes correct return true so the script continues.
    }
    //Function run to start both coroutines to playback and record the gameplay.
	/*public void StartPlayback(){
		currentTime = Time.time;
		inGameSceneManager = GameObject.Find ("Player1");

		loadedPositions = PlayerPrefsX.GetVector3Array("Positions-"+trackId);
		loadedRotations = PlayerPrefsX.GetVector3Array("Rotations-"+trackId);

		bestTime = PlayerPrefs.GetFloat ("Ghost-Time-"+trackId, 1000000);

		GhostCar.mesh = carManager.GetComponent<CarManager>().Cars[PlayerPrefs.GetInt ("ghost-carID-" + trackId)];
		GhostChar.mesh = characterManager.GetComponent<CharacterCostumizer> ().CharsHalf [PlayerPrefs.GetInt ("ghost-charID-" + trackId)];

		StartCoroutine (Replay ());
		StartCoroutine (Record ());
	}*/


    //Function for showing the motion though a ghost.
	IEnumerator Replay(){
		for (int i = 0; i < loadedPositions.Length; i++) {
			//New position
			Ghost.transform.position = loadedPositions [i];
			Ghost.transform.eulerAngles = loadedRotations [i];

			yield return new WaitForSeconds (0.005f);
		}
	}
    //Function run to record player actions.
	IEnumerator Record ()
	{
		while (!TrialisFinished) {
			Positions.Add (player.transform.position);
			Rotations.Add (player.transform.eulerAngles);
			yield return new WaitForSeconds (0.005f);
		}
	}
    //Function for turning on and off the settings screen.
	public void settings(){
		if (!settingstrue) {
			Time.timeScale = 0;
			settingsCanvas.SetActive (true);
			settingstrue = true;
		} else {
			Time.timeScale = 1;
			settingsCanvas.SetActive (false);
			settingstrue = false;
		}
	}
    //Funtion for changing the volume through a slider in the settings menu.
	public void Settings_Volume(float volume){
		AudioListener.volume = volume;
		PlayerPrefs.SetFloat("SavedVolume", volume);
	}
    //Function for going back to the main menu.
	public void load_MainMenu(){
		Time.timeScale = 1;                                             //Set the timescale back to 1 to unpause.
        StartCoroutine(LoadAsync(1));                                   //Call the function to load the scene with a loading screen.
    }
    //Function for restarting the timetrial.
	public void Restart(){
		Time.timeScale = 1;
        StartCoroutine(LoadAsync(SceneManager.GetActiveScene().buildIndex));
	}
    //Function for changing the controll scheme.
	public void setcontroll(int controlltype){
		PlayerPrefs.SetInt ("Sr_Type", controlltype);
	}
    //function to start a loading screen.
    IEnumerator LoadAsync(int sceneId)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneId, LoadSceneMode.Single);
        loadingCanvas.SetActive(true);

        var slider = loadingSlider;

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            slider.value = progress;
            loadingText.text = (progress * 100).ToString("0") + "%";

            yield return null;
        }
    }
    //Save the players race if the time is faster than before.
    IEnumerator SavePosition()
    {

        endTime = (endTime - currentTime);

        if (endTime < bestTime && !TimeSet)
        {
            TimeSet = true;
            PlayerPrefs.SetFloat("Ghost-Time-" + trackId, endTime);

            Debug.Log("Convert to array");
            Vector3[] SavePositions = Positions.ToArray();
            Vector3[] SaveRotations = Rotations.ToArray();

            Debug.Log("Save to array");
            PlayerPrefsX.SetVector3Array("Positions-" + trackId, SavePositions);
            PlayerPrefsX.SetVector3Array("Rotations-" + trackId, SaveRotations);
            Debug.Log("Save car ID");
            PlayerPrefs.SetInt("ghost-carID-" + trackId, PlayerPrefs.GetInt("CURRENTCARID"));
            PlayerPrefs.SetInt("ghost-charID-" + trackId, PlayerPrefs.GetInt("CURRENTCHARID"));

        }
        GameObject.Find("Player Cam Pivot").GetComponent<CameraFollow>().enabled = false;
        yield return new WaitForSeconds(2f);
        GameObject.Find("FinishManager").GetComponent<FinishManager>().Endgame();
    }
}

//A class containing information about the different tracks.
[System.Serializable]
public class trackInfo
{
    public string name;
    public int laps;
    public GameObject trackObject;
    public Vector3 startPos;
    public int startRotation;
    public Material skyBox;
}