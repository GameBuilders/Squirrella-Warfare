using UnityEngine;
using JetBrains.Annotations;

public class Game : MonoBehaviour {
	public static Game game;
	public static NetworkManager networkManager;
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
	
	[UsedImplicitly] void OnGUI () {
		Cursor.visible = showMenu;
		Cursor.lockState = showMenu ? CursorLockMode.None : CursorLockMode.Locked;
		if (!NetworkManager.Connected)
		{
		  showMenu = true;
		  showLoadoutMenu = false;
		}
	}

	public static void JoinedGame () {
		showMenu = false;
		showLoadoutMenu = true;
	}

	public static void SpawnSquirrell () {Network.Instantiate(assets.squirrell, Vector3.zero, Quaternion.identity, 0);}
	public static bool showMenu;
 	public static bool showLoadoutMenu;
}
