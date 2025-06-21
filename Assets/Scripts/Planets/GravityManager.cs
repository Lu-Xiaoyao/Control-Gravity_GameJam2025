using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityManager : MonoBehaviour
{
    public static GravityManager Instance;
    
    [Header("重力设置")]
    [SerializeField] private float maxGravityForce = 10f; // 最大重力力
    [SerializeField] private bool enableGravityOverlap = true; // 是否启用重力叠加
    
    private Dictionary<GameObject, List<Vector2>> objectGravityForces = new Dictionary<GameObject, List<Vector2>>();
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// 注册重力影响
    /// </summary>
    /// <param name="target">目标物体</param>
    /// <param name="gravityForce">重力向量</param>
    public void RegisterGravityForce(GameObject target, Vector2 gravityForce)
    {
        if (!enableGravityOverlap)
        {
            // 如果不启用叠加，只保留最强的重力
            Vector2 strongestForce = GetStrongestGravityForce(target);
            if (gravityForce.magnitude > strongestForce.magnitude)
            {
                ClearGravityForces(target);
                AddGravityForce(target, gravityForce);
            }
            return;
        }
        
        AddGravityForce(target, gravityForce);
    }
    
    /// <summary>
    /// 移除重力影响
    /// </summary>
    /// <param name="target">目标物体</param>
    /// <param name="gravityForce">要移除的重力向量</param>
    public void UnregisterGravityForce(GameObject target, Vector2 gravityForce)
    {
        RemoveGravityForce(target, gravityForce);
    }
    
    /// <summary>
    /// 添加重力力
    /// </summary>
    private void AddGravityForce(GameObject target, Vector2 gravityForce)
    {
        if (!objectGravityForces.ContainsKey(target))
        {
            objectGravityForces[target] = new List<Vector2>();
        }
        
        objectGravityForces[target].Add(gravityForce);
        UpdateObjectVelocity(target);
    }
    
    /// <summary>
    /// 移除重力力
    /// </summary>
    private void RemoveGravityForce(GameObject target, Vector2 gravityForce)
    {
        if (objectGravityForces.ContainsKey(target))
        {
            objectGravityForces[target].Remove(gravityForce);
            
            if (objectGravityForces[target].Count == 0)
            {
                objectGravityForces.Remove(target);
            }
            
            UpdateObjectVelocity(target);
        }
    }
    
    /// <summary>
    /// 清除物体的所有重力影响
    /// </summary>
    /// <param name="target">目标物体</param>
    public void ClearGravityForces(GameObject target)
    {
        if (objectGravityForces.ContainsKey(target))
        {
            objectGravityForces.Remove(target);
            UpdateObjectVelocity(target);
        }
    }
    
    /// <summary>
    /// 更新物体速度
    /// </summary>
    /// <param name="target">目标物体</param>
    private void UpdateObjectVelocity(GameObject target)
    {
        if (target.CompareTag("Player"))
        {
            PlayerMove playerMove = target.GetComponent<PlayerMove>();
            if (playerMove != null)
            {
                Vector2 totalGravityForce = CalculateTotalGravityForce(target);
                playerMove.ClearAllGravity();
                
                if (totalGravityForce.magnitude > 0.1f)
                {
                    playerMove.AddGravityForce(totalGravityForce);
                }
            }
        }
        else if (target.CompareTag("Star"))
        {
            Rigidbody2D starRb = target.GetComponent<Rigidbody2D>();
            if (starRb != null)
            {
                Vector2 totalGravityForce = CalculateTotalGravityForce(target);
                starRb.velocity += totalGravityForce * Time.deltaTime;
            }
        }
    }
    
    /// <summary>
    /// 计算总重力力
    /// </summary>
    /// <param name="target">目标物体</param>
    /// <returns>总重力向量</returns>
    private Vector2 CalculateTotalGravityForce(GameObject target)
    {
        if (!objectGravityForces.ContainsKey(target))
        {
            return Vector2.zero;
        }
        
        Vector2 totalForce = Vector2.zero;
        foreach (Vector2 force in objectGravityForces[target])
        {
            totalForce += force;
        }
        
        // 限制最大重力力
        if (totalForce.magnitude > maxGravityForce)
        {
            totalForce = totalForce.normalized * maxGravityForce;
        }
        
        return totalForce;
    }
    
    /// <summary>
    /// 获取最强的重力力
    /// </summary>
    /// <param name="target">目标物体</param>
    /// <returns>最强的重力向量</returns>
    private Vector2 GetStrongestGravityForce(GameObject target)
    {
        if (!objectGravityForces.ContainsKey(target) || objectGravityForces[target].Count == 0)
        {
            return Vector2.zero;
        }
        
        Vector2 strongestForce = Vector2.zero;
        float maxMagnitude = 0f;
        
        foreach (Vector2 force in objectGravityForces[target])
        {
            if (force.magnitude > maxMagnitude)
            {
                maxMagnitude = force.magnitude;
                strongestForce = force;
            }
        }
        
        return strongestForce;
    }
    
    /// <summary>
    /// 获取物体当前的重力力数量
    /// </summary>
    /// <param name="target">目标物体</param>
    /// <returns>重力力数量</returns>
    public int GetGravityForceCount(GameObject target)
    {
        if (objectGravityForces.ContainsKey(target))
        {
            return objectGravityForces[target].Count;
        }
        return 0;
    }
    
    /// <summary>
    /// 设置是否启用重力叠加
    /// </summary>
    /// <param name="enable">是否启用</param>
    public void SetGravityOverlap(bool enable)
    {
        enableGravityOverlap = enable;
    }
} 