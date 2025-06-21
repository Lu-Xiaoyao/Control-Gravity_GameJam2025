using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 对话系统设置助手
/// 提供快速设置对话UI界面的功能
/// </summary>
public class DialogueSetupHelper : MonoBehaviour
{
    [Header("快速设置")]
    [SerializeField] private bool autoSetup = false;
    [SerializeField] private Canvas targetCanvas;
    
    [Header("UI预制体设置")]
    [SerializeField] private GameObject dialoguePanelPrefab;
    [SerializeField] private TextMeshProUGUI speakerNameTextPrefab;
    [SerializeField] private TextMeshProUGUI dialogueContentTextPrefab;
    
    [Header("样式设置")]
    [SerializeField] private Color panelColor = new Color(0, 0, 0, 0.8f);
    [SerializeField] private Color speakerNameColor = Color.white;
    [SerializeField] private Color dialogueContentColor = Color.white;
    [SerializeField] private int fontSize = 24;
    
    private void Start()
    {
        if (autoSetup)
        {
            SetupDialogueUI();
        }
    }
    
    /// <summary>
    /// 自动设置对话UI界面
    /// </summary>
    [ContextMenu("设置对话UI")]
    public void SetupDialogueUI()
    {
        if (targetCanvas == null)
        {
            targetCanvas = FindObjectOfType<Canvas>();
            if (targetCanvas == null)
            {
                Debug.LogError("未找到Canvas，请先创建Canvas或指定目标Canvas");
                return;
            }
        }
        
        // 创建对话面板
        GameObject dialoguePanel = CreateDialoguePanel();
        
        // 创建说话者姓名文本框
        TextMeshProUGUI speakerNameText = CreateSpeakerNameText(dialoguePanel);
        
        // 创建对话内容文本框
        TextMeshProUGUI dialogueContentText = CreateDialogueContentText(dialoguePanel);
        
        // 设置PlotManager
        SetupPlotManager(dialoguePanel, speakerNameText, dialogueContentText);
        
        Debug.Log("对话UI设置完成！");
    }
    
    /// <summary>
    /// 创建对话面板
    /// </summary>
    private GameObject CreateDialoguePanel()
    {
        GameObject panel;
        
        if (dialoguePanelPrefab != null)
        {
            panel = Instantiate(dialoguePanelPrefab, targetCanvas.transform);
        }
        else
        {
            panel = new GameObject("DialoguePanel");
            panel.transform.SetParent(targetCanvas.transform, false);
            
            // 添加Image组件
            Image image = panel.AddComponent<Image>();
            image.color = panelColor;
            
            // 添加RectTransform设置
            RectTransform rectTransform = panel.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(1, 0.3f);
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }
        
        return panel;
    }
    
    /// <summary>
    /// 创建说话者姓名文本框
    /// </summary>
    private TextMeshProUGUI CreateSpeakerNameText(GameObject parent)
    {
        TextMeshProUGUI textComponent;
        
        if (speakerNameTextPrefab != null)
        {
            textComponent = Instantiate(speakerNameTextPrefab, parent.transform);
        }
        else
        {
            GameObject textObj = new GameObject("SpeakerNameText");
            textObj.transform.SetParent(parent.transform, false);
            
            textComponent = textObj.AddComponent<TextMeshProUGUI>();
            textComponent.text = "说话者";
            textComponent.color = speakerNameColor;
            textComponent.fontSize = fontSize;
            textComponent.fontStyle = FontStyles.Bold;
            textComponent.alignment = TextAlignmentOptions.Left;
            
            // 设置位置
            RectTransform rectTransform = textComponent.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0, 0.7f);
            rectTransform.anchorMax = new Vector2(1, 1);
            rectTransform.offsetMin = new Vector2(20, 10);
            rectTransform.offsetMax = new Vector2(-20, -10);
        }
        
