using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using JetBrains.Annotations;

public class NetworkManager : MonoBehaviour {
	static void StartServer (string lobbyName) {
		Network.InitializeServer(4, 12, !Network.HavePublicAddress());
		MasterServer.RegisterHost(gameTypeName, lobbyName);
	}
	[UsedImplicitly] void OnServerInitialized () {
		Debug.Log("Server Initialized.");
		Game.JoinedGame();
	}
	const string gameTypeName = "Suirrella Warfare";
	static bool Connected {get {return Network.isClient || Network.isServer;}}
    ListDisplay<HostData> hostsListDisplay = new ListDisplay<HostData>(new List<HostData>());
	void RequestHosts () {MasterServer.RequestHostList(gameTypeName);}
	[UsedImplicitly] void OnMasterServerEvent (MasterServerEvent masterServerEvent) {
		if (masterServerEvent == MasterServerEvent.HostListReceived) {
			Debug.Log("Host List Received");
            hostsListDisplay.contents = MasterServer.PollHostList().ToList();
		}
	}
	void Join (HostData hostData) {Network.Connect(hostData);}
	// ReSharper disable once InconsistentNaming
	[UsedImplicitly] void OnGUI () {
		if (!Connected)
			showMenu = true;
		if (showMenu)
			DrawMenu();
	}
	void DrawMenu () {
		if (!Connected) {
			if (GUI.Button(new Rect(100, 100, 250, 100), "Start Server"))
				StartServer("AsdfGame");
			if (GUI.Button(new Rect(100, 250, 250, 100), "Refresh Hosts"))
				RequestHosts();
			
            hostsListDisplay.Draw(new Rect(200, 200, 50, 50));
            
            hostsListDisplay.OnChoose(Join);
			}
		
		else if (GUI.Button(new Rect(10, 10, 100, 25), "Disconnect"))
			Disconnect();
	}
	[UsedImplicitly] void OnPlayerDisconnected (NetworkPlayer player) {
	}
	bool showMenu;
	[UsedImplicitly] void Update () {
		if (Input.GetKeyDown("escape"))
			showMenu = !showMenu;
	}
	void Disconnect () {
		Network.Disconnect();
		FindObjectsOfType<NetworkView>().Select(networkView => networkView.gameObject).ForEach(Destroy);
	}
	[UsedImplicitly] void OnConnectedToServer () {
		Debug.Log("Server Joined");
		Game.JoinedGame();
	}
}
