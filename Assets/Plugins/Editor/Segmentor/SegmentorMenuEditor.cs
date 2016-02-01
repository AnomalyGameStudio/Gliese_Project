using UnityEngine;
using System.Collections;
using UnityEditor;
using Segmentor;

public class SegmentorMenuEditor : Editor
{

  [MenuItem("GameObject/3D Object/Segmentor Level")]
  private static void CreateLevel()
  {
    Object prefab = AssetDatabase.LoadAssetAtPath("Assets/Plugins/Segmentor/Default Content/Segmentor Level.prefab", typeof(GameObject));
    GameObject lvl = Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
    lvl.name = "Segmentor Level";
  }
}
