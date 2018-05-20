using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

    public Transform tilePrefab; //префаб тайлов
    public Vector2 mapSize; //размер карты

    [Range(0,1)]
    public float outlinePercent; //размер расстояния между тайлами

    private void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        string holderName = "Generated Map";
        if (transform.Find(holderName))
        {
            DestroyImmediate(transform.Find(holderName).gameObject);
        }

        Transform mapHolder = new GameObject(holderName).transform;
        mapHolder.parent = transform;

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                Vector3 tilePosition = new Vector3(-mapSize.x / 2 + 0.5f + x,0,-mapSize.y / 2 + 0.5f + y); //определение координат тайлов
                Transform newTile = Instantiate(tilePrefab,tilePosition,Quaternion.Euler(Vector3.right * 90)) as Transform; //создание тайлов
                newTile.localScale = Vector3.one * (1 - outlinePercent);
                newTile.parent = mapHolder;
            }
        }
    }

}
