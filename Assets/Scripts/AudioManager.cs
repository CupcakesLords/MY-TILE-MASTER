using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioClip destroy;
    
    private AudioSource Sound;
    private AudioSource Des;

    bool Effect = true;

    void Start()
    {
        instance = GetComponent<AudioManager>();
        Sound = GetComponent<AudioSource>();
        Sound.Play(0);

        GameObject temp = new GameObject("Sound");
        Des = temp.AddComponent<AudioSource>();
    }

    public void PlayDestroySound()
    {
        if (Effect)
        {
            Des.PlayOneShot(destroy);
        }
    }

    public void ChangeMusic()
    {
        if(Sound.isPlaying)
        {
            Sound.Pause();
        }
        else
        {
            Sound.UnPause();
        }
    }

    public void ChangeSound()
    {
        Effect = !Effect;
    }
}
