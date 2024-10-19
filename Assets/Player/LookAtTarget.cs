using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtTarget : MonoBehaviour
{
    public Transform moveTarget;
    public Transform target;

    Vector3 oldPosition;

    void Start()
    {
        oldPosition = target.position;
    }

    void Update()
    {
        //if(target == null) return;
        if(moveTarget != null)
            transform.position = moveTarget.position;

        transform.LookAt(oldPosition);
        oldPosition = target.position;

        float yOffset = target.position.y - transform.position.y;
        if (Mathf.Abs(yOffset) > 0.05)
        {
            oldPosition.y -= Mathf.Sign(yOffset) * Mathf.Clamp(Mathf.Abs(yOffset) * 5f, 1f, 5f) * Time.deltaTime;
        }
    }
}
