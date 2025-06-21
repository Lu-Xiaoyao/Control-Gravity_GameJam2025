using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 小地图自定义工具
/// 用于调整地图大小、比例和图例样式
/// </summary>
public class MinimapCustomizer : MonoBehaviour
{
    [Header("当前小地图")]
    [SerializeField] private SimpleMinimap simpleMinimap;
    [SerializeField] private AdvancedMinimap advancedMinimap;
    [SerializeField] private DebugMinimap debugMinimap;
    
    [Header("大小调整")]
    [SerializeField] private float minimapSize = 150f;
    [SerializeField] private Vector2 minimapPosition = new Vector2(20, -20);
    [SerializeField] private bool autoAdjustSize = true;
    [SerializeField] private float screenSizeRatio = 0.15f;
    
    [Header("比例调整")]
    [SerializeField] private float mapRadius = 30f;
    [SerializeField] private float iconSize = 8f;
    [SerializeField] private bool autoDetectBounds = true;
    
    [Header("样式调整")]
    [SerializeField] private Color backgroundColor = new Color(0, 0, 0, 0.5f);
    [SerializeField] private Color borderColor = Color.white;
    [SerializeField] private float borderWidth = 2f;
    [SerializeField] private bool showBorder = true;
    [SerializeField] private bool showBackground = true;
    
    [Header("图例样式")]
    [SerializeField] private Color playerColor = Color.blue;
    [SerializeField] private Color planetColor = Color.green;
    [SerializeField] private Color doorColor = Color.yellow;
    [SerializeField] private Color starColor = Color.white;
    [SerializeField] private bool showLegend = true;
    [SerializeField] private Vector2 legendPosition = new Vector2(10, -10);
    
    [Header("预设样式")]
    [SerializeField] private MinimapStyle[] presetStyles;
    [SerializeField] private int selectedPreset = 0;
    
    [System.Serializable]
    public class MinimapStyle
    {
        public string styleName;
        public float size = 150f;
        public Vector2 position = new Vector2(20, -20);
        public Color bgColor = new Color(0, 0, 0, 0.5f);
        public Color borderColor = Color.white;
        public float borderWidth = 2f;
        public float iconSize = 8f;
        public Color playerColor = Color.blue;
        public Color planetColor = Color.green;
        public Color doorColor = Color.yellow;
        public Color starColor = Color.white;
    }
    
    void Start()
    {
        FindMinimapComponents();
        CreateDefaultPresets();
        ApplyCurrentSettings();
    }
    
    void FindMinimapComponents()
    {
        if (simpleMinimap == null)
            simpleMinimap = FindObjectOfType<SimpleMinimap>();
        if (advancedMinimap == null)
            advancedMinimap = FindObjectOfType<AdvancedMinimap>();
        if (debugMinimap == null)
            debugMinimap = FindObjectOfType<DebugMinimap>();
    }
    
    void CreateDefaultPresets()
    {
        if (presetStyles == null || presetStyles.Length == 0)
        {
            presetStyles = new MinimapStyle[]
            {
                new MinimapStyle
                {
                    styleName = "默认样式",
                    size = 150f,
                    position = new Vector2(20, -20),
                    bgColor = new Color(0, 0, 0, 0.5f),
                    borderColor = Color.white,
                    borderWidth = 2f,
                    iconSize = 8f,
                    playerColor = Color.blue,
                    planetColor = Color.green,
                    doorColor = Color.yellow,
                    starColor = Color.white
                },
                new MinimapStyle
                {
                    styleName = "大号样式",
                    size = 250f,
                    position = new Vector2(30, -30),
                    bgColor = new Color(0, 0, 0, 0.7f),
                    borderColor = Color.cyan,
                    borderWidth = 3f,
                    iconSize = 12f,
                    playerColor = Color.blue,
                    planetColor = Color.green,
                    doorColor = Color.yellow,
                    starColor = Color.white
                },
                new MinimapStyle
                {
                    styleName = "紧凑样式",
                    size = 100f,
                    position = new Vector2(10, -10),
                    bgColor = new Color(0, 0, 0, 0.3f),
                    borderColor = Color.gray,
                    borderWidth = 1f,
                    iconSize = 6f,
                    playerColor = Color.blue,
                    planetColor = Color.green,
                    doorColor = Color.yellow,
                    starColor = Color.white
                }
            };
        }
    }
    
    void Update()
    {
        if (autoAdjustSize)
        {
            AdjustSizeToScreen();
        }
    }
    
    void AdjustSizeToScreen()
    {
        float newSize = Mathf.Min(Screen.width, Screen.height) * screenSizeRatio;
        if (Mathf.Abs(newSize - minimapSize) > 1f)
        {
            minimapSize = newSize;
            ApplySizeSettings();
        }
    }
    
