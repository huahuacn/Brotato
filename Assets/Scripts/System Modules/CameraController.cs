using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : Singleton<CameraController>
{
    
    public Transform target;
    public float smoothing = 0.1f;

    public Vector3 min;
    public Vector3 max;

    void Start()
    {
        
    }

    void LateUpdate()
    {
        if (target == null) return;

        if (transform.position == target.position) return;

        Vector3 targetPos = target.position;


        Vector3 tmp = Vector3.Lerp(transform.position, targetPos, smoothing);
        tmp.z = -20;
        transform.position = tmp;
    }

    public void Follow()
    {

    }

    void Update() 
    {
    }

    
}
