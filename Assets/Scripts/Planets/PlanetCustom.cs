using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AllControl;

[System.Serializable]
public class ColliderSettings
{
    public int imageIndex;
    public float planetColliderRadius = 4.21f;
    public float gravityColliderRadius = 0.5f;
    public Vector2 planetColliderOffset = Vector2.zero;
    public Vector2 gravityColliderOffset = Vector2.zero;
    public TouchEffectType touchEffectType = TouchEffectType.None;
    public GameObject controlledObject;
    public bool isControllable = true;
}

public class PlanetCustom : MonoBehaviour
{
    // 静态字典，用于存储所有星球的外观设置，实现全局同步
    private static Dictionary<int, ColliderSettings> globalColliderSettings = new Dictionary<int, ColliderSettings>();
    
    [SerializeField] private List<Sprite> images = new List<Sprite>();
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private PlanetGravity planetGravity;

    //外观设置
    [SerializeField] protected internal int imageIndex = 0;
    [SerializeField] protected internal float planetSize = 1f;
    [SerializeField] protected internal float gravityExtent = 1.5f;
    [SerializeField] protected internal float gravitySize = 20f;
    
    //触碰效果和无法控制效果设置
    [SerializeField] protected internal TouchEffectType touchEffectType = TouchEffectType.None;
    [SerializeField] protected internal bool isControllable = true;
    
    // 碰撞器设置
    [SerializeField] private List<ColliderSettings> colliderSettings = new List<ColliderSettings>();
    [SerializeField] private CircleCollider2D planetCollider;
    [SerializeField] private CircleCollider2D gravityCollider;

    void Awake()
    {
        //planetGravity = transform.Find("GravityArea").GetComponent<PlanetGravity>();
        
        // 获取碰撞器组件
        if (planetCollider == null)
        {
            planetCollider = GetComponent<CircleCollider2D>();
        }
        if (gravityCollider == null)
        {
            Transform gravityArea = transform.Find("GravityArea");
            if (gravityArea != null)
            {
                gravityCollider = gravityArea.GetComponent<CircleCollider2D>();
            }
        }
        
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

        //设置大小和重力
        transform.localScale = new Vector3(planetSize, planetSize, 1);
        transform.Find("GravityArea").localScale = new Vector3(gravitySize, gravitySize, 1);
        
        // 只在没有设置过的情况下才随机设置gravityExtent
        if (gravityExtent == 1.5f) // 默认值
        {
            gravityExtent = Random.Range(GameManager.Instance.gravityExtentMin, GameManager.Instance.gravityExtentMax);
        }
        //planetGravity.gravityExtent = gravityExtent;

        // 应用碰撞器设置
        ApplyColliderSettings();
        
        // 应用碰撞效果和控制性设置（保持当前值）
        ApplyTouchEffectSettings();
        ApplyControllableSettings();
    }
    
    /// <summary>
    /// 应用当前外观的碰撞器设置
    /// </summary>
    private void ApplyColliderSettings()
    {
        ColliderSettings settings = GetColliderSettings(imageIndex);
        if (settings != null)
        {
            if (planetCollider != null)
            {
                planetCollider.radius = settings.planetColliderRadius;
                planetCollider.offset = settings.planetColliderOffset;
            }
            if (gravityCollider != null)
            {
                gravityCollider.radius = settings.gravityColliderRadius;
                gravityCollider.offset = settings.gravityColliderOffset;
            }
            // 碰撞效果和控制性设置不从全局设置加载，保持当前星球的值
        }
    }
    
