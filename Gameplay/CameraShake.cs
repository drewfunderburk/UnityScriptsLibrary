using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CameraShake : MonoBehaviour
{
    [System.Serializable]
    private class NoiseSettings
    {
        public Vector2 XAxisNoiseStartPosition = new Vector2();
        public Vector2 XAxisNoiseScrollSpeed = new Vector2(20.0f, 20.0f);
        [Space]
        public Vector2 YAxisNoiseStartPosition = new Vector2(10.0f, 10.0f);
        public Vector2 YAxisNoiseScrollSpeed = new Vector2(20.0f, 20.0f);
        [Space]
        public Vector2 ZAxisNoiseStartPosition = new Vector2(20.0f, 20.0f);
        public Vector2 ZAxisNoiseScrollSpeed = new Vector2(20.0f, 20.0f);
    }

    [Tooltip("These settings are usually fine as default, but they're here if you'd like manual control over the noise")]
    [SerializeField] private NoiseSettings _noiseSettings;
    [Space]
    [Tooltip("Maximum distance the camera will translate on each axis at maximum trauma")]
    [SerializeField] private Vector3 _maximumTranslationalOffset = new Vector3(0.3f, 0.3f, 0.0f);
    [Tooltip("Maximum distance the camera will rotate on each axis at maximum trauma")]
    [SerializeField] private Vector3 _maximumRotationalOffset = new Vector3(10.0f, 10.0f, 10.0f);
    [Tooltip("Higher values will require the trauma to be higher before any intense shake is performed")]
    [SerializeField] private float _traumaExponent = 2.0f;
    [Tooltip("How quickly should trauma decay from a certain value")]
    [SerializeField] private AnimationCurve _traumaDecayCurve = AnimationCurve.Linear(0, 0, 1, 1);

    private float _trauma = 0;
    private Vector2 _noiseScrollX;
    private Vector2 _noiseScrollY;
    private Vector2 _noiseScrollZ;

    private void Start()
    {
        _noiseScrollX = _noiseSettings.XAxisNoiseStartPosition;
        _noiseScrollY = _noiseSettings.YAxisNoiseStartPosition;
        _noiseScrollZ = _noiseSettings.ZAxisNoiseStartPosition;
    }

    /// <summary>
    /// Traumatize the camera. Trauma is clamped between 0 and 1. For a light shake, 0.3. For a heavy shake, 0.6.
    /// </summary>
    /// <param name="trauma"></param>
    public void AddTrauma(float trauma)
    {
        _trauma = Mathf.Clamp01(_trauma + trauma);
    }

    private void Update()
    {
        // No need to process if there is no trauma
        if (_trauma <= 0)
            return;

        // Apply translational shake
        if (_maximumTranslationalOffset != Vector3.zero)
        {
            // Sample noise
            Vector3 noiseSample = SampleNoise();
            
            // Calculate positions
            float translationX = _maximumTranslationalOffset.x * Mathf.Pow(_trauma, _traumaExponent) * noiseSample.x;
            float translationY = _maximumTranslationalOffset.y* Mathf.Pow(_trauma, _traumaExponent) * noiseSample.y;
            float translationZ = _maximumTranslationalOffset.z * Mathf.Pow(_trauma, _traumaExponent) * noiseSample.z;

            // Apply translation
            transform.localPosition = new Vector3(translationX, translationY, translationZ); ;
        }

        // Apply rotational shake
        if (_maximumRotationalOffset != Vector3.zero)
        {
            // Sample noise
            Vector3 noiseSample = SampleNoise();

            // Calculate positions
            Quaternion rotationX = Quaternion.AngleAxis(_maximumRotationalOffset.x * Mathf.Pow(_trauma, _traumaExponent) * noiseSample.x, Vector3.right);
            Quaternion rotationY = Quaternion.AngleAxis(_maximumRotationalOffset.y * Mathf.Pow(_trauma, _traumaExponent) * noiseSample.y, Vector3.up);
            Quaternion rotationZ = Quaternion.AngleAxis(_maximumRotationalOffset.z * Mathf.Pow(_trauma, _traumaExponent) * noiseSample.z, Vector3.forward);
            
            transform.localRotation = rotationX * rotationY * rotationZ;
        }

        // Trauma decay
        _trauma = Mathf.Max(0, _trauma - _traumaDecayCurve.Evaluate(_trauma) * Time.deltaTime);
    }

    private Vector3 SampleNoise()
    {
        // Sample noise
        float noiseSampleX = Mathf.PerlinNoise(_noiseScrollX.x, _noiseScrollX.y) * 2 - 1;
        float noiseSampleY = Mathf.PerlinNoise(_noiseScrollY.x, _noiseScrollY.y) * 2 - 1;
        float noiseSampleZ = Mathf.PerlinNoise(_noiseScrollZ.x, _noiseScrollZ.y) * 2 - 1;

        // Scroll noise
        _noiseScrollX += _noiseSettings.XAxisNoiseScrollSpeed * Time.deltaTime;
        _noiseScrollY += _noiseSettings.YAxisNoiseScrollSpeed * Time.deltaTime;
        _noiseScrollZ += _noiseSettings.ZAxisNoiseScrollSpeed * Time.deltaTime;

        return new Vector3(noiseSampleX, noiseSampleY, noiseSampleZ);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(CameraShake))]
public class CameraShakeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        string text = "This camera shake script uses a trauma system to give both rotational and translational shake to an object based on Perlin Noise." +
            "\n\n" +
            "Place directly on the object you want shaken. Object will be reset to its local origin when no trauma is applied. " +
            "Child the object to an empty game object and affect it with any other scripts instead.";
        EditorGUILayout.HelpBox(text, MessageType.Info);

        base.OnInspectorGUI();
    }
}
#endif