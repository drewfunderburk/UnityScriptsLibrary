using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UICircleProgressIndicator : MonoBehaviour
{
    [Tooltip("Total time this indicator takes to complete")]
    public float ProgressTime = 1.0f;
    [Tooltip("Speed of the progress bar at a given time")]
    public AnimationCurve ProgressCurve = AnimationCurve.Linear(0, 0, 1, 1);
    [Tooltip("Is this indicator actively progressing?")]
    [SerializeField] private bool _isProgressing = false;

    private Image _image;
    private float _timer = 0.0f;

    /// <summary>
    /// Whether or not this indicator actively progressing
    /// </summary>
    public bool IsProgressing { get => _isProgressing; set => _isProgressing = value; } // Needs to be a property to work with UnityEvents

    /// <summary>
    /// Current progress from 0 - 1
    /// </summary>
    public float Progress { get { return _image.fillAmount; } }

    private void Awake()
    {
        _image = GetComponentInChildren<Image>();
    }

    private void Update()
    {
        if (IsProgressing)
            _timer = Mathf.Clamp(_timer + Time.deltaTime, 0, ProgressTime);
        else if (!IsProgressing)
            _timer = Mathf.Clamp(_timer - Time.deltaTime, 0, ProgressTime);

        _image.fillAmount = ProgressCurve.Evaluate(_timer / ProgressTime);
    }
}
