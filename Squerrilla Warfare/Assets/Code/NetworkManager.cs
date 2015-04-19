using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using JetBrains.Annotations;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviour
{
    const string gameTypeName = "Squirrella Warfare";
    bool joining = false;

    public void StartServerButton()
    {
        GameObject[] uiElements = UnityEngine.GameObject.FindGameObjectsWithTag("ServerUI");

        foreach (GameObject uiElement in uiElements)
        {
            if (uiElement.name.Equals("InputField"))
                StartServer(uiElement.GetComponent<InputField>().text);
        }
    }

    private void StartServer(string lobbyName)
    {
        Network.InitializeServer(4, 12, !Network.HavePublicAddress());
        MasterServer.RegisterHost(gameTypeName, lobbyName);
        hideall();
    }

    [UsedImplicitly]
    void OnServerInitialized()
    {
        //Debug.Log("Server Initialized.");
        Game.JoinedGame();
    }

    public static bool Connected { get { return Network.isClient || Network.isServer; } }
    ListDisplay<HostData> hostsListDisplay = new ListDisplay<HostData>(new List<HostData>());
    public void RequestHosts() { MasterServer.RequestHostList(gameTypeName); }
    [UsedImplicitly]
    void OnMasterServerEvent(MasterServerEvent masterServerEvent)
    {
        if (masterServerEvent == MasterServerEvent.HostListReceived)
            Debug.Log("Host List Received");
        hostsListDisplay.contents = MasterServer.PollHostList().ToList();
    }
    void Join(HostData hostData)
    {
        Network.Connect(hostData);
        joining = true;
    }
    void PlayerHitJoin(HostData hostData)
    {
        if (!joining)
            Join(hostData);
    }
    public void DrawMenu()
    {
        if (!Connected)
        {
            hostsListDisplay.Draw(new Rect(200, 200, 50, 50));
        }
        else if (GUI.Button(new Rect(10, 10, 100, 25), "Disconnect"))
            Disconnect();
    }

    [UsedImplicitly]
    void OnPlayerDisconnected(NetworkPlayer player)
    {
    }

    [UsedImplicitly]
    void Start()
    {
        hostsListDisplay.OnChoose(PlayerHitJoin);
    }

    [UsedImplicitly]
    void Update()
    {
        if (Input.GetKeyDown("escape"))
            Game.showMenu = !Game.showMenu;
    }

    void Disconnect()
    {
        Network.Disconnect();
        FindObjectsOfType<NetworkView>().Select(networkView => networkView.gameObject).ForEach(Destroy);
        showall();
    }

    [UsedImplicitly]
    void OnConnectedToServer()
    {
        Debug.Log("Server Joined");
        Game.JoinedGame();
        joining = false;
    }

    //UI STUFF
    private void hideall()
    {
        GameObject[] uiElements = UnityEngine.GameObject.FindGameObjectsWithTag("ServerUI");

        foreach (GameObject uiElement in uiElements)
            uiElement.SetActive(false);
    }

    private void showall()
    {
        GameObject[] uiElements = UnityEngine.GameObject.FindGameObjectsWithTag("ServerUI");

        foreach (GameObject uiElement in uiElements)
            uiElement.SetActive(true);
    }
}
