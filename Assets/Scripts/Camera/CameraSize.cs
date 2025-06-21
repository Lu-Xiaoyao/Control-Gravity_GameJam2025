using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AllControl;

public class CameraSize : MonoBehaviour
{
    
    void Start()
    {
        //GetComponent<Camera>().orthographicSize = GameManager.Instance.cameraSizeNormal;
    }
    public void CameraEnlarge()
    {
        //GetComponent<Camera>().orthographicSize = GameManager.Instance.cameraSizeEnlarge;
        //Invoke("CameraReset",5f);
        
    }
    void CameraReset()
    {
        //GetComponent<Camera>().orthographicSize = GameManager.Instance.cameraSizeNormal;
    }
}
