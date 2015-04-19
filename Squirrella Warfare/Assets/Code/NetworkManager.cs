using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using JetBrains.Annotations;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviour
{
    const string gameTypeName = "Squirrella Warfare";
    bool joining = false;
    private InputField serverField;

    public void StartServerButton()
    {
        serverField = UnityEngine.GameObject.FindGameObjectWithTag("ServerName").GetComponent<InputField>();
    }

    private void StartServer(string lobbyName)
    {
        Network.InitializeServer(4, 12, !Network.HavePublicAddress());
        MasterServer.RegisterHost(gameTypeName, lobbyName);
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

    [UsedImplicitly]
    void OnPlayerDisconnected(NetworkPlayer player)
    {
    }

    [UsedImplicitly]
    void Start()
    {
        hostsListDisplay.OnChoose(PlayerHitJoin);
    }


    void Disconnect()
    {
        Network.Disconnect();
        FindObjectsOfType<NetworkView>().Select(networkView => networkView.gameObject).ForEach(Destroy);
    }

    [UsedImplicitly]
    void OnConnectedToServer()
    {
        Debug.Log("Server Joined");
        Game.JoinedGame();
        joining = false;
    }

}
