using UnityEngine;
using System.Collections;
using UnityEditor;
using Segmentor;

[CustomEditor(typeof(Level))]
public class LevelEditor : Editor
{

  public override void OnInspectorGUI()
  {
    DrawDefaultInspector();

    Rect general = EditorGUILayout.BeginVertical("Button");
    GUI.Box(general, "");
    EditorGUILayout.LabelField("General");
    Level l = (Level)target;
    l.size.x = EditorGUILayout.IntField("Width ", (int)l.size.x) * 1.0f;
    l.size.y = EditorGUILayout.IntField("Height ", (int)l.size.y) * 1.0f;
    if (l.size.x > 10)
      l.size.x = 10;
    if (l.size.y > 10)
      l.size.y = 10;
    if (GUILayout.Button("Build Object"))
      l.init();
    if (GUILayout.Button("Calculate level"))
      l.calculateLevel();

    EditorGUILayout.EndVertical();

    GUILayout.Space(25);
    Rect editing = EditorGUILayout.BeginVertical("Button");
    GUI.Box(editing, "");
    EditorGUILayout.LabelField("Editing");
    EditorGUILayout.BeginHorizontal("Button");
       
    l.bHall = GUILayout.Toggle(l.bHall, "Hall", GUILayout.ExpandWidth(false));
    if (l.currentType != 0)
      GUI.color = new Color(1.2f - l.currentType * 0.1f, 0.5f + l.currentType * 0.1f, 0);
    else
      GUI.color = Color.white; 
    if (GUILayout.Button("Current type: " + l.currentType.ToString()))
      l.cycleType();
    GUI.color = Color.white;

    EditorGUILayout.EndHorizontal();
    EditorGUILayout.EndVertical();
  }
}
