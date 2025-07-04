using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlanetGravityShift : MonoBehaviour, IPointerDownHandler
{
    private PlanetGravity planetGravity;
    private SpriteRenderer balancedEffect;
    private SpriteRenderer excludeEffect;
    private bool isControllable = true;
    private AudioSource attract;
    private AudioSource exclude;
    private AudioSource planetShift;
    void Start()
    {
        planetGravity = transform.Find("GravityArea").GetComponent<PlanetGravity>();
        balancedEffect = transform.Find("BalancedEffect").GetComponent<SpriteRenderer>();
        excludeEffect = transform.Find("ExcludeEffect").GetComponent<SpriteRenderer>();
        isControllable = GetComponent<PlanetCustom>().isControllable;
        attract = GameObject.Find("Audio/Attract").GetComponent<AudioSource>();
        exclude = GameObject.Find("Audio/Exclude").GetComponent<AudioSource>();
        planetShift = GameObject.Find("Audio/PlanetShift").GetComponent<AudioSource>();
    }

   public void OnPointerDown(PointerEventData eventData)
   {
        if(!isControllable)
        {
            return;
        }
        planetGravity.gravityState = (GravityState)(((int)planetGravity.gravityState + 1) % 3);
        // 重置重力方向，确保状态切换时方向被正确更新
        planetGravity.direction = Vector2.zero;
        planetGravity.angleDirection = Vector2.zero;
        
        Debug.Log("重力模式切换为" + planetGravity.gravityState);
        if(planetGravity.gravityState == GravityState.Balanced)
        {
            balancedEffect.enabled = true;
            excludeEffect.enabled = false;
        }
        else if(planetGravity.gravityState == GravityState.Exclude)
        {
            excludeEffect.enabled = true;
            balancedEffect.enabled = false;
        }
        else
        {
            balancedEffect.enabled = false;
            excludeEffect.enabled = false;
        }
        planetShift.Play();
        if(planetGravity.gravityState == GravityState.Attract)
        {
            attract.Play();
        }
        else if(planetGravity.gravityState == GravityState.Exclude)
        {
            exclude.Play();
        }
   }
}
