using System.Collections;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class EyeMover : MonoBehaviour
{
	[SerializeField]
	private float secondsPause = 2;

	//[SerializeField]
	private float XDelta = 1;
	//[SerializeField]
	private float YDelta = 1;

	private Vector2 rootPosition;
	private Vector2 currentPosition;

	private RectTransform rectTrans
	{
		get { return _rectTrans ??= this.GetComponent<RectTransform>(); }
	}
	private RectTransform _rectTrans;

	private IEnumerator Start()
	{
		rootPosition = currentPosition = rectTrans.anchoredPosition;

		while (true)
		{
			currentPosition.x = rootPosition.x + Random.Range(-XDelta, XDelta);
			currentPosition.y = rootPosition.y + Random.Range(-YDelta, YDelta);
			rectTrans.anchoredPosition = currentPosition;

			yield return new WaitForSeconds(secondsPause);
		}
	}
}
