﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.Globalization;


public class EnergyScript : MonoBehaviour
{

    public static EnergyScript instance;

    public Text EnergyLabel;
    public Text nextEnergyText;
    public const int MAXENERGY = 10;
    public static int currentEnergy;


    public const int SECONDSTONEWENERGY = 4320; //1 free energy in 72 minutes
    public float energyTimer;

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
        Application.quitting += Application_quitting;
    }

    private void Application_quitting()
    {
        //update player data on quit
        //if game is on - autoloose
        // 
        GameManager.instance.ApplicationQuitDataUpdate(currentEnergy, (int)energyTimer, GameManager.instance.isGameOver, GameManager.instance.isFirebaseLoaded);  //prod

        //GameManager.instance.ApplicationQuitDataUpdate(currentEnergy, (int)energyTimer, false, GameManager.instance.isFirebaseLoaded); //test
    }

    // Update is called once per frame
    void Update()
    {
        if (currentEnergy < MAXENERGY && !GameManager.instance.singlePlayerWithoutLogginIn)
        {
            energyTimer += Time.deltaTime;
            if ((int)energyTimer > SECONDSTONEWENERGY)
            {
                currentEnergy++;
                if (SceneManager.GetActiveScene().name == "Menu") UIHandler.instance.DisplayEnergy();
                energyTimer = 0;
                GameManager.instance.EnergyDataUpdate(currentEnergy, (int)energyTimer, false);
            }
            if (((int)energyTimer % 60) == 0 && SceneManager.GetActiveScene().name == "Menu") UIHandler.instance.DisplayEnergyTimer();
        }
    }

    public IEnumerator OnLogonEnergyCount(DateTime serverDateTime)
    {
        currentEnergy = PlayerManager.instance.playerEnergy;
        energyTimer = PlayerManager.instance.playerEnergyTimer;

        if (currentEnergy >= MAXENERGY) energyTimer = 0;
        else
        {
            Debug.Log(" server date time =  " + serverDateTime);
            Debug.Log(" logout time is  =  " + PlayerManager.instance.playerLogoutDateTime);

            DateTime newDate = Convert.ToDateTime(PlayerManager.instance.playerLogoutDateTime, DateTimeFormatInfo.InvariantInfo);

            currentEnergy += (int)serverDateTime.Subtract(newDate).TotalSeconds / SECONDSTONEWENERGY; ;
            energyTimer += (int)DateTime.Now.Subtract(newDate).TotalSeconds % SECONDSTONEWENERGY;

            Debug.Log("current energy = " + currentEnergy);
            Debug.Log(" current timer = " + energyTimer);

            if (currentEnergy >= MAXENERGY)
            {
                currentEnergy = MAXENERGY;
                energyTimer = 0;
            }
        }

        UIHandler.instance.DisplayEnergy();
        UIHandler.instance.DisplayEnergyTimer();
        GameManager.instance.taskCounter++;
        yield return null;
    }
}

   
