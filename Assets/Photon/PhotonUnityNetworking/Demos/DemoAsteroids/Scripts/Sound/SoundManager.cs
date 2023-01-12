using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    // sound sfx
    public static int CLICK_SFX = 0;
    public static int CLOSE_SFX = CLICK_SFX + 1;
    public static int OPEN_SFX = CLOSE_SFX + 1;
    public static int SHOOT_SFX = OPEN_SFX + 1;
    public static int ITEM_SFX = SHOOT_SFX + 1;
    public static int BULLET_HIT_SFX = ITEM_SFX + 1;
    public static int NITRO_BOOST_SFX = BULLET_HIT_SFX + 1;
    public static int ITEM_SELECTED_SFX = NITRO_BOOST_SFX + 1;

    // sound music
    public static int MENUBG_MUSIC = 0;
    public static int LEVEL_01_MUSIC = MENUBG_MUSIC + 1;

    public string SAVE_SOUND_SFX = "SAVE_SOUND_SFX";
    public string SAVE_SOUND_MUSIC = "SAVE_SOUND_MUSIC";


    public AudioClip[] listSoundSFX;
    
    [SerializeField]
    private int m_startSourceNumber = 5;
    private List<AudioSource> m_sources = new List<AudioSource>();


    public AudioClip[] listSoundMusic;
    private AudioSource audioMusic;

    public int currentSoundMusic;
    public bool isSoundSfx;
    public bool isSoundMusic;
    [Tooltip("Maximum number of ShootingSoundMaxVolume")]
    [Range(0.1f, 1.0f)]public float ShootingSoundMaxVolume = 1.0f;
        

    void Start()
    {
        audioMusic = gameObject.AddComponent<AudioSource>() as AudioSource;
        ShootingSoundMaxVolume = 0.3f;
        if (PlayerPrefs.HasKey(SAVE_SOUND_SFX))
        {
            isSoundSfx = PlayerPrefs.GetInt(SAVE_SOUND_SFX) == 1 ? true : false;
        }
        else    
        {
            isSoundSfx = true;
        }

        if (PlayerPrefs.HasKey(SAVE_SOUND_MUSIC))
        {
            isSoundMusic = PlayerPrefs.GetInt(SAVE_SOUND_MUSIC) == 1 ? true : false;
        }
        else
        {
            isSoundMusic = true;
        }

        AddAudioSource(m_startSourceNumber - m_sources.Count);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlaySFX(int index)
    {
        if(isSoundSfx)
        {
            AudioSource src = GetAudioSourceFree();
            if(index == SHOOT_SFX){
                src.volume = ShootingSoundMaxVolume;
            }
            src.PlayOneShot(listSoundSFX[index]);

        }
    }

    public void PlaySFX(AudioClip aClip)
    {
        if (isSoundSfx)
        {
            if (aClip)
            {
                AudioSource src = GetAudioSourceFree();
                src.PlayOneShot(aClip);
            }
        }
    }

    public void PlayMusic(int index, bool loop)
    {
        if(!isPlayingSoundMusic() && isSoundMusic)
        {
            audioMusic.loop = loop;
            audioMusic.clip = listSoundMusic[index];
            audioMusic.Play();
            currentSoundMusic = index;
        }
    }

    public bool isPlayingSoundMusic()
    {
        return audioMusic.isPlaying;
    }


    public void StopSoundMusic()
    {
        audioMusic.Stop();
    }


    private void AddAudioSource(int count = 1)
    {
        for (int i = 0; i < count; i++)
            m_sources.Add(gameObject.AddComponent<AudioSource>());
    }

    private AudioSource GetAudioSourceFree()
    {
        foreach(AudioSource src in m_sources)
        {
            if (!src.isPlaying)
                return src;
        }

        AddAudioSource();
        return m_sources[m_sources.Count - 1];
    }


}
