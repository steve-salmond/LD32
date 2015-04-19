using UnityEngine;
using System.Collections;

public class RandomSprite : MonoBehaviour {

    public Sprite[] Sprites;

    private SpriteRenderer _renderer;

    void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        var index = Random.Range(0, Sprites.Length);
        _renderer.sprite = Sprites[index];
    }
}
