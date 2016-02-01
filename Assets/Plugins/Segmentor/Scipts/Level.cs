/*
 * Author: Alice von Spades
 * Created: 22.11.2014
 * Last Edit: 25.11.14
 * Package: Segmentor
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Segmentor;

namespace Segmentor
{
  public class Level : MonoBehaviour
  {
    [HideInInspector]
    public TwoDArray
      map;   
    [HideInInspector]
    public Rect
      size;
    [HideInInspector]
    public Transform
      segmentPrefab;
    [HideInInspector]
    public Transform
      segments;
    
    [HideInInspector]
    public int
      currentType = 0;
    public SegmentSet[] set;
    [HideInInspector]
    public bool
      bHall = false;

    [HideInInspector]
    public GameObject
      level;

    [HideInInspector]
    public static Level
      currentLevel;
    
    public void init()
    {
      map = new TwoDArray((int)size.x + 2, (int)size.y + 2);
      destroy();  
      
      for (int x = 1; x < size.x+1; x++)
        for (int y = 1; y < size.y+1; y++)
          {
            map [x] [y] = 0;
            Transform t = (Instantiate(segmentPrefab) as Transform);
            t.GetComponent<LevelSegment>().x = x;
            t.GetComponent<LevelSegment>().y = y;
            t.GetComponent<LevelSegment>().level = this;
            t.transform.localPosition = new Vector3(x, 0, y);
            t.parent = segments;

          }
        segments.localScale = new Vector3(1, 1, -1);
        segments.localPosition = new Vector3(-(size.x + 1) / 2, 0, (size.y + 1) / 2); 

    }
    
      public void destroy()
      {
        if (segments != null)
          GameObject.DestroyImmediate(segments.gameObject);
        segments = new GameObject().transform;
        segments.hideFlags = HideFlags.HideInHierarchy;
        segments.name = "Segments";
        segments.parent = transform;
      }
    
      public int calculatePoint(int x, int y)
      {
        if (x <= 0 || x > size.x || y <= 0 || y > size.y)
          return 0;

        if (map [x] [y] > 0)
          {
            if (map [x - 1] [y] <= 0 && map [x + 1] [y] > 0 && map [x] [y - 1] <= 0 && map [x] [y + 1] > 0)
              return LevelSegment.UPPER_LEFT + 5;
            else if (map [x - 1] [y] > 0 && map [x + 1] [y] > 0 && map [x] [y - 1] <= 0 && map [x] [y + 1] > 0)
              return LevelSegment.UPPER + 5;
            else if (map [x - 1] [y] > 0 && map [x + 1] [y] <= 0 && map [x] [y - 1] <= 0 && map [x] [y + 1] > 0)
              return LevelSegment.UPPER_RIGHT + 5;
            else if (map [x - 1] [y] <= 0 && map [x + 1] [y] > 0 && map [x] [y - 1] > 0 && map [x] [y + 1] > 0)
              return LevelSegment.LEFT + 5;
            else if (map [x - 1] [y] > 0 && map [x + 1] [y] <= 0 && map [x] [y - 1] > 0 && map [x] [y + 1] > 0)
              return LevelSegment.RIGHT + 5;
            else if (map [x - 1] [y] <= 0 && map [x + 1] [y] > 0 && map [x] [y - 1] > 0 && map [x] [y + 1] <= 0)
              return LevelSegment.LOWER_LEFT + 5;
            else if (map [x - 1] [y] > 0 && map [x + 1] [y] > 0 && map [x] [y - 1] > 0 && map [x] [y + 1] <= 0)
              return LevelSegment.LOWER + 5;
            else if (map [x - 1] [y] > 0 && map [x + 1] [y] <= 0 && map [x] [y - 1] > 0 && map [x] [y + 1] <= 0)
              return LevelSegment.LOWER_RIGHT + 5;
            else if (map [x - 1] [y] > 0 && map [x + 1] [y] > 0 && map [x] [y - 1] > 0 && map [x] [y + 1] > 0 && map [x + 1] [y + 1] <= 0)
              return LevelSegment.INNER_LOWER_RIGHT;
            else if (map [x - 1] [y] > 0 && map [x + 1] [y] > 0 && map [x] [y - 1] > 0 && map [x] [y + 1] > 0 && map [x - 1] [y + 1] <= 0)
              return LevelSegment.INNER_LOWER_LEFT;
            else if (map [x - 1] [y] > 0 && map [x + 1] [y] > 0 && map [x] [y - 1] > 0 && map [x] [y + 1] > 0 && map [x + 1] [y - 1] <= 0)
              return LevelSegment.INNER_UPPER_RIGHT;
            else if (map [x - 1] [y] > 0 && map [x + 1] [y] > 0 && map [x] [y - 1] > 0 && map [x] [y + 1] > 0 && map [x - 1] [y - 1] <= 0)
              return LevelSegment.INNER_UPPER_LEFT;
            else if (map [x - 1] [y] > 0 && map [x + 1] [y] > 0 && map [x] [y - 1] > 0 && map [x] [y + 1] > 0)
              return LevelSegment.CENTER + 5;
          }


          if (map [x] [y] < 0)
            {
              /*Halls*/
              if (map [x - 1] [y] >= 0 && map [x + 1] [y] > 0 && map [x] [y - 1] < 0 && map [x] [y + 1] < 0)
                return -23;
              else if (map [x - 1] [y] < 0 && map [x + 1] [y] < 0 && map [x] [y - 1] >= 0 && map [x] [y + 1] > 0)
                return -24;
              else if (map [x - 1] [y] >= 0 && map [x + 1] [y] >= 0 && map [x] [y - 1] < 0 && map [x] [y + 1] < 0)
                return LevelSegment.HALL_VERTICAL;
              else if (map [x - 1] [y] < 0 && map [x + 1] [y] < 0 && map [x] [y - 1] >= 0 && map [x] [y + 1] >= 0)
                return LevelSegment.HALL_HORIZONTAL;
              else if (map [x - 1] [y] < 0 && map [x + 1] [y] >= 0 && map [x] [y - 1] < 0 && map [x] [y + 1] >= 0)
                return LevelSegment.HALL_CORNER_LOWER_LEFT;
              else if (map [x - 1] [y] >= 0 && map [x + 1] [y] < 0 && map [x] [y - 1] < 0 && map [x] [y + 1] >= 0)
                return LevelSegment.HALL_CORNER_LOWER_RIGHT;
              else if (map [x - 1] [y] < 0 && map [x + 1] [y] >= 0 && map [x] [y - 1] >= 0 && map [x] [y + 1] < 0)
                return LevelSegment.HALL_CORNER_UPPER_LEFT;
              else if (map [x - 1] [y] >= 0 && map [x + 1] [y] < 0 && map [x] [y - 1] >= 0 && map [x] [y + 1] < 0)
                return LevelSegment.HALL_CORNER_UPPER_RIGHT;
              /*Halls 3ways*/
              else if (map [x - 1] [y] < 0 && map [x + 1] [y] < 0 && map [x] [y - 1] < 0 && map [x] [y + 1] >= 0) 
                return LevelSegment.HALL_T_UPPER;
              else if (map [x - 1] [y] < 0 && map [x + 1] [y] < 0 && map [x] [y - 1] >= 0 && map [x] [y + 1] < 0)
                return LevelSegment.HALL_T_LOWER;
              else if (map [x - 1] [y] < 0 && map [x + 1] [y] >= 0 && map [x] [y - 1] < 0 && map [x] [y + 1] < 0)
                return LevelSegment.HALL_T_LEFT;
              else if (map [x - 1] [y] >= 0 && map [x + 1] [y] < 0 && map [x] [y - 1] < 0 && map [x] [y + 1] < 0)
                return LevelSegment.HALL_T_RIGHT;
              /*Halls ends*/
              else if (map [x - 1] [y] >= 0 && map [x + 1] [y] >= 0 && map [x] [y - 1] >= 0 && map [x] [y + 1] < 0)
                return LevelSegment.HALL_END_UPPER;
              else if (map [x - 1] [y] >= 0 && map [x + 1] [y] >= 0 && map [x] [y - 1] < 0 && map [x] [y + 1] >= 0)
                return LevelSegment.HALL_END_LOWER;
              else if (map [x - 1] [y] < 0 && map [x + 1] [y] >= 0 && map [x] [y - 1] >= 0 && map [x] [y + 1] >= 0)
                return LevelSegment.HALL_END_LEFT;
              else if (map [x - 1] [y] >= 0 && map [x + 1] [y] < 0 && map [x] [y - 1] >= 0 && map [x] [y + 1] >= 0)
                return LevelSegment.HALL_END_RIGHT;
        /*4way*/
         else if (map [x - 1] [y] < 0 && map [x + 1] [y] < 0 && map [x] [y - 1] < 0 && map [x] [y + 1] < 0)
                return LevelSegment.HALL_4;
            }
      
            return 0;
      }
    
          public void calculateLevel()
          {
            Vector3 levelPos = transform.position;
            if (level != null)
              {
                levelPos = level.transform.position;
                GameObject.DestroyImmediate(level.gameObject);
              }
              level = new GameObject();
              level.name = "Finished level";
              level.transform.parent = transform;
        
              level.transform.rotation = segments.rotation;
      
              for (int x = 1; x < size.x+1; x++)
                for (int y = 1; y < size.y+1; y++)
                  {
                    if (map [x] [y] != 0)
                      set [Mathf.Abs(map [x] [y]) - 1].createMesh(x - (int)size.x / 2 + 1, y - (int)size.y / 2 + 1, calculatePoint(x, y), level.transform);
                  }

                level.transform.position = levelPos;
                level.transform.localScale = new Vector3(1, 1, -1);
          }
    
              public void cycleType()
              {
                if (Event.current.button == 0)
                  currentType++;
                else if (Event.current.button == 1)
                  currentType--;
                if (currentType > set.Length)
                  currentType = 0;
                if (currentType < 0)
                  currentType = set.Length;
              }
    
              public void toggleHall()
              {
                bHall = !bHall;
              }
  }
}