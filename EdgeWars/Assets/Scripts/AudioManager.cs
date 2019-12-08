using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public static AudioManager instance;

    public AudioSource m_MyAudioSource;

    private bool isSoundOn = true;

    public bool IsSoundOn
    {
        get {
            return isSoundOn;
        }

        set {

            if (value)
            {
                m_MyAudioSource.Play();
                PlayerPrefs.SetInt("Sound", 1);
            }
            else
            {
                m_MyAudioSource.Stop();
                PlayerPrefs.SetInt("Sound", 0);
            }
            PlayerPrefs.Save();

            isSoundOn = value;

        }

    }

    private void Awake()
    {
        m_MyAudioSource = GetComponent<AudioSource>();
        DontDestroyOnLoad(this.gameObject);

        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("Sound"))
        {
            if (PlayerPrefs.GetInt("Sound") == 1)
            {
                m_MyAudioSource.Play();
                IsSoundOn = true;
            }
            else
            {
                m_MyAudioSource.Stop();
                IsSoundOn = false;
            }
            
        }
        else
        {
            m_MyAudioSource.Play();
            IsSoundOn = true;
            PlayerPrefs.SetInt("Sound", 1);
            PlayerPrefs.Save();
        }
    }

}
