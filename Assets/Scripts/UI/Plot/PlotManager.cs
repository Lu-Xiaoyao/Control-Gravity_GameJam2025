using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using System.IO;

[System.Serializable]
public class DialogueLine
{
    public string speakerName;
    public string content;
    public string extraData;
}

[System.Serializable]
public class DialogueSegment
{
    public int sceneIndex;
    public int segmentIndex;
    public List<DialogueLine> lines = new List<DialogueLine>();
}

public class PlotManager : MonoBehaviour, InputActions.IUIActions
{
    [Header("UI组件")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI speakerNameText;
    [SerializeField] private TextMeshProUGUI dialogueContentText;
    
    [Header("对话触发器")]
    [SerializeField] private GameObject[] dialogueTriggers; // 场景中的对话触发点
    
    [Header("设置")]
    [SerializeField] private float typingSpeed = 0.05f; // 打字机效果速度
    
    private InputActions inputActions;
    private List<DialogueSegment> allDialogues = new List<DialogueSegment>();
    private DialogueSegment currentSegment;
    private int currentLineIndex = 0;
    private bool isDialogueActive = false;
    private bool isTyping = false;
    private Coroutine typingCoroutine;
    
    // 当前场景索引
    private int currentSceneIndex = 1; // 默认从第1关开始
    private string currentSceneName = "";
    
    private void Awake()
    {
        // 初始化输入系统
        inputActions = new InputActions();
        inputActions.UI.SetCallbacks(this);
        
        // 隐藏对话面板
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }
    
    private void Start()
    {
        LoadDialogueData();
        UpdateCurrentSceneIndex();
        PlaySceneStartDialogue();
    }
    
    private void Update()
    {
        // 检测场景是否发生变化
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if (currentScene != currentSceneName)
        {
            currentSceneName = currentScene;
            UpdateCurrentSceneIndex();
            PlaySceneStartDialogue();
        }
    }
    
    private void OnEnable()
    {
        inputActions.UI.Enable();
    }
    
    private void OnDisable()
    {
        inputActions.UI.Disable();
    }
    
    private void OnDestroy()
    {
        inputActions?.Dispose();
    }
    
    /// <summary>
    /// 从CSV文件加载对话数据
    /// </summary>
    private void LoadDialogueData()
    {
        allDialogues.Clear();
        
        // 使用Resources.Load加载CSV文件
        TextAsset csvFile = Resources.Load<TextAsset>("Plot");
        
        if (csvFile == null)
        {
            Debug.LogError("无法加载对话文件 Plot.csv，请确保文件位于 Assets/Resources 文件夹中");
            return;
        }
        
        try
        {
            string[] lines = csvFile.text.Split('\n');
            DialogueSegment currentSegment = null;
            
            foreach (string line in lines)
            {
                string trimmedLine = line.Trim();
                
                // 跳过空行和%开头的行
                if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith("%"))
                    continue;
                
                // 检查是否是段落分割标志
                if (trimmedLine.StartsWith("#"))
                {
                    // 保存之前的段落
                    if (currentSegment != null && currentSegment.lines.Count > 0)
                    {
                        allDialogues.Add(currentSegment);
                    }
                    
                    // 解析新的段落信息
                    string[] parts = trimmedLine.Split(',');
                    if (parts.Length >= 3)
                    {
                        currentSegment = new DialogueSegment();
                        if (int.TryParse(parts[1], out int sceneIndex))
                            currentSegment.sceneIndex = sceneIndex;
                        if (int.TryParse(parts[2], out int segmentIndex))
                            currentSegment.segmentIndex = segmentIndex;
                    }
                }
                else if (currentSegment != null)
                {
                    // 解析对话行
                    string[] parts = trimmedLine.Split(',');
                    if (parts.Length >= 2)
                    {
                        DialogueLine dialogueLine = new DialogueLine
                        {
                            speakerName = parts[0].Trim(),
                            content = parts[1].Trim(),
                            extraData = parts.Length > 2 ? parts[2].Trim() : ""
                        };
                        currentSegment.lines.Add(dialogueLine);
                    }
                }
            }
            
            // 添加最后一个段落
            if (currentSegment != null && currentSegment.lines.Count > 0)
            {
                allDialogues.Add(currentSegment);
            }
            
            Debug.Log($"成功加载 {allDialogues.Count} 个对话段落");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"加载对话文件时出错: {e.Message}");
        }
    }
    
    /// <summary>
    /// 播放关卡开始时的对话（segmentIndex = 0）
    /// </summary>
    private void PlaySceneStartDialogue()
    {
        PlayDialogueSegment(currentSceneIndex, 0);
    }
    
    /// <summary>
    /// 播放指定场景和段落的对话
    /// </summary>
    public void PlayDialogueSegment(int sceneIndex, int segmentIndex)
    {
        DialogueSegment segment = allDialogues.Find(s => s.sceneIndex == sceneIndex && s.segmentIndex == segmentIndex);
        
        if (segment != null && segment.lines.Count > 0)
        {
            currentSegment = segment;
            currentLineIndex = 0;
            StartDialogue();
        }
        else
        {
            Debug.LogWarning($"未找到对话段落: 场景{sceneIndex}, 段落{segmentIndex}");
        }
    }
    
    /// <summary>
    /// 开始对话
    /// </summary>
    private void StartDialogue()
    {
        if (isDialogueActive) return;
        
        isDialogueActive = true;
        Time.timeScale = 0f; // 暂停游戏
        
        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);
        
        DisplayCurrentLine();
    }
    
    /// <summary>
    /// 显示当前对话行
    /// </summary>
    private void DisplayCurrentLine()
    {
        if (currentSegment == null || currentLineIndex >= currentSegment.lines.Count)
        {
            EndDialogue();
            return;
        }
        
        DialogueLine line = currentSegment.lines[currentLineIndex];
        
        if (speakerNameText != null)
            speakerNameText.text = line.speakerName;
        
        if (dialogueContentText != null)
        {
            // 停止之前的打字机效果
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);
            
            // 开始新的打字机效果
            typingCoroutine = StartCoroutine(TypeText(line.content));
        }
    }
    
    /// <summary>
    /// 打字机效果协程
    /// </summary>
    private IEnumerator TypeText(string text)
    {
        isTyping = true;
        dialogueContentText.text = "";
        
        foreach (char c in text)
        {
            dialogueContentText.text += c;
            yield return new WaitForSecondsRealtime(typingSpeed);
        }
        
        isTyping = false;
    }
    
    /// <summary>
    /// 显示下一行对话
    /// </summary>
    private void ShowNextLine()
    {
        if (isTyping)
        {
            // 如果正在打字，直接显示完整文本
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);
            
            if (currentSegment != null && currentLineIndex < currentSegment.lines.Count)
            {
                dialogueContentText.text = currentSegment.lines[currentLineIndex].content;
            }
            isTyping = false;
            return;
        }
        
        currentLineIndex++;
        DisplayCurrentLine();
    }
    
    /// <summary>
    /// 结束对话
    /// </summary>
    private void EndDialogue()
    {
        isDialogueActive = false;
        Time.timeScale = 1f; // 恢复游戏
        
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
        
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
    }
    
    /// <summary>
    /// 设置当前场景索引
    /// </summary>
    public void SetCurrentScene(int sceneIndex)
    {
        currentSceneIndex = sceneIndex;
    }
    
    /// <summary>
    /// 检查是否正在播放对话
    /// </summary>
    public bool IsDialogueActive()
    {
        return isDialogueActive;
    }
    
    // 输入系统回调
    public void OnNext(InputAction.CallbackContext context)
    {
        if (context.performed && isDialogueActive)
        {
            ShowNextLine();
        }
    }
    
    // 用于在Inspector中测试对话的公共方法
    [ContextMenu("测试对话段落")]
    public void TestDialogue()
    {
        PlayDialogueSegment(currentSceneIndex, 1);
    }
    
    /// <summary>
    /// 根据当前场景名称更新场景索引
    /// </summary>
    private void UpdateCurrentSceneIndex()
    {
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        
        // 根据场景名称设置场景索引
        if (sceneName.Contains("Level1"))
        {
            currentSceneIndex = 1;
        }
        else if (sceneName.Contains("Level2"))
        {
            currentSceneIndex = 2;
        }
        else if (sceneName.Contains("Level3"))
        {
            currentSceneIndex = 3;
        }
        else if (sceneName.Contains("Level4"))
        {
            currentSceneIndex = 4;
        }
        else if (sceneName.Contains("End"))
        {
            currentSceneIndex = 5; // 假设End场景对应场景5
        }
        else
        {
            // 尝试从场景名称中提取数字
            string number = System.Text.RegularExpressions.Regex.Match(sceneName, @"\d+").Value;
            if (!string.IsNullOrEmpty(number) && int.TryParse(number, out int sceneNum))
            {
                currentSceneIndex = sceneNum;
            }
        }
        
        Debug.Log($"场景切换检测: {sceneName} -> 场景索引 {currentSceneIndex}");
    }
} 