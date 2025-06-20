using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AreaShow : MonoBehaviour
{
    public UnityEvent onAreaShow;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            onAreaShow.Invoke();
        }
    }
}
