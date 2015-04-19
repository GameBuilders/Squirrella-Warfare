using UnityEngine;
using JetBrains.Annotations;

public class Game : MonoBehaviour {
	// ReSharper disable once MemberCanBePrivate.Global
	// ReSharper disable once NotAccessedField.Global
	public static Game game;
	// ReSharper disable once MemberCanBePrivate.Global
	public static NetworkManager networkManager;
	// ReSharper disable once MemberCanBePrivate.Global
	public static Assets assets;
	[UsedImplicitly] void Start () {
		game = this;
		networkManager = GetComponent<NetworkManager>();
		assets = GetComponent<Assets>();
	}
	[UsedImplicitly] void Update () {
		//if (Input.GetKeyDown("1"))
		//	SpawnSquirrell();
	}
	// ReSharper disable once InconsistentNaming
	[UsedImplicitly] void OnGUI () {
		Cursor.visible = showMenu;
		Cursor.lockState = showMenu ? CursorLockMode.None : CursorLockMode.Locked;
		if (!NetworkManager.Connected)
			showMenu = true;
		if (showMenu)
			networkManager.DrawMenu();
	}
	public static void JoinedGame () {
		SpawnSquirrell();
		showMenu = false;
	}
	public static void SpawnSquirrell () {Network.Instantiate(assets.squirrell, Vector3.zero, Quaternion.identity, 0);}
	public static bool showMenu;
}
