using UnityEngine;
using System.Collections;

[System.Serializable]
public class ColorToPrefab
{
	public Color32 color;
	public GameObject prefab;
}

public class LevelLoader : MonoBehaviour {
	//public Texture2D levelMap;
	public string levelFileName;
	public ColorToPrefab[] colorToPrefab;

	void Awake () 
	{
		LoadMap();
	}

	void EmptyMap()
	{
		// Remove todos os childrens desse elemento
		while(transform.childCount > 0)
		{
			Transform c = transform.GetChild(0);
			Destroy(c);
			c.SetParent(null);
		}
	}

	void LoadMap()
	{
		EmptyMap();

		string filePath = Application.dataPath + "/StreamingAssets/" + levelFileName;

		byte[] bytes = System.IO.File.ReadAllBytes(filePath);

		Texture2D levelMap = new Texture2D(2, 2);
		levelMap.LoadImage(bytes);


		Color32[] allPixels = levelMap.GetPixels32();
		int width = levelMap.width;
		int height = levelMap.height;

		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				SpawnTileAt(allPixels[(y * width) + x] , x, y);
			}
		}
	}

	void SpawnTileAt( Color32 c, int x, int y)
	{
		if(c.a <= 0)
		{
			return;
		}

		foreach(ColorToPrefab ctp in colorToPrefab)
		{
			Color32 color = ctp.color;
			//Debug.Log(color.ToString());
			if(c.Equals(color))
			{
				GameObject go = Instantiate(ctp.prefab, new Vector3(x, y, 0), Quaternion.identity) as GameObject;
				go.name = go.name.Substring(0, go.name.LastIndexOf("(Clone)"));
				go.transform.SetParent(this.transform);
				return;
			}
		}

		Debug.LogError("No Color to Prefab found for: " + c.ToString());
	}
}
