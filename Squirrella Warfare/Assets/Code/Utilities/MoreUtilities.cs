using UnityEngine;

public static partial class Extensions {
	/*public static void PlayWithRandomOffsetUnlessLevelManifestingForScreenshot (this AudioSource audioSource) {
		if (!Game.levelManifestingForScreenshot)
			audioSource.PlayWithRandomOffset();
	}*/
	public static void PlayWithRandomOffset (this AudioSource audioSource) {
		audioSource.Play(/*(ulong) (((float) 44100 / audioSource.clip.frequency) * UnityEngine.Random.Range(0, audioSource.clip.samples))*/); //TODO use system.Random instead and use ulong instead of int.
		audioSource.timeSamples = Random.Range(0, audioSource.clip.samples);
	}
	/*public static void SetFrame (this AudioSource audioSource, int frame) {
		audioSource.timeSamples = Mathf.RoundToInt(Global.ToTime(frame) / audioSource.clip.frequency);
	}*/
	public static Renderer GetRenderer (this Component component) {
		return component.GetComponent<Renderer>();
	}
	public static float CalcMaxWidth (this GUIStyle style, string text) {
		return style.CalcMaxWidth(new GUIContent(text));
	}
	public static float CalcMaxWidth (this GUIStyle style, GUIContent content) {
		float min, max;
		style.CalcMinMaxWidth(content, out min, out max);
		return max;
	}
	public static bool HasComponent<T> (this Component c) where T : Component {return c.GetComponent<T>() != null;}//constraints necessary because they cause the "!=" operator to be Unity's overloaded crap which handles their fake null.
	public static bool HasComponent<T> (this GameObject g) where T : Component {return g.GetComponent<T>() != null;}
	// ReSharper disable once RedundantNameQualifier
    [RPC]
	public static GameObject InstantiateChild (this GameObject parent, UnityEngine.Object original, Vector3 offset = default(Vector3)) {
		return InstantiateChild(parent, original, offset, Quaternion.identity);
	}
	// ReSharper disable once RedundantNameQualifier
    [RPC]
	public static GameObject InstantiateChild (this GameObject parent, UnityEngine.Object original, Vector3 offset, Quaternion relativeRotation) {
		var instantiated = MonoBehaviour.Instantiate(original, parent.transform.position + offset, parent.transform.rotation * relativeRotation);
		var transform = instantiated as Transform ?? ((GameObject) instantiated).transform;
		transform.parent = parent.transform;
		return transform.gameObject;
	}
	//http://answers.unity3d.com/questions/799429/transformfindstring-no-longer-finds-grandchild.html
	public static Transform DeepFind (this Transform transform, string name) {
		var result = transform.Find(name);
		if (result != null) return result;
		foreach (Transform child in transform) {
			result = child.DeepFind(name);
			if (result != null)
				return result;
		}
		return null;
		//my implementation appears not to work because of Unity's null buggery.
		//return (transform.name == name ? transform : null) ?? transform.Cast<Transform>().AggregateOrDefault((working, next) => working ?? next.FindSelfOrDescendant(name));
	}
	public static void SetLayer (this GameObject gameObject, int layer) {
		gameObject.layer = layer;
		foreach (Transform child in gameObject.transform)
			child.gameObject.SetLayer(layer);
		foreach (var light in gameObject.transform.GetComponentsInChildren<Light>())
			light.cullingMask = 1 << layer | ~((1 << 0) | (1 << 9) | (1 << 8));//Fixed set of layers which may be used to exclude light.
	}
}