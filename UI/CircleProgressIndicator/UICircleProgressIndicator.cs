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

    [Space]
    [Tooltip("Invoked when progress is started")]
    public UnityEvent OnPlay;
    [Tooltip("Invoked when progress is stopped")]
    public UnityEvent OnStop;
    [Tooltip("Invoked when progress is complete")]
    public UnityEvent OnComplete;

    private Image _image;
    private float _timer = 0.0f;
    private bool _isStarted = false;
    private bool _isComplete = false;

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
        // Increase or decrease progress
        if (_isStarted)
        {
            _timer = Mathf.Clamp(_timer + Time.deltaTime, 0, ProgressTime);
            if (_timer == ProgressTime && !_isComplete)
            {
                _isComplete = true;
                OnComplete.Invoke();
            }
        }
        else if (!_isStarted)
            _timer = Mathf.Clamp(_timer - Time.deltaTime, 0, ProgressTime);

        // Update image fill amount
        _image.fillAmount = ProgressCurve.Evaluate(_timer / ProgressTime);
    }

    public void Play()
    {
        if (!_isStarted)
        {
            _isStarted = true;
            OnPlay.Invoke();
        }
    }

    public void Rewind()
    {
        if (_isStarted)
        {
            _isStarted = false;
            _isComplete = false;
            OnStop.Invoke();
        }
    }
}
