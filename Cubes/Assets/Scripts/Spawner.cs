using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spawner : MonoBehaviour {

    public Wave[] waves; //массив волн
    public Enemy enemy; //префаб enemy

    LivingEntity playerEntity;
    Transform playerT;

    Wave currentWave; //текущая волна
    int currentWaveNumber; //номер текущей волны

    MapGenerator map;
    
    int enemyRemainingToSpawn; //количество enemy для спавна
    int enemyRemainingAlive; //количество enemy живых
    float nextSpawnTime; //время между спавном префаба

    float timeBetweenCampingChecks = 2f;
    float campThresholdDistance = 1f;
    float nextCampCheckTime;
    Vector3 campPositionOld;
    bool isCamping;

    bool isDisabled;

    private void Start()
    {
        playerEntity = FindObjectOfType<Player>();
        playerT = playerEntity.transform;

        nextCampCheckTime = Time.time + timeBetweenCampingChecks;
        campPositionOld = playerT.position;
        playerEntity.OnDeath += OnPlayerDeath;

        map = FindObjectOfType<MapGenerator>();

        NextWave();
    }

    private void Update()
    {
        if(!isDisabled)

        if(Time.time > nextCampCheckTime)
        {
            nextCampCheckTime = Time.time + timeBetweenCampingChecks;

            isCamping = (Vector3.Distance(playerT.position,campPositionOld) < campThresholdDistance);
            campPositionOld = playerT.position;
        }

        if(enemyRemainingToSpawn > 0 && Time.time > nextSpawnTime)
        {
            enemyRemainingToSpawn--;
            nextSpawnTime = Time.time + currentWave.timeBetweenSpawn;

            StartCoroutine(SpawnEnemy());
        }
    }

    IEnumerator SpawnEnemy()
    {
        float spawnDelay = 1;
        float tileFlashSpeed = 4;

        Transform spawnTile = map.GetRandomOpenTile();
        if (isCamping)
        {
            spawnTile = map.GetTileFromPosition(playerT.position);
        }
        Material tileMat = spawnTile.GetComponent<Renderer>().material;
        Color initialColor = tileMat.color;
        Color flashColor = Color.red;
        float spawnTimer = 0;

        while(spawnTimer < spawnDelay)
        {
            tileMat.color = Color.Lerp(initialColor,flashColor,Mathf.PingPong(spawnTimer * tileFlashSpeed,1));

            spawnTimer += Time.deltaTime;
            yield return null;
        }
        Enemy spawnedEnemy = Instantiate(enemy,spawnTile.position + Vector3.up,Quaternion.identity) as Enemy; //создание префаба енеми
        spawnedEnemy.OnDeath += OnEnemyDeath; //добавление событию метода OnEnemyDeath()
    }

    void OnPlayerDeath()
    {
        isDisabled = true;
    }

    void OnEnemyDeath()
    {
        enemyRemainingAlive--; //уменьшение количества живых енеми

        if(enemyRemainingAlive == 0) //если количество живых енеми == 0 то происходит запуск новой волны
        {
            NextWave();
        }
    }

    void NextWave()
    {
        currentWaveNumber++;
        print("Wave: " + currentWaveNumber);
        if (currentWaveNumber - 1 < waves.Length)
        {
            currentWave = waves[currentWaveNumber - 1];

            enemyRemainingToSpawn = currentWave.enemyCount;
            enemyRemainingAlive = enemyRemainingToSpawn;
        }
    }

    [System.Serializable]
    public class Wave {
        public int enemyCount;
        public float timeBetweenSpawn;
    }


}
