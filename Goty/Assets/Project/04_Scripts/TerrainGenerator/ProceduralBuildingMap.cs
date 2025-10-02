using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MapShape
{
    Corner_UP_LEFT = 1,
    Corner_UP_RIGHT,
    Straight
}
public class ProceduralBuildingMap : MonoBehaviour
{
    [System.Serializable]
    public struct MapShapePrefab
    {
        public MapShape shape;
        public GameObject[] prefabs;
    }

    [SerializeField] private List<MapShapePrefab> prefabMapList;
    [SerializeField] int offset = 20;
    [SerializeField] int mapSize = 20;
    private Dictionary<MapShape, GameObject[]> prefabMap;
    private MapShape lastShape;
    private Transform curMapTransform;
    private Transform nextMapTransform;
    // Start is called before the first frame update
    void Awake()
    {
        prefabMap = new Dictionary<MapShape, GameObject[]>();
        foreach (var entry in prefabMapList)
        {
            prefabMap[entry.shape] = entry.prefabs;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void DrawMap()
    {
        for (int e = 0; e < mapSize; e++)
        {
            int randNum = Random.Range(1, (int)MapShape.Straight + 1);
            lastShape = (MapShape)randNum;
            foreach (KeyValuePair<MapShape, GameObject[]> entry in prefabMap)
            {
                if (entry.Key == (MapShape)randNum)
                {
                    if ((MapShape)randNum == MapShape.Corner_UP_RIGHT)
                    {
                        for (int i = 0; i < entry.Value.Length; i++)
                        {
                            int randGameObject = Random.Range(0, entry.Value.Length);
                            GameObject newMap = Instantiate(entry.Value[randGameObject]);
                            lastShape = (MapShape)randNum;
                        }
                    }
                    else if ((MapShape)randNum == MapShape.Corner_UP_LEFT)
                    {
                        int randGameObject = Random.Range(0, entry.Value.Length);
                        GameObject newMap = Instantiate(entry.Value[randGameObject]);
                        lastShape = (MapShape)randNum;
                    }
                    if ((MapShape)randNum == MapShape.Straight && lastShape != MapShape.Straight)
                    {
                        int randGameObject = Random.Range(0, entry.Value.Length);
                        GameObject newMap = Instantiate(entry.Value[randGameObject]);
                        lastShape = (MapShape)randNum;
                    }
                }
            }

        }

    }
}
