using UnityEngine;

/// <summary>
/// 小地图调整示例
/// 展示如何根据游戏状态动态调整小地图
/// </summary>
public class MinimapAdjustmentExample : MonoBehaviour
{
    [Header("调整目标")]
    [SerializeField] private MinimapCustomizer customizer;
    
    [Header("预设配置")]
    [SerializeField] private float normalSize = 150f;
    [SerializeField] private float largeSize = 250f;
    [SerializeField] private float smallSize = 100f;
    
    [Header("自动调整")]
    [SerializeField] private bool enableAutoAdjust = true;
    [SerializeField] private float adjustInterval = 5f;
    
    private float lastAdjustTime;
    
    void Start()
    {
        if (customizer == null)
        {
            customizer = FindObjectOfType<MinimapCustomizer>();
        }
        
        // 应用默认设置
        ApplyNormalSettings();
    }
    
    void Update()
    {
        if (enableAutoAdjust && Time.time - lastAdjustTime > adjustInterval)
        {
            AutoAdjustBasedOnGameState();
            lastAdjustTime = Time.time;
        }
        
        // 测试按键
        HandleTestInputs();
    }
    
    void HandleTestInputs()
    {
        // 数字键1-3切换不同大小
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ApplySmallSettings();
            Debug.Log("应用小号设置");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ApplyNormalSettings();
            Debug.Log("应用正常设置");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ApplyLargeSettings();
            Debug.Log("应用大号设置");
        }
        
