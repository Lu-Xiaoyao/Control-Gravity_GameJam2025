using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AllControl;

public enum GravityState
{
    Attract,
    Balanced,
    Exclude
}

public class PlanetGravity : MonoBehaviour
{
    [SerializeField] protected internal GravityState gravityState; //三种状态，玩家可以改变
    [SerializeField] protected internal float gravityExtent; //星球重力的大小
    [SerializeField] protected internal float moveSpeed; //牵引物体的基础速度，全局统一，待调试确定
    [SerializeField] protected internal GravityState startGravityState;
    [SerializeField] protected internal Vector2 direction = Vector2.zero;
    [SerializeField] protected internal Vector2 angleDirection = Vector2.zero;
    Dictionary<GravityState, int> gravityStateDict = new Dictionary<GravityState, int>();
    void Start()
    {
            moveSpeed = GameManager.Instance.moveSpeed;
            startGravityState = GravityState.Attract;
            gravityExtent = transform.parent.GetComponent<PlanetCustom>().gravityExtent;
            gravityState = startGravityState;
            gravityStateDict.Add(GravityState.Attract, 1);
            gravityStateDict.Add(GravityState.Exclude, -1);
            gravityStateDict.Add(GravityState.Balanced, 0);
    }
    void OnTriggerEnter2D(Collider2D target)
    {
        if(target.gameObject.CompareTag("Player") || target.gameObject.CompareTag("Star"))
        {
            if(target.transform.parent == null)
            {
                target.transform.SetParent(transform);
            }
            target.GetComponent<PlayerMove>().AddDirection(this);
        }
    }

    void OnTriggerStay2D(Collider2D target)
    {
        if(target.gameObject.CompareTag("Player") || target.gameObject.CompareTag("Star"))
        {
            direction = (transform.position - target.transform.position).normalized * gravityStateDict[gravityState] * gravityExtent;
            angleDirection = Vector2.Perpendicular((transform.position - target.transform.position).normalized) * gravityExtent;
        }
    }
    
    void OnTriggerExit2D(Collider2D target)
    {
        if(target.gameObject.CompareTag("Player") || target.gameObject.CompareTag("Star"))
        {
            target.transform.SetParent(null);
            target.GetComponent<PlayerMove>().RemoveDirection(this);
        }
   }
}