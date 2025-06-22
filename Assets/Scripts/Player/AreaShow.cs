using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AreaShow : MonoBehaviour
{
    public UnityEvent onAreaShow;

    void Awake()
    {
        // 不再创建自己的InputActions实例，使用全局实例
    }

    public void OnAreaShow()
    {
        onAreaShow.Invoke();
    }
}
