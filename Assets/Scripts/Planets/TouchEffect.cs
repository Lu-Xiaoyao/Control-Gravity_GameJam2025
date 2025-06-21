using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum TouchEffectType
{
    None,
    Show,
    Hide,
    Death,
}

public class TouchEffect : MonoBehaviour
{
    TouchEffectType touchEffectType;
    [SerializeField] private GameObject controlledObject;

    void Start()
    {
        touchEffectType = GetComponent<PlanetCustom>().touchEffectType;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            switch(touchEffectType)
            {
                case TouchEffectType.Death:
                    collision.gameObject.GetComponent<PlayerDeath>().Die();
                    break;
                case TouchEffectType.Show:
                    if(controlledObject != null)
                    {
                        controlledObject.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 1f);
                        controlledObject.GetComponent<PlayerMove>().enabled = true;
                    }
                    break;
                case TouchEffectType.Hide:
                    if(controlledObject != null)
                    {
                        controlledObject.GetComponent<SpriteRenderer>().color = new Color(106, 106, 106, 0.8f);
                        controlledObject.GetComponent<PlayerMove>().enabled = false;
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
