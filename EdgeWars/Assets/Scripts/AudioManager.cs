using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public static AudioManager instance;

    private bool isSoundOn = true;

    public bool IsSoundOn
    {
        get {
            return isSoundOn;
        }

        set {

            if (value) PlayerPrefs.SetInt("Sound", 1);
            else PlayerPrefs.SetInt("Sound", 0);
            PlayerPrefs.Save();

            isSoundOn = value;

        }

    }

    private void Awake()
    {
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
            if (PlayerPrefs.GetInt("Sound") == 1) IsSoundOn = true;
            else IsSoundOn = false;
            
        }
        else
        {
            IsSoundOn = true;
            PlayerPrefs.SetInt("Sound", 1);
            PlayerPrefs.Save();
        }
    }

}
