using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCostumizer : MonoBehaviour {
	[Header("Editor Objects")]
	public GameObject CurrentCharObj;
	public GameObject CurrentCharTXT;

	[Header("Updateable")]
	public Mesh CurrentCharFull;
	public Mesh CurrentCharHalf;

	public Mesh[] CharsFull;
	public Mesh[] CharsHalf;
	public string[] CharNames;

	public bool[] OwnedChars;
	public int[] CharPrices;
	private string[] OwnedcharsS;

	public string WriteTo;

	void Start () {
		CurrentCharFull = CharsFull [PlayerPrefs.GetInt ("CURRENTCHARID", 0)];
		CurrentCharHalf = CharsHalf [PlayerPrefs.GetInt ("CURRENTCHARID", 0)];
		CurrentCharObj.GetComponent<MeshFilter> ().mesh = CurrentCharFull;
		CurrentCharTXT.GetComponent<Text> ().text = CharNames [PlayerPrefs.GetInt ("CURRENTCHARID", 0)];
		//Ownedcars = PlayerPrefs.GetInt ("OWNEDCARS", 0);


	}
	void FixedUpdate(){
		CurrentCharFull = CharsFull [PlayerPrefs.GetInt ("CURRENTCHARID", 0)];
		CurrentCharHalf = CharsHalf [PlayerPrefs.GetInt ("CURRENTCHARID", 0)];
		if (CurrentCharObj != null && CurrentCharTXT != null) {
			CurrentCharObj.GetComponent<MeshFilter> ().mesh = CurrentCharFull;
			CurrentCharTXT.GetComponent<Text> ().text = CharNames [PlayerPrefs.GetInt ("CURRENTCHARID", 0)];
		}
		//Ownedcars = PlayerPrefs.GetInt ("OWNEDCARS", 0);
	}
	private void ReadOwnedChars(){
		char[] splitters = { '-' };
		//string[] OwnedcarsS; 
		OwnedcharsS = (PlayerPrefs.GetString ("OWNEDCHARS").Split(splitters, System.StringSplitOptions.RemoveEmptyEntries));

		for (int i = 0; i < OwnedcharsS.Length; i++) {
			OwnedChars [i] = (OwnedcharsS [i] == "True") ? true : false;
		}
	}

	public void SetChar(int CharId){
		Debug.Log ("Car set, ID: " + CharId);
		PlayerPrefs.SetInt("CURRENTCHARID", CharId);
		PlayerPrefs.Save ();
	}
	public void BuyChar(int CharId){
		Debug.Log ("Buying Car: " + CharNames [CharId] + "Cost: "+ CharPrices[CharId]);

		if ((PlayerPrefs.GetInt ("PIXELCOUNT") - CharPrices [CharId]) >= 0) {
			UpdateOwnedChars (CharId);
			PlayerPrefs.SetInt ("PIXELCOUNT", (PlayerPrefs.GetInt ("PIXELCOUNT") - CharPrices [CharId]));

			GameObject.Find("SceneManager").GetComponent<Mainmenu>().Pixelcount.text = PlayerPrefs.GetInt("PIXELCOUNT", 0).ToString ();
			GameObject.Find("SceneManager").GetComponent<Mainmenu>().ClosePopup ();
			Debug.Log (PlayerPrefs.GetInt ("PIXELCOUNT", 0));
		} else {
			Debug.Log ("You Cannot affort this car yet.");
			GameObject.Find("SceneManager").GetComponent<Mainmenu>().CreatePopup (this.gameObject, this.GetComponent<CharacterCostumizer>(), "Oops!", "You cannot affort this car yet! ");
		}

	}
	public void UpdateOwnedChars(int CharID){
		OwnedChars [CharID] = true;

		WriteTo = "";

		for (int i = 0; i < OwnedChars.Length; i++) {
			WriteTo = WriteTo + OwnedChars [i].ToString () + "-";
		}

		PlayerPrefs.SetString ("OWNEDChARS", WriteTo);
		ReadOwnedChars ();
	}
}
