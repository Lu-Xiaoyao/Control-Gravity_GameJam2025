using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minimap : MonoBehaviour
{
    [Header("小地图设置")]
    [SerializeField] private Camera minimapCamera;
    [SerializeField] private RawImage minimapImage;
    [SerializeField] private RectTransform minimapRect;
    [SerializeField] private float minimapSize = 200f;
    [SerializeField] private Vector2 minimapPosition = new Vector2(20, -20); // 左上角位置
    
    [Header("地图对象")]
    [SerializeField] private Transform player;
    [SerializeField] private List<Transform> importantObjects = new List<Transform>();
    [SerializeField] private List<MinimapIcon> minimapIcons = new List<MinimapIcon>();
    
    [Header("小地图图标预制体")]
    [SerializeField] private GameObject playerIconPrefab;
    [SerializeField] private GameObject planetIconPrefab;
    [SerializeField] private GameObject doorIconPrefab;
    [SerializeField] private GameObject starIconPrefab;
    
    [Header("小地图控制")]
    [SerializeField] private bool isMinimapVisible = true;
    [SerializeField] private bool canToggleMinimap = true;
    [SerializeField] private KeyCode toggleKey = KeyCode.M;
    
    private Dictionary<Transform, GameObject> iconInstances = new Dictionary<Transform, GameObject>();
    private Camera mainCamera;
    private Vector3 mapCenter;
    private float mapRadius = 50f;
    
    [System.Serializable]
    public class MinimapIcon
    {
        public string objectTag;
        public GameObject iconPrefab;
        public Color iconColor = Color.white;
        public float iconSize = 1f;
    }
    
    void Start()
    {
        InitializeMinimap();
        SetupMinimapCamera();
        CreateMinimapIcons();
        PositionMinimap();
    }
    
    void Update()
    {
        if (canToggleMinimap && Input.GetKeyDown(toggleKey))
        {
            ToggleMinimap();
        }
        
        if (isMinimapVisible)
        {
            UpdateMinimapCamera();
            UpdatePlayerIcon();
            UpdateImportantObjectIcons();
        }
    }
    
    void InitializeMinimap()
    {
        // 查找主相机
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            mainCamera = FindObjectOfType<Camera>();
        }
        
        // 查找玩家
        if (player == null)
        {
            GameObject playerObj = GameObject.Find("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }
        
        // 计算地图中心点
        CalculateMapCenter();
    }
    
    void CalculateMapCenter()
    {
        // 获取所有重要对象的中心点
        Vector3 totalPosition = Vector3.zero;
        int objectCount = 0;
        
        if (player != null)
        {
            totalPosition += player.position;
            objectCount++;
        }
        
        foreach (Transform obj in importantObjects)
        {
            if (obj != null)
            {
                totalPosition += obj.position;
                objectCount++;
            }
        }
        
        if (objectCount > 0)
        {
            mapCenter = totalPosition / objectCount;
        }
        else
        {
            mapCenter = Vector3.zero;
        }
    }
    
    void SetupMinimapCamera()
    {
        if (minimapCamera == null)
        {
            // 创建小地图相机
            GameObject cameraObj = new GameObject("MinimapCamera");
            minimapCamera = cameraObj.AddComponent<Camera>();
            minimapCamera.transform.SetParent(transform);
        }
        
        // 设置小地图相机属性
        minimapCamera.orthographic = true;
        minimapCamera.orthographicSize = mapRadius;
        minimapCamera.cullingMask = LayerMask.GetMask("Default"); // 根据需要调整层级
        minimapCamera.depth = -1;
        minimapCamera.clearFlags = CameraClearFlags.SolidColor;
        minimapCamera.backgroundColor = new Color(0, 0, 0, 0.3f);
        
        // 设置渲染纹理
        if (minimapImage != null)
        {
            RenderTexture renderTexture = new RenderTexture(256, 256, 16);
            minimapCamera.targetTexture = renderTexture;
            minimapImage.texture = renderTexture;
        }
    }
    
    void CreateMinimapIcons()
    {
        // 创建玩家图标
        if (player != null && playerIconPrefab != null)
        {
            GameObject playerIcon = Instantiate(playerIconPrefab, minimapRect);
            iconInstances[player] = playerIcon;
        }
        
        // 创建重要对象图标
        foreach (Transform obj in importantObjects)
        {
            if (obj != null)
            {
                GameObject iconPrefab = GetIconPrefabForObject(obj);
                if (iconPrefab != null)
                {
                    GameObject icon = Instantiate(iconPrefab, minimapRect);
                    iconInstances[obj] = icon;
                }
            }
        }
    }
    
    GameObject GetIconPrefabForObject(Transform obj)
    {
        // 根据对象标签或组件类型返回对应的图标预制体
        if (obj.CompareTag("Planet"))
        {
            return planetIconPrefab;
        }
        else if (obj.CompareTag("Door"))
        {
            return doorIconPrefab;
        }
        else if (obj.CompareTag("Star"))
        {
            return starIconPrefab;
        }
        
        // 如果没有找到匹配的预制体，返回默认的星球图标
        return planetIconPrefab;
    }
    
    void PositionMinimap()
    {
        if (minimapRect != null)
        {
            // 设置小地图位置和大小
            minimapRect.anchorMin = new Vector2(0, 1);
            minimapRect.anchorMax = new Vector2(0, 1);
            minimapRect.sizeDelta = new Vector2(minimapSize, minimapSize);
            minimapRect.anchoredPosition = minimapPosition;
        }
    }
    
    void UpdateMinimapCamera()
    {
        if (minimapCamera != null && player != null)
        {
            // 更新小地图相机位置，跟随玩家
            Vector3 cameraPos = player.position;
            cameraPos.z = -10f; // 确保相机在游戏对象前方
            minimapCamera.transform.position = cameraPos;
        }
    }
    
    void UpdatePlayerIcon()
    {
        if (player != null && iconInstances.ContainsKey(player))
        {
            GameObject playerIcon = iconInstances[player];
            if (playerIcon != null)
            {
                // 玩家图标始终在中心，所以不需要更新位置
                // 但可以更新旋转以显示玩家朝向
                playerIcon.transform.rotation = player.rotation;
            }
        }
    }
    
    void UpdateImportantObjectIcons()
    {
        foreach (var kvp in iconInstances)
        {
            Transform obj = kvp.Key;
            GameObject icon = kvp.Value;
            
            if (obj != null && icon != null && obj != player)
            {
                // 计算对象在小地图上的位置
                Vector3 relativePos = obj.position - player.position;
                Vector2 minimapPos = new Vector2(
                    (relativePos.x / mapRadius) * (minimapSize * 0.5f),
                    (relativePos.y / mapRadius) * (minimapSize * 0.5f)
                );
                
                // 限制图标在小地图范围内
                minimapPos = Vector2.ClampMagnitude(minimapPos, minimapSize * 0.4f);
                
                icon.GetComponent<RectTransform>().anchoredPosition = minimapPos;
            }
        }
    }
    
    public void ToggleMinimap()
    {
        isMinimapVisible = !isMinimapVisible;
        if (minimapRect != null)
        {
            minimapRect.gameObject.SetActive(isMinimapVisible);
        }
        if (minimapCamera != null)
        {
            minimapCamera.gameObject.SetActive(isMinimapVisible);
        }
    }
    
    public void SetMinimapVisible(bool visible)
    {
        isMinimapVisible = visible;
        if (minimapRect != null)
        {
            minimapRect.gameObject.SetActive(isMinimapVisible);
        }
        if (minimapCamera != null)
        {
            minimapCamera.gameObject.SetActive(isMinimapVisible);
        }
    }
    
    public void AddImportantObject(Transform obj)
    {
        if (!importantObjects.Contains(obj))
        {
            importantObjects.Add(obj);
            
            // 为新对象创建图标
            GameObject iconPrefab = GetIconPrefabForObject(obj);
            if (iconPrefab != null)
            {
                GameObject icon = Instantiate(iconPrefab, minimapRect);
                iconInstances[obj] = icon;
            }
        }
    }
    
    public void RemoveImportantObject(Transform obj)
    {
        if (importantObjects.Contains(obj))
        {
            importantObjects.Remove(obj);
            
            // 移除对象图标
            if (iconInstances.ContainsKey(obj))
            {
                if (iconInstances[obj] != null)
                {
                    Destroy(iconInstances[obj]);
                }
                iconInstances.Remove(obj);
            }
        }
    }
    
    void OnDestroy()
    {
        // 清理渲染纹理
        if (minimapCamera != null && minimapCamera.targetTexture != null)
        {
            minimapCamera.targetTexture.Release();
        }
    }
} 