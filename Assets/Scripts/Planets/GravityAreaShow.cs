using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AllControl;

public class GravityAreaShow : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private AreaShow areaShow;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        areaShow = GameObject.Find("Player").GetComponent<AreaShow>();
    }
    void Start()
    {
        spriteRenderer.enabled = false;
        areaShow.onAreaShow.AddListener(ShowArea);
    }

    void ShowArea()
    {
        spriteRenderer.enabled = true;
        Invoke("HideArea", GameManager.Instance.gravityAreaShowTime);
    }

    void HideArea()
    {
        spriteRenderer.enabled = false;
    }

}
