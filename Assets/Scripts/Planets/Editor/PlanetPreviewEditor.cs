#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlanetCustom))]
public class PlanetPreviewEditor : Editor
{
    private PlanetCustom planetCustom;
    private SerializedProperty imagesProp;
    private SerializedProperty imageIndexProp;
    private SerializedProperty planetSizeProp;
    private SerializedProperty gravitySizeProp;
    private SerializedProperty gravityExtentProp;
    private SerializedProperty colliderSettingsProp;
    private SerializedProperty touchEffectTypeProp;
    private SerializedProperty isControllableProp;
    
    // 碰撞器半径调整
    private float planetColliderRadius = 4.21f;
    private float gravityColliderRadius = 0.5f;
    private Vector2 planetColliderOffset = Vector2.zero;
    private Vector2 gravityColliderOffset = Vector2.zero;
    
    // 碰撞效果设置
    private TouchEffectType touchEffectType = TouchEffectType.None;
    private GameObject controlledObject;
    
    // 控制性设置
    private bool isControllable = true;
    
    void OnEnable()
    {
        planetCustom = (PlanetCustom)target;
        imagesProp = serializedObject.FindProperty("images");
        imageIndexProp = serializedObject.FindProperty("imageIndex");
        planetSizeProp = serializedObject.FindProperty("planetSize");
        gravitySizeProp = serializedObject.FindProperty("gravitySize");
        gravityExtentProp = serializedObject.FindProperty("gravityExtent");
        colliderSettingsProp = serializedObject.FindProperty("colliderSettings");
        touchEffectTypeProp = serializedObject.FindProperty("touchEffectType");
        isControllableProp = serializedObject.FindProperty("isControllable");
        
        // 加载当前碰撞器设置
        LoadCurrentColliderSettings();
    }
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        // 显示原有的属性
        EditorGUILayout.PropertyField(imagesProp);
        EditorGUILayout.PropertyField(imageIndexProp);
        EditorGUILayout.PropertyField(planetSizeProp);
        EditorGUILayout.PropertyField(gravitySizeProp);
        EditorGUILayout.PropertyField(gravityExtentProp);
        EditorGUILayout.PropertyField(touchEffectTypeProp);
        EditorGUILayout.PropertyField(isControllableProp);
        
        // 添加预览控制
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("编辑器预览", EditorStyles.boldLabel);
        
        if (imagesProp.arraySize > 0)
        {
            // 显示当前图片预览
            EditorGUILayout.LabelField("当前图片:");
            Sprite currentSprite = imagesProp.GetArrayElementAtIndex(imageIndexProp.intValue)?.objectReferenceValue as Sprite;
            if (currentSprite != null)
            {
                // 使用ObjectField来显示Sprite预览
                EditorGUILayout.ObjectField("预览", currentSprite, typeof(Sprite), false, GUILayout.Height(100));
            }
            
            EditorGUILayout.Space();
            
            // 快速切换按钮
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("上一张"))
            {
                int newIndex = (imageIndexProp.intValue - 1 + imagesProp.arraySize) % imagesProp.arraySize;
                imageIndexProp.intValue = newIndex;
                LoadCurrentColliderSettings();
                ApplyPreview();
            }
            
            if (GUILayout.Button("下一张"))
            {
                int newIndex = (imageIndexProp.intValue + 1) % imagesProp.arraySize;
                imageIndexProp.intValue = newIndex;
                LoadCurrentColliderSettings();
                ApplyPreview();
            }
            EditorGUILayout.EndHorizontal();
            
            // 随机按钮
            if (GUILayout.Button("随机图片"))
            {
                int randomIndex = Random.Range(0, imagesProp.arraySize);
                imageIndexProp.intValue = randomIndex;
                LoadCurrentColliderSettings();
                ApplyPreview();
            }
            
