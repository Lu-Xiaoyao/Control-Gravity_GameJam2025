using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProportionalMinimap : MonoBehaviour
{
    [Header("小地图设置")]
    [SerializeField] private RectTransform minimapContainer;
    [SerializeField] private float minimapBaseSize = 200f; // 基础大小（最大边长）
    [SerializeField] private Vector2 minimapPosition = new Vector2(20, -20);
    [SerializeField] private Color backgroundColor = new Color(0, 0, 0, 0.7f);
    [SerializeField] private Color borderColor = Color.white;
    [SerializeField] private float borderWidth = 2f;
    [SerializeField] private bool maintainAspectRatio = true; // 是否保持宽高比
    [SerializeField] private float maxAspectRatio = 2.0f; // 最大宽高比（防止过宽或过高）
    
    [Header("图标设置")]
    [SerializeField] private Sprite playerIconSprite;
    [SerializeField] private Sprite planetIconSprite;
    [SerializeField] private Sprite doorIconSprite;
    [SerializeField] private Sprite starIconSprite;
    [SerializeField] private Sprite trapIconSprite;
    [SerializeField] private Sprite starPlaceIconSprite;
    [SerializeField] private float iconSize = 12f;
    
    [Header("地图范围")]
    [SerializeField] private bool useCameraBounds = true;
    [SerializeField] private Vector2 manualMapBounds = new Vector2(50f, 50f);
    [SerializeField] private float mapPadding = 5f; // 地图边界的额外空间
    
    [Header("控制设置")]
    [SerializeField] private bool isVisible = true;
    [SerializeField] private KeyCode toggleKey = KeyCode.M;
    [SerializeField] private bool canToggle = true;
    
    // 场景边界
    private float sceneMinX, sceneMaxX, sceneMinY, sceneMaxY;
    private float mapWidth, mapHeight;
    
    // 游戏对象
    private Transform player;
    private List<Transform> planets = new List<Transform>();
    private List<Transform> doors = new List<Transform>();
    private List<Transform> stars = new List<Transform>();
    private List<Transform> traps = new List<Transform>();
    private List<Transform> starPlaces = new List<Transform>();
    
    // UI元素
    private GameObject playerIcon;
    private List<GameObject> planetIcons = new List<GameObject>();
    private List<GameObject> doorIcons = new List<GameObject>();
    private List<GameObject> starIcons = new List<GameObject>();
    private List<GameObject> trapIcons = new List<GameObject>();
    private List<GameObject> starPlaceIcons = new List<GameObject>();
    
    private Image backgroundImage;
    private RectTransform minimapRect;
    
    void Start()
    {
        InitializeMinimap();
        FindGameObjects();
        CreateMinimapUI();
        CreateIcons();
    }
    
    void Update()
    {
        if (canToggle && Input.GetKeyDown(toggleKey))
        {
            ToggleMinimap();
        }
        
        if (isVisible)
        {
            UpdateIcons();
        }
    }
    
    void InitializeMinimap()
    {
        // 查找玩家
        GameObject playerObj = GameObject.Find("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        
        // 计算地图边界
        CalculateMapBounds();
    }
    
    void CalculateMapBounds()
    {
        if (useCameraBounds)
        {
            // 优先使用CameraArea的边界
            GameObject cameraArea = GameObject.Find("Camera Area");
            
            if (cameraArea != null)
            {
                // 获取CameraArea的SpriteRenderer组件
                SpriteRenderer spriteRenderer = cameraArea.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    // 计算CameraArea的实际边界
                    Vector3 areaPosition = cameraArea.transform.position;
                    Vector3 areaSize = spriteRenderer.bounds.size;
                    
                    // 计算边界（考虑缩放）
                    sceneMinX = areaPosition.x - areaSize.x * 0.5f;
                    sceneMaxX = areaPosition.x + areaSize.x * 0.5f;
                    sceneMinY = areaPosition.y - areaSize.y * 0.5f;
                    sceneMaxY = areaPosition.y + areaSize.y * 0.5f;
                    
                    Debug.Log($"使用CameraArea边界: 位置({areaPosition.x:F1}, {areaPosition.y:F1}), 大小({areaSize.x:F1}, {areaSize.y:F1})");
                }
                else
                {
                    Debug.LogWarning("CameraArea没有SpriteRenderer组件，尝试使用CameraMin/CameraMax");
                    UseCameraMinMaxBounds();
                }
            }
            else
            {
                Debug.LogWarning("未找到CameraArea，尝试使用CameraMin/CameraMax");
                UseCameraMinMaxBounds();
            }
        }
        else
        {
            // 使用手动设置的边界
            sceneMinX = -manualMapBounds.x * 0.5f;
            sceneMaxX = manualMapBounds.x * 0.5f;
            sceneMinY = -manualMapBounds.y * 0.5f;
            sceneMaxY = manualMapBounds.y * 0.5f;
        }
        
        // 添加边距
        sceneMinX -= mapPadding;
        sceneMaxX += mapPadding;
        sceneMinY -= mapPadding;
        sceneMaxY += mapPadding;
        
        // 计算地图尺寸
        mapWidth = sceneMaxX - sceneMinX;
        mapHeight = sceneMaxY - sceneMinY;
        
        Debug.Log($"最终地图边界: X({sceneMinX:F1}, {sceneMaxX:F1}), Y({sceneMinY:F1}, {sceneMaxY:F1})");
        Debug.Log($"地图尺寸: {mapWidth:F1} x {mapHeight:F1}");
    }
    
    void UseCameraMinMaxBounds()
    {
        // 使用CameraMin和CameraMax作为备选方案
        GameObject cameraMin = GameObject.Find("CameraMin");
        GameObject cameraMax = GameObject.Find("CameraMax");
        
        if (cameraMin != null && cameraMax != null)
        {
            sceneMinX = cameraMin.transform.position.x;
            sceneMinY = cameraMin.transform.position.y;
            sceneMaxX = cameraMax.transform.position.x;
            sceneMaxY = cameraMax.transform.position.y;
            
            Debug.Log($"使用CameraMin/CameraMax边界: Min({sceneMinX:F1}, {sceneMinY:F1}), Max({sceneMaxX:F1}, {sceneMaxY:F1})");
        }
        else
        {
            Debug.LogWarning("未找到CameraMin或CameraMax，使用自动检测边界");
            AutoDetectMapBounds();
        }
    }
    
    void AutoDetectMapBounds()
    {
        // 查找所有重要对象来确定地图边界
        GameObject[] allPlanets = GameObject.FindGameObjectsWithTag("Planet");
        GameObject[] allDoors = GameObject.FindGameObjectsWithTag("Door");
        GameObject[] allStars = GameObject.FindGameObjectsWithTag("Star");
        GameObject[] allTraps = GameObject.FindGameObjectsWithTag("Trap");
        
        float minX = float.MaxValue, maxX = float.MinValue;
        float minY = float.MaxValue, maxY = float.MinValue;
        
        // 检查玩家位置
        if (player != null)
        {
            minX = Mathf.Min(minX, player.position.x);
            maxX = Mathf.Max(maxX, player.position.x);
            minY = Mathf.Min(minY, player.position.y);
            maxY = Mathf.Max(maxY, player.position.y);
        }
        
        // 检查所有对象
        CheckObjectsBounds(allPlanets, ref minX, ref maxX, ref minY, ref maxY);
        CheckObjectsBounds(allDoors, ref minX, ref maxX, ref minY, ref maxY);
        CheckObjectsBounds(allStars, ref minX, ref maxX, ref minY, ref maxY);
        CheckObjectsBounds(allTraps, ref minX, ref maxX, ref minY, ref maxY);
        
        // 设置地图边界
        sceneMinX = minX - mapPadding;
        sceneMaxX = maxX + mapPadding;
        sceneMinY = minY - mapPadding;
        sceneMaxY = maxY + mapPadding;
        
        mapWidth = sceneMaxX - sceneMinX;
        mapHeight = sceneMaxY - sceneMinY;
    }
    
    void CheckObjectsBounds(GameObject[] objects, ref float minX, ref float maxX, ref float minY, ref float maxY)
    {
        foreach (GameObject obj in objects)
        {
            if (obj != null)
            {
                minX = Mathf.Min(minX, obj.transform.position.x);
                maxX = Mathf.Max(maxX, obj.transform.position.x);
                minY = Mathf.Min(minY, obj.transform.position.y);
                maxY = Mathf.Max(maxY, obj.transform.position.y);
            }
        }
    }
    
    void FindGameObjects()
    {
        // 查找所有游戏对象
        FindObjectsWithTag("Planet", planets);
        FindObjectsWithTag("Door", doors);
        FindObjectsWithTag("Star", stars);
        FindObjectsWithTag("Trap", traps);
        FindObjectsWithTag("StarPlace", starPlaces);
    }
    
    void FindObjectsWithTag(string tag, List<Transform> targetList)
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
        foreach (GameObject obj in objects)
        {
            targetList.Add(obj.transform);
        }
    }
    
    void CreateMinimapUI()
    {
        // 创建小地图容器
        if (minimapContainer == null)
        {
            GameObject containerObj = new GameObject("ProportionalMinimapContainer");
            minimapContainer = containerObj.AddComponent<RectTransform>();
            
            // 添加到Canvas
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                // 如果没有Canvas，创建一个
                GameObject canvasObj = new GameObject("Canvas");
                canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasObj.AddComponent<CanvasScaler>();
                canvasObj.AddComponent<GraphicRaycaster>();
            }
            
            minimapContainer.SetParent(canvas.transform, false);
        }
        
        // 计算小地图的实际大小（根据场景比例）
        Vector2 minimapSize = CalculateMinimapSize();
        
        // 设置小地图位置和大小
        minimapContainer.anchorMin = new Vector2(0, 1);
        minimapContainer.anchorMax = new Vector2(0, 1);
        minimapContainer.sizeDelta = minimapSize;
        minimapContainer.anchoredPosition = minimapPosition;
        
        // 创建背景
        GameObject backgroundObj = new GameObject("MinimapBackground");
        backgroundObj.transform.SetParent(minimapContainer, false);
        
        backgroundImage = backgroundObj.AddComponent<Image>();
        backgroundImage.color = borderColor;
        
        RectTransform bgRect = backgroundObj.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;
        bgRect.anchoredPosition = Vector2.zero;
        
        // 创建边框
        GameObject borderObj = new GameObject("MinimapBorder");
        borderObj.transform.SetParent(minimapContainer, false);
        
        Image borderImage = borderObj.AddComponent<Image>();
        borderImage.color = backgroundColor;
        
        RectTransform borderRect = borderObj.GetComponent<RectTransform>();
        borderRect.anchorMin = Vector2.zero;
        borderRect.anchorMax = Vector2.one;
        borderRect.sizeDelta = new Vector2(-borderWidth * 2, -borderWidth * 2);
        borderRect.anchoredPosition = Vector2.zero;
        
        minimapRect = minimapContainer;
    }
    
    Vector2 CalculateMinimapSize()
    {
        if (!maintainAspectRatio)
        {
            // 如果不保持宽高比，使用正方形
            return new Vector2(minimapBaseSize, minimapBaseSize);
        }
        
        // 计算场景的宽高比
        float sceneAspectRatio = mapWidth / mapHeight;
        
        // 限制最大宽高比
        sceneAspectRatio = Mathf.Clamp(sceneAspectRatio, 1f / maxAspectRatio, maxAspectRatio);
        
        float minimapWidth, minimapHeight;
        
        if (sceneAspectRatio > 1f)
        {
            // 场景更宽
            minimapWidth = minimapBaseSize;
            minimapHeight = minimapBaseSize / sceneAspectRatio;
        }
        else
        {
            // 场景更高
            minimapWidth = minimapBaseSize * sceneAspectRatio;
            minimapHeight = minimapBaseSize;
        }
        
        Debug.Log($"场景比例: {mapWidth:F1} x {mapHeight:F1} (比例: {sceneAspectRatio:F2})");
        Debug.Log($"小地图大小: {minimapWidth:F1} x {minimapHeight:F1}");
        
        return new Vector2(minimapWidth, minimapHeight);
    }
    
    void CreateIcons()
    {
        // 创建玩家图标
        if (player != null)
        {
            playerIcon = CreateIcon("PlayerIcon", playerIconSprite, Color.white);
        }
        
        // 创建星球图标
        CreateIconsForObjects(planets, planetIcons, planetIconSprite, Color.blue);
        
        // 创建门图标
        CreateIconsForObjects(doors, doorIcons, doorIconSprite, Color.green);
        
        // 创建星星图标
        CreateIconsForObjects(stars, starIcons, starIconSprite, Color.yellow);
        
        // 创建陷阱图标
        CreateIconsForObjects(traps, trapIcons, trapIconSprite, Color.red);
        
        // 创建StarPlace图标
        CreateIconsForObjects(starPlaces, starPlaceIcons, starPlaceIconSprite, Color.cyan);
    }
    
    void CreateIconsForObjects(List<Transform> objects, List<GameObject> iconList, Sprite iconSprite, Color defaultColor)
    {
        foreach (Transform obj in objects)
        {
            GameObject icon = CreateIcon($"{obj.name}Icon", iconSprite, defaultColor);
            iconList.Add(icon);
        }
    }
    
    GameObject CreateIcon(string name, Sprite iconSprite, Color color)
    {
        GameObject iconObj = new GameObject(name);
        iconObj.transform.SetParent(minimapContainer, false);
        
        Image iconImage = iconObj.AddComponent<Image>();
        if (iconSprite != null)
        {
            iconImage.sprite = iconSprite;
        }
        else
        {
            // 如果没有指定图片，创建一个简单的彩色方块
            iconImage.color = color;
        }
        
        RectTransform iconRect = iconObj.GetComponent<RectTransform>();
        iconRect.sizeDelta = new Vector2(iconSize, iconSize);
        iconRect.anchorMin = new Vector2(0.5f, 0.5f);
        iconRect.anchorMax = new Vector2(0.5f, 0.5f);
        
        return iconObj;
    }
    
    void UpdateIcons()
    {
        // 更新玩家图标
        if (player != null && playerIcon != null)
        {
            UpdateIconPosition(playerIcon, player.position);
        }
        
        // 更新其他图标
        UpdateIconsForObjects(planets, planetIcons);
        UpdateIconsForObjects(doors, doorIcons);
        UpdateIconsForObjects(stars, starIcons);
        UpdateIconsForObjects(traps, trapIcons);
        UpdateIconsForObjects(starPlaces, starPlaceIcons);
    }
    
    void UpdateIconsForObjects(List<Transform> objects, List<GameObject> icons)
    {
        for (int i = 0; i < objects.Count && i < icons.Count; i++)
        {
            if (objects[i] != null && icons[i] != null)
            {
                UpdateIconPosition(icons[i], objects[i].position);
            }
        }
    }
    
    void UpdateIconPosition(GameObject icon, Vector3 worldPosition)
    {
        if (icon == null || minimapRect == null) return;
        
        // 将世界坐标转换为小地图坐标
        Vector2 minimapPosition = WorldToMinimapPosition(worldPosition);
        
        RectTransform iconRect = icon.GetComponent<RectTransform>();
        iconRect.anchoredPosition = minimapPosition;
    }
    
    Vector2 WorldToMinimapPosition(Vector3 worldPosition)
    {
        // 计算世界坐标在地图范围内的比例
        float xRatio = (worldPosition.x - sceneMinX) / mapWidth;
        float yRatio = (worldPosition.y - sceneMinY) / mapHeight;
        
        // 获取实际的小地图尺寸
        Vector2 actualMinimapSize = minimapRect.sizeDelta;
        
        // 计算内部背景的尺寸
        float innerWidth = actualMinimapSize.x - borderWidth * 2;
        float innerHeight = actualMinimapSize.y - borderWidth * 2;

        // 图标中心点可以移动的有效范围要再减去图标本身的大小
        float movableWidth = innerWidth - iconSize;
        float movableHeight = innerHeight - iconSize;

        // 将比例转换为小地图坐标
        float minimapX = (xRatio - 0.5f) * movableWidth;
        float minimapY = (yRatio - 0.5f) * movableHeight;
        
        return new Vector2(minimapX, minimapY);
    }
    
    public void ToggleMinimap()
    {
        isVisible = !isVisible;
        SetMinimapVisible(isVisible);
    }
    
    public void SetMinimapVisible(bool visible)
    {
        isVisible = visible;
        if (minimapContainer != null)
        {
            minimapContainer.gameObject.SetActive(visible);
        }
    }
    
    // 公共方法，用于动态添加/移除对象
    public void AddPlanet(Transform planet)
    {
        if (!planets.Contains(planet))
        {
            planets.Add(planet);
            GameObject icon = CreateIcon($"{planet.name}Icon", planetIconSprite, Color.blue);
            planetIcons.Add(icon);
        }
    }
    
    public void RemovePlanet(Transform planet)
    {
        int index = planets.IndexOf(planet);
        if (index >= 0)
        {
            planets.RemoveAt(index);
            if (index < planetIcons.Count)
            {
                if (planetIcons[index] != null)
                {
                    Destroy(planetIcons[index]);
                }
                planetIcons.RemoveAt(index);
            }
        }
    }
    
    public void AddDoor(Transform door)
    {
        if (!doors.Contains(door))
        {
            doors.Add(door);
            GameObject icon = CreateIcon($"{door.name}Icon", doorIconSprite, Color.green);
            doorIcons.Add(icon);
        }
    }
    
    public void AddStar(Transform star)
    {
        if (!stars.Contains(star))
        {
            stars.Add(star);
            GameObject icon = CreateIcon($"{star.name}Icon", starIconSprite, Color.yellow);
            starIcons.Add(icon);
        }
    }
    
    public void AddTrap(Transform trap)
    {
        if (!traps.Contains(trap))
        {
            traps.Add(trap);
            GameObject icon = CreateIcon($"{trap.name}Icon", trapIconSprite, Color.red);
            trapIcons.Add(icon);
        }
    }
    
    public void AddStarPlace(Transform starPlace)
    {
        if (!starPlaces.Contains(starPlace))
        {
            starPlaces.Add(starPlace);
            GameObject icon = CreateIcon($"{starPlace.name}Icon", starPlaceIconSprite, Color.cyan);
            starPlaceIcons.Add(icon);
        }
    }
    
    // 获取地图信息
    public Vector2 GetMapBounds()
    {
        return new Vector2(mapWidth, mapHeight);
    }
    
    public Vector4 GetSceneBounds()
    {
        return new Vector4(sceneMinX, sceneMaxX, sceneMinY, sceneMaxY);
    }
} 