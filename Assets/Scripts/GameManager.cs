using UnityEngine;
using System.Collections;

public class GameManager : Singleton<GameManager> 
{
    public int FrameRate;

	void Start() 
    {
        Application.targetFrameRate = FrameRate;
	}
	
}
