using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class MultiplayerDiscovery : NetworkDiscovery
{
    public List<GameObject> Buttons = new List<GameObject>();

    Component Manager;
    GameObject DiscoveryButton;
    GameObject DiscoveryPanel;

    private void Start()
    {
        this.broadcastInterval = 5000;
        var manager = this.GetComponent<MultiplayerManager>();
        DiscoveryButton = manager.DiscoveryButton;
        DiscoveryPanel = manager.DiscoveryPanel;
    }
    public override void OnReceivedBroadcast(string fromAddress, string data)
    {
        List<GameObject> ButtonsToRemove = new List<GameObject>();
        for (int i = 0; i < Buttons.Count; i++)
        {
            if(Buttons[i] != null && Buttons[i].GetComponentInChildren<Text>().text == data)
            {
                ButtonsToRemove.Add(Buttons[i]);
            }
        }
        for (int i = 0; i < ButtonsToRemove.Count; i++){ Destroy(ButtonsToRemove[i]); }
        ButtonsToRemove.Clear();

        var button = Instantiate(DiscoveryButton, this.transform.position, this.transform.rotation) as GameObject;
        button.transform.SetParent(DiscoveryPanel.transform, false);
        button.GetComponentInChildren<Text>().text = data;

        button.GetComponent<Button>().onClick.RemoveAllListeners();
        button.GetComponent<Button>().onClick.AddListener(delegate { ConnectDiscovery(fromAddress); });

        Buttons.Add(button);
    }
    public void ConnectDiscovery(string adress)
    {
        var manager = this.GetComponent<MultiplayerManager>();
        manager.ipadress = adress;
        manager.joinGame();
    }
    public void destroyButtons()
    {
        for (int i = 0; i < Buttons.Count; i++)
        {
            if(Buttons[i] != null)
            {
                Destroy(Buttons[i]);
            }
        }
        Buttons.Clear();
    }
}
