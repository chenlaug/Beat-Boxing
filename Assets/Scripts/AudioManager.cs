using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;

public class AudioManager : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioMixer m_audioMixer;
    [SerializeField] private AudioClip clickSound;
    [SerializeField] private AudioSource[] audioSources;

    public void SetVolumeMaster(float volume)
    {
        Debug.Log(volume);
        m_audioMixer.SetFloat("MasterVolume", volume);
    }

    public void SetVolumeMusic(float volume)
    {
        Debug.Log(volume);
        m_audioMixer.SetFloat("MusicVolume", volume);
    }

    public void SetVolumeEffect(float volume)
    {
        Debug.Log(volume);
        m_audioMixer.SetFloat("EffectVolume", volume);
    }
    public void PlayClickSound()
    {
        if(audioSources.Length > 1)
        {
            audioSources[1].PlayOneShot(clickSound);
        }
        else
        {
            Debug.LogWarning("Audio source not found");
        }
    }
}
