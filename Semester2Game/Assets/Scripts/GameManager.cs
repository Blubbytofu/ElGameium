using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private string levelName;
    [SerializeField] private int maxEnemies;
    private int currentEnemiesKilled;
    [SerializeField] private int maxSecrets;
    private int currentSecretsFound;

    public string GetLevelName()
    {
        return levelName;
    }

    public string GetPassedTime()
    {
        float timer = Time.timeSinceLevelLoad;
        int minutes = Mathf.FloorToInt(timer / 60F);
        int seconds = Mathf.FloorToInt(timer - minutes * 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void AddEnemyKilled()
    {
        currentEnemiesKilled++;
    }

    public void AddSecretFound()
    {
        currentSecretsFound++;
    }

    public int GetMaxEnemies()
    {
        return maxEnemies;
    }
    
    public int GetCurrentEnemiesKilled()
    {
        return currentEnemiesKilled;
    }

    public int GetMaxSecrets()
    {
        return maxSecrets;
    }

    public int GetCurrentSecretsFound()
    {
        return currentSecretsFound;
    }
}
