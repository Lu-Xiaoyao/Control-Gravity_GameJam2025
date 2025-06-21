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
    private Dictionary<GameObject, Transform> starPuzzle = new Dictionary<GameObject, Transform>();
    public Dictionary<GravityState, int> gravityStateDict = new Dictionary<GravityState, int>();
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
        if (target.gameObject.CompareTag("Player"))
        {
            if(target.transform.parent == null)
            {
                target.transform.SetParent(transform);
            }
            target.GetComponent<PlayerMove>().AddDirection(this);
        }
        else if (target.gameObject.CompareTag("Star"))
        {
            if (!starPuzzle.ContainsKey(target.gameObject))
            {
                starPuzzle.Add(target.gameObject, target.transform.parent);
            }
            target.transform.SetParent(transform);
            target.GetComponent<PlayerMove>().AddDirection(this);
        }
    }

    void OnTriggerStay2D(Collider2D target)
    {
        if(target.gameObject.CompareTag("Player") || target.gameObject.CompareTag("Star"))
        {
            if (gravityStateDict.ContainsKey(gravityState))
            {
                direction = (transform.position - target.transform.position).normalized * gravityStateDict[gravityState] * gravityExtent;
                angleDirection = Vector2.Perpendicular((transform.position - target.transform.position).normalized) * gravityExtent;
                
                Debug.Log($"星球 {gameObject.name} 重力状态: {gravityState}, 方向: {direction}, 重力系数: {gravityStateDict[gravityState]}");
            }
            else
            {
                Debug.LogError($"重力状态 {gravityState} 在字典中不存在！");
                direction = Vector2.zero;
                angleDirection = Vector2.zero;
            }
        }
    }
    
    void OnTriggerExit2D(Collider2D target)
    {
        if(target.gameObject.CompareTag("Player"))
        {
            target.transform.SetParent(null);
            target.GetComponent<PlayerMove>().RemoveDirection(this);
        }
        else if(target.gameObject.CompareTag("Star"))
        {
            if (starPuzzle.ContainsKey(target.gameObject))
            {
                target.transform.rotation = Quaternion.Euler(0, 0, 0);
                target.transform.SetParent(starPuzzle[target.gameObject]);
                starPuzzle.Remove(target.gameObject);
            }
            target.GetComponent<PlayerMove>().RemoveDirection(this);
        }
   }
}