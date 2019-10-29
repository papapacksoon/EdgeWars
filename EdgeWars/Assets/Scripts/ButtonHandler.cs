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
    public GameObject MainPanel;

    public Button settingsLoginOutButton;
    public Button settingsSoundButton;
    public GameObject settingsPanel;

    private bool fromMain = false;

    void Start()
    {
                    
    }

    public void MainMenuClick()
    {
        SceneManager.UnloadSceneAsync("Main");
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);

        if (GameManager.instance.singlePlayerWithoutLogginIn)
        {
            //showadd
            startGamePanel.SetActive(true);
        }
        else
        {
            MainPanel.SetActive(true);
        }
    }

    public void RestartClick()
    {
        if (GameManager.instance.singlePlayerWithoutLogginIn)
        {
            
            SceneManager.UnloadSceneAsync("Main");
            SceneManager.LoadScene("Menu", LoadSceneMode.Single);
            //showadd
            startGamePanel.SetActive(true);
        }
        else if (EnergyScript.currentEnergy > 0)
        {
            EnergyScript.instance.DisplayEnergy();
            SceneManager.UnloadSceneAsync("Main");
            SceneManager.LoadScene("Main", LoadSceneMode.Single);
                        
        }
        else
        {
            SceneManager.UnloadSceneAsync("Main");
            SceneManager.LoadScene("Menu", LoadSceneMode.Single);
            MainPanel.SetActive(true);
        }
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

    public void BackFromAbout()
    {
        aboutPanel.SetActive(false);

        if (fromMain)
        {
            MainPanel.SetActive(true);
        }
        else
        {
            startGamePanel.SetActive(true);
        }
        
    }

    public void GotoSettingsPanelFromStart()
    {
        startGamePanel.SetActive(false);
        settingsPanel.SetActive(true);

        if (AudioManager.instance.IsSoundOn)
        {
            settingsSoundButton.GetComponent<Text>().text = "Sound is on";
        }
        else
        {
            settingsSoundButton.GetComponent<Text>().text = "Sound is off";
        }

        if (GameManager.instance.singlePlayerWithoutLogginIn)
        {
            settingsLoginOutButton.gameObject.SetActive(false);
        }
        else
        {
            settingsLoginOutButton.gameObject.SetActive(true);
        }
        
    }

    public void BackFromSettings()
    {
        
        settingsPanel.SetActive(false);

        if (fromMain)
        {
            MainPanel.SetActive(true);
        }
        else
        {
            startGamePanel.SetActive(true);
        }
        
    }

    public void SoundOnOff()
    {
        if (AudioManager.instance.IsSoundOn)
        {
            AudioManager.instance.IsSoundOn = false;
            PlayerPrefs.SetInt("Sound", 0);
            PlayerPrefs.Save();
            settingsSoundButton.GetComponent<Text>().text = "Sound is off";
        }
        else
        {
            AudioManager.instance.IsSoundOn = true;
            PlayerPrefs.SetInt("Sound", 1);
            PlayerPrefs.Save();
            settingsSoundButton.GetComponent<Text>().text = "Sound is on";
        }
        
    }




}
