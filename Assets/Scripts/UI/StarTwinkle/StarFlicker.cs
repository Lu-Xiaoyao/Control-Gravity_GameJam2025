using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarFlicker : MonoBehaviour
{
    // 【可在Inspector调整】闪烁速度：数值越大，透明度变化越快
    public float flickerSpeed = 1.0f;

    // 【可在Inspector调整】透明度范围：min是最暗，max是最亮（0~1）
    public float minAlpha = 0.2f;
    public float maxAlpha = 1.0f;

    // 存储Image组件，用来改透明度
    private UnityEngine.UI.Image starImage;

    // 目标透明度（每次随机变化）
    private float targetAlpha;

    void Start()
    {
        // 获取Image组件（如果是Sprite，就找SpriteRenderer）
        starImage = GetComponent<UnityEngine.UI.Image>();

        // 初始化随机透明度目标
        targetAlpha = Random.Range(minAlpha, maxAlpha);
    }

    void Update()
    {
        // 1. 平滑过渡到目标透明度（让闪烁更自然）
        Color currentColor = starImage.color;
        currentColor.a = Mathf.Lerp(currentColor.a, targetAlpha, Time.deltaTime * flickerSpeed);
        starImage.color = currentColor;

        // 2. 接近目标时，随机换目标透明度（实现“忽明忽暗”）
        if (Mathf.Abs(currentColor.a - targetAlpha) < 0.01f)
        {
            targetAlpha = Random.Range(minAlpha, maxAlpha);
        }
    }
}
