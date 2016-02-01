using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Line
{
  public List<int> value;

  public Line(int length)
  {
    value = new List<int>();
    for (int i = 0; i < length; i++)
      {
        value.Add(0);
      }
  }

    public int this [int i] {
      get {
        return value [i];
      }
      set {
        this.value [i] = value;
      }
    }
}
  [System.Serializable]
  public class TwoDArray
  {
    public List<Line> array;

    public TwoDArray(int width, int height)
    {
      array = new List<Line>();
      for (int i = 0; i < width; i++)
        {
          array.Add(new Line(height));
        }
    }

      public Line this [int i] {
        get {
          return array [i];
        }
        set {
          array [i] = value;
        }
      }
  }
