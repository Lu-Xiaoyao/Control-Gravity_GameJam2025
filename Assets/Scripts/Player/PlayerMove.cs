using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AllControl;

public class PlayerMove : MonoBehaviour
{
    protected Rigidbody2D rb;
    private Animator animator;
    private int finalState = 0;
    [SerializeField] protected internal List<PlanetGravity> directions = new List<PlanetGravity>();
    [SerializeField] protected internal Vector2 finalDirectDirection;
    //[SerializeField] protected internal Vector2 finalAngleDirection;
    //[SerializeField] protected internal Vector2 finalDirection;
    [SerializeField] protected internal float moveSpeed;
    //[SerializeField] private int activeTriggersCount = 0;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        moveSpeed = GameManager.Instance.moveSpeed;
        ResetSpeed();
    }

    public void ResetSpeed()
    {
        finalDirectDirection = Vector2.right;
        //activeTriggersCount = 0;
    }

    void FixedUpdate()
    {
        if(directions.Count > 0)
        {
            finalDirectDirection = Vector2.zero;
            finalState = 0;
            foreach(PlanetGravity planet in directions)
            {
                finalDirectDirection += planet.direction;
                finalState += planet.gravityStateDict[planet.gravityState];
                //finalAngleDirection += planet.angleDirection;
            }
            //finalDirection = finalDirectDirection + finalAngleDirection;
        }
        else
        {
            finalState = 0;
            //finalDirection = finalDirectDirection;
        }
        rb.velocity = finalDirectDirection * moveSpeed;
        if(animator != null)
        {
            animator.SetInteger("State", finalState);
            Debug.Log(finalState);
        }
    }

    public void AddDirection(PlanetGravity planet)
    {
        //activeTriggersCount++;       
        directions.Add(planet);
    }

    public void RemoveDirection(PlanetGravity planet)
    {
        directions.Remove(planet);
        //activeTriggersCount--;
    }
}
