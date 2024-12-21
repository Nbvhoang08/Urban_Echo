using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPCamera : MonoBehaviour
{   
    public Transform target; // Nhân vật hoặc đối tượng cần theo dõi
    public float smoothSpeed = 0.125f;
    public Vector3 offset;

    private void FixedUpdate()
    {
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
        //transform.LookAt(target);

    }
}
