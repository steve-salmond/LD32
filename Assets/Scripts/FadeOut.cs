using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeOut : MonoBehaviour
{
    private Image _image;

    public float Duration;
    public float Delay;

    void Start()
    {
        _image = GetComponent<Image>();
        StopAllCoroutines();
        StartCoroutine("FadeRoutine");
    }

    IEnumerator FadeRoutine()
    {
        var initial = _image.color;
        if (Delay > 0)
            yield return new WaitForSeconds(Delay);

        // Fade to transparent.
        var wait = new WaitForEndOfFrame();
        var transparent = new Color(initial.r, initial.g, initial.b, 0);
        if (Duration > 0)
        {
            var t0 = Time.time;
            var t1 = t0 + Duration;
            while (Time.time < t1)
            {
                var t = Time.time;
                var f = (t - t0) / (t1 - t0);
                _image.color = Color.Lerp(initial, transparent, f);
                yield return wait;
            }
        }

        // Now fully transparent.
        _image.color = transparent;

    }
}
