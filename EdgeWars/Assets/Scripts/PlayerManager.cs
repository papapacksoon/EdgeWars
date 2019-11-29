using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Firebase.Auth;
using System.Globalization;

public class PlayerManager : MonoBehaviour
{
    public string playerName;
    public int playerRank;
    public int playerEnergy;
    public int playerEnergyTimer;
    public string playerLogoutDateTime = DateTime.Now.ToString("g",DateTimeFormatInfo.InvariantInfo);

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
