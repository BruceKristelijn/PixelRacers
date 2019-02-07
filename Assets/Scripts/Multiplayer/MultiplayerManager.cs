using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MultiplayerManager : NetworkManager
{

    public GameObject SearchingMenu;
    public Component DiscoveryComp;
    public GameObject DiscoveryButton;
    public GameObject DiscoveryPanel;
    public Button SearchButton;
    public string RoomName;
    public string ipadress;
    public int Trackedconnections;

    int index;

    private void Start()
    {
        DiscoveryComp = this.GetComponent<MultiplayerDiscovery>();
        RoomName = PlayerPrefs.GetString("USERNAME")+"'s Room.";
        base.maxConnections = 4;
    }
    public override void OnServerConnect(NetworkConnection conn)
    {
        ++Trackedconnections;
        sendConnections();
        base.OnServerConnect(conn);
        int players = Trackedconnections;
        Debug.Log("Connection: " + players);
        if(players > 4)
        {
            conn.Disconnect();
        }
        if(players == 4)
        {
            var MpDisc = this.GetComponent<MultiplayerDiscovery>();
            MpDisc.StopBroadcast();
        }
    }
    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        NetworkManager.Shutdown();
    }
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        Debug.Log("User Disconnected");
        NetworkServer.DestroyPlayersForConnection(conn);
        conn.Disconnect();
        --Trackedconnections;
        sendConnections();
        int players = Trackedconnections;
        if (players < 4)
        {
            var MpDisc = this.GetComponent<MultiplayerDiscovery>();
            MpDisc.Initialize();
            MpDisc.broadcastData = RoomName;
            MpDisc.StartAsServer();
        }
    }
    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
    }

    public void stop()
    {
        NetworkManager.Shutdown();
    }
    void sendConnections()
    {
        var players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++)
        {
            var comp = players[i].GetComponent<MultiplayerPlayerManager>();
            comp.connections = Trackedconnections;
        }
    }
    public void startHost()
    {
        SearchingMenu.SetActive(false);

        SetPort();
        NetworkManager.singleton.StartHost();
        var MpDisc = this.GetComponent<MultiplayerDiscovery>();
        if (MpDisc.isClient) { MpDisc.StopBroadcast(); }
        MpDisc.Initialize();
        MpDisc.broadcastData = RoomName;
        MpDisc.StartAsServer();
    }
    public void joinGame()
    {
        SearchingMenu.SetActive(false);

        SetPort();
        NetworkManager.singleton.networkAddress = ipadress;
        NetworkManager.singleton.StartClient();
    }
    void SetPort()
    {
        NetworkManager.singleton.networkPort = 7777;
    }
    public void SearchLocally()
    {
        this.GetComponent<MultiplayerDiscovery>().destroyButtons();
        this.GetComponent<MultiplayerDiscovery>().Initialize();
        this.GetComponent<MultiplayerDiscovery>().StartAsClient();
        SearchButton.interactable = false;
        StartCoroutine(SearchRoutine());
    }
    IEnumerator SearchRoutine()
    {
        yield return new WaitForSeconds(6f);
        if (this.GetComponent<MultiplayerDiscovery>().isClient || this.GetComponent<MultiplayerDiscovery>().isServer)
        {
            this.GetComponent<MultiplayerDiscovery>().Initialize();
            this.GetComponent<MultiplayerDiscovery>().StopBroadcast();
        }
        SearchButton.interactable = true;
    }
}