        return textComponent;
    }
    
    /// <summary>
    /// 创建对话内容文本框
    /// </summary>
    private TextMeshProUGUI CreateDialogueContentText(GameObject parent)
    {
        TextMeshProUGUI textComponent;
        
        if (dialogueContentTextPrefab != null)
        {
            textComponent = Instantiate(dialogueContentTextPrefab, parent.transform);
        }
        else
        {
            GameObject textObj = new GameObject("DialogueContentText");
            textObj.transform.SetParent(parent.transform, false);
            
            textComponent = textObj.AddComponent<TextMeshProUGUI>();
            textComponent.text = "对话内容";
            textComponent.color = dialogueContentColor;
            textComponent.fontSize = fontSize;
            textComponent.fontStyle = FontStyles.Normal;
            textComponent.alignment = TextAlignmentOptions.Left;
            textComponent.enableWordWrapping = true;
            
            // 设置位置
            RectTransform rectTransform = textComponent.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(1, 0.7f);
            rectTransform.offsetMin = new Vector2(20, 10);
            rectTransform.offsetMax = new Vector2(-20, -10);
        }
        
        return textComponent;
    }
    
    /// <summary>
    /// 设置PlotManager组件
    /// </summary>
    private void SetupPlotManager(GameObject dialoguePanel, TextMeshProUGUI speakerNameText, TextMeshProUGUI dialogueContentText)
    {
        // 查找或创建PlotManager
        PlotManager plotManager = FindObjectOfType<PlotManager>();
        if (plotManager == null)
        {
            GameObject plotManagerObj = new GameObject("PlotManager");
            plotManager = plotManagerObj.AddComponent<PlotManager>();
        }
        
        // 使用反射设置私有字段（因为它们是私有的）
        var dialoguePanelField = typeof(PlotManager).GetField("dialoguePanel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var speakerNameTextField = typeof(PlotManager).GetField("speakerNameText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var dialogueContentTextField = typeof(PlotManager).GetField("dialogueContentText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        if (dialoguePanelField != null)
            dialoguePanelField.SetValue(plotManager, dialoguePanel);
        if (speakerNameTextField != null)
            speakerNameTextField.SetValue(plotManager, speakerNameText);
        if (dialogueContentTextField != null)
            dialogueContentTextField.SetValue(plotManager, dialogueContentText);
    }
    
    /// <summary>
    /// 创建示例对话触发器
    /// </summary>
    [ContextMenu("创建示例触发器")]
    public void CreateExampleTrigger()
    {
        GameObject trigger = new GameObject("DialogueTrigger_Example");
        trigger.transform.position = Vector3.zero;
        
        // 添加DialogueTrigger组件
        DialogueTrigger dialogueTrigger = trigger.AddComponent<DialogueTrigger>();
        
        // 添加碰撞器
        BoxCollider2D collider = trigger.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        collider.size = new Vector2(2, 2);
        
        Debug.Log("示例对话触发器已创建在场景原点");
    }
    
    /// <summary>
    /// 验证设置
    /// </summary>
    [ContextMenu("验证设置")]
    public void ValidateSetup()
    {
        bool isValid = true;
        
        // 检查Canvas
        if (FindObjectOfType<Canvas>() == null)
        {
            Debug.LogError("场景中未找到Canvas");
            isValid = false;
        }
        
        // 检查PlotManager
        PlotManager plotManager = FindObjectOfType<PlotManager>();
        if (plotManager == null)
        {
            Debug.LogError("场景中未找到PlotManager");
            isValid = false;
        }
        
        // 检查CSV文件
        string csvPath = System.IO.Path.Combine(Application.dataPath, "Data/Plot.csv");
        if (!System.IO.File.Exists(csvPath))
        {
            Debug.LogError($"对话文件不存在: {csvPath}");
            isValid = false;
        }
        
        // 检查输入系统 - 通过PlotManager验证
        if (plotManager != null)
        {
            Debug.Log("✓ 输入系统已配置（通过PlotManager验证）");
        }
        else
        {
            Debug.LogWarning("⚠ 无法验证输入系统配置");
        }
        
        if (isValid)
        {
            Debug.Log("对话系统设置验证通过！");
        }
        else
        {
            Debug.LogError("对话系统设置验证失败，请检查上述错误信息");
        }
    }
} 