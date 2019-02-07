using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;


public class MultiplayerPlayerManager : NetworkBehaviour
{
    public Behaviour[] disabledcomponents;
    public GameObject[] disabledObjects;

    public int connections;
    [SyncVar (hook = "onReadyChange")] public bool ready = false;
    [SyncVar] public string playername;
    [SyncVar] public int wins;

    [SyncVar(hook = "spawnCar")] public int trackID;
    [SyncVar] public int spawnID;
    public GameObject carPrefab;
    public int laps;
    //public int 

    public bool inLobby;
    public bool inTrackSelect;
    public bool inGame;
    public bool inEndScreen;

    GameObject Manager;
    GameObject Lobby;
    GameObject Car;

    void Start()
    {
        Lobby = GameObject.Find("Lobby");
        Manager = GameObject.Find("MultiplayerManager");
        if (!isLocalPlayer)
        {
            for (int i = 0; i < disabledcomponents.Length; i++)
            {
                disabledcomponents[i].enabled = false;
            }
            for (int i = 0; i < disabledObjects.Length; i++)
            {
                disabledObjects[i].SetActive(false);
            }
        }
        else
        {
            playername = "Harry" + Random.Range(1, 90).ToString();
            wins = Random.Range(100, 110);
        }
        if (isServer)
        {
            ready = true;
        }
    }
    private void Update()
    {
        connections = Manager.GetComponent<MultiplayerManager>().Trackedconnections;
    }
    void onReadyChange(bool ready)
    {
        if (inLobby)
        {
            this.GetComponent<MultiplayerLobbyManager>().DisplayPlates();
        }
    }
    void spawnCar(int id)
    {
        Lobby.SetActive(false);
        inLobby = false;
        inTrackSelect = false;
        inGame = true;

        MultiplayerLobbyManager comp = this.GetComponent<MultiplayerLobbyManager>();
        comp.CmdgoReady();

        var track = comp.trackPrefabs[trackID];
        var startPos = track.startPos[spawnID];
        var startRot = track.startRot;

        var car = (GameObject)Instantiate(carPrefab, startPos, Quaternion.Euler(startRot.x, startRot.y, startRot.z), this.transform);
        NetworkServer.Spawn(car);
    }
}