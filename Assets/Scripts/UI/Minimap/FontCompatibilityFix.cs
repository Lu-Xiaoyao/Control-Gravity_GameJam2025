using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 字体兼容性修复工具
/// 解决Unity版本更新导致的字体问题
/// </summary>
public class FontCompatibilityFix : MonoBehaviour
{
    [Header("字体设置")]
    [SerializeField] private Font fallbackFont;
    [SerializeField] private bool autoFixOnStart = true;
    
    [Header("字体选项")]
    [SerializeField] private string[] fontNames = {
        "LegacyRuntime.ttf",
        "Arial.ttf", 
        "LiberationSans.ttf"
    };
    
    private static Font cachedFont;
    
    void Start()
    {
        if (autoFixOnStart)
        {
            InitializeFont();
        }
    }
    
    void InitializeFont()
    {
        cachedFont = GetCompatibleFont();
        if (cachedFont != null)
        {
            Debug.Log($"已加载兼容字体: {cachedFont.name}");
        }
        else
        {
            Debug.LogWarning("无法找到兼容字体，将使用系统默认字体");
        }
    }
    
    /// <summary>
    /// 获取兼容的字体
    /// </summary>
    public static Font GetCompatibleFont()
    {
        if (cachedFont != null)
        {
            return cachedFont;
        }
        
        // 尝试加载内置字体
        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (font != null)
        {
            cachedFont = font;
            return font;
        }
        
        // 尝试其他字体
        font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        if (font != null)
        {
            cachedFont = font;
            return font;
        }
        
        font = Resources.GetBuiltinResource<Font>("LiberationSans.ttf");
        if (font != null)
        {
            cachedFont = font;
            return font;
        }
        
        // 尝试从操作系统加载字体
        font = Font.CreateDynamicFontFromOSFont("Arial", 12);
        if (font != null)
        {
            cachedFont = font;
            return font;
        }
        
        font = Font.CreateDynamicFontFromOSFont("Helvetica", 12);
        if (font != null)
        {
            cachedFont = font;
            return font;
        }
        
        // 最后的备选方案
        font = Font.CreateDynamicFontFromOSFont("System", 12);
        if (font != null)
        {
            cachedFont = font;
            return font;
        }
        
        Debug.LogError("无法找到任何兼容的字体！");
        return null;
    }
    
    /// <summary>
    /// 为Text组件设置兼容字体
    /// </summary>
    public static void SetCompatibleFont(Text textComponent)
    {
        if (textComponent != null)
        {
            Font font = GetCompatibleFont();
            if (font != null)
            {
                textComponent.font = font;
            }
        }
    }
    
    /// <summary>
    /// 为所有Text组件设置兼容字体
    /// </summary>
    [ContextMenu("修复所有Text组件字体")]
    public void FixAllTextFonts()
    {
        Text[] allTexts = FindObjectsOfType<Text>();
        int fixedCount = 0;
        
        foreach (Text text in allTexts)
        {
            if (text.font == null || text.font.name.Contains("Arial"))
            {
                SetCompatibleFont(text);
                fixedCount++;
            }
        }
        
        Debug.Log($"已修复 {fixedCount} 个Text组件的字体");
    }
    
    /// <summary>
    /// 测试字体加载
    /// </summary>
    [ContextMenu("测试字体加载")]
    public void TestFontLoading()
    {
        Debug.Log("=== 字体加载测试 ===");
        
        foreach (string fontName in fontNames)
        {
            Font font = Resources.GetBuiltinResource<Font>(fontName);
            if (font != null)
            {
                Debug.Log($"✅ 成功加载字体: {fontName}");
            }
            else
            {
                Debug.LogWarning($"❌ 无法加载字体: {fontName}");
            }
        }
        
        Font compatibleFont = GetCompatibleFont();
        if (compatibleFont != null)
        {
            Debug.Log($"✅ 兼容字体: {compatibleFont.name}");
        }
        else
        {
            Debug.LogError("❌ 无法获取兼容字体");
        }
        
        Debug.Log("=== 字体测试完成 ===");
    }
    
    /// <summary>
    /// 创建测试UI
    /// </summary>
    [ContextMenu("创建字体测试UI")]
    public void CreateFontTestUI()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("TestCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }
        
        GameObject testObj = new GameObject("FontTest");
        testObj.transform.SetParent(canvas.transform, false);
        
        Text testText = testObj.AddComponent<Text>();
        testText.text = "字体测试 - Font Test";
        testText.fontSize = 24;
        testText.color = Color.white;
        testText.alignment = TextAnchor.MiddleCenter;
        
        RectTransform rect = testObj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(300, 50);
        rect.anchoredPosition = Vector2.zero;
        
        SetCompatibleFont(testText);
        
        Debug.Log("已创建字体测试UI");
    }
    
    /// <summary>
    /// 获取当前使用的字体信息
    /// </summary>
    [ContextMenu("显示字体信息")]
    public void ShowFontInfo()
    {
        Font currentFont = GetCompatibleFont();
        if (currentFont != null)
        {
            Debug.Log($"当前字体: {currentFont.name}");
            Debug.Log($"字体类型: {currentFont.GetType().Name}");
            Debug.Log($"是否为动态字体: {currentFont.dynamic}");
        }
        else
        {
            Debug.LogWarning("当前没有可用的字体");
        }
    }
} 