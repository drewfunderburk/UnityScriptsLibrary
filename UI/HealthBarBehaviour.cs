using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles a UI health bar with a slider.
/// 
/// Depends on HealthBehaviour
/// </summary>
[RequireComponent(typeof(Slider))]
public class HealthBarBehaviour : MonoBehaviour
{
    [SerializeField] private HealthBehaviour _health;
    [SerializeField] private Image _fill;
    [SerializeField] private Gradient _healthGradient;

    private Slider _slider;

    void Start()
    {
        _slider = GetComponent<Slider>();
        _slider.maxValue = _health.Health;
        _fill.color = _healthGradient.Evaluate(1f);
    }

    void Update()
    {
        _slider.value = _health.Health;
        _fill.color = _healthGradient.Evaluate(_slider.value / _slider.maxValue);
    }
}