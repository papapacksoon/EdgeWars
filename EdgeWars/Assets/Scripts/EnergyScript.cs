using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;


public class EnergyScript : MonoBehaviour
{

    public static EnergyScript instance;

    public Text EnergyLabel;
    public Text nextEnergyText;
    public const int MAXENERGY = 10;
    public static int currentEnergy;


    public const int SECONDSTONEWENERGY = 8640;
    public float energyTimer;

    private void Awake()
    {
        //DontDestroyOnLoad(this.gameObject);

        if (instance == null)
        {
            instance = this;
        }
    }
    private void Start()
    {
        Application.quitting += Application_quitting;

        //count how much energy restored
        if (PlayerPrefs.HasKey("Energy") == true)
        {

            currentEnergy = PlayerPrefs.GetInt("Energy");

            if (PlayerPrefs.HasKey("EnergyTimer") == true)
            {
                energyTimer = PlayerPrefs.GetFloat("EnergyTimer");

                if (currentEnergy == MAXENERGY) energyTimer = 0;

            }
            else energyTimer = 0;

            if (currentEnergy < MAXENERGY)
            {
                if (PlayerPrefs.HasKey("DateTime"))
                {

                    currentEnergy += (int)(System.DateTime.Now.Subtract(Convert.ToDateTime(PlayerPrefs.GetString("DateTime")))).TotalSeconds / SECONDSTONEWENERGY; ;
                    energyTimer += (int)(System.DateTime.Now.Subtract(Convert.ToDateTime(PlayerPrefs.GetString("DateTime")))).TotalSeconds % SECONDSTONEWENERGY;
                    if (energyTimer > SECONDSTONEWENERGY)
                    {
                        currentEnergy++;
                        energyTimer -= SECONDSTONEWENERGY;
                    }
                    
                    if (currentEnergy > MAXENERGY)
                    {
                        currentEnergy = MAXENERGY;
                        energyTimer = 0;
                    }
                }
            }
        }
        else currentEnergy = MAXENERGY;

        UIHandler.instance.DisplayEnergy();
        if (currentEnergy < MAXENERGY) UIHandler.instance.DisplayEnergyTimer();
    }

    private void Application_quitting()
    {
       //SaveEnergyTimer to UserData
    }

    // Update is called once per frame
    void Update()
    {
        //display current energy

        if (currentEnergy < MAXENERGY)
        {
            energyTimer += Time.deltaTime;
            if ((int)energyTimer > SECONDSTONEWENERGY)
            {
                currentEnergy++;
                UIHandler.instance.DisplayEnergy();
                energyTimer = 0;
            }
            //display energy text
            if (((int)energyTimer % 60) == 0) UIHandler.instance.DisplayEnergyTimer();
        }

    }

    private void SceneManager_sceneUnloaded(Scene arg0)
    {
        //save data (ENERGY)
        //  Debug.Log("Scene Unloaded " + arg0.name);
        if (arg0.name == "Menu")
        {
            PlayerPrefs.SetInt("Energy", currentEnergy);
            PlayerPrefs.SetFloat("EnergyTimer", energyTimer);
            PlayerPrefs.SetString("DateTime", System.DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
            PlayerPrefs.Save();
        }

        //last date time
    }

 

    public IEnumerator OnLogonEnergyCount(DateTime serverDateTime)
    {
        //-----------------------
        //we have retrieved in PlayerManager instance
        //serverDateTime hold a server date time 
        //need to count currentEnergy
        //currentEnergyTimer
        //And add (substract) user offline time 
        Debug.Log("EnergyUpdated");

        yield return null;
    }
}

   
