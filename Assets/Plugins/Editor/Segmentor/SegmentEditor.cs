using UnityEngine;
using System.Collections;
using UnityEditor;
using Segmentor;

[CustomEditor(typeof(SegmentSet))]
public class SegmentEditor : Editor
{

  public override void OnInspectorGUI()
  {


    if (GUILayout.Button("Calculate size"))
      (target as SegmentSet).calculateSize();
    DrawDefaultInspector();
  }
}
