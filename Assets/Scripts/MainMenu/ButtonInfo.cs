using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonInfo : MonoBehaviour {

	public int ButtonID;
	public GameObject Manager;
	public GameObject SceneManager;

	private bool Owned;

	public bool cars;

	public GameObject Lock;
	public Component[] MeshFilters;

	// Use this for initialization
	void Start () {
		if (cars) {
			Manager = GameObject.Find ("CarManager");
			Owned = Manager.GetComponent<CarManager> ().OwnedCars [ButtonID]; 
		} else {
			Manager = GameObject.Find ("CharManager");
			Owned = Manager.GetComponent<CharacterManager> ().characters [ButtonID].owned; 
		}
		SceneManager = GameObject.Find ("SceneManager");
		MeshFilters = this.GetComponentsInChildren<MeshFilter> ();
		foreach (MeshFilter joint in MeshFilters) {
			if (cars) {
				joint.mesh = Manager.GetComponent<CarManager> ().Cars [ButtonID];
			} else {
				joint.mesh = Manager.GetComponent<CharacterManager> ().characters [ButtonID].fullChar;
			}
		}

		if (!Owned) {
			Text[] PriceTXT = this.GetComponentsInChildren<Text> ();
			Lock.SetActive (true);
			foreach (Text joint in PriceTXT) {
				if (cars) {
					joint.text = Manager.GetComponent<CarManager> ().CarPrices [ButtonID].ToString ();
				} else {
					joint.text = Manager.GetComponent<CharacterManager> ().characters [ButtonID].price.ToString ();
				}
			}
		}
	}
	public void Geklikt(){
		if (Owned) {
			if (cars) {
				Manager.GetComponent<CarManager> ().SetCar (ButtonID);
			} else {
				Manager.GetComponent<CharacterManager> ().SetChar (ButtonID);
			}
		} else {
			if (cars) {
				SceneManager.GetComponent<Mainmenu> ().CreatePopup (this.gameObject, Manager.GetComponent<CarManager> (), "Buy Car", "Are you sure you want to buy the car " + Manager.GetComponent<CarManager> ().CarNames [ButtonID] + "? This will cost you " + Manager.GetComponent<CarManager> ().CarPrices [ButtonID] + " Pixels.", ButtonID, "CarBuy");
			} else {
				SceneManager.GetComponent<Mainmenu> ().CreatePopup (this.gameObject, Manager.GetComponent<CharacterManager> (), "Buy Car", "Are you sure you want to buy the character " + Manager.GetComponent<CharacterManager> ().characters [ButtonID].name + "? This will cost you " + Manager.GetComponent<CharacterManager> ().characters[ButtonID].price + " Pixels.", ButtonID, "CharBuy");
			}
		}
	}

	// Update is called once per frame
	void FixedUpdate () {
		if (cars) {
			Owned = Manager.GetComponent<CarManager> ().OwnedCars [ButtonID];
		} else {
			Owned = Manager.GetComponent<CharacterManager> ().characters [ButtonID].owned;
		}
		if (Owned) {
			Lock.SetActive (false);
			Text[] PriceTXT = this.GetComponentsInChildren<Text> ();
			foreach (Text joint in PriceTXT) {
				joint.text = "";
			}
		}
	}
}
