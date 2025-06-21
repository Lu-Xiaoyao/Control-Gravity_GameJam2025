using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StarReset : MonoBehaviour, IPointerDownHandler
{
    private Transform startPoint;

    void Awake()
    {
        startPoint = transform.parent.Find("StarStartPoint");
    }
    void Start()
    {
        transform.position = startPoint.position;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        ResetStar();
    }

    public void ResetStar()
    {
        transform.position = startPoint.position;
    }
}
