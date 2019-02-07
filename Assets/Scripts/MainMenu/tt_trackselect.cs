using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class tt_trackselect : MonoBehaviour {

	public string track_name;
	//TextAreaAttribute(int minLines, int maxLines);
	[TextArea(15,20)]
	public string track_info;
	public Sprite track_image;
	public int track_id;
	public int track_sceneId;

	public void Click(){
		GameObject.Find ("SceneManager").GetComponent<Mainmenu> ().tt_showinfo (track_name,track_info,track_image,track_id,track_sceneId);
	}
}
