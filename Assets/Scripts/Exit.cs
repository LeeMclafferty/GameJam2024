#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class Exit : MonoBehaviour
{
	/// <summary> Called by exit button's UnityEvent </summary>
	public void OnExit()
	{
#if UNITY_EDITOR
		EditorApplication.ExitPlaymode();
#elif UNITY_WEBGL
        // do nothing... or Application.OpenURL(webplayerQuitURL);
#else
        Application.Quit();
#endif
	}
}
