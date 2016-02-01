using UnityEngine;
using System.Collections;

namespace Segmentor
{
  public class SegmentSet : MonoBehaviour
  {
    public Transform CORNER;
    public Transform EDGE;
    public Transform CENTER;
    public Transform OUTER_CORNER;
    public Transform HALL;
    public Transform HALL_CORNER;
    public Transform HALL_4WAY;
    public Transform HALL_END;
    public Transform HALL_T;

    [SerializeField]
    public float
      defaultSizeX;
    [SerializeField]
    public float
      defaultSizeY;

    public void calculateSize()
    {
      Transform t = Instantiate(CENTER) as Transform;
      Renderer[] renders = t .GetComponentsInChildren<Renderer>();
      Bounds defaultBounds = renders [0].bounds;
      for (int i = 1; i < renders.Length; i++)
        defaultBounds.Encapsulate(renders [i].bounds);

      defaultSizeX = defaultBounds.size.x;
      defaultSizeY = defaultBounds.size.z;
      DestroyImmediate(t.gameObject);
    }

    public Transform createMesh(int x, int y, int type, Transform level)
    {
      Transform t = null;
      if (type == 5)
        t = Instantiate(CENTER) as Transform;
      else if (type == 2 || type == 4 || type == 6 || type == 8)
        t = Instantiate(EDGE) as Transform;
      else if (type == 1 || type == 3 || type == 7 || type == 9)
        t = Instantiate(CORNER) as Transform;
      else if (type == 13 || type == 12 || type == 11 || type == 10)
        t = Instantiate(OUTER_CORNER) as Transform;
      else if (type == -1 || type == -2 || type == -23 || type == -24)
        t = Instantiate(HALL) as Transform;
      else if (type == -3 || type == -4 || type == -5 || type == -6)
        t = Instantiate(HALL_CORNER) as Transform;
      else if (type == -7 || type == -8 || type == -9 || type == -10)
        t = Instantiate(HALL_T) as Transform;
      else if (type <= -11 && type >= -14)
        t = Instantiate(HALL_END) as Transform;
      else if (type == -15)
        t = Instantiate(HALL_4WAY) as Transform;
      else
        return null;
      t.transform.parent = level;

      if (type == 1)
        t.localScale = new Vector3(-1, 1, 1);
      if (type == 2)
        t.RotateAround(t.position, Vector3.up, -90);
      if (type == 3)
        t.localScale = new Vector3(-1, -1, 1);
      if (type == 6)
        t.localScale = new Vector3(-1, 1, 1);
      if (type == 8)
        t.RotateAround(t.position, Vector3.up, 90);
      if (type == 9)
        t.localScale = new Vector3(1, -1, 1);
      if (type == 10)
        t.localScale = new Vector3(1, -1, 1);
      if (type == 12)
        t.localScale = new Vector3(-1, -1, 1);
      if (type == 13)
        t.localScale = new Vector3(-1, 1, 1);

      if (type == -1)
        t.RotateAround(t.position, Vector3.up, 90);
      if (type == -3)
        t.localScale = new Vector3(-1, 1, 1);
      if (type == -5)
        t.localScale = new Vector3(-1, -1, 1);
      if (type == -6)
        t.localScale = new Vector3(1, -1, 1);
      if (type == -8)
        t.localScale = new Vector3(1, -1, 1);
      if (type == -9)
        t.RotateAround(t.position, Vector3.up, -90);
      if (type == -10)
        t.RotateAround(t.position, Vector3.up, 90);
      if (type == -11)
        t.RotateAround(t.position, Vector3.up, 90);
      if (type == -12)
        t.RotateAround(t.position, Vector3.up, -90);
      if (type == -13)
        t.localScale = new Vector3(1, -1, 1);
      if (type == -23)
        t.RotateAround(t.position, Vector3.up, -90);
      if (type == -24)
        t.localScale = new Vector3(-1, 1, 1);


      t.transform.localPosition = new Vector3(x * defaultSizeX, 0, y * defaultSizeY);
      t.hideFlags = HideFlags.HideInHierarchy;
      return t;
    }
  }

}