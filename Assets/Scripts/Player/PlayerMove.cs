using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AllControl;

public class PlayerMove : MonoBehaviour
{
    protected Rigidbody2D rb;
    private Vector2 baseVelocity; // 基础移动速度
    private Vector2 gravityVelocity; // 重力影响的速度
    private bool isInGravityField = false;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        baseVelocity = Vector2.right * GameManager.Instance.moveSpeed;
        gravityVelocity = Vector2.zero;
        UpdateVelocity();
    }

    public void ResetSpeed()
    {
        baseVelocity = Vector2.right * GameManager.Instance.moveSpeed;
        UpdateVelocity();
    }

    /// <summary>
    /// 更新最终速度
    /// </summary>
    private void UpdateVelocity()
    {
        if (isInGravityField)
        {
            // 在引力场中，使用重力速度
            rb.velocity = gravityVelocity;
        }
        else
        {
            // 不在引力场中，使用基础速度
            rb.velocity = baseVelocity;
        }
    }

    /// <summary>
    /// 添加重力影响
    /// </summary>
    /// <param name="gravityForce">重力力向量</param>
    public void AddGravityForce(Vector2 gravityForce)
    {
        gravityVelocity += gravityForce;
        isInGravityField = true;
        UpdateVelocity();
    }

    /// <summary>
    /// 移除重力影响
    /// </summary>
    /// <param name="gravityForce">要移除的重力力向量</param>
    public void RemoveGravityForce(Vector2 gravityForce)
    {
        gravityVelocity -= gravityForce;
        
        // 检查是否还在任何引力场中
        if (gravityVelocity.magnitude < 0.1f)
        {
            isInGravityField = false;
            gravityVelocity = Vector2.zero;
        }
        
        UpdateVelocity();
    }

    /// <summary>
    /// 清除所有重力影响
    /// </summary>
    public void ClearAllGravity()
    {
        gravityVelocity = Vector2.zero;
        isInGravityField = false;
        UpdateVelocity();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Planet"))
        {
            // 进入星球引力场，设置父物体
            transform.SetParent(collision.transform);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Planet"))
        {
            // 离开星球引力场，清除父物体
            transform.SetParent(null);
        }
    }
}
