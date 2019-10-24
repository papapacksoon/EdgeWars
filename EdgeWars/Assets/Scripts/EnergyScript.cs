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
    private const int MAXENERGY = 10;
    public static int currentEnergy;
    

    private const int SECONDSTONEWENERGY = 8640;
    private float energyTimer;

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
        SceneManager.sceneUnloaded += SceneManager_sceneUnloaded;
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

        DisplayEnergy();
        if (currentEnergy < MAXENERGY) DisplayEnergyTimer();
    }

    private void Application_quitting()
    {
        //  Debug.Log("Quitting");

        PlayerPrefs.SetInt("Energy", currentEnergy);
        PlayerPrefs.SetFloat("EnergyTimer", energyTimer);
        PlayerPrefs.SetString("DateTime", System.DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
        PlayerPrefs.Save();
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
                DisplayEnergy();
                energyTimer = 0;
            }
            //display energy text
            if (((int)energyTimer % 60) == 0) DisplayEnergyTimer();
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

    public void DisplayEnergy()
    {
        EnergyLabel.text = "ENERGY " + currentEnergy + "/10";
        if (currentEnergy == 0) EnergyLabel.color = Color.red;
        else EnergyLabel.color = new Color(0, 255, 244);

    }

    public void DisplayEnergyTimer()
    {
        int currentSecondsToNewEnergy = SECONDSTONEWENERGY - (int)energyTimer;
        int hoursToNewEnergy = currentSecondsToNewEnergy / 3600;
        int minutesToNewEnergy = (currentSecondsToNewEnergy % 3600) / 60;
        nextEnergyText.text = "You get energy in " + hoursToNewEnergy + " hours " + minutesToNewEnergy + " minutes";
    }
}

   
