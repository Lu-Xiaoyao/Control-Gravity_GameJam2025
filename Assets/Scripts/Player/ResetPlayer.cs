using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetPlayer : MonoBehaviour
{
    private PlayerDeath playerDeath;
    void Start()
    {
        playerDeath = GameObject.Find("Player").GetComponent<PlayerDeath>();
    }
    public void ResetPosition()
    {
        playerDeath.Die();
    }
}