    [ContextMenu("应用当前设置")]
    public void ApplyCurrentSettings()
    {
        ApplySizeSettings();
        ApplyStyleSettings();
        ApplyIconSettings();
        CreateLegend();
        Debug.Log("已应用小地图设置");
    }
    
    void ApplySizeSettings()
    {
        if (simpleMinimap != null)
        {
            SetPrivateField(simpleMinimap, "minimapSize", minimapSize);
            SetPrivateField(simpleMinimap, "minimapPosition", minimapPosition);
        }
        
        if (advancedMinimap != null)
        {
            SetPrivateField(advancedMinimap, "minimapSize", minimapSize);
            SetPrivateField(advancedMinimap, "minimapPosition", minimapPosition);
        }
        
        if (debugMinimap != null)
        {
            SetPrivateField(debugMinimap, "minimapSize", minimapSize);
            SetPrivateField(debugMinimap, "minimapPosition", minimapPosition);
        }
        
        UpdateMinimapUI();
    }
    
    void ApplyStyleSettings()
    {
        if (simpleMinimap != null)
        {
            SetPrivateField(simpleMinimap, "backgroundColor", backgroundColor);
            SetPrivateField(simpleMinimap, "borderColor", borderColor);
        }
        
        if (advancedMinimap != null)
        {
            SetPrivateField(advancedMinimap, "backgroundColor", backgroundColor);
            SetPrivateField(advancedMinimap, "borderColor", borderColor);
        }
        
        if (debugMinimap != null)
        {
            SetPrivateField(debugMinimap, "backgroundColor", backgroundColor);
            SetPrivateField(debugMinimap, "borderColor", borderColor);
        }
        
        UpdateBackgroundAndBorder();
    }
    
    void ApplyIconSettings()
    {
        if (simpleMinimap != null)
        {
            SetPrivateField(simpleMinimap, "iconSize", iconSize);
            SetPrivateField(simpleMinimap, "mapRadius", mapRadius);
        }
        
        if (advancedMinimap != null)
        {
            SetPrivateField(advancedMinimap, "iconSize", iconSize);
            SetPrivateField(advancedMinimap, "mapRadius", mapRadius);
        }
    }
    
    void UpdateMinimapUI()
    {
        RectTransform minimapContainer = FindMinimapContainer();
        if (minimapContainer != null)
        {
            minimapContainer.sizeDelta = new Vector2(minimapSize, minimapSize);
            minimapContainer.anchoredPosition = minimapPosition;
        }
    }
    
    void UpdateBackgroundAndBorder()
    {
        Image backgroundImage = FindMinimapBackground();
        if (backgroundImage != null)
        {
            backgroundImage.color = backgroundColor;
            backgroundImage.gameObject.SetActive(showBackground);
        }
        
        Image borderImage = FindMinimapBorder();
        if (borderImage != null)
        {
            borderImage.color = borderColor;
            borderImage.gameObject.SetActive(showBorder);
            
            RectTransform borderRect = borderImage.GetComponent<RectTransform>();
            if (borderRect != null)
            {
                borderRect.sizeDelta = new Vector2(borderWidth, borderWidth);
            }
        }
    }
    
    void CreateLegend()
    {
        if (!showLegend) return;
        
        GameObject legendObj = GameObject.Find("MinimapLegend");
        if (legendObj == null)
        {
            legendObj = new GameObject("MinimapLegend");
            legendObj.transform.SetParent(FindMinimapContainer(), false);
        }
        
        RectTransform legendRect = legendObj.GetComponent<RectTransform>();
        if (legendRect == null)
        {
            legendRect = legendObj.AddComponent<RectTransform>();
        }
        
        legendRect.anchorMin = new Vector2(1, 1);
        legendRect.anchorMax = new Vector2(1, 1);
        legendRect.sizeDelta = new Vector2(120, 80);
        legendRect.anchoredPosition = legendPosition;
        
        CreateLegendBackground(legendObj);
        CreateLegendItems(legendObj);
    }
    
    void CreateLegendBackground(GameObject legendObj)
    {
        GameObject bgObj = legendObj.transform.Find("LegendBackground")?.gameObject;
        if (bgObj == null)
        {
            bgObj = new GameObject("LegendBackground");
            bgObj.transform.SetParent(legendObj.transform, false);
        }
        
        Image bgImage = bgObj.GetComponent<Image>();
        if (bgImage == null)
        {
            bgImage = bgObj.AddComponent<Image>();
        }
        
        bgImage.color = new Color(0, 0, 0, 0.7f);
        
        RectTransform bgRect = bgObj.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;
        bgRect.anchoredPosition = Vector2.zero;
    }
    
