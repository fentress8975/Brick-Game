using UnityEngine;

public class SoundManager : SingletonMono<SoundManager>
{
    [SerializeField] private float m_FXVolume;
    [SerializeField] private float m_UIVolume;
    [SerializeField] private float m_MusicVolume;
    [SerializeField] private AudioSource m_AudioSourceFX;
    [SerializeField] private AudioSource m_AudioSourceUI;
    [SerializeField] private AudioSource m_AudioSourceMusic;



    private void Start()
    {

        m_AudioSourceFX.volume = m_FXVolume;
        m_AudioSourceFX.loop = false;
        m_AudioSourceUI.volume = m_UIVolume;
        m_AudioSourceUI.loop = false;
        m_AudioSourceMusic.volume = m_MusicVolume;
        DontDestroyOnLoad(this);
    }

    public void PlayFX(AudioClip sound)
    {
        m_AudioSourceFX.Stop();
        m_AudioSourceFX.PlayOneShot(sound);
    }

    public void PlayUI(AudioClip sound)
    {
        m_AudioSourceUI.Stop();
        m_AudioSourceUI.PlayOneShot(sound);
    }

    public void PlayMusic(AudioClip sound)
    {
        m_AudioSourceMusic.Stop();
        m_AudioSourceMusic.PlayOneShot(sound);
    }

    public void ChangeFXVolume(float volume)
    {
        m_FXVolume = volume;
    }

    public void ChangeUIVolume(float volume)
    {
        m_UIVolume = volume;
    }

    public void ChangeMusicVolume(float volume)
    {
        m_MusicVolume = volume;
    }
}
