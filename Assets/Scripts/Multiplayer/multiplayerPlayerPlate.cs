using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class multiplayerPlayerPlate : MonoBehaviour {

    public Text playerName;
    public Text playerNumber;
    public Text playerWins;
    public Image readyCircle;
    public GameObject kickButton;
    public GameObject readyPanel;
    public NetworkConnection plateConn;

    private void Start()
    {
        this.transform.SetParent(GameObject.Find("LobbyPlayerList").transform, false);
        if(plateConn == null)
        {
            kickButton.SetActive(false);
        }
    }
}
