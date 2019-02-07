using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class MultiplayerLobbyManager : NetworkBehaviour {
    public GameObject lobby_playerList;
    public GameObject lobby_playerPlate;
    public GameObject networkManager;
    public List<GameObject> playerPlates;

    public GameObject leaveButton;
    public GameObject readyButton;

    public Tracks[] trackPrefabs;
    public int chosenTrack = 0;

    float setTime = 2.5f;

    private void Start()
    {
        networkManager = GameObject.Find("MultiplayerManager");
        var comp = this.GetComponent<MultiplayerPlayerManager>();
        CmdSetInfo(this.gameObject, comp.playername, comp.wins);
        DisplayPlates();
        setupButtons();
    }
    private void Update()
    {
        var playerComp = this.GetComponent<MultiplayerPlayerManager>();
        if (!isLocalPlayer || !playerComp.inLobby)
        {
            return;
        }
        if (Time.time > setTime) {
            DisplayPlates();
            setTime = Time.time + 5;
        }
    }
    public void DisplayPlates()
    {
        for (int i = 0; i < playerPlates.Count; i++) { GameObject.Destroy(playerPlates[i]); }
        playerPlates.Clear();
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++)
        {
            var player = players[i].GetComponent<MultiplayerPlayerManager>();
            var plate = Instantiate(lobby_playerPlate, lobby_playerList.transform);
            var platecomp = plate.GetComponent<multiplayerPlayerPlate>();
            platecomp.playerName.text = player.playername;
            platecomp.playerWins.text = player.wins.ToString();
            platecomp.playerNumber.text = (i + 1).ToString();
            if (player.ready)
            {
                platecomp.readyCircle.GetComponent<Image>().fillAmount = 1;
            }
            else
            {
                platecomp.readyCircle.GetComponent<Image>().fillAmount = 0;
            }
            if (isServer && i != 0) {
                platecomp.plateConn = player.gameObject.GetComponent<NetworkIdentity>().connectionToClient;
                platecomp.kickButton.GetComponent<Button>().onClick.AddListener(delegate { kickPlayer(platecomp.plateConn); });
            }
            playerPlates.Add(plate);
        }
    }
    void setupButtons()
    {
        leaveButton.GetComponent<Button>().onClick.AddListener(networkManager.GetComponent<MultiplayerManager>().stop);
        if (isLocalPlayer && !isServer)
        {
            readyButton.GetComponent<Button>().onClick.AddListener(delegate { CmdgoReady(); });
        }
        else if (isLocalPlayer && isServer)
        {
            readyButton.GetComponent<Button>().onClick.AddListener(delegate { startGame(); });
            readyButton.GetComponentInChildren<Text>().text = "Start Game";
        }
    }
    public void kickPlayer(NetworkConnection conn)
    {
        conn.Disconnect();
        setTime = Time.time + 0.2f;
    }
    [Command]  // Go ready function for setting the player to ready. This can be used in the lobby and in the start of a race.
    public void CmdgoReady()
    {
        var comp = this.GetComponent<MultiplayerPlayerManager>();                       //Getting the networkPlayer component <Networkbehaviour>.
        comp.ready = !comp.ready;                                                       //Setting the ready to the opposide of ready. This is a syncvar.
        setTime = Time.time + 0.2f;                                                     //Set the interval time to happen quicker so the user gets feedback from their actions.
    }
    public void startGame()                                                             //Function to start the game. This will check the amount of users as a host and will cause players to load a track.
    {
        if (isServer)                                                                   //Check if server so the amount of players ready will work properly
        {
            int playersready = countReady();                                            //Count the amount of players ready to play.
            int connections = this.GetComponent<MultiplayerPlayerManager>().connections;//Check for the amount of connections to the server. This is different from NetworkServer.connections because this remembers disconnected connections.
            if (playersready == connections && connections > 1)                         //Check if everyone is ready an there are atleast 2 players connected
            {
                int laps = trackPrefabs[chosenTrack].Laps;
                RpcStartRace();
                setStartPositions();
                spawnTrack();
                Debug.Log("Start game");
            }
            else
            {
                Debug.Log("Not everyone is ready / enough players connected.");
            }
        }
    }
    int countReady()                                                                    //Function to count the amount of players ready to play.
    {
        var players = GameObject.FindGameObjectsWithTag("Player");                      //Find users.
        int amountready = 0;                                                            //create a variable of type int to use as a return.
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<MultiplayerPlayerManager>().ready)
            {
                ++amountready;
            }
        }
        return amountready;
    }
    void spawnTrack()
    {
        var track = (GameObject)Instantiate(trackPrefabs[chosenTrack].track, Vector3.zero, Quaternion.Euler(0,0,0), this.transform.parent);
        NetworkServer.Spawn(track); 
    }
    void setStartPositions()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++)
        {
            var player = players[i];
            MultiplayerPlayerManager playercomp = player.GetComponent<MultiplayerPlayerManager>();
            playercomp.spawnID = i;
            playercomp.trackID = chosenTrack;
        }
    }
    [Command]
    void CmdSetInfo(GameObject player, string name, int wins)
    {
        var comp = player.GetComponent<MultiplayerPlayerManager>();
        comp.playername = name;
        comp.wins = wins;
    }
    [ClientRpc]
    void RpcStartRace()
    {
        //Spawn the user's car
        Debug.Log("called");
    }
}
[System.Serializable]
public class PlateInfo
{
    public string playerName;
    public int wins;
}
[System.Serializable]
public class Tracks
{
    public string Name;
    public int    Laps;
    public GameObject track;
    public Vector3[] startPos;
    public Vector3 startRot;
}