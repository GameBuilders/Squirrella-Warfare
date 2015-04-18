using UnityEngine;
using JetBrains.Annotations;

public class Game : MonoBehaviour {
	// ReSharper disable once MemberCanBePrivate.Global
	public static Game game;
	// ReSharper disable once MemberCanBePrivate.Global
	public static Assets assets;
	[UsedImplicitly] void Start () {
		game = this;
		assets = GetComponent<Assets>();
	}
	[UsedImplicitly] void Update () {
		//if (Input.GetKeyDown("1"))
		//	SpawnSquirrell();
	}
	public static void JoinedGame () {SpawnSquirrell();}
	public static void SpawnSquirrell () {Network.Instantiate(assets.squirrell, Vector3.zero, Quaternion.identity, 0);}
}
