using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] List<Collider> childColliders = new List<Collider>();
    [SerializeField] private List<MapShapePrefab> prefabMapList;
    [SerializeField] int offset = 20;
    [SerializeField] int mapSize = 20;
    private MapShape lastShape;
    private Transform curMapTransform;
    private Transform nextMapTransform;
    public Player playerController;
    // Start is called before the first frame update
    void Awake()
    {
        CreateMainTerrain();
    }
    void CreateMainTerrain ( )
    {
        if (childColliders.Count <= 0)
        {
            Transform terrainPart = Instantiate(prefabMapList[0].prefabs[0].transform, transform);
            childColliders.Add(terrainPart.GetComponent<Collider>());
            curMapTransform = terrainPart.transform;
            terrainPart.GetComponent<TerrainPart>().Show();
        }
    }
    // Update is called once per frame
    void Update()
    {
        PlayerInsideColliders();
        if(curMapTransform == nextMapTransform)DrawNextMap();
    }
    public void PlayerInsideColliders ( )
    {
        bool insideAny = false;

        foreach (var entry in childColliders)
        {
            if (entry.bounds.Contains(playerController.transform.position))
            {
                insideAny = true;

                TerrainPart terrainPart = entry.GetComponent<TerrainPart>();
                if (!terrainPart.IsActived())
                {
                    terrainPart.Show();
                    nextMapTransform = curMapTransform;
                    curMapTransform = terrainPart.transform;
                }

                Debug.Log("Player is INSIDE " + entry.name);
                break;
            }
            else
            {
                entry.GetComponent<TerrainPart>().Hide();
            }
        }

        if (!insideAny)
            Debug.Log("Player is OUTSIDE all colliders");
    }
    public void DrawNextMap ( )
    {
        int randNum = Random.Range(1,(int)MapShape.Straight);
        MapShape newShape = (MapShape)randNum;
            //lastShape = (MapShape)randNum;
        foreach (MapShapePrefab elem in prefabMapList)
        {
            if(elem.shape == newShape && elem.prefabs.Length > 0)
            {
                int randGameObject = Random.Range(0, elem.prefabs.Length);
                GameObject newMap = Instantiate(elem.prefabs[randGameObject]);
                if (newShape == MapShape.Straight)
                {
                    newMap.transform.position = curMapTransform.position + Vector3.down * offset;
                }
                else if(newShape == MapShape.Corner_UP_LEFT)
                {
                    newMap.transform.position = curMapTransform.position + Vector3.down * offset + Vector3.left * offset;
                    GameObject newStaticMap = Instantiate(elem.prefabs[randGameObject]);
                    newStaticMap.transform.position = curMapTransform.position + Vector3.down * offset;
                }
                else if(newShape == MapShape.Corner_UP_RIGHT)
                {
                    newMap.transform.position = curMapTransform.position + Vector3.down * offset + Vector3.right * offset;
                    GameObject newStaticMap = Instantiate(elem.prefabs[randGameObject]);
                    newStaticMap.transform.position = curMapTransform.position + Vector3.down * offset;
                }
            }
        }
    }
}
