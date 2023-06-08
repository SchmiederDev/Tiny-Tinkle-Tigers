using UnityEngine;
using UnityEngine.Audio;
using System;

[Serializable]
public class Sound
{
    public AudioClip clip;

    [HideInInspector]
    public AudioSource source;

    public string name;

    [Range(0f, 1f)]
    public float volume;

    [Range(-1f, 3f)]
    public float pitch;

    public bool loop;
}
