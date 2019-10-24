using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonHandler : MonoBehaviour
{

    public GameObject registerGamePanel;
    public GameObject startGamePanel;
    public Text registerStatusText;
    public InputField inputRegisterEmail;
    public InputField inputRegisterPassword;
    public InputField inputRegisterPasswordVerify;


    void Start()
    {
                    
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
            EnergyScript.instance.DisplayEnergy();
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
            EnergyScript.instance.DisplayEnergy();
            SceneManager.UnloadSceneAsync("Menu");
            SceneManager.LoadScene("Main", LoadSceneMode.Single);
        }
        //message get more energy;
    }

    public void GetEnergy()
    {
        AdManager.instance.ShowAd();
    }

    public void ConfirmRegistration()
    {
        

        if (string.IsNullOrEmpty(inputRegisterEmail.text))
        {
            Debug.Log("Email empty");
            registerStatusText.text = "Email field is empty";
            registerStatusText.color = Color.red;
        }
        else if (string.IsNullOrEmpty(inputRegisterPassword.text))
        {
            registerStatusText.text = "Password field is empty";
            registerStatusText.color = Color.red;
        }
        else if (string.IsNullOrEmpty(inputRegisterPasswordVerify.text))
        {
            registerStatusText.text = "Passwords dont match";
            registerStatusText.color = Color.red;
        }
        else
        {
            registerStatusText.text = "Try to register";
            registerStatusText.color = Color.green;
        }

    }

    public void CancelRegistration()
    {
        //check if Auth workflow is running then abort Auth workflow
        registerGamePanel.SetActive(false);
        startGamePanel.SetActive(true);
        
    }

    public void GotoRegisterPanel()
    {
        startGamePanel.SetActive(false);
        registerStatusText.text = "";
        registerStatusText.color = Color.white;
        inputRegisterEmail.text = "";
        inputRegisterPassword.text = "";
        inputRegisterPasswordVerify.text =  "";
        registerGamePanel.SetActive(true);
    }


   
       

       
  


}
