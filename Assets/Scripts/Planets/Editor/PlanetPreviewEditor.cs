#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RendererLoad))]
public class PlanetPreviewEditor : Editor
{
    private RendererLoad rendererLoad;
    private SerializedProperty imagesProp;
    private SerializedProperty imageIndexProp;
    
    void OnEnable()
    {
        rendererLoad = (RendererLoad)target;
        imagesProp = serializedObject.FindProperty("images");
        imageIndexProp = serializedObject.FindProperty("imageIndex");
    }
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        // 显示原有的属性
        EditorGUILayout.PropertyField(imagesProp);
        EditorGUILayout.PropertyField(imageIndexProp);
        
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
                ApplyPreview();
            }
            
            if (GUILayout.Button("下一张"))
            {
                int newIndex = (imageIndexProp.intValue + 1) % imagesProp.arraySize;
                imageIndexProp.intValue = newIndex;
                ApplyPreview();
            }
            EditorGUILayout.EndHorizontal();
            
            // 随机按钮
            if (GUILayout.Button("随机图片"))
            {
                int randomIndex = Random.Range(0, imagesProp.arraySize);
                imageIndexProp.intValue = randomIndex;
                ApplyPreview();
            }
            
            // 显示图片索引信息
            EditorGUILayout.LabelField($"图片 {imageIndexProp.intValue + 1} / {imagesProp.arraySize}");
        }
        else
        {
            EditorGUILayout.HelpBox("请先添加图片到列表中", MessageType.Info);
        }
        
        serializedObject.ApplyModifiedProperties();
    }
    
    private void ApplyPreview()
    {
        // 强制更新预览
        if (rendererLoad != null)
        {
            rendererLoad.SendMessage("Start", SendMessageOptions.DontRequireReceiver);
        }
    }
}
#endif 