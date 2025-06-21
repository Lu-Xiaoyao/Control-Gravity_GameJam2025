using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

#if UNITY_EDITOR
public class MinimapSetupHelper : MonoBehaviour
{
    [Header("自动设置")]
    [SerializeField] private bool autoSetupOnStart = true;
    [SerializeField] private ProportionalMinimap minimap;
    
    [Header("图标资源路径")]
    [SerializeField] private string playerIconPath = "Images/PlayerIdel";
    [SerializeField] private string planetIconPath = "Images/Planet0";
    [SerializeField] private string doorIconPath = "Images/Door";
    [SerializeField] private string starIconPath = "Images/Star";
    [SerializeField] private string trapIconPath = "Images/Trap1";
    
    void Start()
    {
        if (autoSetupOnStart)
        {
            SetupMinimap();
        }
    }
    
    [ContextMenu("设置小地图")]
    public void SetupMinimap()
    {
        if (minimap == null)
        {
            minimap = FindObjectOfType<ProportionalMinimap>();
            if (minimap == null)
            {
                Debug.LogError("未找到ProportionalMinimap组件！");
                return;
            }
        }
        
        LoadAndSetIcons();
        Debug.Log("小地图设置完成！");
    }
    
    void LoadAndSetIcons()
    {
        // 加载玩家图标
        Sprite playerIcon = LoadSprite(playerIconPath);
        if (playerIcon != null)
        {
            SetPrivateField(minimap, "playerIconSprite", playerIcon);
            Debug.Log($"已设置玩家图标: {playerIconPath}");
        }
        
        // 加载星球图标
        Sprite planetIcon = LoadSprite(planetIconPath);
        if (planetIcon != null)
        {
            SetPrivateField(minimap, "planetIconSprite", planetIcon);
            Debug.Log($"已设置星球图标: {planetIconPath}");
        }
        
        // 加载门图标
        Sprite doorIcon = LoadSprite(doorIconPath);
        if (doorIcon != null)
        {
            SetPrivateField(minimap, "doorIconSprite", doorIcon);
            Debug.Log($"已设置门图标: {doorIconPath}");
        }
        
        // 加载星星图标
        Sprite starIcon = LoadSprite(starIconPath);
        if (starIcon != null)
        {
            SetPrivateField(minimap, "starIconSprite", starIcon);
            Debug.Log($"已设置星星图标: {starIconPath}");
        }
        
        // 加载陷阱图标
        Sprite trapIcon = LoadSprite(trapIconPath);
        if (trapIcon != null)
        {
            SetPrivateField(minimap, "trapIconSprite", trapIcon);
            Debug.Log($"已设置陷阱图标: {trapIconPath}");
        }
    }
    
    Sprite LoadSprite(string path)
    {
        Sprite sprite = Resources.Load<Sprite>(path);
        if (sprite == null)
        {
            // 尝试从Assets文件夹加载
            sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/{path}.png");
        }
        return sprite;
    }
    
    void SetPrivateField(object target, string fieldName, object value)
    {
        var field = target.GetType().GetField(fieldName, 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (field != null)
        {
            field.SetValue(target, value);
        }
        else
        {
            Debug.LogWarning($"未找到字段: {fieldName}");
        }
    }
    
    [ContextMenu("检查场景设置")]
    public void CheckSceneSetup()
    {
        Debug.Log("=== 场景设置检查 ===");
        
        // 检查CameraArea（推荐）
        GameObject cameraArea = GameObject.Find("Camera Area");
        if (cameraArea != null)
        {
            SpriteRenderer spriteRenderer = cameraArea.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                Vector3 areaSize = spriteRenderer.bounds.size;
                Debug.Log($"✓ 找到CameraArea: 位置({cameraArea.transform.position}), 大小({areaSize.x:F1}, {areaSize.y:F1})");
            }
            else
            {
                Debug.LogWarning("✗ CameraArea没有SpriteRenderer组件");
            }
        }
        else
        {
            Debug.LogWarning("✗ 未找到CameraArea对象");
        }
        
        // 检查Camera边界（备选）
        GameObject cameraMin = GameObject.Find("CameraMin");
        GameObject cameraMax = GameObject.Find("CameraMax");
        
        if (cameraMin != null && cameraMax != null)
        {
            Debug.Log($"✓ 找到CameraMin/CameraMax: Min({cameraMin.transform.position}), Max({cameraMax.transform.position})");
        }
        else
        {
            Debug.LogWarning("✗ 未找到CameraMin或CameraMax对象");
        }
        
        // 检查玩家
        GameObject player = GameObject.Find("Player");
        if (player != null)
        {
            Debug.Log($"✓ 找到玩家对象: {player.name}");
        }
        else
        {
            Debug.LogWarning("✗ 未找到Player对象");
        }
        
        // 检查标签对象
        CheckTaggedObjects("Planet");
        CheckTaggedObjects("Door");
        CheckTaggedObjects("Star");
        CheckTaggedObjects("Trap");
        
        // 检查Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            Debug.Log($"✓ 找到Canvas: {canvas.name}");
        }
        else
        {
            Debug.LogWarning("✗ 未找到Canvas，小地图会自动创建一个");
        }
        
        // 检查ProportionalMinimap
        ProportionalMinimap minimap = FindObjectOfType<ProportionalMinimap>();
        if (minimap != null)
        {
            Debug.Log($"✓ 找到ProportionalMinimap组件");
        }
        else
        {
            Debug.LogWarning("✗ 未找到ProportionalMinimap组件");
        }
    }
    
    void CheckTaggedObjects(string tag)
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
        if (objects.Length > 0)
        {
            Debug.Log($"✓ 找到 {objects.Length} 个 {tag} 对象");
        }
        else
        {
            Debug.LogWarning($"✗ 未找到 {tag} 标签的对象");
        }
    }
    
    [ContextMenu("创建完整设置")]
    public void CreateCompleteSetup()
    {
        Debug.Log("=== 创建完整的小地图设置 ===");
        
        // 1. 检查或创建Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            Debug.Log("✓ 创建了Canvas");
        }
        
        // 2. 检查或创建ProportionalMinimap
        ProportionalMinimap minimap = FindObjectOfType<ProportionalMinimap>();
        if (minimap == null)
        {
            GameObject minimapObj = new GameObject("ProportionalMinimap");
            minimap = minimapObj.AddComponent<ProportionalMinimap>();
            Debug.Log("✓ 创建了ProportionalMinimap组件");
        }
        
        // 3. 设置图标
        LoadAndSetIcons();
        
        Debug.Log("✓ 完整设置创建完成！");
    }
}
#endif 