using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 场景对话管理器
/// 负责在场景切换时通知PlotManager更新场景索引
/// </summary>
public class SceneDialogueManager : MonoBehaviour
{
    [Header("场景设置")]
    [SerializeField] private bool autoDetectScene = true;
    [SerializeField] private int manualSceneIndex = 1;
    
    private PlotManager plotManager;
    private string currentSceneName = "";
    
    private void Start()
    {
        // 获取PlotManager引用
        plotManager = FindObjectOfType<PlotManager>();
        if (plotManager == null)
        {
            Debug.LogError("未找到PlotManager，SceneDialogueManager无法工作");
            return;
        }
        
        // 初始化场景
        UpdateSceneIndex();
        
        // 监听场景加载事件
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    private void OnDestroy()
    {
        // 移除事件监听
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    /// <summary>
    /// 场景加载完成时的回调
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (plotManager != null)
        {
            UpdateSceneIndex();
        }
    }
    
    /// <summary>
    /// 更新场景索引
    /// </summary>
    private void UpdateSceneIndex()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        
        if (sceneName != currentSceneName)
        {
            currentSceneName = sceneName;
            
            if (autoDetectScene)
            {
                int sceneIndex = DetectSceneIndex(sceneName);
                SetSceneIndex(sceneIndex);
            }
            else
            {
                SetSceneIndex(manualSceneIndex);
            }
        }
    }
    
    /// <summary>
    /// 检测场景索引
    /// </summary>
    private int DetectSceneIndex(string sceneName)
    {
        // 根据场景名称设置场景索引
        if (sceneName.Contains("Level1"))
        {
            return 1;
        }
        else if (sceneName.Contains("Level2"))
        {
            return 2;
        }
        else if (sceneName.Contains("Level3"))
        {
            return 3;
        }
        else if (sceneName.Contains("Level4"))
        {
            return 4;
        }
        else if (sceneName.Contains("End"))
        {
            return 5;
        }
        else
        {
            // 尝试从场景名称中提取数字
            string number = System.Text.RegularExpressions.Regex.Match(sceneName, @"\d+").Value;
            if (!string.IsNullOrEmpty(number) && int.TryParse(number, out int sceneNum))
            {
                return sceneNum;
            }
        }
        
        return 1; // 默认返回1
    }
    
    /// <summary>
    /// 设置场景索引
    /// </summary>
    public void SetSceneIndex(int sceneIndex)
    {
        if (plotManager != null)
        {
            plotManager.SetCurrentScene(sceneIndex);
            Debug.Log($"场景对话管理器: {currentSceneName} -> 场景索引 {sceneIndex}");
        }
    }
    
    /// <summary>
    /// 手动设置场景索引（用于测试）
    /// </summary>
    [ContextMenu("设置为场景1")]
    public void SetToScene1() { SetSceneIndex(1); }
    
    [ContextMenu("设置为场景2")]
    public void SetToScene2() { SetSceneIndex(2); }
    
    [ContextMenu("设置为场景3")]
    public void SetToScene3() { SetSceneIndex(3); }
    
    [ContextMenu("设置为场景4")]
    public void SetToScene4() { SetSceneIndex(4); }
    
    /// <summary>
    /// 播放当前场景的开始对话
    /// </summary>
    [ContextMenu("播放场景开始对话")]
    public void PlaySceneStartDialogue()
    {
        if (plotManager != null)
        {
            plotManager.PlayDialogueSegment(GetCurrentSceneIndex(), 0);
        }
    }
    
    /// <summary>
    /// 获取当前场景索引
    /// </summary>
    public int GetCurrentSceneIndex()
    {
        if (autoDetectScene)
        {
            return DetectSceneIndex(currentSceneName);
        }
        return manualSceneIndex;
    }
    
    /// <summary>
    /// 获取当前场景名称
    /// </summary>
    public string GetCurrentSceneName()
    {
        return currentSceneName;
    }
} 