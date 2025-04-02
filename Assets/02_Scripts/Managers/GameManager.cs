using UnityEngine;

public class GameManager : SingleTon<GameManager>
{
    void Start()
    {
        Application.targetFrameRate = 60;
    }

}
