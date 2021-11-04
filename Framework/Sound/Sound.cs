using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Container for an audio file, pitch, volume, and play functionality
/// </summary>
[System.Serializable]
public class Sound
{
    public string Name = "New Sound";
    public AudioClip Clip;
    [Range(-3, 3)] public float Pitch = 1f;
    [Range(0, 1)] public float Volume = 1f;

    private float _initialPitch = -1;
    private float _initialVolume = -1;

    /// <summary>
    /// Play this sound from the specified audio source
    /// </summary>
    public void Play(AudioSource source)
    {
        SetInitialValues();

        source.pitch = Pitch;
        source.volume = Volume;
        source.PlayOneShot(Clip);
    }

    public void RandomizePitch(float maxOffset)
    {
        SetInitialValues();
        Pitch = Random.Range(_initialPitch - maxOffset, _initialPitch + maxOffset);
    }

    public void RandomizeVolume(float maxOffset)
    {
        SetInitialValues();
        Volume = Random.Range(_initialVolume - maxOffset, _initialVolume + maxOffset);
    }

    private void SetInitialValues()
    {
        if (_initialPitch < 0)
            _initialPitch = Pitch;
        if (_initialVolume < 0)
            _initialVolume = Volume;
    }
}