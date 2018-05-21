using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

    public Transform tilePrefab; //префаб тайлов
    public Transform obstaclePrefab; //префаб стен
    public Vector2 mapSize; //размер карты

    [Range(0,1)]
    public float outlinePercent; //размер расстояния между тайлами

    List<Coord> allTileCoords; //координаты всех тайлов
    Queue<Coord> shuffledTileCoords; //перетасованные координаты

    public int seed = 10;

    private void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        allTileCoords = new List<Coord>();
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                allTileCoords.Add(new Coord(x,y));
            }
        }
        shuffledTileCoords = new Queue<Coord>(Utility.ShuffleArray(allTileCoords.ToArray(),seed)); //перетасованная очередь

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
                Vector3 tilePosition = CoordToPosition(x,y); //определение координат тайлов
                Transform newTile = Instantiate(tilePrefab,tilePosition,Quaternion.Euler(Vector3.right * 90)) as Transform; //создание тайлов
                newTile.localScale = Vector3.one * (1 - outlinePercent);
                newTile.parent = mapHolder;
            }
        }

        int obstacleCount = 10;
        for (int i = 0; i < obstacleCount; i++) //создание стен
        {
            Coord randomCoord = GetRandomCoord();
            Vector3 obstaclePosition = CoordToPosition(randomCoord.x,randomCoord.y);
            Transform newObstacle = Instantiate(obstaclePrefab,obstaclePosition + Vector3.up * 0.5f,Quaternion.identity) as Transform;
            newObstacle.parent = mapHolder;
        }
    }

    Vector3 CoordToPosition(int x, int y)
    {
        return new Vector3(-mapSize.x / 2 + 0.5f + x,0,-mapSize.y / 2 + 0.5f + y); 
    }

    //Выборка координат первого в очереди элемента
    public Coord GetRandomCoord()
    {
        Coord randomCoord = shuffledTileCoords.Dequeue();
        shuffledTileCoords.Enqueue(randomCoord);
        return randomCoord;
    }

    public struct Coord
    {
        public int x;
        public int y;

        public Coord(int _x, int _y)
        {
            x = _x;
            y = _y;
        }
    }

}
