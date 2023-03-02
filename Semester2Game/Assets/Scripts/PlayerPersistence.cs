using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPersistence : MonoBehaviour
{
    public static GameObject playerInstance;

    void Awake()
    {
        if (playerInstance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            playerInstance = gameObject;
            DontDestroyOnLoad(playerInstance);
        }
    }
}
