using UnityEngine;
using System.Collections;
using UnityEditor;
using Segmentor;

[CustomEditor(typeof(Segmentor.LevelSegment))]
public class LevelSegmentEditor : Editor
{

  private bool bReady;
  private LevelSegment segment;
  void OnSceneGUI()
  {
    segment = (LevelSegment)target;

    if (bReady)
      onSelect();
    bReady = false;
    if (Selection.activeGameObject != segment) 
      bReady = true;
    Selection.activeObject = segment.level;
    EditorUtility.SetSelectedWireframeHidden(segment.level.GetComponent<Renderer>(), false);
  }

  void onSelect()
  {
    segment.ChangeType(segment.level.currentType);
  }

}
