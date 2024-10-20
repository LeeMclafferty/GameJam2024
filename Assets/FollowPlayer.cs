using System.Collections;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    private Vector3 _startingPosition;

    private void Awake()
    {
        _startingPosition = transform.position;
    }

    private void Update()
    {
        if (Camera.main)
        {
            Vector3 position = transform.position;

            float distance = Camera.main.transform.position.x - _startingPosition.x;
            position.x = _startingPosition.x - distance;
            position.y = Camera.main.transform.position.y;
            position.z = Camera.main.transform.position.z;

            transform.position = position;
        }
    }
}
