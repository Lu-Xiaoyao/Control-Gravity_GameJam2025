using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AllControl;

public class PlayerMove : MonoBehaviour
{
    protected Rigidbody2D rb;
    [SerializeField] protected internal List<PlanetGravity> directions = new List<PlanetGravity>();
    [SerializeField] protected internal Vector2 finalDirectDirection;
    //[SerializeField] protected internal Vector2 finalAngleDirection;
    //[SerializeField] protected internal Vector2 finalDirection;
    [SerializeField] protected internal float moveSpeed;
    //[SerializeField] private int activeTriggersCount = 0;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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
            foreach(PlanetGravity planet in directions)
            {
                finalDirectDirection += planet.direction;
                //finalAngleDirection += planet.angleDirection;
            }
            //finalDirection = finalDirectDirection + finalAngleDirection;
        }
        else
        {
            //finalDirection = finalDirectDirection;
        }
        rb.velocity = finalDirectDirection * moveSpeed;
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
