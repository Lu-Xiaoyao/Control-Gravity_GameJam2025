using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 小地图快速设置脚本
/// 这个脚本可以帮助您快速设置小地图系统
/// </summary>
public class MinimapSetup : MonoBehaviour
{
    [Header("小地图类型选择")]
    [SerializeField] private bool useSimpleMinimap = true;
    [SerializeField] private bool useAdvancedMinimap = false;
    
    [Header("快速设置")]
    [SerializeField] private bool autoSetup = true;
    [SerializeField] private bool createCanvasIfNeeded = true;
    [SerializeField] private bool setupTags = true;
    
    [Header("预设配置")]
    [SerializeField] private MinimapPreset[] presets;
    [SerializeField] private int selectedPreset = 0;
    
    [System.Serializable]
    public class MinimapPreset
    {
        public string presetName;
        public float minimapSize = 150f;
        public Vector2 minimapPosition = new Vector2(20, -20);
        public Color backgroundColor = new Color(0, 0, 0, 0.5f);
        public Color borderColor = Color.white;
        public float iconSize = 8f;
        public float mapRadius = 30f;
        public KeyCode toggleKey = KeyCode.M;
    }
    
    void Start()
    {
        if (autoSetup)
        {
            SetupMinimap();
        }
    }
    
    [ContextMenu("设置小地图")]
    public void SetupMinimap()
    {
        // 检查并设置标签
        if (setupTags)
        {
            SetupTags();
        }
        
        // 创建或查找Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null && createCanvasIfNeeded)
        {
            canvas = CreateCanvas();
        }
        
        // 创建小地图管理器
        GameObject minimapManager = CreateMinimapManager();
        
        // 应用预设配置
        if (presets != null && presets.Length > 0 && selectedPreset < presets.Length)
        {
            ApplyPreset(minimapManager, presets[selectedPreset]);
        }
        
        Debug.Log("小地图设置完成！");
    }
    
    void SetupTags()
    {
        // 检查玩家标签
        GameObject player = GameObject.Find("Player");
        if (player != null && player.tag != "Player")
        {
            player.tag = "Player";
            Debug.Log("已设置Player标签");
        }
        
        // 检查星球标签
        GameObject[] planets = GameObject.FindGameObjectsWithTag("Planet");
        if (planets.Length == 0)
        {
            // 查找可能的星球对象
            GameObject[] allObjects = FindObjectsOfType<GameObject>();
            foreach (GameObject obj in allObjects)
            {
                if (obj.GetComponent<PlanetCustom>() != null && obj.tag != "Planet")
                {
                    obj.tag = "Planet";
                    Debug.Log($"已设置Planet标签: {obj.name}");
                }
            }
        }
        
        // 检查门标签
        GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");
        if (doors.Length == 0)
        {
            GameObject[] allObjects = FindObjectsOfType<GameObject>();
            foreach (GameObject obj in allObjects)
            {
                if (obj.GetComponent<DoorOpen>() != null && obj.tag != "Door")
                {
                    obj.tag = "Door";
                    Debug.Log($"已设置Door标签: {obj.name}");
                }
            }
        }
        
        // 检查星星标签
        GameObject[] stars = GameObject.FindGameObjectsWithTag("Star");
        if (stars.Length == 0)
        {
            GameObject[] allObjects = FindObjectsOfType<GameObject>();
            foreach (GameObject obj in allObjects)
            {
                if (obj.GetComponent<StarFit>() != null && obj.tag != "Star")
                {
                    obj.tag = "Star";
                    Debug.Log($"已设置Star标签: {obj.name}");
                }
            }
        }
    }
    
