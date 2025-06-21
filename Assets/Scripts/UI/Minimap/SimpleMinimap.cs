using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimpleMinimap : MonoBehaviour
{
    [Header("小地图设置")]
    [SerializeField] private RectTransform minimapContainer;
    [SerializeField] private float minimapSize = 150f;
    [SerializeField] private Vector2 minimapPosition = new Vector2(20, -20);
    [SerializeField] private Color backgroundColor = new Color(0, 0, 0, 0.5f);
    [SerializeField] private Color borderColor = Color.white;
    
    [Header("图标设置")]
    [SerializeField] private GameObject playerIconPrefab;
    [SerializeField] private GameObject planetIconPrefab;
    [SerializeField] private GameObject doorIconPrefab;
    [SerializeField] private GameObject starIconPrefab;
    [SerializeField] private float iconSize = 8f;
    
    [Header("地图范围")]
    [SerializeField] private float mapRadius = 30f;
    [SerializeField] private bool autoDetectMapBounds = true;
    [SerializeField] private Vector2 mapBounds = new Vector2(50f, 50f);
    
    [Header("控制设置")]
    [SerializeField] private bool isVisible = true;
    [SerializeField] private KeyCode toggleKey = KeyCode.M;
    [SerializeField] private bool canToggle = true;
    
    private Transform player;
    private List<Transform> planets = new List<Transform>();
    private List<Transform> doors = new List<Transform>();
    private List<Transform> stars = new List<Transform>();
    
    private GameObject playerIcon;
    private List<GameObject> planetIcons = new List<GameObject>();
    private List<GameObject> doorIcons = new List<GameObject>();
    private List<GameObject> starIcons = new List<GameObject>();
    
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
        if (autoDetectMapBounds)
        {
            CalculateMapBounds();
        }
    }
    
    void CalculateMapBounds()
    {
        // 查找所有重要对象来确定地图边界
        GameObject[] allPlanets = GameObject.FindGameObjectsWithTag("Planet");
        GameObject[] allDoors = GameObject.FindGameObjectsWithTag("Door");
        GameObject[] allStars = GameObject.FindGameObjectsWithTag("Star");
        
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
        
        // 检查所有星球
        foreach (GameObject planet in allPlanets)
        {
            minX = Mathf.Min(minX, planet.transform.position.x);
            maxX = Mathf.Max(maxX, planet.transform.position.x);
            minY = Mathf.Min(minY, planet.transform.position.y);
            maxY = Mathf.Max(maxY, planet.transform.position.y);
        }
        
        // 检查所有门
        foreach (GameObject door in allDoors)
        {
            minX = Mathf.Min(minX, door.transform.position.x);
            maxX = Mathf.Max(maxX, door.transform.position.x);
            minY = Mathf.Min(minY, door.transform.position.y);
            maxY = Mathf.Max(maxY, door.transform.position.y);
        }
        
        // 检查所有星星
        foreach (GameObject star in allStars)
        {
            minX = Mathf.Min(minX, star.transform.position.x);
            maxX = Mathf.Max(maxX, star.transform.position.x);
            minY = Mathf.Min(minY, star.transform.position.y);
            maxY = Mathf.Max(maxY, star.transform.position.y);
        }
        
        // 设置地图边界
        mapBounds.x = Mathf.Max(maxX - minX, 20f); // 最小宽度
        mapBounds.y = Mathf.Max(maxY - minY, 20f); // 最小高度
        mapRadius = Mathf.Max(mapBounds.x, mapBounds.y) * 0.6f;
    }
    
    void FindGameObjects()
    {
        // 查找所有星球
        GameObject[] planetObjects = GameObject.FindGameObjectsWithTag("Planet");
        foreach (GameObject planet in planetObjects)
        {
            planets.Add(planet.transform);
        }
        
        // 查找所有门
        GameObject[] doorObjects = GameObject.FindGameObjectsWithTag("Door");
        foreach (GameObject door in doorObjects)
        {
            doors.Add(door.transform);
        }
        
        // 查找所有星星
        GameObject[] starObjects = GameObject.FindGameObjectsWithTag("Star");
        foreach (GameObject star in starObjects)
        {
            stars.Add(star.transform);
        }
    }
    
    void CreateMinimapUI()
    {
        // 创建小地图容器
        if (minimapContainer == null)
        {
            GameObject containerObj = new GameObject("MinimapContainer");
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
        
        // 设置小地图位置和大小
        minimapContainer.anchorMin = new Vector2(0, 1);
        minimapContainer.anchorMax = new Vector2(0, 1);
        minimapContainer.sizeDelta = new Vector2(minimapSize, minimapSize);
        minimapContainer.anchoredPosition = minimapPosition;
        
        // 创建背景
        GameObject backgroundObj = new GameObject("MinimapBackground");
        backgroundObj.transform.SetParent(minimapContainer, false);
        
        backgroundImage = backgroundObj.AddComponent<Image>();
        backgroundImage.color = backgroundColor;
        
        RectTransform bgRect = backgroundObj.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;
        bgRect.anchoredPosition = Vector2.zero;
        
        // 创建边框
        GameObject borderObj = new GameObject("MinimapBorder");
        borderObj.transform.SetParent(minimapContainer, false);
        
        Image borderImage = borderObj.AddComponent<Image>();
        borderImage.color = borderColor;
        
        RectTransform borderRect = borderObj.GetComponent<RectTransform>();
        borderRect.anchorMin = Vector2.zero;
        borderRect.anchorMax = Vector2.one;
        borderRect.sizeDelta = new Vector2(2, 2); // 边框宽度
        borderRect.anchoredPosition = Vector2.zero;
        
        minimapRect = minimapContainer;
    }
    
    void CreateIcons()
    {
        // 创建玩家图标
        if (player != null && playerIconPrefab != null)
        {
            playerIcon = Instantiate(playerIconPrefab, minimapRect);
            RectTransform playerIconRect = playerIcon.GetComponent<RectTransform>();
            playerIconRect.sizeDelta = new Vector2(iconSize, iconSize);
        }
        else if (player != null)
        {
            // 如果没有预制体，创建一个简单的玩家图标
            playerIcon = CreateSimpleIcon("PlayerIcon", Color.blue);
        }
        
        // 创建星球图标
        foreach (Transform planet in planets)
        {
            GameObject icon = null;
            if (planetIconPrefab != null)
            {
                icon = Instantiate(planetIconPrefab, minimapRect);
            }
            else
            {
                icon = CreateSimpleIcon("PlanetIcon", Color.green);
            }
            
            RectTransform iconRect = icon.GetComponent<RectTransform>();
            iconRect.sizeDelta = new Vector2(iconSize, iconSize);
            planetIcons.Add(icon);
        }
        
        // 创建门图标
        foreach (Transform door in doors)
        {
            GameObject icon = null;
            if (doorIconPrefab != null)
            {
                icon = Instantiate(doorIconPrefab, minimapRect);
            }
            else
            {
                icon = CreateSimpleIcon("DoorIcon", Color.yellow);
            }
            
            RectTransform iconRect = icon.GetComponent<RectTransform>();
            iconRect.sizeDelta = new Vector2(iconSize, iconSize);
            doorIcons.Add(icon);
        }
        
        // 创建星星图标
        foreach (Transform star in stars)
        {
            GameObject icon = null;
            if (starIconPrefab != null)
            {
                icon = Instantiate(starIconPrefab, minimapRect);
            }
            else
            {
                icon = CreateSimpleIcon("StarIcon", Color.white);
            }
            
            RectTransform iconRect = icon.GetComponent<RectTransform>();
            iconRect.sizeDelta = new Vector2(iconSize, iconSize);
            starIcons.Add(icon);
        }
    }
    
    GameObject CreateSimpleIcon(string name, Color color)
    {
        GameObject icon = new GameObject(name);
        icon.transform.SetParent(minimapRect, false);
        
        Image iconImage = icon.AddComponent<Image>();
        iconImage.color = color;
        
        RectTransform iconRect = icon.GetComponent<RectTransform>();
        iconRect.sizeDelta = new Vector2(iconSize, iconSize);
        
        return icon;
    }
    
    void UpdateIcons()
    {
        if (player == null) return;
        
        // 更新玩家图标位置（始终在中心）
        if (playerIcon != null)
        {
            RectTransform playerIconRect = playerIcon.GetComponent<RectTransform>();
            playerIconRect.anchoredPosition = Vector2.zero;
            
            // 更新玩家朝向
            playerIconRect.rotation = player.rotation;
        }
        
        // 更新星球图标位置
        for (int i = 0; i < planets.Count && i < planetIcons.Count; i++)
        {
            if (planets[i] != null && planetIcons[i] != null)
            {
                UpdateIconPosition(planetIcons[i], planets[i].position);
            }
        }
        
        // 更新门图标位置
        for (int i = 0; i < doors.Count && i < doorIcons.Count; i++)
        {
            if (doors[i] != null && doorIcons[i] != null)
            {
                UpdateIconPosition(doorIcons[i], doors[i].position);
            }
        }
        
        // 更新星星图标位置
        for (int i = 0; i < stars.Count && i < starIcons.Count; i++)
        {
            if (stars[i] != null && starIcons[i] != null)
            {
                UpdateIconPosition(starIcons[i], stars[i].position);
            }
        }
    }
    
    void UpdateIconPosition(GameObject icon, Vector3 worldPosition)
    {
        // 计算相对于玩家的位置
        Vector3 relativePos = worldPosition - player.position;
        
        // 转换为小地图坐标
        Vector2 minimapPos = new Vector2(
            (relativePos.x / mapRadius) * (minimapSize * 0.4f),
            (relativePos.y / mapRadius) * (minimapSize * 0.4f)
        );
        
        // 限制在小地图范围内
        minimapPos = Vector2.ClampMagnitude(minimapPos, minimapSize * 0.35f);
        
        // 更新图标位置
        RectTransform iconRect = icon.GetComponent<RectTransform>();
        iconRect.anchoredPosition = minimapPos;
    }
    
    public void ToggleMinimap()
    {
        isVisible = !isVisible;
        if (minimapRect != null)
        {
            minimapRect.gameObject.SetActive(isVisible);
        }
    }
    
    public void SetMinimapVisible(bool visible)
    {
        isVisible = visible;
        if (minimapRect != null)
        {
            minimapRect.gameObject.SetActive(isVisible);
        }
    }
    
    public void AddPlanet(Transform planet)
    {
        if (!planets.Contains(planet))
        {
            planets.Add(planet);
            
            GameObject icon = null;
            if (planetIconPrefab != null)
            {
                icon = Instantiate(planetIconPrefab, minimapRect);
            }
            else
            {
                icon = CreateSimpleIcon("PlanetIcon", Color.green);
            }
            
            RectTransform iconRect = icon.GetComponent<RectTransform>();
            iconRect.sizeDelta = new Vector2(iconSize, iconSize);
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
} 