            // 显示图片索引信息
            EditorGUILayout.LabelField($"图片 {imageIndexProp.intValue + 1} / {imagesProp.arraySize}");
        }
        else
        {
            EditorGUILayout.HelpBox("请先添加图片到列表中", MessageType.Info);
        }
        
        // 添加大小预览区域
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("大小预览", EditorStyles.boldLabel);
        
        // 显示当前大小信息
        EditorGUILayout.LabelField($"星球大小: {planetSizeProp.floatValue:F2}");
        EditorGUILayout.LabelField($"引力场大小: {gravitySizeProp.floatValue:F2}");
        EditorGUILayout.LabelField($"引力强度: {gravityExtentProp.floatValue:F2}");
        
        // 添加快速调整按钮
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("快速调整", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("小星球"))
        {
            planetSizeProp.floatValue = 0.5f;
            gravitySizeProp.floatValue = 10f;
            ApplyPreview();
        }
        if (GUILayout.Button("中星球"))
        {
            planetSizeProp.floatValue = 1f;
            gravitySizeProp.floatValue = 20f;
            ApplyPreview();
        }
        if (GUILayout.Button("大星球"))
        {
            planetSizeProp.floatValue = 2f;
            gravitySizeProp.floatValue = 40f;
            ApplyPreview();
        }
        EditorGUILayout.EndHorizontal();
        
        // 添加引力强度快速调整
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("弱引力"))
        {
            gravityExtentProp.floatValue = 0.5f;
            ApplyPreview();
        }
        if (GUILayout.Button("中引力"))
        {
            gravityExtentProp.floatValue = 1.5f;
            ApplyPreview();
        }
        if (GUILayout.Button("强引力"))
        {
            gravityExtentProp.floatValue = 3f;
            ApplyPreview();
        }
        EditorGUILayout.EndHorizontal();
        
        // 添加碰撞器调整区域
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("碰撞器调整", EditorStyles.boldLabel);
        
        // 显示当前碰撞器信息
        EditorGUILayout.LabelField($"当前外观: 图片 {imageIndexProp.intValue + 1}");
        
        // 碰撞器半径调整滑块
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("星球碰撞器半径:", EditorStyles.boldLabel);
        float newPlanetRadius = EditorGUILayout.Slider(planetColliderRadius, 0.1f, 10f);
        if (newPlanetRadius != planetColliderRadius)
        {
            planetColliderRadius = newPlanetRadius;
            ApplyColliderRadius();
        }
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("引力碰撞器半径:", EditorStyles.boldLabel);
        float newGravityRadius = EditorGUILayout.Slider(gravityColliderRadius, 0.1f, 5f);
        if (newGravityRadius != gravityColliderRadius)
        {
            gravityColliderRadius = newGravityRadius;
            ApplyColliderRadius();
        }
        
        // 添加offset调整
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("星球碰撞器偏移:", EditorStyles.boldLabel);
        Vector2 newPlanetOffset = EditorGUILayout.Vector2Field("偏移", planetColliderOffset);
        if (newPlanetOffset != planetColliderOffset)
        {
            planetColliderOffset = newPlanetOffset;
            ApplyColliderRadius();
        }
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("引力碰撞器偏移:", EditorStyles.boldLabel);
        Vector2 newGravityOffset = EditorGUILayout.Vector2Field("偏移", gravityColliderOffset);
        if (newGravityOffset != gravityColliderOffset)
        {
            gravityColliderOffset = newGravityOffset;
            ApplyColliderRadius();
        }
        
        // 添加碰撞效果调整区域
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("碰撞效果调整", EditorStyles.boldLabel);
        
        // 碰撞效果类型选择
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("碰撞效果类型:", EditorStyles.boldLabel);
        TouchEffectType newTouchEffectType = (TouchEffectType)EditorGUILayout.EnumPopup("效果类型", touchEffectType);
        if (newTouchEffectType != touchEffectType)
        {
            touchEffectType = newTouchEffectType;
            ApplyTouchEffectSettings();
        }
        
        // 受控制对象选择（仅当效果类型为Show或Hide时显示）
        if (touchEffectType == TouchEffectType.Show || touchEffectType == TouchEffectType.Hide)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("受控制对象:", EditorStyles.boldLabel);
            GameObject newControlledObject = (GameObject)EditorGUILayout.ObjectField("控制对象", controlledObject, typeof(GameObject), true);
            if (newControlledObject != controlledObject)
            {
                controlledObject = newControlledObject;
                ApplyTouchEffectSettings();
            }
            
            if (controlledObject == null)
            {
                EditorGUILayout.HelpBox("⚠️ 请选择要控制的对象", MessageType.Warning);
            }
        }
        
        // 显示碰撞效果说明
        EditorGUILayout.Space();
        ShowTouchEffectInfo();
        
        // 添加控制性调整区域
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("控制性调整", EditorStyles.boldLabel);
        
        // 控制性开关
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("玩家控制:", EditorStyles.boldLabel);
        bool newIsControllable = EditorGUILayout.Toggle("能否被玩家控制", isControllable);
        if (newIsControllable != isControllable)
        {
            isControllable = newIsControllable;
            ApplyControllableSettings();
        }
        
        // 显示控制性说明
        EditorGUILayout.Space();
        ShowControllableInfo();
        
        // 显示同步信息
        EditorGUILayout.Space();
        ShowSyncInfo();
        
        // 保存设置按钮
        EditorGUILayout.Space();
        if (GUILayout.Button("保存当前外观的碰撞器设置"))
        {
            SaveColliderSettings();
        }
        
        // 显示保存的设置
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("已保存的设置", EditorStyles.boldLabel);
        ShowSavedSettings();
        
        serializedObject.ApplyModifiedProperties();
    }
    
    private void LoadCurrentColliderSettings()
    {
        if (planetCustom != null)
        {
            ColliderSettings settings = planetCustom.GetColliderSettings(imageIndexProp.intValue);
            if (settings != null)
            {
                planetColliderRadius = settings.planetColliderRadius;
                gravityColliderRadius = settings.gravityColliderRadius;
                planetColliderOffset = settings.planetColliderOffset;
                gravityColliderOffset = settings.gravityColliderOffset;
                // 碰撞效果和控制性设置不从全局设置加载，保持当前值
            }
            else
            {
                // 使用默认值
                planetColliderRadius = 4.21f;
                gravityColliderRadius = 0.5f;
                planetColliderOffset = Vector2.zero;
                gravityColliderOffset = Vector2.zero;
                // 碰撞效果和控制性设置保持当前值，不重置
            }
        }
    }
    
    private void ApplyColliderRadius()
    {
        if (planetCustom != null)
        {
            // 更新星球碰撞器
            CircleCollider2D planetCollider = planetCustom.GetComponent<CircleCollider2D>();
            if (planetCollider != null)
            {
                planetCollider.radius = planetColliderRadius;
                planetCollider.offset = planetColliderOffset;
                EditorUtility.SetDirty(planetCollider);
            }
            
            // 更新引力碰撞器
            Transform gravityArea = planetCustom.transform.Find("GravityArea");
            if (gravityArea != null)
            {
                CircleCollider2D gravityCollider = gravityArea.GetComponent<CircleCollider2D>();
                if (gravityCollider != null)
                {
                    gravityCollider.radius = gravityColliderRadius;
                    gravityCollider.offset = gravityColliderOffset;
                    EditorUtility.SetDirty(gravityCollider);
                }
            }
        }
    }
    
    private void SaveColliderSettings()
    {
        if (planetCustom != null)
        {
            planetCustom.SaveColliderSettings(planetColliderRadius, gravityColliderRadius, planetColliderOffset, gravityColliderOffset, touchEffectType, controlledObject, isControllable);
            EditorUtility.SetDirty(planetCustom);
            Debug.Log($"已保存图片 {imageIndexProp.intValue + 1} 的设置:\n" +
                     $"碰撞器设置(同步): 星球半径={planetColliderRadius:F2}, 引力半径={gravityColliderRadius:F2}, 星球偏移={planetColliderOffset}, 引力偏移={gravityColliderOffset}\n" +
                     $"碰撞效果设置(仅当前): {touchEffectType}, 控制性设置(仅当前): {isControllable}");
        }
    }
    
    private void ShowSavedSettings()
    {
        if (planetCustom != null)
        {
            for (int i = 0; i < imagesProp.arraySize; i++)
            {
                ColliderSettings settings = planetCustom.GetColliderSettings(i);
                if (settings != null)
                {
                    EditorGUILayout.LabelField($"图片 {i + 1}: 星球半径={settings.planetColliderRadius:F2}, 引力半径={settings.gravityColliderRadius:F2}");
                    EditorGUILayout.LabelField($"  星球偏移={settings.planetColliderOffset}, 引力偏移={settings.gravityColliderOffset}");
                    EditorGUILayout.HelpBox("碰撞效果和控制性设置仅对当前星球生效，不在此处显示", MessageType.Info);
                }
            }
        }
    }
    
    private void ShowSyncInfo()
    {
        if (planetCustom != null)
        {
            // 使用反射获取imageIndex
            var imageIndexField = typeof(PlanetCustom).GetField("imageIndex", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            int currentImageIndex = 0;
            if (imageIndexField != null)
            {
                currentImageIndex = (int)imageIndexField.GetValue(planetCustom);
            }
            
            // 计算场景中使用相同外观的星球数量
            PlanetCustom[] allPlanets = FindObjectsOfType<PlanetCustom>();
            int sameAppearanceCount = 0;
            
            foreach (var planet in allPlanets)
            {
                if (planet != planetCustom)
                {
                    var planetImageIndexField = typeof(PlanetCustom).GetField("imageIndex", 
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    int planetImageIndex = 0;
                    if (planetImageIndexField != null)
                    {
                        planetImageIndex = (int)planetImageIndexField.GetValue(planet);
                    }
                    
                    if (planetImageIndex == currentImageIndex)
                    {
                        sameAppearanceCount++;
                    }
                }
            }
            
            if (sameAppearanceCount > 0)
            {
                EditorGUILayout.HelpBox($"⚠️ 碰撞器设置将同步到 {sameAppearanceCount} 个使用相同外观的星球\n💡 碰撞效果和控制性设置仅对当前星球生效", MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox("✅ 当前场景中没有其他星球使用相同外观\n💡 碰撞效果和控制性设置仅对当前星球生效", MessageType.Info);
            }
        }
    }
    
    private void ApplyPreview()
    {
        // 强制更新预览
        if (planetCustom != null)
        {
            // 更新星球大小
            planetCustom.transform.localScale = new Vector3(planetSizeProp.floatValue, planetSizeProp.floatValue, 1);
            
            // 更新引力场大小
            Transform gravityArea = planetCustom.transform.Find("GravityArea");
            if (gravityArea != null)
            {
                gravityArea.localScale = new Vector3(gravitySizeProp.floatValue, gravitySizeProp.floatValue, 1);
            }
            
            // 更新引力强度 - 使用反射来访问protected internal字段
            PlanetGravity planetGravity = planetCustom.GetComponent<PlanetGravity>();
            if (planetGravity != null)
            {
                var gravityExtentField = typeof(PlanetGravity).GetField("gravityExtent", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (gravityExtentField != null)
                {
                    gravityExtentField.SetValue(planetGravity, gravityExtentProp.floatValue);
                }
            }
            
            // 应用碰撞器设置
            ApplyColliderRadius();
            
            // 应用碰撞效果设置
            ApplyTouchEffectSettings();
            
            // 应用控制性设置
            ApplyControllableSettings();
            
            // 标记为已修改，确保场景保存时包含这些更改
            EditorUtility.SetDirty(planetCustom);
            EditorUtility.SetDirty(planetCustom.transform);
            if (gravityArea != null)
            {
                EditorUtility.SetDirty(gravityArea);
            }
        }
    }
    
    private void ApplyTouchEffectSettings()
    {
        if (planetCustom != null)
        {
            // 直接更新当前星球的碰撞效果设置
            var touchEffectTypeField = typeof(PlanetCustom).GetField("touchEffectType", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (touchEffectTypeField != null)
            {
                touchEffectTypeField.SetValue(planetCustom, touchEffectType);
            }
            
            // 更新TouchEffect组件
            TouchEffect touchEffect = planetCustom.GetComponent<TouchEffect>();
            if (touchEffect != null)
            {
                // 使用反射更新TouchEffect组件的设置
                var touchEffectTypeField2 = typeof(TouchEffect).GetField("touchEffectType", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var controlledObjectField = typeof(TouchEffect).GetField("controlledObject", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                
                if (touchEffectTypeField2 != null)
                {
                    touchEffectTypeField2.SetValue(touchEffect, touchEffectType);
                }
                if (controlledObjectField != null)
                {
                    controlledObjectField.SetValue(touchEffect, controlledObject);
                }
                
                EditorUtility.SetDirty(touchEffect);
            }
            
            // 标记为已修改
            EditorUtility.SetDirty(planetCustom);
        }
    }
    
    private void ShowTouchEffectInfo()
    {
        string info = "";
        switch (touchEffectType)
        {
            case TouchEffectType.None:
                info = "无特殊效果";
                break;
            case TouchEffectType.Death:
                info = "玩家触碰时死亡";
                break;
            case TouchEffectType.Show:
                info = "玩家触碰时显示控制对象";
                break;
            case TouchEffectType.Hide:
                info = "玩家触碰时隐藏控制对象";
                break;
        }
        
        EditorGUILayout.HelpBox($"当前效果: {info}\n💡 此设置仅对当前星球生效", MessageType.Info);
        
        if (touchEffectType == TouchEffectType.Show || touchEffectType == TouchEffectType.Hide)
        {
            if (controlledObject != null)
            {
                EditorGUILayout.HelpBox($"将控制对象: {controlledObject.name}", MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox("⚠️ 未设置控制对象，效果将不会生效", MessageType.Warning);
            }
        }
    }
    
    private void ApplyControllableSettings()
    {
        if (planetCustom != null)
        {
            // 直接更新当前星球的控制性设置
            var isControllableField = typeof(PlanetCustom).GetField("isControllable", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (isControllableField != null)
            {
                isControllableField.SetValue(planetCustom, isControllable);
            }
            
            EditorUtility.SetDirty(planetCustom);
        }
    }
    
    private void ShowControllableInfo()
    {
        string info = isControllable ? "✅ 可以被玩家控制" : "❌ 不能被玩家控制";
        string description = isControllable ? 
            "玩家可以控制这个星球的引力和移动" : 
            "玩家无法控制这个星球，它将保持静态或按预设行为运行";
        
        EditorGUILayout.HelpBox($"{info}\n{description}\n💡 此设置仅对当前星球生效", MessageType.Info);
    }
}
#endif 