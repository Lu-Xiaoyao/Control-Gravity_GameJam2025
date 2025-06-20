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
   Dictionary<GravityState, int> gravityStateDict = new Dictionary<GravityState, int>();
   void Start()
   {
        moveSpeed = GameManager.Instance.moveSpeed;
        gravityExtent = Random.Range(GameManager.Instance.gravityExtentMin, GameManager.Instance.gravityExtentMax);
        startGravityState = GravityState.Attract;
        gravityState = startGravityState;
        gravityStateDict.Add(GravityState.Attract, 1);
        gravityStateDict.Add(GravityState.Exclude, -1);
        gravityStateDict.Add(GravityState.Balanced, 0);
   }
   void OnTriggerStay2D(Collider2D target)
   {
    if(target.gameObject.CompareTag("Player") || target.gameObject.CompareTag("Star"))
    {
        target.transform.SetParent(transform);
        Vector3 direction = (transform.position - target.transform.position).normalized;
        target.GetComponent<Rigidbody2D>().velocity = direction * gravityStateDict[gravityState] * gravityExtent * moveSpeed;
    }
   }
   void OnTriggerExit2D(Collider2D target)
   {
    if(target.gameObject.CompareTag("Player"))
    {
        target.transform.SetParent(null);
    }
   }
}
