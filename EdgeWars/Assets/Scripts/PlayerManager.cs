using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Firebase.Auth;



public class PlayerManager : MonoBehaviour
{
    public string playerName;
    public int playerRank;
    

    public static PlayerManager instance;
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        if (instance == null)
        {
            instance = this;
        }
    }


}
