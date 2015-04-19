using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using JetBrains.Annotations;

public class Game : MonoBehaviour {
    public static GameObject squirrell;
	// ReSharper disable once MemberCanBePrivate.Global
	// ReSharper disable once NotAccessedField.Global
	public static Game game;
	// ReSharper disable once MemberCanBePrivate.Global
	public static NetworkManager networkManager;
	// ReSharper disable once MemberCanBePrivate.Global
	public static Assets assets;
    private static UI uiCanvas;
	public Transform spawnPointParent;
	[UsedImplicitly] void Start () {
		game = this;
		networkManager = GetComponent<NetworkManager>();
		assets = GetComponent<Assets>();
	    Instantiate(assets.camera);
	    Instantiate(assets.quad);
	    Instantiate(assets.light);
		uiCanvas = Instantiate(assets.uiCanvas).GetComponent<UI>();
	}
	[UsedImplicitly] void Update () {
		//if (Input.GetKeyDown("1"))
		//	SpawnSquirrell();
	}

    public static bool showMenu { get { return uiCanvas.MenuShowing; } }

	public static void JoinedGame () {
	}
	static IEnumerable<Transform> SpawnPoints { get { return game.spawnPointParent.Cast<Transform>(); } }
	// ReSharper disable once MemberCanBePrivate.Global
	public static IEnumerable<Squirrell> Squirrells {get {return FindObjectsOfType<Squirrell>();}}
	//static void SpawnSquirrell () {SpawnSquirrellAt(NextSpawnPoint);}
	static Transform NextSpawnPoint {get {return Squirrells.Any() ? SpawnPoints.ArgMax(candidate => Squirrells.Min(squirrell => (candidate.position - squirrell.transform.position).magnitude)) : SpawnPoints.RandomElement();}}
	/*static void SpawnSquirrellAt (Transform spawnPoint) {
		Network.Instantiate(assets.squirrell, spawnPoint.position, Quaternion.identity /*should be spawnPoint.rotation, but squirrel stuff is bugged.*//*, 0);
		//Network.Instantiate(assets.squirrell, Vector3.zero, Quaternion.identity, 0);*/
	//}
	public static void SpawnSquirrell () {squirrell = Network.Instantiate(assets.squirrell, Vector3.zero, Quaternion.identity, 0)as GameObject;}
}
