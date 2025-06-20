using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSize : MonoBehaviour
{
    private Camera camera;
    void Start()
    {
        camera = GetComponent<Camera>();
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            CameraEnlarge();
        }
        
    }
    void CameraEnlarge()
    {
        camera.orthographicSize = 25f;
        Invoke("CameraReset",5f);
        
    }
    void CameraReset()
    {
        camera.orthographicSize = 10f;
    }
}
