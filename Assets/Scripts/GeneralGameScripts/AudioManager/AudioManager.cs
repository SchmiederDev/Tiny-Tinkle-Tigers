using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public List<Sound> GameSounds;
    public List<Sound> GameMusic;

    private Sound CurrentSong;

    // Start is called before the first frame update
    void Awake()
    {
        Init_GameMusic();
        Init_GameSounds();
    }

    private void Init_GameSounds()
    {
        foreach (Sound gameSound in GameSounds)
        {
            Init_AudioSource(gameSound);
        }
    }

    private void Init_GameMusic()
    {
        foreach(Sound musicTheme in GameMusic)
        {
            Init_AudioSource(musicTheme);
        }
    }

    private void Init_AudioSource(Sound sound)
    {
        sound.source = gameObject.AddComponent<AudioSource>();

        sound.source.clip = sound.clip;
        sound.source.volume = sound.volume;
        sound.source.pitch = sound.pitch;
        sound.source.loop = sound.loop;

    }

    public void PlaySound(string name)
    {
        Sound SoundToPlay = GameSounds.Find(SoundElement => SoundElement.name == name);
        
        if(SoundToPlay != null)
            SoundToPlay.source.Play();

        else
            Debug.LogWarning("Sound " + name + " not found");
    }

    public void PlayBackgroundMusic(string name)
    {
        Sound SongToPlay = GameMusic.Find(SoundElement => SoundElement.name == name);

        if (SongToPlay != null)
            SongToPlay.source.Play();

        else
            Debug.LogWarning("Music " + name + " not found");
    }


    public void Play_RandomSong()
    {
        int randomIndex = Random.Range(0, GameMusic.Count);
        Sound songToPlay = GameMusic[randomIndex];

        if (songToPlay != null)
        {
            CurrentSong = songToPlay;
            CurrentSong.source.Play();
        }
        else
            Debug.LogWarning("Music not found");
    }

    public void Stop_BackgroundMusic()
    {   
        CurrentSong.source.Stop();
    }

}
