using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlanetGravityShift : MonoBehaviour, IPointerDownHandler
{
    private PlanetGravity planetGravity;
    void Start()
    {
        planetGravity = transform.Find("GravityArea").GetComponent<PlanetGravity>();
    }

   public void OnPointerDown(PointerEventData eventData)
   {
        planetGravity.gravityState = (GravityState)(((int)planetGravity.gravityState + 1) % 3);
        Debug.Log("重力模式切换为" + planetGravity.gravityState);
   }
}
