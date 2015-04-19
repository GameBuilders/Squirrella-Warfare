using System.Collections.Generic;
using System.Linq;
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
	[UsedImplicitly] public Transform spawnPointParent;
    public GameObject SelfDeleteAudioSource;
	[UsedImplicitly] void Start () {
		game = this;
		networkManager = GetComponent<NetworkManager>();
		assets = GetComponent<Assets>();
		//dummy for testing:
		//var spawnPoint = SpawnPoints.First();
		//Instantiate(assets.squirrell, spawnPoint.position, Quaternion.identity);
	}
	// ReSharper disable once InconsistentNaming
	[UsedImplicitly] void OnGUI () {
		Cursor.visible = showMenu;
		Cursor.lockState = showMenu ? CursorLockMode.None : CursorLockMode.Locked;
		if (!NetworkManager.Connected)
			showMenu = true;
		if (showMenu) {
			networkManager.DrawMenu();
		    if (NetworkManager.Connected && !HasSquirrell)
		    {
		        //Draw Loadout UI
		        GUI.Box(new Rect(0, Screen.height/6, Screen.width/4, 2*Screen.height/3), "Loadout");
		        //First Loadout
		        GUI.BeginGroup(new Rect(0, 0, Screen.width, Screen.height));
		        if (GUI.Button(new Rect(Screen.width/5, Screen.height/3, Screen.width/25, Screen.height/20), ">"))
						DoNothing(); //Change Weapon 1
		        if (GUI.Button(new Rect(Screen.width/100, Screen.height/3, Screen.width/25, Screen.height/20), "<"))
						DoNothing(); //Change Weapon 1
		        if (GUI.Button(new Rect(Screen.width/5, Screen.height/3*2, Screen.width/25, Screen.height/20), ">"))
						DoNothing(); //Change Weapon 2
		        if (GUI.Button(new Rect(Screen.width/100, Screen.height/3*2, Screen.width/25, Screen.height/20), "<"))
						DoNothing(); //Change Weapon 2
		        GUI.Box(new Rect(Screen.width/4, Screen.height/6, Screen.width/4, Screen.height/3), "Primary Weapon");
		        //GUI.Label(new Rect(Screen.width/4 + 2, Screen.height/6 + Screen.height/12, Screen.width/4, Screen.height/3),
		        //primaryDescription);
		        GUI.Box(new Rect(Screen.width/4, Screen.height/2, Screen.width/4, Screen.height/3), "Secondary Weapon");
		        //GUI.Label(new Rect(Screen.width/4 + 2, Screen.height/2 + Screen.height/12, Screen.width/4, Screen.height/3),
		        //primaryWeaponType);
		        GUI.EndGroup();
		        if (GUI.Button(new Rect(Screen.width/3, Screen.height/6*5, Screen.width/3, Screen.height/6), "Spawn"))
		            FinishChoosingWeapons();
		    }
		}
	}
	static void FinishChoosingWeapons () {
		showMenu = false;
		SpawnMySquirrell();
	}
	static void DoNothing () {}
	public static void JoinedGame () {
	}
	static IEnumerable<Transform> SpawnPoints {get {return game.spawnPointParent.Cast<Transform>();}}
	// ReSharper disable once MemberCanBePrivate.Global
	public static IEnumerable<Squirrell> Squirrells {get {return FindObjectsOfType<Squirrell>();}}
	static void SpawnMySquirrell () {SpawnMySquirrellAt(NextSpawnPoint);}
	static Transform NextSpawnPoint {get {return Squirrells.Any() ? SpawnPoints.ArgMax(candidate => Squirrells.Min(squirrell => (candidate.position - squirrell.transform.position).magnitude)) : SpawnPoints.RandomElement();}}
	static void SpawnMySquirrellAt (Transform spawnPoint) {
		var instantiated = (GameObject) Network.Instantiate(assets.squirrell, spawnPoint.position, Quaternion.identity /*should be spawnPoint.rotation, but squirrel stuff is bugged.*/, 0);
		//Network.Instantiate(assets.squirrell, Vector3.zero, Quaternion.identity, 0);
		game.mySquirrell = instantiated.GetComponent<Squirrell>();
	}
	public static bool showMenu;
	public Squirrell mySquirrell;
	public bool HasSquirrell {get {return mySquirrell != null;}}

    public void playSound(Vector3 position, AudioClip[] sound)
    {
        var audioSource = (GameObject)Instantiate(SelfDeleteAudioSource, position, Quaternion.identity);
        AudioSource soundClip = audioSource.GetComponent<AudioSource>();
        soundClip.clip = sound.RandomElement();
    }
}
