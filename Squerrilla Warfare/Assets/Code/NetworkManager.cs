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
		Game.JoinedGame();
	}
	const string gameTypeName = "Suirrella Warfare";
	public static bool Connected { get { return Network.isClient || Network.isServer; } }
	ListDisplay<HostData> hostsListDisplay = new ListDisplay<HostData>(new List<HostData>());
	void RequestHosts () { MasterServer.RequestHostList(gameTypeName); }
	[UsedImplicitly] void OnMasterServerEvent (MasterServerEvent masterServerEvent) {
		if (masterServerEvent == MasterServerEvent.HostListReceived)
			hostsListDisplay.contents = MasterServer.PollHostList().ToList();
	}
	void Join (HostData hostData) {
		Network.Connect(hostData);
		joining = true;
	}
	void PlayerHitJoin (HostData hostData) {
		if (!joining)
			Join(hostData);
	}
	string myServerGameName = "Squerrilla Warfare Game";
	public void DrawMenu () {
		if (!Connected) {
			GUI.Label(new Rect(100, 215, 90, 20), "Server Name: ");
			myServerGameName = GUI.TextField(new Rect(195, 215, 155, 20), myServerGameName);
			if (GUI.Button(new Rect(100, 100, 250, 100), "Start Server"))
				StartServer(myServerGameName);
			if (GUI.Button(new Rect(100, 250, 250, 100), "Refresh Hosts"))
				RequestHosts();
			var listDisplayRect = new Rect(400, 100, Screen.width - 400 - 50, Screen.height - 100 - 50);
			GUI.Box(listDisplayRect, "");
			hostsListDisplay.Draw(listDisplayRect);
		}
		else if (GUI.Button(new Rect(10, 10, 100, 25), "Disconnect"))
			Disconnect();
	}
	[UsedImplicitly] void OnPlayerDisconnected (NetworkPlayer player) {
	}
	[UsedImplicitly] void Update () {
		if (Input.GetKeyDown("escape"))
			Game.showMenu = !Game.showMenu;
	}
	void Disconnect () {
		Network.Disconnect();
		FindObjectsOfType<NetworkView>().Select(networkView => networkView.gameObject).ForEach(Destroy);
	}
	[UsedImplicitly] void OnConnectedToServer () {
		Game.JoinedGame();
		joining = false;
	}
	bool joining = false;
	[UsedImplicitly] void Start () {
		hostsListDisplay.OnChoose(PlayerHitJoin).SetPrinter(hostData => string.Join(".", hostData.ip) + ": " + hostData.gameName);
		RequestHosts();
	}
}
