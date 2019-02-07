using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupButton : MonoBehaviour {

	public string Title;
	public string Text;
	public string Function;
	public int FunctionInt;
	public GameObject Manager;

	public void geklikt(){
		Manager.GetComponent<Mainmenu>().CreatePopup (this.gameObject, Manager.GetComponent<Mainmenu>(), Title, Text, FunctionInt, Function);
	}
}
