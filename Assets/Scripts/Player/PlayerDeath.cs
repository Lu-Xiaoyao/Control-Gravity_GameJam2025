using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ResetPlayer;

public class PlayerDeath : MonoBehaviour
{
    private Transform spawnPoint;
    private PlayerMove playerMove;
    private Transform minPoint;
    private Transform maxPoint;

    void Start()
    {
        spawnPoint = GameObject.Find("LocatePoints").transform.Find("PlayerSpawn");
        playerMove = GetComponent<PlayerMove>();
        minPoint = GameObject.Find("LocatePoints").transform.Find("PlayerMin");
        maxPoint = GameObject.Find("LocatePoints").transform.Find("PlayerMax");
        transform.position = spawnPoint.position;
    }

    void FixedUpdate()
    {
        if(transform.position.x < minPoint.position.x || transform.position.x > maxPoint.position.x
        || transform.position.y < minPoint.position.y || transform.position.y > maxPoint.position.y)
        {
            Die();
        }
    }
    public void Die()
    {
        transform.position = spawnPoint.position;
        playerMove.ResetSpeed();
        AllControl.GameManager.Instance.deathCount++;
    }
}
