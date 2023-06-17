using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PTS_Controller : MonoBehaviour
{
    [SerializeField]
    ParticleSystem Win_PTS;
    // Start is called before the first frame update
    void Awake()
    {
        Win_PTS = GetComponent<ParticleSystem>();
    }

    public void Activate_PTS()
    {
        Win_PTS.Play();
    }

    public bool GetPlayState()
    {
        return Win_PTS.isPlaying;
    }
}
