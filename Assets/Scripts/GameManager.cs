using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameManager : Singleton<GameManager> 
{
    public int FrameRate;

    public GameObject GameOverScreen;

	void Start() 
    {
        Application.targetFrameRate = FrameRate;
	    
        StartCoroutine("GameRoutine");
    }


    IEnumerator GameRoutine()
    {
        GameOverScreen.SetActive(false);

        var wait = new WaitForEndOfFrame();
        while (PlayerController.Instance.Health > 0)
            yield return wait;

        var fadeOutTime = 5;
        ScreenFadeManager.Instance.Fade(Color.black, fadeOutTime, 10000);
        yield return new WaitForSeconds(fadeOutTime);

        GameOverScreen.SetActive(true);

        while (!Input.anyKey)
            yield return wait;

        Application.LoadLevel(Application.loadedLevel);
    }
	
}
