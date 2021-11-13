using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance = null;

    public AudioSource musicSource;
    public AudioSource soundSource;

    float m_volume = 1f;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

    }

    public void PlayBackgroundMusic(string _musicdName, bool loop = true, float _volume = -1)
    {
        // Resources/Audio klasöründe bul
        string path = "Audio/Musics/" + _musicdName;

        AudioClip clip = Resources.Load<AudioClip>(path);

        // Volume ayarý
        float vol = 0;

        if (_volume == -1)
            vol = m_volume;
        else
            vol = _volume;


        if (clip != null)
        {
            musicSource.clip = clip;
            musicSource.volume = vol;
            musicSource.loop = loop;
            musicSource.Play();
        }

    }

    public void PlaySound(string _soundName, float _volume = -1)
    {
        // Resources/Audio klasöründe bul
        string path = "Audio/Sounds/" + _soundName;

        AudioClip clip = Resources.Load<AudioClip>(path);

        // Volume ayarý
        float vol = 0;

        if (_volume == -1)
            vol = m_volume;
        else
            vol = _volume;


        if (clip != null)
        {
            soundSource.clip = clip;
            soundSource.volume = vol;
            soundSource.Play();
        }

    }

    public void StopBackgroundSound()
    {

        if (musicSource.isPlaying)
        {
            musicSource.Stop();
        }
    }
}