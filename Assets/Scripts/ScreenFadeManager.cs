using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScreenFadeManager : Singleton<ScreenFadeManager>
{

    private Image _image;

    private Color _targetColor = Color.black;

    private float _attack;
    private float _decay;

    void Start()
    {
        _image = GetComponent<Image>();
    }

    public void Fade(Color color, float attack, float decay)
    {
        _targetColor = color;
        _attack = attack;
        _decay = decay;
        StopAllCoroutines();
        StartCoroutine("FadeRoutine");
    }

    IEnumerator FadeRoutine()
    {
        var c0 = _image.color;
        var c1 = _targetColor;
        var wait = new WaitForEndOfFrame();

        // Fade up to target color.
        if (_attack > 0)
        {
            var t0 = Time.time;
            var t1 = t0 + _attack;
            while (Time.time < t1)
            {
                var t = Time.time;
                var f = (t - t0) / (t1 - t0);
                _image.color = Color.Lerp(c0, c1, f);
                yield return wait;
            }
        }

        // Now at target color.
        _image.color = c1;

        // Fade down to black.
        if (_decay > 0)
        {
            var t0 = Time.time;
            var t1 = t0 + _decay;
            while (Time.time < t1)
            {
                var t = Time.time;
                var f = (t - t0) / (t1 - t0);
                _image.color = Color.Lerp(c1, Color.black, f);
                yield return wait;
            }
        }

        // Now fully black.
        _image.color = Color.black;

    }
}
