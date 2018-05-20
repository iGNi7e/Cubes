using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spawner : MonoBehaviour {

    public Wave[] waves; //массив волн
    public Enemy enemy; //префаб enemy

    Wave currentWave; //текущая волна
    int currentWaveNumber; //номер текущей волны
    
    int enemyRemainingToSpawn; //количество enemy для спавна
    int enemyRemainingAlive; //количество enemy живых
    float nextSpawnTime; //время между спавном префаба

    private void Start()
    {
        NextWave();
    }

    private void Update()
    {
        if(enemyRemainingToSpawn > 0 && Time.time > nextSpawnTime)
        {
            enemyRemainingToSpawn--;
            nextSpawnTime = Time.time + currentWave.timeBetweenSpawn;

            Enemy spawnedEnemy = Instantiate(enemy,Vector3.zero,Quaternion.identity) as Enemy; //создание префаба енеми
            spawnedEnemy.OnDeath += OnEnemyDeath; //добавление событию метода OnEnemyDeath()
        }
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
