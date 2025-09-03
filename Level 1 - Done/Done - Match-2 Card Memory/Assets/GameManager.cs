using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    void Awake()
    {
        instance = this;
    }
    public LevelInfomation easy = new LevelInfomation("easy", 6, 3, 30f, 2, new Vector3(3.5f,1.3f,-10), new Vector3(6.7f,2,0), -2);
    public LevelInfomation medium = new LevelInfomation("medium", 9, 4, 30f, 3, Vector2.zero, Vector2.zero, -8);
    public LevelInfomation hard = new LevelInfomation("hard", 18, 8, 30f, 3, Vector2.zero, Vector2.zero, -8);
    
}

[System.Serializable]
public class LevelInfomation
{
    public string name;
    public int sizeX;
    public int sizeY;
    public float timer;
    public float cameraSize;
    public Vector3 cameraPosition;
    public Vector3 pairedCardStartPosition;
    public float pairedCardEndPosition;

    public LevelInfomation(string name, int sizeX, int sizeY, float timer, float cameraSize, Vector2 cameraPosition, Vector2 pairedCardStartPosition, float pairedCardEndPosition)
    {
        this.name = name; this.sizeX = sizeX; this.sizeY = sizeY; this.timer = timer; this.timer = timer; this.cameraSize = cameraSize; this.cameraPosition = cameraPosition; this.pairedCardStartPosition = pairedCardStartPosition; this.pairedCardEndPosition = pairedCardEndPosition;
    }

    public LevelInfomation()
    {
        
    }
}
