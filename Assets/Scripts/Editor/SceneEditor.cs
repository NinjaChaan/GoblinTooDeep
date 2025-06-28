using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomPropertyDrawer(typeof(SceneNameAttribute))]
public class SceneNameDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType != SerializedPropertyType.String)
        {
            EditorGUI.LabelField(position, label.text, "SceneName attribute can only be used with string fields");
            return;
        }
        List<string> sceneNames = GetAllSceneNames();
        
        sceneNames.Insert(0, "None");
        
        string currentValue = property.stringValue;
        int currentIndex = sceneNames.IndexOf(currentValue);
        if (currentIndex < 0) currentIndex = 0; // Default to "None" if not found
        
        EditorGUI.BeginProperty(position, label, property);
        
        int newIndex = EditorGUI.Popup(position, label.text, currentIndex, sceneNames.ToArray());
        
        if (newIndex != currentIndex)
        {
            property.stringValue = newIndex == 0 ? "" : sceneNames[newIndex];
        }
        
        EditorGUI.EndProperty();
    }
    
    private List<string> GetAllSceneNames()
    {
        List<string> sceneNames = new List<string>();
        
        for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
        {
            var scene = EditorBuildSettings.scenes[i];
            if (scene.enabled)
            {
                string scenePath = scene.path;
                string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
                sceneNames.Add(sceneName);
            }
        }
        
        return sceneNames;
    }
}
