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
    
    private Dictionary<GravityState, int> gravityStateDict = new Dictionary<GravityState, int>();
    private Dictionary<GameObject, Vector2> affectedObjects = new Dictionary<GameObject, Vector2>(); // 记录受影响的物体及其重力向量
    
    void Start()
    {
        moveSpeed = GameManager.Instance.moveSpeed;
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
            if (gravityState == GravityState.Balanced)
            {
                // Balanced状态：只设置父物体，不应用重力
                if (!affectedObjects.ContainsKey(target.gameObject))
                {
                    affectedObjects[target.gameObject] = Vector2.zero;
                }
            }
            else
            {
                // Attract/Exclude状态：应用重力
                Vector2 direction = (transform.position - target.transform.position).normalized;
                Vector2 gravityForce = direction * gravityStateDict[gravityState] * gravityExtent * moveSpeed;
                
                // 记录当前重力向量
                affectedObjects[target.gameObject] = gravityForce;
                
                // 使用重力管理器注册重力影响
                if (GravityManager.Instance != null)
                {
                    GravityManager.Instance.RegisterGravityForce(target.gameObject, gravityForce);
                }
                else
                {
                    // 如果没有重力管理器，使用旧方法
                    ApplyGravityDirectly(target.gameObject, gravityForce);
                }
            }
        }
    }
    
    void OnTriggerExit2D(Collider2D target)
    {
        if(target.gameObject.CompareTag("Player") || target.gameObject.CompareTag("Star"))
        {
            // 移除重力影响
            if (affectedObjects.ContainsKey(target.gameObject))
            {
                Vector2 gravityForce = affectedObjects[target.gameObject];
                affectedObjects.Remove(target.gameObject);
                
                if (gravityState != GravityState.Balanced && GravityManager.Instance != null)
                {
                    GravityManager.Instance.UnregisterGravityForce(target.gameObject, gravityForce);
                }
                else if (gravityState != GravityState.Balanced)
                {
                    // 如果没有重力管理器，使用旧方法
                    RemoveGravityDirectly(target.gameObject, gravityForce);
                }
            }
        }
    }
    
    /// <summary>
    /// 直接应用重力（旧方法，用于兼容性）
    /// </summary>
    private void ApplyGravityDirectly(GameObject target, Vector2 gravityForce)
    {
        if (target.CompareTag("Player"))
        {
            PlayerMove playerMove = target.GetComponent<PlayerMove>();
            if (playerMove != null)
            {
                playerMove.AddGravityForce(gravityForce);
            }
        }
        else if (target.CompareTag("Star"))
        {
            Rigidbody2D starRb = target.GetComponent<Rigidbody2D>();
            if (starRb != null)
            {
                starRb.velocity += gravityForce * Time.deltaTime;
            }
        }
    }
    
    /// <summary>
    /// 直接移除重力（旧方法，用于兼容性）
    /// </summary>
    private void RemoveGravityDirectly(GameObject target, Vector2 gravityForce)
    {
        if (target.CompareTag("Player"))
        {
            PlayerMove playerMove = target.GetComponent<PlayerMove>();
            if (playerMove != null)
            {
                playerMove.RemoveGravityForce(gravityForce);
            }
        }
    }
    
    /// <summary>
    /// 获取当前重力状态
    /// </summary>
    /// <returns>重力状态</returns>
    public GravityState GetGravityState()
    {
        return gravityState;
    }
    
    /// <summary>
    /// 设置重力状态
    /// </summary>
    /// <param name="newState">新的重力状态</param>
    public void SetGravityState(GravityState newState)
    {
        GravityState oldState = gravityState;
        gravityState = newState;
        
        // 如果状态改变，更新所有受影响物体
        if (oldState != newState)
        {
            UpdateAllAffectedObjects(oldState, newState);
        }
    }
    
    /// <summary>
    /// 更新所有受影响物体的重力
    /// </summary>
    private void UpdateAllAffectedObjects(GravityState oldState, GravityState newState)
    {
        foreach (var kvp in affectedObjects)
        {
            GameObject obj = kvp.Key;
            Vector2 oldForce = kvp.Value;
            
            if (newState == GravityState.Balanced)
            {
                // 切换到平衡状态：移除重力，保持父物体关系
                if (oldState != GravityState.Balanced)
                {
                    if (GravityManager.Instance != null)
                    {
                        GravityManager.Instance.UnregisterGravityForce(obj, oldForce);
                    }
                    else
                    {
                        RemoveGravityDirectly(obj, oldForce);
                    }
                }
                affectedObjects[obj] = Vector2.zero;
            }
            else
            {
                // 切换到重力状态：应用新的重力
                Vector2 direction = (transform.position - obj.transform.position).normalized;
                Vector2 newForce = direction * gravityStateDict[newState] * gravityExtent * moveSpeed;
                
                affectedObjects[obj] = newForce;
                
                if (GravityManager.Instance != null)
                {
                    GravityManager.Instance.RegisterGravityForce(obj, newForce);
                }
                else
                {
                    ApplyGravityDirectly(obj, newForce);
                }
            }
        }
    }
}
