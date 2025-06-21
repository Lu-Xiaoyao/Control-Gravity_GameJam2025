using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AreaShow : MonoBehaviour
{
    public UnityEvent onAreaShow;
    private InputActions inputActions;

    void Awake()
    {
        inputActions = new InputActions();
        inputActions.player.Enable();
    }

    public void OnAreaShow()
    {
        onAreaShow.Invoke();
    }
}
