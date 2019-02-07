using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarManager : MonoBehaviour {
	[Header("Editor Objects")]
	public GameObject CurrentCarObj;
	public GameObject CurrentCarTXT;

	[Header("Updateable")]
	public Mesh CurrentCar;

	public Mesh[] Cars;
	public Vector3[] FrontWheelloc;
	public Vector3[] Backhweelloc;

	public string[] CarNames;

	public bool[] OwnedCars;
	public int[] CarPrices;
	private string[] OwnedcarsS;

	public string WriteTo;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    void Start (){
		CurrentCar = Cars [PlayerPrefs.GetInt ("CURRENTCARID", 0)];
		CurrentCarObj.GetComponent<MeshFilter> ().mesh = CurrentCar;
		CurrentCarTXT.GetComponent<Text> ().text = CarNames [PlayerPrefs.GetInt ("CURRENTCARID", 0)];

		ReadOwnedCars ();
	}
	void FixedUpdate(){
		CurrentCar = Cars [PlayerPrefs.GetInt ("CURRENTCARID", 0)];
		if(CurrentCarObj != null && CurrentCarTXT != null){
		CurrentCarObj.GetComponent<MeshFilter> ().mesh = CurrentCar;
		CurrentCarTXT.GetComponent<Text> ().text = CarNames [PlayerPrefs.GetInt ("CURRENTCARID", 0)];
		//Ownedcars = PlayerPrefs.GetInt ("OWNEDCARS", 0);
		}
	}
	private void ReadOwnedCars(){
		char[] splitters = { '-' };
		//string[] OwnedcarsS; 
		OwnedcarsS = (PlayerPrefs.GetString ("OWNEDCARS").Split(splitters, System.StringSplitOptions.RemoveEmptyEntries));

		for (int i = 0; i < OwnedcarsS.Length; i++) {
			OwnedCars [i] = (OwnedcarsS [i] == "True") ? true : false;
		}
	}

	public void SetCar(int CarId){
		Debug.Log ("Car set, ID: " + CarId);
		PlayerPrefs.SetInt("CURRENTCARID", CarId);
		PlayerPrefs.Save ();
	}
	public void BuyCar(int CarId){
		Debug.Log ("Buying Car: " + CarNames [CarId] + "Cost: "+ CarPrices[CarId]);

		if ((PlayerPrefs.GetInt ("PIXELCOUNT") - CarPrices [CarId]) >= 0) {
			UpdateOwnedCars (CarId);
			PlayerPrefs.SetInt ("PIXELCOUNT", (PlayerPrefs.GetInt ("PIXELCOUNT") - CarPrices [CarId]));

			GameObject.Find("SceneManager").GetComponent<Mainmenu>().Pixelcount.text = PlayerPrefs.GetInt("PIXELCOUNT", 0).ToString ();
			GameObject.Find("SceneManager").GetComponent<Mainmenu>().ClosePopup ();
			Debug.Log (PlayerPrefs.GetInt ("PIXELCOUNT", 0));
		} else {
			Debug.Log ("You Cannot affort this car yet.");
			GameObject.Find("SceneManager").GetComponent<Mainmenu>().CreatePopup (this.gameObject, this.GetComponent<CarManager>(), "Oops!", "You cannot affort this car yet! ");
		}

	}
	public void UpdateOwnedCars(int CarID){
		OwnedCars [CarID] = true;

		WriteTo = "";

		for (int i = 0; i < OwnedCars.Length; i++) {
			WriteTo = WriteTo + OwnedCars [i].ToString () + "-";
		}

		PlayerPrefs.SetString ("OWNEDCARS", WriteTo);
		ReadOwnedCars ();
	}
}
