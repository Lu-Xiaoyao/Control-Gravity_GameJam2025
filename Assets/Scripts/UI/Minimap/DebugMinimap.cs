using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 调试版小地图 - 用于排查小地图不显示的问题
/// </summary>
public class DebugMinimap : MonoBehaviour
{
    [Header("调试设置")]
    [SerializeField] private bool enableDebug = true;
    [SerializeField] private bool forceShow = true;
    [SerializeField] private bool showDebugInfo = true;
    
    [Header("小地图设置")]
    [SerializeField] private float minimapSize = 200f;
    [SerializeField] private Vector2 minimapPosition = new Vector2(50, -50);
    [SerializeField] private Color backgroundColor = Color.red; // 使用红色便于识别
    [SerializeField] private Color borderColor = Color.white;
    
    [Header("测试设置")]
    [SerializeField] private bool createTestIcons = true;
    [SerializeField] private int testIconCount = 5;
    
    private RectTransform minimapRect;
    private Canvas canvas;
    private bool isInitialized = false;
    
    void Start()
    {
        if (enableDebug)
        {
            Debug.Log("=== 小地图调试开始 ===");
            InitializeDebugMinimap();
        }
    }
    
    void Update()
    {
        if (enableDebug && isInitialized)
        {
            // 测试按键
            if (Input.GetKeyDown(KeyCode.M))
            {
                ToggleMinimap();
                Debug.Log("按下了M键，切换小地图显示");
            }
            
            if (Input.GetKeyDown(KeyCode.R))
            {
                ResetMinimap();
                Debug.Log("按下了R键，重置小地图");
            }
        }
    }
    
    void InitializeDebugMinimap()
    {
        Debug.Log("开始初始化调试小地图...");
        
        // 1. 查找或创建Canvas
        canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.Log("未找到Canvas，正在创建...");
            canvas = CreateCanvas();
        }
        else
        {
            Debug.Log($"找到现有Canvas: {canvas.name}");
        }
        
        // 2. 创建小地图容器
        CreateMinimapContainer();
        
        // 3. 创建测试图标
        if (createTestIcons)
        {
            CreateTestIcons();
        }
        
        // 4. 显示调试信息
        if (showDebugInfo)
        {
            ShowDebugInfo();
        }
        
