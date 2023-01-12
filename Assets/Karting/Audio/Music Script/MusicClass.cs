using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicClass : MonoBehaviour
{
    public static MusicClass Instance;
    //private AudioSource _audioSource;
    public Sprite[] ButtonImg;
    public GameObject bgMusic;
    public AudioClip[] ListMusic;
    private void Awake()
    {
        //DontDestroyOnLoad(transform.gameObject);
        //_audioSource = bgMusic.GetComponent<AudioSource>();
        if(Instance == null)
        {
            Instance = GetComponent<MusicClass>();
        }
    }
    public void PlayMusic()
    {
        //if (_audioSource.isPlaying) return;
        //_audioSource.Play();
        if(bgMusic != null)
        {
            bgMusic.gameObject.SetActive(true);
        }
        
    }
    public void StopMusic()
    {
        //_audioSource.Stop();
        bgMusic.gameObject.SetActive(false);
    }

    public void RandomMusic()
    {
        if (bgMusic.activeSelf)
        {
            bgMusic.GetComponent<AudioSource>().clip = ListMusic[Random.Range(0, ListMusic.Length)] as AudioClip;
            bgMusic.gameObject.SetActive(false);
            if (!bgMusic.activeSelf)
            {
                bgMusic.gameObject.SetActive(true);
            }
        }
        

    }
}