    Canvas CreateCanvas()
    {
        GameObject canvasObj = new GameObject("Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;
        
        canvasObj.AddComponent<GraphicRaycaster>();
        
        Debug.Log("已创建Canvas");
        return canvas;
    }
    
    GameObject CreateMinimapManager()
    {
        // 检查是否已存在小地图管理器
        GameObject existingManager = GameObject.Find("MinimapManager");
        if (existingManager != null)
        {
            Debug.Log("已存在小地图管理器，将使用现有管理器");
            return existingManager;
        }
        
        // 创建新的小地图管理器
        GameObject minimapManager = new GameObject("MinimapManager");
        
        if (useSimpleMinimap)
        {
            minimapManager.AddComponent<SimpleMinimap>();
            Debug.Log("已添加SimpleMinimap组件");
        }
        else if (useAdvancedMinimap)
        {
            minimapManager.AddComponent<AdvancedMinimap>();
            Debug.Log("已添加AdvancedMinimap组件");
        }
        else
        {
            // 默认使用简单小地图
            minimapManager.AddComponent<SimpleMinimap>();
            Debug.Log("已添加SimpleMinimap组件（默认）");
        }
        
        return minimapManager;
    }
    
    void ApplyPreset(GameObject minimapManager, MinimapPreset preset)
    {
        SimpleMinimap simpleMinimap = minimapManager.GetComponent<SimpleMinimap>();
        AdvancedMinimap advancedMinimap = minimapManager.GetComponent<AdvancedMinimap>();
        
        if (simpleMinimap != null)
        {
            // 通过反射设置私有字段（仅用于快速设置）
            SetPrivateField(simpleMinimap, "minimapSize", preset.minimapSize);
            SetPrivateField(simpleMinimap, "minimapPosition", preset.minimapPosition);
            SetPrivateField(simpleMinimap, "backgroundColor", preset.backgroundColor);
            SetPrivateField(simpleMinimap, "borderColor", preset.borderColor);
            SetPrivateField(simpleMinimap, "iconSize", preset.iconSize);
            SetPrivateField(simpleMinimap, "mapRadius", preset.mapRadius);
            SetPrivateField(simpleMinimap, "toggleKey", preset.toggleKey);
        }
        else if (advancedMinimap != null)
        {
            SetPrivateField(advancedMinimap, "minimapSize", preset.minimapSize);
            SetPrivateField(advancedMinimap, "minimapPosition", preset.minimapPosition);
            SetPrivateField(advancedMinimap, "backgroundColor", preset.backgroundColor);
            SetPrivateField(advancedMinimap, "borderColor", preset.borderColor);
            SetPrivateField(advancedMinimap, "iconSize", preset.iconSize);
            SetPrivateField(advancedMinimap, "mapRadius", preset.mapRadius);
            SetPrivateField(advancedMinimap, "toggleKey", preset.toggleKey);
        }
        
        Debug.Log($"已应用预设: {preset.presetName}");
    }
    
    void SetPrivateField(object obj, string fieldName, object value)
    {
        var field = obj.GetType().GetField(fieldName, 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (field != null)
        {
            field.SetValue(obj, value);
        }
    }
    
    [ContextMenu("创建默认预设")]
    public void CreateDefaultPresets()
    {
        presets = new MinimapPreset[]
        {
            new MinimapPreset
            {
                presetName = "默认小地图",
                minimapSize = 150f,
                minimapPosition = new Vector2(20, -20),
                backgroundColor = new Color(0, 0, 0, 0.5f),
                borderColor = Color.white,
                iconSize = 8f,
                mapRadius = 30f,
                toggleKey = KeyCode.M
            },
            new MinimapPreset
            {
                presetName = "大号小地图",
                minimapSize = 250f,
                minimapPosition = new Vector2(30, -30),
                backgroundColor = new Color(0, 0, 0, 0.7f),
                borderColor = Color.cyan,
                iconSize = 12f,
                mapRadius = 50f,
                toggleKey = KeyCode.M
            },
            new MinimapPreset
            {
                presetName = "紧凑小地图",
                minimapSize = 100f,
                minimapPosition = new Vector2(10, -10),
                backgroundColor = new Color(0, 0, 0, 0.3f),
                borderColor = Color.gray,
                iconSize = 6f,
                mapRadius = 20f,
                toggleKey = KeyCode.M
            }
        };
        
        Debug.Log("已创建默认预设");
    }
    
    [ContextMenu("测试小地图")]
    public void TestMinimap()
    {
        SimpleMinimap simpleMinimap = FindObjectOfType<SimpleMinimap>();
        AdvancedMinimap advancedMinimap = FindObjectOfType<AdvancedMinimap>();
        
        if (simpleMinimap != null)
        {
            simpleMinimap.ToggleMinimap();
            Debug.Log("测试SimpleMinimap切换");
        }
        else if (advancedMinimap != null)
        {
            advancedMinimap.ToggleMinimap();
            Debug.Log("测试AdvancedMinimap切换");
        }
        else
        {
            Debug.LogWarning("未找到小地图组件，请先设置小地图");
        }
    }
    
    void OnValidate()
    {
        // 确保只能选择一个类型
        if (useSimpleMinimap && useAdvancedMinimap)
        {
            useAdvancedMinimap = false;
        }
    }
} 