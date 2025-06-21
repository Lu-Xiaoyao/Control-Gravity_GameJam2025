using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Header("对话设置")]
    [SerializeField] private int sceneIndex = 1;
    [SerializeField] private int segmentIndex = 1;
    [SerializeField] private bool triggerOnce = true; // 是否只触发一次
    
    [Header("触发设置")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private bool useCollider = true; // 是否使用碰撞器触发
    [SerializeField] private float triggerDistance = 2f; // 如果不使用碰撞器，使用距离触发
    
    private bool hasTriggered = false;
    private PlotManager plotManager;
    private Transform playerTransform;
    
    private void Start()
    {
        // 获取PlotManager引用
        plotManager = FindObjectOfType<PlotManager>();
        if (plotManager == null)
        {
            Debug.LogError("场景中未找到PlotManager组件！");
        }
        
        // 获取玩家引用
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogWarning($"未找到标签为 {playerTag} 的玩家对象");
        }
        
        // 如果不使用碰撞器，确保有碰撞器组件用于距离检测
        if (!useCollider && GetComponent<Collider>() == null)
        {
            Debug.LogWarning("DialogueTrigger需要碰撞器组件用于距离检测");
        }
    }
    
    private void Update()
    {
        // 如果不使用碰撞器，使用距离检测
        if (!useCollider && playerTransform != null && !hasTriggered)
        {
            float distance = Vector3.Distance(transform.position, playerTransform.position);
            if (distance <= triggerDistance)
            {
                TriggerDialogue();
            }
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (useCollider && other.CompareTag(playerTag) && !hasTriggered)
        {
            TriggerDialogue();
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (useCollider && other.CompareTag(playerTag) && !hasTriggered)
        {
            TriggerDialogue();
        }
    }
    
    /// <summary>
    /// 触发对话
    /// </summary>
    private void TriggerDialogue()
    {
        if (plotManager != null && !hasTriggered)
        {
            plotManager.PlayDialogueSegment(sceneIndex, segmentIndex);
            
            if (triggerOnce)
            {
                hasTriggered = true;
                // 可选：禁用触发器
                // gameObject.SetActive(false);
            }
        }
    }
    
    /// <summary>
    /// 重置触发器状态（用于关卡重置等）
    /// </summary>
    public void ResetTrigger()
    {
        hasTriggered = false;
    }
    
    /// <summary>
    /// 手动触发对话（用于测试或其他脚本调用）
    /// </summary>
    [ContextMenu("手动触发对话")]
    public void ManualTrigger()
    {
        TriggerDialogue();
    }
    
    // 在Scene视图中绘制触发范围（仅用于距离触发）
    private void OnDrawGizmosSelected()
    {
        if (!useCollider)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, triggerDistance);
        }
    }
} 