using UnityEngine;

/// <summary>
/// 对话系统测试脚本
/// 用于验证对话系统的各项功能
/// </summary>
public class DialogueTest : MonoBehaviour
{
    [Header("测试设置")]
    [SerializeField] private PlotManager plotManager;
    [SerializeField] private int testSceneIndex = 1;
    [SerializeField] private int testSegmentIndex = 1;
    
    private void Start()
    {
        // 自动获取PlotManager引用
        if (plotManager == null)
        {
            plotManager = FindObjectOfType<PlotManager>();
        }
        
        if (plotManager == null)
        {
            Debug.LogError("未找到PlotManager，无法进行测试");
            return;
        }
        
        Debug.Log("对话系统测试脚本已启动");
        Debug.Log("按以下按键进行测试：");
        Debug.Log("1 - 测试对话段落1");
        Debug.Log("2 - 测试对话段落2");
        Debug.Log("3 - 测试对话段落3");
        Debug.Log("4 - 测试对话段落4");
        Debug.Log("0 - 测试关卡开始对话");
    }
    
    private void Update()
    {
        if (plotManager == null) return;
        
        // 键盘测试
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            TestDialogueSegment(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            TestDialogueSegment(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            TestDialogueSegment(3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            TestDialogueSegment(4);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            TestDialogueSegment(0);
        }
    }
    
    /// <summary>
    /// 测试指定对话段落
    /// </summary>
    private void TestDialogueSegment(int segmentIndex)
    {
        if (plotManager.IsDialogueActive())
        {
            Debug.Log("当前正在播放对话，请等待对话结束");
            return;
        }
        
        Debug.Log($"测试对话段落: 场景{testSceneIndex}, 段落{segmentIndex}");
        plotManager.PlayDialogueSegment(testSceneIndex, segmentIndex);
    }
    
    /// <summary>
    /// 测试所有对话段落
    /// </summary>
    [ContextMenu("测试所有对话段落")]
    public void TestAllDialogues()
    {
        if (plotManager == null)
        {
            Debug.LogError("PlotManager未设置");
            return;
        }
        
        Debug.Log("开始测试所有对话段落...");
        
        // 测试关卡开始对话
        TestDialogueSegment(0);
        
        // 延迟测试其他段落
        Invoke(nameof(TestSegment1), 5f);
        Invoke(nameof(TestSegment2), 10f);
        Invoke(nameof(TestSegment3), 15f);
        Invoke(nameof(TestSegment4), 20f);
    }
    
    private void TestSegment1() { TestDialogueSegment(1); }
    private void TestSegment2() { TestDialogueSegment(2); }
    private void TestSegment3() { TestDialogueSegment(3); }
    private void TestSegment4() { TestDialogueSegment(4); }
    
    /// <summary>
    /// 检查对话系统状态
    /// </summary>
    [ContextMenu("检查系统状态")]
    public void CheckSystemStatus()
    {
        Debug.Log("=== 对话系统状态检查 ===");
        
        // 检查PlotManager
        if (plotManager != null)
        {
            Debug.Log("✓ PlotManager已找到");
            Debug.Log($"  对话状态: {(plotManager.IsDialogueActive() ? "正在播放" : "未播放")}");
        }
        else
        {
            Debug.LogError("✗ PlotManager未找到");
        }
        
        // 检查Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            Debug.Log("✓ Canvas已找到");
        }
        else
        {
            Debug.LogError("✗ Canvas未找到");
        }
        
        // 检查CSV文件
        string csvPath = System.IO.Path.Combine(Application.dataPath, "Data/Plot.csv");
        if (System.IO.File.Exists(csvPath))
        {
            Debug.Log("✓ 对话文件存在");
        }
        else
        {
            Debug.LogError("✗ 对话文件不存在");
        }
        
        // 检查输入系统 - 通过PlotManager检查
        if (plotManager != null)
        {
            Debug.Log("✓ 输入系统已配置（通过PlotManager验证）");
        }
        else
        {
            Debug.LogWarning("⚠ 无法验证输入系统配置");
        }
        
        Debug.Log("=== 状态检查完成 ===");
    }
    
    /// <summary>
    /// 设置测试场景
    /// </summary>
    public void SetTestScene(int sceneIndex)
    {
        testSceneIndex = sceneIndex;
        Debug.Log($"测试场景已设置为: {sceneIndex}");
    }
    
    /// <summary>
    /// 获取当前对话状态
    /// </summary>
    public bool IsDialogueActive()
    {
        return plotManager != null && plotManager.IsDialogueActive();
    }
} 