    void CreateLegendItems(GameObject legendObj)
    {
        for (int i = legendObj.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = legendObj.transform.GetChild(i);
            if (child.name != "LegendBackground")
            {
                DestroyImmediate(child.gameObject);
            }
        }
        
        CreateLegendItem(legendObj, "Player", playerColor, new Vector2(10, -15));
        CreateLegendItem(legendObj, "Planet", planetColor, new Vector2(10, -35));
        CreateLegendItem(legendObj, "Door", doorColor, new Vector2(10, -55));
        CreateLegendItem(legendObj, "Star", starColor, new Vector2(10, -75));
    }
    
    void CreateLegendItem(GameObject legendObj, string label, Color color, Vector2 position)
    {
        GameObject itemObj = new GameObject($"LegendItem_{label}");
        itemObj.transform.SetParent(legendObj.transform, false);
        
        GameObject iconObj = new GameObject("Icon");
        iconObj.transform.SetParent(itemObj.transform, false);
        
        Image iconImage = iconObj.AddComponent<Image>();
        iconImage.color = color;
        
        RectTransform iconRect = iconObj.GetComponent<RectTransform>();
        iconRect.anchorMin = new Vector2(0, 0.5f);
        iconRect.anchorMax = new Vector2(0, 0.5f);
        iconRect.sizeDelta = new Vector2(10, 10);
        iconRect.anchoredPosition = position;
        
        GameObject textObj = new GameObject("Label");
        textObj.transform.SetParent(itemObj.transform, false);
        
        Text text = textObj.AddComponent<Text>();
        text.text = label;
        text.color = Color.white;
        
        // 使用字体兼容性修复系统
        FontCompatibilityFix.SetCompatibleFont(text);
        text.fontSize = 10;
        text.alignment = TextAnchor.MiddleLeft;
        
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0, 0.5f);
        textRect.anchorMax = new Vector2(1, 0.5f);
        textRect.sizeDelta = new Vector2(-20, 15);
        textRect.anchoredPosition = position + new Vector2(15, 0);
    }
    
    Font GetCompatibleFont()
    {
        // 使用字体兼容性修复系统
        return FontCompatibilityFix.GetCompatibleFont();
    }
    
    RectTransform FindMinimapContainer()
    {
        GameObject[] containers = {
            GameObject.Find("MinimapContainer"),
            GameObject.Find("AdvancedMinimapContainer"),
            GameObject.Find("DebugMinimapContainer")
        };
        
        foreach (GameObject container in containers)
        {
            if (container != null)
            {
                return container.GetComponent<RectTransform>();
            }
        }
        
        return null;
    }
    
    Image FindMinimapBackground()
    {
        GameObject bgObj = GameObject.Find("MinimapBackground");
        return bgObj?.GetComponent<Image>();
    }
    
    Image FindMinimapBorder()
    {
        GameObject borderObj = GameObject.Find("MinimapBorder");
        return borderObj?.GetComponent<Image>();
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
    
    [ContextMenu("应用预设样式")]
    public void ApplyPresetStyle()
    {
        if (presetStyles != null && selectedPreset < presetStyles.Length)
        {
            MinimapStyle style = presetStyles[selectedPreset];
            
            minimapSize = style.size;
            minimapPosition = style.position;
            backgroundColor = style.bgColor;
            borderColor = style.borderColor;
            borderWidth = style.borderWidth;
            iconSize = style.iconSize;
            playerColor = style.playerColor;
            planetColor = style.planetColor;
            doorColor = style.doorColor;
            starColor = style.starColor;
            
            ApplyCurrentSettings();
            Debug.Log($"已应用预设样式: {style.styleName}");
        }
    }
    
    [ContextMenu("调整到屏幕大小")]
    public void AdjustToScreenSize()
    {
        minimapSize = Mathf.Min(Screen.width, Screen.height) * screenSizeRatio;
        ApplySizeSettings();
        Debug.Log($"已调整小地图大小为: {minimapSize}");
    }
    
    [ContextMenu("切换图例显示")]
    public void ToggleLegend()
    {
        showLegend = !showLegend;
        GameObject legendObj = GameObject.Find("MinimapLegend");
        if (legendObj != null)
        {
            legendObj.SetActive(showLegend);
        }
        else if (showLegend)
        {
            CreateLegend();
        }
    }
    
    public void SetMinimapSize(float size)
    {
        minimapSize = size;
        ApplySizeSettings();
    }
    
    public void SetMinimapPosition(Vector2 position)
    {
        minimapPosition = position;
        ApplySizeSettings();
    }
    
    public void SetBackgroundColor(Color color)
    {
        backgroundColor = color;
        ApplyStyleSettings();
    }
    
    public void SetIconSize(float size)
    {
        iconSize = size;
        ApplyIconSettings();
    }
} 