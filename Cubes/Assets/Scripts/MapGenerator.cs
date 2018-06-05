using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

    public Map[] maps;
    public int mapIndex;

    public Transform tilePrefab; //префаб тайлов
    public Transform obstaclePrefab; //префаб стен
    public Transform navmeshFloor;//navmesh
    public Transform navmeshMaskPrefab;//ограничение navmesh по краям карты
    public Vector2 maxMapSize;

    [Range(0,1)]
    public float outlinePercent; //размер расстояния между тайлами

    public float tileSize;

    List<Coord> allTileCoords; //координаты всех тайлов
    Queue<Coord> shuffledTileCoords; //перетасованные координаты

    Map currentMap;

    private void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        currentMap = maps[mapIndex];
        System.Random prng = new System.Random(currentMap.seed);
        GetComponent<BoxCollider>().size = new Vector3(currentMap.mapSize.x * tileSize,0.05f,currentMap.mapSize.y * tileSize);

        //генерация координат
        allTileCoords = new List<Coord>();
        for (int x = 0; x < currentMap.mapSize.x; x++)
        {
            for (int y = 0; y < currentMap.mapSize.y; y++)
            {
                allTileCoords.Add(new Coord(x,y));
            }
        }
        shuffledTileCoords = new Queue<Coord>(Utility.ShuffleArray(allTileCoords.ToArray(),currentMap.seed)); //перетасованная очередь

#region Отрисовка и корректировка расположения объектов в иерархии
        string holderName = "Generated Map";
        if (transform.Find(holderName))
        {
            DestroyImmediate(transform.Find(holderName).gameObject);
        }
        Transform mapHolder = new GameObject(holderName).transform;
        mapHolder.parent = transform;
        #endregion

#region Создание тайлов карты
        for (int x = 0; x < currentMap.mapSize.x; x++)
        {
            for (int y = 0; y < currentMap.mapSize.y; y++)
            {
                Vector3 tilePosition = CoordToPosition(x,y); //определение координат тайлов
                Transform newTile = Instantiate(tilePrefab,tilePosition,Quaternion.Euler(Vector3.right * 90)) as Transform; //создание тайлов
                newTile.localScale = Vector3.one * (1 - outlinePercent) * tileSize;
                newTile.parent = mapHolder;
            }
        }
        #endregion

#region Создание obstacles
        bool[,] obstacleMap = new bool[(int)currentMap.mapSize.x,(int)currentMap.mapSize.y];

        int obstacleCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y * currentMap.obstaclePercent);
        int currentObstacleCount = 0;
        for (int i = 0; i < obstacleCount; i++) //создание стен
        {
            Coord randomCoord = GetRandomCoord();
            obstacleMap[randomCoord.x,randomCoord.y] = true;
            currentObstacleCount++;
            if (randomCoord.x != currentMap.mapCenter.x && randomCoord.y != currentMap.mapCenter.y && MapIsFullAccessible(obstacleMap,currentObstacleCount))
            {
                float obstacleHeight = Mathf.Lerp(currentMap.minObstacleHeight,currentMap.maxObstacleHeight,(float)prng.NextDouble());

                Vector3 obstaclePosition = CoordToPosition(randomCoord.x,randomCoord.y);

                Transform newObstacle = Instantiate(obstaclePrefab,obstaclePosition + Vector3.up * obstacleHeight/2,Quaternion.identity) as Transform;
                newObstacle.parent = mapHolder;
                newObstacle.localScale = new Vector3((1 - outlinePercent) * tileSize,obstacleHeight,(1 - outlinePercent) * tileSize);

                Renderer obstacleRenderer = newObstacle.GetComponent<Renderer>();
                Material obstacleMaterial = new Material(obstacleRenderer.sharedMaterial);
                float colourPercent = randomCoord.y / (float)currentMap.mapSize.y;
                obstacleMaterial.color = Color.Lerp(currentMap.foregroundColour,currentMap.backgroundColour,colourPercent);
                obstacleRenderer.sharedMaterial = obstacleMaterial;

                //allOpenCoords.Remove(randomCoord);
            }
            else
            {
                obstacleMap[randomCoord.x,randomCoord.y] = false;
                currentObstacleCount--;
            }
        }
#endregion

#region Создание navmesh
        //создание по краям navmesh obstacle
        Transform maskleft = Instantiate(navmeshMaskPrefab,Vector3.left * (currentMap.mapSize.x + currentMap.mapSize.x) / 4f * tileSize,Quaternion.identity) as Transform;
        maskleft.parent = mapHolder;
        maskleft.localScale = new Vector3((currentMap.mapSize.x - currentMap.mapSize.x) / 2f,1,currentMap.mapSize.y) * tileSize;

        Transform maskRight = Instantiate(navmeshMaskPrefab,Vector3.right * (currentMap.mapSize.x + currentMap.mapSize.x) / 4f * tileSize,Quaternion.identity) as Transform;
        maskRight.parent = mapHolder;
        maskRight.localScale = new Vector3((currentMap.mapSize.x - currentMap.mapSize.x) / 2f,1,currentMap.mapSize.y) * tileSize;

        Transform maskTop = Instantiate(navmeshMaskPrefab,Vector3.forward * (currentMap.mapSize.y + currentMap.mapSize.y) / 4f * tileSize,Quaternion.identity) as Transform;
        maskTop.parent = mapHolder;
        maskTop.localScale = new Vector3(currentMap.mapSize.x,1,(currentMap.mapSize.y - currentMap.mapSize.y) / 2f) * tileSize;

        Transform maskBot = Instantiate(navmeshMaskPrefab,Vector3.back * (currentMap.mapSize.y + currentMap.mapSize.y) / 4f * tileSize,Quaternion.identity) as Transform;
        maskBot.parent = mapHolder;
        maskBot.localScale = new Vector3(currentMap.mapSize.x,1,(currentMap.mapSize.y - currentMap.mapSize.y) / 2f) * tileSize;


        navmeshFloor.localScale = new Vector3(currentMap.mapSize.x,currentMap.mapSize.y) * tileSize; //вычисление размера navmesh
#endregion

    }

    bool MapIsFullAccessible(bool[,] obstacleMap, int currentObstacleCount) //определение возможности поставки препятствия на карте, алгоритм заливки Flood fild
    {
        bool[,] mapFlags = new bool[obstacleMap.GetLength(0),obstacleMap.GetLength(1)];
        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue(currentMap.mapCenter);
        mapFlags[currentMap.mapCenter.x,currentMap.mapCenter.y] = true;

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

        int targetAccessibleTielCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y - currentObstacleCount);
        return targetAccessibleTielCount == accessibleTileCount;
    }

    Vector3 CoordToPosition(int x, int y)
    {
        return new Vector3(-currentMap.mapSize.x / 2 + 0.5f + x,0,-currentMap.mapSize.y / 2 + 0.5f + y) * tileSize; 
    }

    //Выборка координат первого в очереди элемента
    public Coord GetRandomCoord()
    {
        Coord randomCoord = shuffledTileCoords.Dequeue();
        shuffledTileCoords.Enqueue(randomCoord);
        return randomCoord;
    }

    [System.Serializable]
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

    [System.Serializable]
    public class Map
    {
        public Coord mapSize;
        [Range(0,1)]
        public float obstaclePercent;
        public int seed;
        public float minObstacleHeight;
        public float maxObstacleHeight;
        public Color foregroundColour;
        public Color backgroundColour;

        public Coord mapCenter
        {
            get { return new Coord((int)mapSize.x / 2,(int)mapSize.y / 2); }
        }
    }

}
