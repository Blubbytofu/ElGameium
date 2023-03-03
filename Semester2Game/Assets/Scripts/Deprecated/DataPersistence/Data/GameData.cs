using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public Vector3 playerPosition;
    public Vector3 playerRotation;

    public GameData()
    {
        playerPosition = new Vector3(0, 0.9f, 0);
        playerRotation = new Vector3(0, 0, 0);
    }
}
