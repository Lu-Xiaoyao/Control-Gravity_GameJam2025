using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ResetStars : MonoBehaviour
{
    public static ResetStars instance;
    public UnityEvent onResetStars;

    void Awake()
    {
        instance = this;
    }

    public void ResetAllStars()
    {
        onResetStars.Invoke();
    }
}
