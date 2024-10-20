using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailSegment : MonoBehaviour
{
    float maxRot = 30f;
    float moveSpeed = 5f;

    float targetX;
    float targetZ;

    void Start()
    {
        targetX = Random.Range(-1 * maxRot, maxRot);
        targetZ = Random.Range(-1 * maxRot, maxRot);
    }

    void Update()
    {
        Vector3 localRot = transform.localEulerAngles;
        localRot.x = WrapAngle(localRot.x);
        localRot.z = WrapAngle(localRot.z);

        if(Vector3.Distance(localRot, new Vector3(targetX, 0, targetZ)) < 5f)
        {
            targetX = Random.Range(-1 * maxRot, maxRot);
            targetZ = Random.Range(-1 * maxRot, maxRot);
        }

        //localRot = Vector3.Slerp(localRot, new Vector3(targetX, 0, targetZ), Time.deltaTime * moveSpeed);

        transform.localRotation = Quaternion.Euler(new Vector3(Mathf.Lerp(localRot.x, targetX, Time.deltaTime * moveSpeed), 0, Mathf.Lerp(localRot.z, targetZ, Time.deltaTime * moveSpeed)));
    }

    public static float WrapAngle(float angle)
    {
        if (angle > 180) return angle - 360f;
        if (angle < -180) return angle + 360f;
        return angle;
    }
}
