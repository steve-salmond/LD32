using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameManager : Singleton<GameManager> 
{
    public int FrameRate;

    public int Recycles;

    public GameObject GameOverScreen;
    public GameObject WinScreen;

    private bool _win;

	void Start() 
    {
        Application.targetFrameRate = FrameRate;

        ScreenFadeManager.Instance.Fade(Color.black, 0, 3);

        StartCoroutine("GameRoutine");
    }


    IEnumerator GameRoutine()
    {
        GameOverScreen.SetActive(false);

        var wait = new WaitForEndOfFrame();
        while (!_win && PlayerController.Instance.Health > 0)
            yield return wait;

        var fadeOutTime = 5;
        ScreenFadeManager.Instance.Fade(Color.black, fadeOutTime, 10000);
        yield return new WaitForSeconds(fadeOutTime);

        if (_win)
            WinScreen.SetActive(true);
        else
            GameOverScreen.SetActive(true);

        while (!Input.anyKey)
            yield return wait;

        Application.LoadLevel(Application.loadedLevel);
    }

    public void AddRecycleable(GameObject gameObject)
    {
        Recycles++;
    }

    public void Recycle(GameObject gameObject)
    {
        if (Recycles <= 0)
            return;

        Recycles = Math.Max(0, Recycles - 1);

        if (Recycles == 0)
            Win();
    }

    void Win()
    { _win = true; }
	
}