        isInitialized = true;
        Debug.Log("=== 小地图调试初始化完成 ===");
    }
    
    Canvas CreateCanvas()
    {
        GameObject canvasObj = new GameObject("DebugCanvas");
        Canvas newCanvas = canvasObj.AddComponent<Canvas>();
        newCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;
        
        canvasObj.AddComponent<GraphicRaycaster>();
        
        Debug.Log("已创建新的Canvas");
        return newCanvas;
    }
    
    void CreateMinimapContainer()
    {
        Debug.Log("创建小地图容器...");
        
        // 创建主容器
        GameObject containerObj = new GameObject("DebugMinimapContainer");
        minimapRect = containerObj.AddComponent<RectTransform>();
        minimapRect.SetParent(canvas.transform, false);
        
        // 设置位置和大小
        minimapRect.anchorMin = new Vector2(0, 1);
        minimapRect.anchorMax = new Vector2(0, 1);
        minimapRect.sizeDelta = new Vector2(minimapSize, minimapSize);
        minimapRect.anchoredPosition = minimapPosition;
        
        Debug.Log($"小地图容器位置: {minimapRect.anchoredPosition}, 大小: {minimapRect.sizeDelta}");
        
        // 创建背景
        GameObject backgroundObj = new GameObject("MinimapBackground");
        backgroundObj.transform.SetParent(minimapRect, false);
        
        Image bgImage = backgroundObj.AddComponent<Image>();
        bgImage.color = backgroundColor;
        
        RectTransform bgRect = backgroundObj.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;
        bgRect.anchoredPosition = Vector2.zero;
        
        Debug.Log($"背景颜色: {backgroundColor}");
        
        // 创建边框
        GameObject borderObj = new GameObject("MinimapBorder");
        borderObj.transform.SetParent(minimapRect, false);
        
        Image borderImage = borderObj.AddComponent<Image>();
        borderImage.color = borderColor;
        
        RectTransform borderRect = borderObj.GetComponent<RectTransform>();
        borderRect.anchorMin = Vector2.zero;
        borderRect.anchorMax = Vector2.one;
        borderRect.sizeDelta = new Vector2(3, 3);
        borderRect.anchoredPosition = Vector2.zero;
        
        // 创建中心点标记
        GameObject centerObj = new GameObject("CenterMark");
        centerObj.transform.SetParent(minimapRect, false);
        
        Image centerImage = centerObj.AddComponent<Image>();
        centerImage.color = Color.yellow;
        
        RectTransform centerRect = centerObj.GetComponent<RectTransform>();
        centerRect.anchorMin = new Vector2(0.5f, 0.5f);
        centerRect.anchorMax = new Vector2(0.5f, 0.5f);
        centerRect.sizeDelta = new Vector2(10, 10);
        centerRect.anchoredPosition = Vector2.zero;
        
        Debug.Log("小地图容器创建完成");
    }
    
    void CreateTestIcons()
    {
        Debug.Log($"创建{testIconCount}个测试图标...");
        
        for (int i = 0; i < testIconCount; i++)
        {
            GameObject iconObj = new GameObject($"TestIcon_{i}");
            iconObj.transform.SetParent(minimapRect, false);
            
            Image iconImage = iconObj.AddComponent<Image>();
            iconImage.color = new Color(Random.value, Random.value, Random.value, 1f);
            
            RectTransform iconRect = iconObj.GetComponent<RectTransform>();
            iconRect.anchorMin = new Vector2(0.5f, 0.5f);
            iconRect.anchorMax = new Vector2(0.5f, 0.5f);
            iconRect.sizeDelta = new Vector2(15, 15);
            
            // 随机位置
            float angle = (360f / testIconCount) * i;
            float radius = 50f;
            Vector2 position = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad) * radius,
                Mathf.Sin(angle * Mathf.Deg2Rad) * radius
            );
            iconRect.anchoredPosition = position;
            
            Debug.Log($"创建测试图标 {i}: 位置 {position}, 颜色 {iconImage.color}");
        }
    }
    
    void ShowDebugInfo()
    {
        Debug.Log("=== 小地图调试信息 ===");
        Debug.Log($"Canvas: {(canvas != null ? canvas.name : "null")}");
        Debug.Log($"Canvas RenderMode: {(canvas != null ? canvas.renderMode.ToString() : "N/A")}");
        Debug.Log($"小地图容器: {(minimapRect != null ? minimapRect.name : "null")}");
        Debug.Log($"小地图位置: {(minimapRect != null ? minimapRect.anchoredPosition.ToString() : "N/A")}");
        Debug.Log($"小地图大小: {(minimapRect != null ? minimapRect.sizeDelta.ToString() : "N/A")}");
        Debug.Log($"小地图是否激活: {(minimapRect != null ? minimapRect.gameObject.activeInHierarchy.ToString() : "N/A")}");
        Debug.Log($"屏幕分辨率: {Screen.width}x{Screen.height}");
        
        // 检查Canvas的子对象
        if (canvas != null)
        {
            Debug.Log($"Canvas子对象数量: {canvas.transform.childCount}");
            for (int i = 0; i < canvas.transform.childCount; i++)
            {
                Transform child = canvas.transform.GetChild(i);
                Debug.Log($"  - {child.name} (激活: {child.gameObject.activeInHierarchy})");
            }
        }
        
        Debug.Log("=== 调试信息结束 ===");
    }
    
    public void ToggleMinimap()
    {
        if (minimapRect != null)
        {
            bool newState = !minimapRect.gameObject.activeInHierarchy;
            minimapRect.gameObject.SetActive(newState);
            Debug.Log($"小地图显示状态切换为: {newState}");
        }
        else
        {
            Debug.LogWarning("小地图容器为空，无法切换显示状态");
        }
    }
    
    public void ResetMinimap()
    {
        Debug.Log("重置小地图...");
        
        if (minimapRect != null)
        {
            minimapRect.gameObject.SetActive(true);
            minimapRect.anchoredPosition = minimapPosition;
            Debug.Log("小地图已重置");
        }
    }
    
    void OnGUI()
    {
        if (enableDebug && showDebugInfo)
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 200));
            GUILayout.Label("=== 小地图调试信息 ===");
            GUILayout.Label($"Canvas: {(canvas != null ? "存在" : "不存在")}");
            GUILayout.Label($"小地图: {(minimapRect != null ? "存在" : "不存在")}");
            GUILayout.Label($"小地图激活: {(minimapRect != null ? minimapRect.gameObject.activeInHierarchy.ToString() : "N/A")}");
            GUILayout.Label($"按M键切换显示");
            GUILayout.Label($"按R键重置位置");
            GUILayout.EndArea();
        }
    }
    
    [ContextMenu("强制显示小地图")]
    public void ForceShowMinimap()
    {
        if (minimapRect != null)
        {
            minimapRect.gameObject.SetActive(true);
            Debug.Log("强制显示小地图");
        }
    }
    
    [ContextMenu("隐藏小地图")]
    public void HideMinimap()
    {
        if (minimapRect != null)
        {
            minimapRect.gameObject.SetActive(false);
            Debug.Log("隐藏小地图");
        }
    }
    
    [ContextMenu("显示调试信息")]
    public void ShowDebugInfoContext()
    {
        ShowDebugInfo();
    }
} 