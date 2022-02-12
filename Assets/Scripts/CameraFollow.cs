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

    void Start()
    {
        Cam = GetComponent<Camera>();
        Cam.orthographicSize = orthosize;
    }

    void FixedUpdate()
    {
        //Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //Vector3 desiredPosition = new Vector3((mouseWorldPosition.x + target.position.x * 4) / 5 , (mouseWorldPosition.y + target.position.y * 2) / 3, offset.z);
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, new Vector3(target.position.x, target.position.y, offset.z), ref velocity, smoothSpeed);
        transform.position = smoothedPosition;
    }
}
