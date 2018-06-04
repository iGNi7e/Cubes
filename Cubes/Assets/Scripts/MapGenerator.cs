using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

    public Transform tilePrefab; //префаб тайлов
    public Transform obstaclePrefab; //префаб стен
    public Transform navmeshFloor;//navmesh
    public Transform navmeshMaskPrefab;//ограничение navmesh по краям карты
    public Vector2 mapSize; //размер карты
    public Vector2 maxMapSize;

    [Range(0,1)]
    public float outlinePercent; //размер расстояния между тайлами

    [Range(0,1)]
    public float obstaclePercent;

    public float tileSize;

    List<Coord> allTileCoords; //координаты всех тайлов
    Queue<Coord> shuffledTileCoords; //перетасованные координаты

    public int seed = 10;
    Coord mapCenter;

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
        mapCenter = new Coord((int)mapSize.x / 2,(int)mapSize.y / 2);

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
                newTile.localScale = Vector3.one * (1 - outlinePercent) * tileSize;
                newTile.parent = mapHolder;
            }
        }

        bool[,] obstacleMap = new bool[(int)mapSize.x,(int)mapSize.y];

        int obstacleCount = (int)(mapSize.x * mapSize.y * obstaclePercent);
        int currentObstacleCount = 0;
        for (int i = 0; i < obstacleCount; i++) //создание стен
        {
            Coord randomCoord = GetRandomCoord();
            obstacleMap[randomCoord.x,randomCoord.y] = true;
            currentObstacleCount++;
            if (randomCoord.x != mapCenter.x && randomCoord.y != mapCenter.y && MapIsFullAccessible(obstacleMap,currentObstacleCount))
            {
                Vector3 obstaclePosition = CoordToPosition(randomCoord.x,randomCoord.y);

                Transform newObstacle = Instantiate(obstaclePrefab,obstaclePosition + Vector3.up * 0.5f,Quaternion.identity) as Transform;
                newObstacle.parent = mapHolder;
                newObstacle.localScale = Vector3.one * (1 - outlinePercent) * tileSize;
            }
            else
            {
                obstacleMap[randomCoord.x,randomCoord.y] = false;
                currentObstacleCount--;
            }
        }

     #region Создание navmesh
        //создание по краям navmesh obstacle
        Transform maskleft = Instantiate(navmeshMaskPrefab,Vector3.left * (mapSize.x + maxMapSize.x) / 4 * tileSize,Quaternion.identity) as Transform;
        maskleft.parent = mapHolder;
        maskleft.localScale = new Vector3((maxMapSize.x - mapSize.x) / 2,1,mapSize.y) * tileSize;

        Transform maskRight = Instantiate(navmeshMaskPrefab,Vector3.right * (mapSize.x + maxMapSize.x) / 4f * tileSize,Quaternion.identity) as Transform;
        maskRight.parent = mapHolder;
        maskRight.localScale = new Vector3((maxMapSize.x - mapSize.x) / 2f,1,mapSize.y) * tileSize;

        Transform maskTop = Instantiate(navmeshMaskPrefab,Vector3.forward * (mapSize.y + maxMapSize.y) / 4f * tileSize,Quaternion.identity) as Transform;
        maskTop.parent = mapHolder;
        maskTop.localScale = new Vector3(maxMapSize.x,1,(maxMapSize.y - mapSize.y) / 2f) * tileSize;

        Transform maskBot = Instantiate(navmeshMaskPrefab,Vector3.back * (mapSize.y + maxMapSize.y) / 4f * tileSize,Quaternion.identity) as Transform;
        maskBot.parent = mapHolder;
        maskBot.localScale = new Vector3(maxMapSize.x,1,(maxMapSize.y - mapSize.y) / 2f) * tileSize;


        navmeshFloor.localScale = new Vector3(maxMapSize.x,maxMapSize.y) * tileSize; //вычисление размера navmesh
#endregion

    }

    bool MapIsFullAccessible(bool[,] obstacleMap, int currentObstacleCount) //определение возможности поставки препятствия на карте, алгоритм заливки Flood fild
    {
        bool[,] mapFlags = new bool[obstacleMap.GetLength(0),obstacleMap.GetLength(1)];
        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue(mapCenter);
        mapFlags[mapCenter.x,mapCenter.y] = true;

        int accessibleTileCount = 1;

        while(queue.Count > 0)
        {
            Coord tile = queue.Dequeue();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    int neighbourX = tile.x + x;
                    int neighbourY = tile.y + y;
                    if (x == 0 || y == 0)
                    {
                        if(neighbourX >=0 && neighbourX < obstacleMap.GetLength(0) && neighbourY >= 0 && neighbourY < obstacleMap.GetLength(1))
                        {
                            if(!mapFlags[neighbourX,neighbourY] && !obstacleMap[neighbourX,neighbourY])
                            {
                                mapFlags[neighbourX,neighbourY] = true;
                                queue.Enqueue(new Coord(neighbourX,neighbourY));
                                accessibleTileCount++;
                            }
                        }
                    }
                }
            }
        }

        int targetAccessibleTielCount = (int)(mapSize.x * mapSize.y - currentObstacleCount);
        return targetAccessibleTielCount == accessibleTileCount;
    }

    Vector3 CoordToPosition(int x, int y)
    {
        return new Vector3(-mapSize.x / 2 + 0.5f + x,0,-mapSize.y / 2 + 0.5f + y) * tileSize; 
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
