using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AdvancedMinimap : MonoBehaviour, IPointerClickHandler, IDragHandler, IScrollHandler
{
    [Header("小地图设置")]
    [SerializeField] private RectTransform minimapContainer;
    [SerializeField] private float minimapSize = 200f;
    [SerializeField] private Vector2 minimapPosition = new Vector2(20, -20);
    [SerializeField] private Color backgroundColor = new Color(0, 0, 0, 0.7f);
    [SerializeField] private Color borderColor = Color.white;
    
    [Header("图标设置")]
    [SerializeField] private GameObject playerIconPrefab;
    [SerializeField] private GameObject planetIconPrefab;
    [SerializeField] private GameObject doorIconPrefab;
    [SerializeField] private GameObject starIconPrefab;
    [SerializeField] private float iconSize = 10f;
    
    [Header("地图范围")]
    [SerializeField] private float mapRadius = 40f;
    [SerializeField] private bool autoDetectMapBounds = true;
    [SerializeField] private Vector2 mapBounds = new Vector2(60f, 60f);
    [SerializeField] private float minZoom = 0.5f;
    [SerializeField] private float maxZoom = 3f;
    [SerializeField] private float currentZoom = 1f;
    
    [Header("交互设置")]
    [SerializeField] private bool isVisible = true;
    [SerializeField] private KeyCode toggleKey = KeyCode.M;
    [SerializeField] private bool canToggle = true;
    [SerializeField] private bool canDrag = true;
    [SerializeField] private bool canZoom = true;
    [SerializeField] private bool canClick = true;
    
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
    private Camera mainCamera;
    private Vector2 dragOffset;
    private bool isDragging = false;
    
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
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            mainCamera = FindObjectOfType<Camera>();
        }
        
        GameObject playerObj = GameObject.Find("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        
        if (autoDetectMapBounds)
        {
            CalculateMapBounds();
        }
    }
    
    void CalculateMapBounds()
    {
        GameObject[] allPlanets = GameObject.FindGameObjectsWithTag("Planet");
        GameObject[] allDoors = GameObject.FindGameObjectsWithTag("Door");
        GameObject[] allStars = GameObject.FindGameObjectsWithTag("Star");
        
        float minX = float.MaxValue, maxX = float.MinValue;
        float minY = float.MaxValue, maxY = float.MinValue;
        
        if (player != null)
        {
            minX = Mathf.Min(minX, player.position.x);
            maxX = Mathf.Max(maxX, player.position.x);
            minY = Mathf.Min(minY, player.position.y);
            maxY = Mathf.Max(maxY, player.position.y);
        }
        
        foreach (GameObject obj in allPlanets)
        {
            minX = Mathf.Min(minX, obj.transform.position.x);
            maxX = Mathf.Max(maxX, obj.transform.position.x);
            minY = Mathf.Min(minY, obj.transform.position.y);
            maxY = Mathf.Max(maxY, obj.transform.position.y);
        }
        
        foreach (GameObject obj in allDoors)
        {
            minX = Mathf.Min(minX, obj.transform.position.x);
            maxX = Mathf.Max(maxX, obj.transform.position.x);
            minY = Mathf.Min(minY, obj.transform.position.y);
            maxY = Mathf.Max(maxY, obj.transform.position.y);
        }
        
        foreach (GameObject obj in allStars)
        {
            minX = Mathf.Min(minX, obj.transform.position.x);
            maxX = Mathf.Max(maxX, obj.transform.position.x);
            minY = Mathf.Min(minY, obj.transform.position.y);
            maxY = Mathf.Max(maxY, obj.transform.position.y);
        }
        
        mapBounds.x = Mathf.Max(maxX - minX, 30f);
        mapBounds.y = Mathf.Max(maxY - minY, 30f);
        mapRadius = Mathf.Max(mapBounds.x, mapBounds.y) * 0.5f;
    }
    
    void FindGameObjects()
    {
        GameObject[] planetObjects = GameObject.FindGameObjectsWithTag("Planet");
        foreach (GameObject planet in planetObjects)
        {
            planets.Add(planet.transform);
        }
        
        GameObject[] doorObjects = GameObject.FindGameObjectsWithTag("Door");
        foreach (GameObject door in doorObjects)
        {
            doors.Add(door.transform);
        }
        
        GameObject[] starObjects = GameObject.FindGameObjectsWithTag("Star");
        foreach (GameObject star in starObjects)
        {
            stars.Add(star.transform);
        }
    }
    
    void CreateMinimapUI()
    {
        if (minimapContainer == null)
        {
            GameObject containerObj = new GameObject("AdvancedMinimapContainer");
            minimapContainer = containerObj.AddComponent<RectTransform>();
            
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                GameObject canvasObj = new GameObject("Canvas");
                canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasObj.AddComponent<CanvasScaler>();
                canvasObj.AddComponent<GraphicRaycaster>();
            }
            
            minimapContainer.SetParent(canvas.transform, false);
        }
        
        minimapContainer.anchorMin = new Vector2(0, 1);
        minimapContainer.anchorMax = new Vector2(0, 1);
        minimapContainer.sizeDelta = new Vector2(minimapSize, minimapSize);
        minimapContainer.anchoredPosition = minimapPosition;
        
        if (canClick || canDrag)
        {
            minimapContainer.gameObject.AddComponent<Image>().color = Color.clear;
        }
        
        GameObject backgroundObj = new GameObject("MinimapBackground");
        backgroundObj.transform.SetParent(minimapContainer, false);
        
        backgroundImage = backgroundObj.AddComponent<Image>();
        backgroundImage.color = backgroundColor;
        
        RectTransform bgRect = backgroundObj.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;
        bgRect.anchoredPosition = Vector2.zero;
        
        GameObject borderObj = new GameObject("MinimapBorder");
        borderObj.transform.SetParent(minimapContainer, false);
        
        Image borderImage = borderObj.AddComponent<Image>();
        borderImage.color = borderColor;
        
        RectTransform borderRect = borderObj.GetComponent<RectTransform>();
        borderRect.anchorMin = Vector2.zero;
        borderRect.anchorMax = Vector2.one;
        borderRect.sizeDelta = new Vector2(3, 3);
        borderRect.anchoredPosition = Vector2.zero;
        
        minimapRect = minimapContainer;
    }
    
    void CreateIcons()
    {
        if (player != null)
        {
            if (playerIconPrefab != null)
            {
                playerIcon = Instantiate(playerIconPrefab, minimapRect);
            }
            else
            {
                playerIcon = CreateSimpleIcon("PlayerIcon", Color.blue);
            }
            
            RectTransform playerIconRect = playerIcon.GetComponent<RectTransform>();
            playerIconRect.sizeDelta = new Vector2(iconSize, iconSize);
        }
        
        CreateObjectIcons(planets, planetIcons, planetIconPrefab, Color.green, "PlanetIcon");
        CreateObjectIcons(doors, doorIcons, doorIconPrefab, Color.yellow, "DoorIcon");
        CreateObjectIcons(stars, starIcons, starIconPrefab, Color.white, "StarIcon");
    }
    
    void CreateObjectIcons(List<Transform> objects, List<GameObject> icons, GameObject prefab, Color defaultColor, string defaultName)
    {
        foreach (Transform obj in objects)
        {
            GameObject icon = null;
            if (prefab != null)
            {
                icon = Instantiate(prefab, minimapRect);
            }
            else
            {
                icon = CreateSimpleIcon(defaultName, defaultColor);
            }
            
            RectTransform iconRect = icon.GetComponent<RectTransform>();
            iconRect.sizeDelta = new Vector2(iconSize, iconSize);
            icons.Add(icon);
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
        
        if (playerIcon != null)
        {
            RectTransform playerIconRect = playerIcon.GetComponent<RectTransform>();
            playerIconRect.anchoredPosition = Vector2.zero;
            playerIconRect.rotation = player.rotation;
        }
        
        UpdateObjectIcons(planets, planetIcons);
        UpdateObjectIcons(doors, doorIcons);
        UpdateObjectIcons(stars, starIcons);
    }
    
    void UpdateObjectIcons(List<Transform> objects, List<GameObject> icons)
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
        Vector3 relativePos = worldPosition - player.position;
        
        Vector2 minimapPos = new Vector2(
            (relativePos.x / (mapRadius * currentZoom)) * (minimapSize * 0.4f),
            (relativePos.y / (mapRadius * currentZoom)) * (minimapSize * 0.4f)
        );
        
        minimapPos = Vector2.ClampMagnitude(minimapPos, minimapSize * 0.35f);
        
        RectTransform iconRect = icon.GetComponent<RectTransform>();
        iconRect.anchoredPosition = minimapPos;
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!canClick) return;
        
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            minimapRect, eventData.position, eventData.pressEventCamera, out localPoint);
        
        Vector3 worldPos = player.position + new Vector3(
            localPoint.x * (mapRadius * currentZoom) / (minimapSize * 0.4f),
            localPoint.y * (mapRadius * currentZoom) / (minimapSize * 0.4f),
            0
        );
        
        Debug.Log($"Minimap clicked at world position: {worldPos}");
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        if (!canDrag) return;
        
        if (!isDragging)
        {
            isDragging = true;
            dragOffset = minimapRect.anchoredPosition - eventData.position;
        }
        
        minimapRect.anchoredPosition = eventData.position + dragOffset;
    }
    
    public void OnScroll(PointerEventData eventData)
    {
        if (!canZoom) return;
        
        float scrollDelta = eventData.scrollDelta.y;
        currentZoom = Mathf.Clamp(currentZoom - scrollDelta * 0.1f, minZoom, maxZoom);
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
    
    public void SetZoom(float zoom)
    {
        currentZoom = Mathf.Clamp(zoom, minZoom, maxZoom);
    }
    
    public void ResetPosition()
    {
        if (minimapRect != null)
        {
            minimapRect.anchoredPosition = minimapPosition;
        }
    }
} 