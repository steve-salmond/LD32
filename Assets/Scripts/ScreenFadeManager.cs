using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScreenFadeManager : Singleton<ScreenFadeManager>
{

    private Image _image;

    private Color _targetColor = Color.black;

    private float _attack;
    private float _decay;
    private float _wait;

    void Start()
    {
        _image = GetComponent<Image>();
    }

    public void Fade(Color color, float attack, float decay, float wait = 0)
    {
        _targetColor = color;
        _attack = attack;
        _decay = decay;
        _wait = wait;
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

        if (_wait > 0)
            yield return new WaitForSeconds(_wait);

        // Fade back to transparent.
        var transparent = new Color(c1.r, c1.g, c1.b, 0);
        if (_decay > 0)
        {
            var t0 = Time.time;
            var t1 = t0 + _decay;
            while (Time.time < t1)
            {
                var t = Time.time;
                var f = (t - t0) / (t1 - t0);
                _image.color = Color.Lerp(c1, transparent, f);
                yield return wait;
            }
        }

        // Now fully transparent.
        _image.color = transparent;

    }
}
