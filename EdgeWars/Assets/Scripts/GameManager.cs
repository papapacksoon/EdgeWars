using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System;
using Firebase;
using Firebase.Analytics;

public class GameManager : MonoBehaviourPunCallbacks
{

    private static GameManager instance;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        if (instance == null)
        {
            instance = this;
        }

    }
     
    
    
       
}
