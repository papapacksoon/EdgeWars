using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonHandler : MonoBehaviour
{
    // Start is called before the first frame update

    private GameObject EnergyText;
    private EnergyScript energyScript;
    private const int SECONDSTONEWENERGY = 8640;

      
    private Vector3 panelScale;


    void Start()
    {
        EnergyText = GameObject.Find("Energy");
        energyScript = EnergyText.GetComponent<EnergyScript>();
     
        
    }

    public void MainMenuClick()
    {
        SceneManager.UnloadSceneAsync("Main");
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }

    public void RestartClick()
    {
        if (EnergyScript.currentEnergy > 0)
        {
       //     EnergyScript.currentEnergy--;               ---------------------------------------------
            energyScript.DisplayEnergy();
            SceneManager.UnloadSceneAsync("Main");
            SceneManager.LoadScene("Main", LoadSceneMode.Single);
        }
        //message get more energy;
    }

    public void NewGame()
    {
        if (EnergyScript.currentEnergy > 0)
        {
           // EnergyScript.currentEnergy--;                         ------------------------------------
            energyScript.DisplayEnergy();
            SceneManager.UnloadSceneAsync("Menu");
            SceneManager.LoadScene("Main", LoadSceneMode.Single);
        }
        //message get more energy;
    }

    public void GetEnergy()
    {
    
        
    }

    public void CloseMessage()
    {
      

    }
   
       

       
  


}
