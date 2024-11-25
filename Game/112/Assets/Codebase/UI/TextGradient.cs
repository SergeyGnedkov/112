using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextGradient : MonoBehaviour
{
    [SerializeField]
    private float _gradientSpeed = 0.1f;
    [SerializeField]
    private Gradient _gradient;
    private float _gradientTime = 0;
    private Text _textRenderer;

    void Start()
    {
        _textRenderer = GetComponent<Text>();
    }


    void Update()
    {
        _gradientTime += _gradientSpeed * Time.deltaTime;
        _gradientTime %= 1f;

        _textRenderer.color = _gradient.Evaluate( _gradientTime );
    }
}
