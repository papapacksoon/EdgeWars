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

    public GameObject logonPanel;
    public Text logonStatusText;
    public InputField inputLogonEmail;
    public InputField inputLogonPassword;

    public GameObject aboutPanel;

    

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
        if (GameManager.instance.singlePlayerWithoutLogginIn)
        {
            //showadd
            SceneManager.UnloadSceneAsync("Main");
            SceneManager.LoadScene("Main", LoadSceneMode.Single);
        }
        else if (EnergyScript.currentEnergy > 0)
        {
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
        else if (inputRegisterPasswordVerify.text != inputRegisterPassword.text)
        {
            registerStatusText.text = "Passwords dont match";
            registerStatusText.color = Color.red;
        }
        else
        {
            registerStatusText.text = "Try to register";
            registerStatusText.color = Color.green;
            //Start Auth registration
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

    public void GotologonPanel()
    {
        startGamePanel.SetActive(false);
        logonStatusText.text = "";
        logonStatusText.color = Color.white;
        inputLogonEmail.text = "";
        inputLogonPassword.text = "";
        logonPanel.SetActive(true);
    }

    public void ConfirmLogon()
    {
        if (string.IsNullOrEmpty(inputLogonEmail.text))
        {
            Debug.Log("Email empty");
            logonStatusText.text = "Email field is empty";
            logonStatusText.color = Color.red;
        }
        else if (string.IsNullOrEmpty(inputLogonPassword.text))
        {
            logonStatusText.text = "Password field is empty";
            logonStatusText.color = Color.red;
        }
        else
        {
            logonStatusText.text = "Try to Login";
            logonStatusText.color = Color.green;
            //Start Auth logon
        }
    }

    public void CancelLogon()
    {
        //logonPanel if Auth workflow is running then abort Auth workflow
        logonPanel.SetActive(false);
        startGamePanel.SetActive(true);
    }

    public void GotoAboutPanel()
    {
        startGamePanel.SetActive(false);
        aboutPanel.SetActive(true);
    }

    public void backFromAbout()
    {
        aboutPanel.SetActive(false);
        //check from we came and back to it
    }






}
