using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 小地图快速修复工具
/// 自动检测和修复常见的小地图问题
/// </summary>
public class MinimapQuickFix : MonoBehaviour
{
    [Header("自动修复设置")]
    [SerializeField] private bool autoFixOnStart = true;
    [SerializeField] private bool createDebugMinimap = true;
    [SerializeField] private bool fixCanvasIssues = true;
    [SerializeField] private bool fixTagIssues = true;
    
    [Header("修复选项")]
    [SerializeField] private bool forceCreateCanvas = false;
    [SerializeField] private bool resetMinimapPosition = true;
    [SerializeField] private Vector2 newMinimapPosition = new Vector2(50, -50);
    
    void Start()
    {
        if (autoFixOnStart)
        {
            StartCoroutine(DelayedFix());
        }
    }
    
    System.Collections.IEnumerator DelayedFix()
    {
        yield return null;
        Debug.Log("=== 开始小地图快速修复 ===");
        FixMinimapIssues();
        Debug.Log("=== 小地图快速修复完成 ===");
    }
    
    [ContextMenu("快速修复小地图")]
    public void FixMinimapIssues()
    {
        if (fixCanvasIssues)
        {
            FixCanvasIssues();
        }
        
        if (fixTagIssues)
        {
            FixTagIssues();
        }
        
        if (createDebugMinimap)
        {
            CreateDebugMinimap();
        }
        
        CheckExistingMinimaps();
    }
    
    void FixCanvasIssues()
    {
        Debug.Log("检查Canvas问题...");
        
        Canvas canvas = FindObjectOfType<Canvas>();
        
        if (canvas == null || forceCreateCanvas)
        {
            Debug.Log("创建新Canvas...");
            canvas = CreateProperCanvas();
        }
        else
        {
            Debug.Log($"修复现有Canvas: {canvas.name}");
            FixCanvasSettings(canvas);
        }
    }
    
    Canvas CreateProperCanvas()
    {
        GameObject canvasObj = new GameObject("FixedCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;
        
        canvasObj.AddComponent<GraphicRaycaster>();
        
        Debug.Log("已创建标准Canvas");
        return canvas;
    }
    
    void FixCanvasSettings(Canvas canvas)
    {
        if (canvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            Debug.Log("已修复Canvas渲染模式");
        }
        
        CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
        if (scaler == null)
        {
            scaler = canvas.gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
            Debug.Log("已添加CanvasScaler");
        }
        
        GraphicRaycaster raycaster = canvas.GetComponent<GraphicRaycaster>();
        if (raycaster == null)
        {
            canvas.gameObject.AddComponent<GraphicRaycaster>();
            Debug.Log("已添加GraphicRaycaster");
        }
        
        if (!canvas.gameObject.activeInHierarchy)
        {
            canvas.gameObject.SetActive(true);
            Debug.Log("已激活Canvas");
        }
    }
    
    void FixTagIssues()
    {
        Debug.Log("检查标签问题...");
        
        GameObject player = GameObject.Find("Player");
        if (player != null && player.tag != "Player")
        {
            player.tag = "Player";
            Debug.Log("已修复Player标签");
        }
        
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        int planetCount = 0;
        foreach (GameObject obj in allObjects)
        {
            if (obj.GetComponent<PlanetCustom>() != null && obj.tag != "Planet")
            {
                obj.tag = "Planet";
                planetCount++;
            }
        }
        if (planetCount > 0)
        {
            Debug.Log($"已修复{planetCount}个Planet标签");
        }
        
        int doorCount = 0;
        foreach (GameObject obj in allObjects)
        {
            if (obj.GetComponent<DoorOpen>() != null && obj.tag != "Door")
            {
                obj.tag = "Door";
                doorCount++;
            }
        }
        if (doorCount > 0)
        {
            Debug.Log($"已修复{doorCount}个Door标签");
        }
        
        int starCount = 0;
        foreach (GameObject obj in allObjects)
        {
            if (obj.GetComponent<StarFit>() != null && obj.tag != "Star")
            {
                obj.tag = "Star";
                starCount++;
            }
        }
        if (starCount > 0)
        {
            Debug.Log($"已修复{starCount}个Star标签");
        }
    }
    
    void CreateDebugMinimap()
    {
        Debug.Log("创建调试小地图...");
        
        DebugMinimap existingDebug = FindObjectOfType<DebugMinimap>();
        if (existingDebug != null)
        {
            Debug.Log("已存在调试小地图，跳过创建");
            return;
        }
        
        GameObject debugObj = new GameObject("DebugMinimap");
        DebugMinimap debugMinimap = debugObj.AddComponent<DebugMinimap>();
        
        Debug.Log("已创建调试小地图");
    }
    
    void CheckExistingMinimaps()
    {
        Debug.Log("检查现有小地图...");
        
        SimpleMinimap simpleMinimap = FindObjectOfType<SimpleMinimap>();
        AdvancedMinimap advancedMinimap = FindObjectOfType<AdvancedMinimap>();
        MinimapSetup minimapSetup = FindObjectOfType<MinimapSetup>();
        
        if (simpleMinimap != null)
        {
            Debug.Log("找到SimpleMinimap");
        }
        
        if (advancedMinimap != null)
        {
            Debug.Log("找到AdvancedMinimap");
        }
        
        if (minimapSetup != null)
        {
            Debug.Log("找到MinimapSetup");
        }
        
        if (simpleMinimap == null && advancedMinimap == null && minimapSetup == null)
        {
            Debug.LogWarning("未找到任何小地图组件，建议使用MinimapSetup或DebugMinimap");
        }
    }
    
    [ContextMenu("检查所有问题")]
    public void CheckAllIssues()
    {
        Debug.Log("=== 全面检查小地图问题 ===");
        
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("❌ 没有找到Canvas");
        }
        else
        {
            Debug.Log($"✅ 找到Canvas: {canvas.name}");
        }
        
        SimpleMinimap simple = FindObjectOfType<SimpleMinimap>();
        AdvancedMinimap advanced = FindObjectOfType<AdvancedMinimap>();
        DebugMinimap debug = FindObjectOfType<DebugMinimap>();
        
        if (simple != null) Debug.Log("✅ 找到SimpleMinimap");
        if (advanced != null) Debug.Log("✅ 找到AdvancedMinimap");
        if (debug != null) Debug.Log("✅ 找到DebugMinimap");
        
        if (simple == null && advanced == null && debug == null)
        {
            Debug.LogError("❌ 没有找到任何小地图组件");
        }
        
        GameObject player = GameObject.Find("Player");
        if (player != null)
        {
            if (player.tag == "Player")
                Debug.Log("✅ Player标签正确");
            else
                Debug.LogWarning($"⚠️ Player标签错误: {player.tag}");
        }
        else
        {
            Debug.LogWarning("⚠️ 没有找到Player对象");
        }
        
        GameObject[] planets = GameObject.FindGameObjectsWithTag("Planet");
        Debug.Log($"✅ 找到{planets.Length}个Planet对象");
        
        Debug.Log("=== 检查完成 ===");
    }
} 