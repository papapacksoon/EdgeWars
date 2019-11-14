using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ButtonHandler : MonoBehaviour
{

    public GameObject registerGamePanel;
    public GameObject startGamePanel;
    public Text registerStatusText;
    public InputField inputRegisterEmail;
    public InputField inputRegisterPassword;
    public InputField inputRegisterPasswordVerify;
    public InputField inputRegisterNickname;

    public GameObject logonPanel;
    public Text logonStatusText;
    public InputField inputLogonEmail;
    public InputField inputLogonPassword;

    public GameObject aboutPanel;
    public GameObject mainPanel;

    public Button settingsLoginOutButton;
    public Button settingsSoundButton;
    public GameObject settingsPanel;
    public GameObject errorPanel;
    public Text errorStatusText;
    public Button errorResenEmailButton;

    public Button logonConfirmButton;
    public Button logonCloseButton;
    public Button logonForgotPasswrodButton;

    public Button registerConfirmButton;
    public Button registerCloseButton;

    public Text playerRank;

    public GameObject quitPanel;

    private bool _fromMain;

    public static ButtonHandler instance;
    private void Awake()
    {
       if (instance == null)
        {
            instance = this;
        }
    }

    public void UserSignedIn()
    {
        startGamePanel.SetActive(false);
        mainPanel.SetActive(true);
        playerRank.text = GameManager.instance.Auth.CurrentUser.DisplayName;
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
            mainPanel.SetActive(true);
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
            mainPanel.SetActive(true);
        }
    }

    public void NewGame()
    {
        if (GameManager.instance.singlePlayerWithoutLogginIn)
        {
            SceneManager.UnloadSceneAsync("Menu");
            SceneManager.LoadScene("Main", LoadSceneMode.Single);
        }
        else if (EnergyScript.currentEnergy > 0)
        {
            EnergyScript.instance.DisplayEnergy();
            SceneManager.UnloadSceneAsync("Menu");
            SceneManager.LoadScene("Main", LoadSceneMode.Single);
        }
        //else message get more energy; 
    }

    public void GetEnergy()
    {
        AdManager.instance.ShowAd();
        //show Ad and get new energy 
    }

    public void ConfirmRegistration()
    {

        if (string.IsNullOrEmpty(inputRegisterNickname.text))
        { 
            Debug.Log("Nickname empty");
            registerStatusText.text = "Nickname field is empty";
            registerStatusText.color = Color.red;
        }
        else if (string.IsNullOrEmpty(inputRegisterEmail.text))
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
        else if (inputRegisterPassword.text.Length < 6)
        {
            registerStatusText.text = "Password to short";
            registerStatusText.color = Color.red;
        }
        else
        {
            registerStatusText.text = "Trying to register";
            registerStatusText.color = Color.green;
            GameManager.instance.userAutoSignedIn = false;

            //block inputs
            registerConfirmButton.gameObject.SetActive(false);
            registerCloseButton.gameObject.SetActive(false);

            GameManager.instance.CreateNewUser(inputRegisterEmail.text.Trim(), inputRegisterPassword.text.Trim(), inputRegisterNickname.text.Trim());
        }

    }

    public void ShowErrorPanel(string errorText, Color color, bool showResendEmailButton)
    {
        if (showResendEmailButton) errorResenEmailButton.gameObject.SetActive(true);
        else errorResenEmailButton.gameObject.SetActive(false);
        errorStatusText.text = errorText;
        errorStatusText.color = color;
        errorPanel.SetActive(true);

        startGamePanel.SetActive(false);
        registerGamePanel.SetActive(false);
        logonPanel.SetActive(false);
    }

    public IEnumerator ShowErrorPanelIEnumrator(string errorText, Color color, bool showResendEmailButton)
    {
        if (showResendEmailButton) errorResenEmailButton.gameObject.SetActive(true);
        else errorResenEmailButton.gameObject.SetActive(false);
        errorStatusText.text = errorText;
        errorStatusText.color = color;
        errorPanel.SetActive(true);

        startGamePanel.SetActive(false);
        registerGamePanel.SetActive(false);
        logonPanel.SetActive(false);

        yield return null;

    }

    public void CloseErrorPanel()
    {
        errorStatusText.text = "";
        errorPanel.SetActive(false);

        startGamePanel.SetActive(true);
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
            logonStatusText.text = "Trying to Login";
            logonStatusText.color = Color.green;
            GameManager.instance.userAutoSignedIn = false;

            //block inputs
            logonCloseButton.gameObject.SetActive(false);
            logonConfirmButton.gameObject.SetActive(false);
            logonForgotPasswrodButton.gameObject.SetActive(false);


            GameManager.instance.UserSingIn(inputLogonEmail.text, inputLogonPassword.text);
        }
    }

    public void CancelLogon()
    {
            logonPanel.SetActive(false);
            startGamePanel.SetActive(true);
    }

    public void GotoAboutPanel(bool fromMain)
    {
        _fromMain = fromMain;
        startGamePanel.SetActive(false);
        aboutPanel.SetActive(true);
    }

    public void BackFromAbout()
    {
        aboutPanel.SetActive(false);

        if (_fromMain)
        {
            mainPanel.SetActive(true);
        }
        else
        {
            startGamePanel.SetActive(true);
        }
        
    }

    public void GotoSettingsPanel(bool fromMain)
    {
        _fromMain = fromMain;
        if (_fromMain)
            mainPanel.SetActive(false);
        else
            startGamePanel.SetActive(false);

        settingsPanel.SetActive(true);
        

        if (AudioManager.instance.IsSoundOn)
        {
         
            settingsSoundButton.GetComponentInChildren<Text>().text = "Sound is on";
        }
        else
        {
         
            settingsSoundButton.GetComponentInChildren<Text>().text = "Sound is off";
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

        if (_fromMain)
        {
            mainPanel.SetActive(true);
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
            settingsSoundButton.GetComponentInChildren<Text>().text = "Sound is off";
        }
        else
        {
            AudioManager.instance.IsSoundOn = true;
            settingsSoundButton.GetComponentInChildren<Text>().text = "Sound is on";
        }
        
    }

    public void SettingsLogout()
    {
        GameManager.instance.UserLogout();
        _fromMain = false;
        BackFromSettings();

    }

    public void ResendVerificationMail()
    {
        errorStatusText.text = "Sending verification e-mail";
        errorStatusText.color = Color.green;
        errorResenEmailButton.gameObject.SetActive(false);
        GameManager.instance.ResendVerificationMail();
    }

    public void ResetUserPassword()
    {
        if (string.IsNullOrEmpty(inputLogonEmail.text))
        {
            logonStatusText.text = "Email field is empty";
            logonStatusText.color = Color.red;
        }
        else
        {
            logonCloseButton.gameObject.SetActive(false);
            logonConfirmButton.gameObject.SetActive(false);
            logonForgotPasswrodButton.gameObject.SetActive(false);
            logonStatusText.text = "Sending reset password e-mail";
            logonStatusText.color = Color.green;
            GameManager.instance.ResetUserPassword();
        }
    }

    public IEnumerator UpdateLogonStatus(string text, Color color)
    {
        logonStatusText.text = text;
        logonStatusText.color = color;
        //release imputs
        logonCloseButton.gameObject.SetActive(true);
        logonConfirmButton.gameObject.SetActive(true);
        logonForgotPasswrodButton.gameObject.SetActive(true);

        yield return null;
    }

    public IEnumerator UserSingInCleanUp()
    {
        logonCloseButton.gameObject.SetActive(true);
        logonConfirmButton.gameObject.SetActive(true);
        logonForgotPasswrodButton.gameObject.SetActive(true);

        yield return null;
    }

    public IEnumerator UserRegisterCleanUp()
    {
        registerConfirmButton.gameObject.SetActive(true);
        registerCloseButton.gameObject.SetActive(true);

        yield return null;
    }

    public IEnumerator UpdateVirificationEmailSendingStatus(string text, Color color, bool showResendBtton)
    {
        errorStatusText.text = text;
        errorStatusText.color = color;

        if (showResendBtton) errorResenEmailButton.gameObject.SetActive(true);
        else errorResenEmailButton.gameObject.SetActive(false);

        yield return null;
    }

    public void ShowQuitPanel()
    {
        mainPanel.SetActive(false);
        startGamePanel.SetActive(false);
        registerGamePanel.SetActive(false);
        logonPanel.SetActive(false);
        settingsPanel.SetActive(false);
        aboutPanel.SetActive(false);
        errorPanel.SetActive(false);
        quitPanel.SetActive(true);
    }

    public void QuitPanelCloseButtonHadler()
    {
        Application.Quit();
    }

    
}
