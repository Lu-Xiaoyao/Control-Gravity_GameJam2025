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
    private Animator animator;

    void Start()
    {
        spawnPoint = GameObject.Find("PlayerSpawn").transform;
        playerMove = GetComponent<PlayerMove>();
        minPoint = GameObject.Find("PlayerMin").transform;
        maxPoint = GameObject.Find("PlayerMax").transform;
        transform.position = spawnPoint.position;
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        if(transform.position.x < minPoint.position.x || transform.position.x > maxPoint.position.x
        || transform.position.y < minPoint.position.y || transform.position.y > maxPoint.position.y)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            playerMove.ResetSpeed();
            AllControl.GameManager.Instance.deathCount++;
        }
    }
    public void Die()
    {
        animator.SetTrigger("Death");
        Invoke("ResetPlayer", 1f);
    }
    public void ResetPlayer()
    {
        transform.position = spawnPoint.position;
        transform.rotation = Quaternion.Euler(0, 0, 0);
        playerMove.ResetSpeed();
        AllControl.GameManager.Instance.deathCount++;
    }
}
