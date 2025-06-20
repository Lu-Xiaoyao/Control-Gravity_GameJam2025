using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RendererLoad : MonoBehaviour
{
    [SerializeField] private List<Sprite> images = new List<Sprite>();
    [SerializeField] private SpriteRenderer spriteRenderer;
    
    [SerializeField] private int imageIndex = 0;
    
    void Start()
    {
        // 如果没有手动指定SpriteRenderer，自动获取
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        
        // 如果有图片，设置第一张图片
        if (images[imageIndex] != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = images[imageIndex];
        }
        else if(spriteRenderer != null && images.Count > 0)
        {
            spriteRenderer.sprite = images[0];
        }
    }
    
    #if UNITY_EDITOR
    /// <summary>
    /// 编辑器中的OnValidate回调，用于实时预览
    /// </summary>
    void OnValidate()
    {
        // 确保索引在有效范围内
        if (images.Count > 0)
        {
            imageIndex = Mathf.Clamp(imageIndex, 0, images.Count - 1);
        }
        else
        {
            imageIndex = 0;
        }
        
        // 在编辑器中实时预览
        if (Application.isPlaying == false)
        {
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }
            
            if (spriteRenderer != null && images.Count > 0 && imageIndex < images.Count)
            {
                spriteRenderer.sprite = images[imageIndex];
            }
        }
    }
    #endif
}