    /// <summary>
    /// 应用当前外观的碰撞效果设置
    /// </summary>
    private void ApplyTouchEffectSettings()
    {
        // 获取或添加TouchEffect组件
        TouchEffect touchEffect = GetComponent<TouchEffect>();
        if (touchEffect == null)
        {
            touchEffect = gameObject.AddComponent<TouchEffect>();
        }
        
        // 使用反射设置TouchEffect组件的属性
        var touchEffectTypeField = typeof(TouchEffect).GetField("touchEffectType", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var controlledObjectField = typeof(TouchEffect).GetField("controlledObject", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        if (touchEffectTypeField != null)
        {
            touchEffectTypeField.SetValue(touchEffect, touchEffectType);
        }
        
        // 注意：controlledObject需要从编辑器传入，这里暂时不设置
        // 如果需要，可以通过其他方式传递
    }
    
    /// <summary>
    /// 应用当前外观的控制性设置
    /// </summary>
    private void ApplyControllableSettings()
    {
        // 控制性设置直接保存在当前星球的字段中
        // 其他脚本可以通过访问isControllable字段来获取这个设置
        // 这里不需要额外的操作，因为字段值已经更新了
    }
    
    /// <summary>
    /// 获取指定外观的碰撞器设置（从全局字典）
    /// </summary>
    /// <param name="index">图片索引</param>
    /// <returns>碰撞器设置</returns>
    public ColliderSettings GetColliderSettings(int index)
    {
        // 优先从全局字典获取
        if (globalColliderSettings.ContainsKey(index))
        {
            return globalColliderSettings[index];
        }
        
        // 如果全局字典没有，从本地设置获取（兼容旧数据）
        foreach (var settings in colliderSettings)
        {
            if (settings.imageIndex == index)
            {
                // 将本地设置同步到全局字典
                globalColliderSettings[index] = settings;
                return settings;
            }
        }
        return null;
    }
    
    /// <summary>
    /// 保存当前外观的碰撞器设置（保存到全局字典）
    /// </summary>
    /// <param name="planetRadius">星球碰撞器半径</param>
    /// <param name="gravityRadius">引力碰撞器半径</param>
    /// <param name="planetOffset">星球碰撞器偏移</param>
    /// <param name="gravityOffset">引力碰撞器偏移</param>
    /// <param name="effectType">碰撞效果类型</param>
    /// <param name="controlledObj">受控制的对象</param>
    /// <param name="controllable">能否被玩家控制</param>
    public void SaveColliderSettings(float planetRadius, float gravityRadius, Vector2 planetOffset, Vector2 gravityOffset, TouchEffectType effectType, GameObject controlledObj, bool controllable)
    {
        // 保存碰撞器设置到全局字典（用于同外观星球同步）
        ColliderSettings settings = GetColliderSettings(imageIndex);
        if (settings == null)
        {
            settings = new ColliderSettings();
            settings.imageIndex = imageIndex;
        }
        
        settings.planetColliderRadius = planetRadius;
        settings.gravityColliderRadius = gravityRadius;
        settings.planetColliderOffset = planetOffset;
        settings.gravityColliderOffset = gravityOffset;
        // 碰撞效果和控制性设置不保存到全局字典，只对当前星球生效
        
        // 保存到全局字典
        globalColliderSettings[imageIndex] = settings;
        
        // 同时保存到本地设置（兼容性）
        bool foundInLocal = false;
        for (int i = 0; i < colliderSettings.Count; i++)
        {
            if (colliderSettings[i].imageIndex == imageIndex)
            {
                colliderSettings[i] = settings;
                foundInLocal = true;
                break;
            }
        }
        if (!foundInLocal)
        {
            colliderSettings.Add(settings);
        }
        
        // 立即应用碰撞器设置
        if (planetCollider != null)
        {
            planetCollider.radius = planetRadius;
            planetCollider.offset = planetOffset;
        }
        if (gravityCollider != null)
        {
            gravityCollider.radius = gravityRadius;
            gravityCollider.offset = gravityOffset;
        }
        
        // 保存碰撞效果和控制性设置到当前星球（不保存到全局字典）
        touchEffectType = effectType;
        isControllable = controllable;
        
        // 应用碰撞效果设置（仅对当前星球）
        ApplyTouchEffectSettings();
        
        // 应用控制性设置（仅对当前星球）
        ApplyControllableSettings();
        
        // 通知所有相同外观的星球更新碰撞器设置（不包括碰撞效果和控制性）
        NotifyAllPlanetsWithSameAppearance();
    }
    
    /// <summary>
    /// 通知所有相同外观的星球更新碰撞器设置（不包括碰撞效果和控制性）
    /// </summary>
    private void NotifyAllPlanetsWithSameAppearance()
    {
        // 查找场景中所有使用相同外观的星球
        PlanetCustom[] allPlanets = FindObjectsOfType<PlanetCustom>();
        var imageIndexField = typeof(PlanetCustom).GetField("imageIndex", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        foreach (var planet in allPlanets)
        {
            if (planet != this)
            {
                int planetImageIndex = 0;
                if (imageIndexField != null)
                {
                    planetImageIndex = (int)imageIndexField.GetValue(planet);
                }
                
                if (planetImageIndex == this.imageIndex)
                {
                    // 只同步碰撞器设置，不同步碰撞效果和控制性设置
                    planet.ApplyColliderSettings();
                }
            }
        }
    }
    
    #if UNITY_EDITOR
    /// <summary>
    /// 编辑器启动时恢复全局设置
    /// </summary>
    [UnityEditor.InitializeOnLoadMethod]
    private static void RestoreGlobalSettings()
    {
        // 清空全局字典，准备重新加载
        globalColliderSettings.Clear();
        
        // 查找场景中所有星球，重新加载设置到全局字典
        PlanetCustom[] allPlanets = FindObjectsOfType<PlanetCustom>();
        foreach (var planet in allPlanets)
        {
            // 触发设置加载
            planet.LoadSettingsToGlobal();
        }
    }
    
    /// <summary>
    /// 将本地设置加载到全局字典
    /// </summary>
    private void LoadSettingsToGlobal()
    {
        foreach (var settings in colliderSettings)
        {
            globalColliderSettings[settings.imageIndex] = settings;
        }
    }
    
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
            
            // 实时更新星球大小
            transform.localScale = new Vector3(planetSize, planetSize, 1);
            
            // 实时更新引力场大小
            Transform gravityArea = transform.Find("GravityArea");
            if (gravityArea != null)
            {
                gravityArea.localScale = new Vector3(gravitySize, gravitySize, 1);
            }
            
            // 实时更新引力强度
            if (planetGravity != null)
            {
                var gravityExtentField = typeof(PlanetGravity).GetField("gravityExtent", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (gravityExtentField != null)
                {
                    gravityExtentField.SetValue(planetGravity, gravityExtent);
                }
            }
            
            // 获取碰撞器组件（如果还没有获取）
            if (planetCollider == null)
            {
                planetCollider = GetComponent<CircleCollider2D>();
            }
            if (gravityCollider == null)
            {
                Transform gravityAreaTransform = transform.Find("GravityArea");
                if (gravityAreaTransform != null)
                {
                    gravityCollider = gravityAreaTransform.GetComponent<CircleCollider2D>();
                }
            }
            
            // 应用已保存的碰撞器设置
            ApplyColliderSettings();
            
            // 注意：碰撞效果和控制性设置只对当前星球生效，不从全局设置读取
        }
    }
    #endif
}
