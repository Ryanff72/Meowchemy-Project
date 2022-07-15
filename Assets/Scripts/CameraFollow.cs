using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    public float smoothSpeed = 0.125f;

    private Vector3 velocity = Vector3.zero;

    public Vector3 offset = new Vector3(0,0,-10);

    private Camera Cam;

    public float orthosize;

    public float targetOrthosize;

    void Start()
    {
        Cam = GetComponent<Camera>();
        Cam.orthographicSize = orthosize;
    }

    void FixedUpdate()
    {
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, new Vector3(target.position.x, target.position.y, offset.z), ref velocity, smoothSpeed);
        transform.position = smoothedPosition;
        Cam.orthographicSize = Mathf.Lerp(Cam.orthographicSize, targetOrthosize, Time.deltaTime * 2);
    }
}
