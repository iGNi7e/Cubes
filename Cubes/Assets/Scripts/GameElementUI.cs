using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameElementUI : MonoBehaviour {
    
    public Text enemyCountText;
    public Slider enemyCountSlider;

    public Text timeBetweenSpawnText;
    public Slider timeBetweenSpawnSlider;

    public Text obstaclePercentText;
    public Slider obstaclePercentSlider;

    public Spawner spawner;

    public MapGenerator mapGenerator;

    private void Update()
    {
        spawner.waves[0].enemyCount = Convert.ToInt32(enemyCountSlider.value);
        enemyCountText.text = "Enemy count: " + spawner.waves[0].enemyCount;

        spawner.waves[0].timeBetweenSpawn = timeBetweenSpawnSlider.value;
        timeBetweenSpawnText.text = "Time between spawn enemies: " + spawner.waves[0].timeBetweenSpawn + " sec";

        mapGenerator.obstaclePercent = obstaclePercentSlider.value;
        obstaclePercentText.text = "Obstacle Percent: " + mapGenerator.obstaclePercent;
        mapGenerator.GenerateMap();
    }

    public void SpawnerEnable()
    {
        spawner.enabled = true;
        gameObject.SetActive(false);
    }
}