        // 数字键4-6切换不同位置
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SetPositionTopLeft();
            Debug.Log("设置左上角位置");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SetPositionTopRight();
            Debug.Log("设置右上角位置");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            SetPositionCenter();
            Debug.Log("设置中心位置");
        }
        
        // 数字键7-9切换不同样式
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            ApplyDarkStyle();
            Debug.Log("应用深色样式");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            ApplyLightStyle();
            Debug.Log("应用浅色样式");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            ApplyTransparentStyle();
            Debug.Log("应用透明样式");
        }
    }
    
    void AutoAdjustBasedOnGameState()
    {
        // 这里可以根据游戏状态自动调整
        // 例如：根据玩家数量、关卡大小、游戏模式等
        
        // 示例：根据玩家位置调整
        GameObject player = GameObject.Find("Player");
        if (player != null)
        {
            float playerY = player.transform.position.y;
            
            if (playerY > 10f) // 玩家在高处
            {
                ApplyLargeSettings();
                Debug.Log("玩家在高处，应用大号设置");
            }
            else if (playerY < -10f) // 玩家在低处
            {
                ApplySmallSettings();
                Debug.Log("玩家在低处，应用小号设置");
            }
            else // 玩家在中间
            {
                ApplyNormalSettings();
                Debug.Log("玩家在中间，应用正常设置");
            }
        }
    }
    
    // 大小设置
    public void ApplySmallSettings()
    {
        if (customizer != null)
        {
            customizer.SetMinimapSize(smallSize);
            customizer.SetIconSize(6f);
        }
    }
    
    public void ApplyNormalSettings()
    {
        if (customizer != null)
        {
            customizer.SetMinimapSize(normalSize);
            customizer.SetIconSize(8f);
        }
    }
    
    public void ApplyLargeSettings()
    {
        if (customizer != null)
        {
            customizer.SetMinimapSize(largeSize);
            customizer.SetIconSize(12f);
        }
    }
    
    // 位置设置
    public void SetPositionTopLeft()
    {
        if (customizer != null)
        {
            customizer.SetMinimapPosition(new Vector2(20, -20));
        }
    }
    
    public void SetPositionTopRight()
    {
        if (customizer != null)
        {
            float x = Screen.width - 170;
            customizer.SetMinimapPosition(new Vector2(x, -20));
        }
    }
    
    public void SetPositionCenter()
    {
        if (customizer != null)
        {
            float x = Screen.width / 2 - 75;
            float y = -Screen.height / 2 + 75;
            customizer.SetMinimapPosition(new Vector2(x, y));
        }
    }
    
    // 样式设置
    public void ApplyDarkStyle()
    {
        if (customizer != null)
        {
            customizer.SetBackgroundColor(new Color(0, 0, 0, 0.8f));
        }
    }
    
    public void ApplyLightStyle()
    {
        if (customizer != null)
        {
            customizer.SetBackgroundColor(new Color(0, 0, 0, 0.3f));
        }
    }
    
    public void ApplyTransparentStyle()
    {
        if (customizer != null)
        {
            customizer.SetBackgroundColor(new Color(0, 0, 0, 0.1f));
        }
    }
    
    // 根据屏幕分辨率调整
    public void AdjustForScreenResolution()
    {
        if (customizer != null)
        {
            float screenRatio = (float)Screen.width / Screen.height;
            
            if (screenRatio > 1.5f) // 宽屏
            {
                customizer.SetMinimapSize(200f);
                customizer.SetMinimapPosition(new Vector2(30, -30));
            }
            else if (screenRatio < 0.8f) // 竖屏
            {
                customizer.SetMinimapSize(120f);
                customizer.SetMinimapPosition(new Vector2(10, -10));
            }
            else // 标准比例
            {
                customizer.SetMinimapSize(150f);
                customizer.SetMinimapPosition(new Vector2(20, -20));
            }
        }
    }
    
    // 根据游戏难度调整
    public void AdjustForDifficulty(int difficulty)
    {
        if (customizer != null)
        {
            switch (difficulty)
            {
                case 1: // 简单
                    customizer.SetMinimapSize(200f);
                    customizer.SetIconSize(12f);
                    break;
                case 2: // 普通
                    customizer.SetMinimapSize(150f);
                    customizer.SetIconSize(8f);
                    break;
                case 3: // 困难
                    customizer.SetMinimapSize(100f);
                    customizer.SetIconSize(6f);
                    break;
            }
        }
    }
    
    // 根据关卡类型调整
    public void AdjustForLevelType(string levelType)
    {
        if (customizer != null)
        {
            switch (levelType.ToLower())
            {
                case "platform":
                    // 平台跳跃关卡
                    customizer.SetMinimapSize(120f);
                    customizer.SetIconSize(8f);
                    break;
                case "exploration":
                    // 探索关卡
                    customizer.SetMinimapSize(200f);
                    customizer.SetIconSize(12f);
                    break;
                case "puzzle":
                    // 解谜关卡
                    customizer.SetMinimapSize(150f);
                    customizer.SetIconSize(10f);
                    break;
                case "boss":
                    // Boss关卡
                    customizer.SetMinimapSize(180f);
                    customizer.SetIconSize(10f);
                    break;
            }
        }
    }
    
    [ContextMenu("应用默认设置")]
    public void ApplyDefaultSettings()
    {
        ApplyNormalSettings();
        SetPositionTopLeft();
        ApplyLightStyle();
        Debug.Log("已应用默认设置");
    }
    
    [ContextMenu("应用移动设备设置")]
    public void ApplyMobileSettings()
    {
        if (customizer != null)
        {
            customizer.SetMinimapSize(120f);
            customizer.SetIconSize(10f);
            customizer.SetMinimapPosition(new Vector2(15, -15));
            customizer.SetBackgroundColor(new Color(0, 0, 0, 0.6f));
        }
        Debug.Log("已应用移动设备设置");
    }
    
    [ContextMenu("应用PC设置")]
    public void ApplyPCSettings()
    {
        if (customizer != null)
        {
            customizer.SetMinimapSize(180f);
            customizer.SetIconSize(10f);
            customizer.SetMinimapPosition(new Vector2(25, -25));
            customizer.SetBackgroundColor(new Color(0, 0, 0, 0.4f));
        }
        Debug.Log("已应用PC设置");
    }
} 