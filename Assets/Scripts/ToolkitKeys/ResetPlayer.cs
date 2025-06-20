using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetPlayer : MonoBehaviour
{
    public static ResetPlayer Instance;
    private PlayerDeath playerDeath;
    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        playerDeath = GameObject.Find("Player").GetComponent<PlayerDeath>();
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            playerDeath.Die();
        }
    }
}
