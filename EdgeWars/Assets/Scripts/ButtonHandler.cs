using System.Collections;
using UnityEngine.SceneManagement;
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

    

    

    public static ButtonHandler instance;
    private void Awake()
    {
       if (instance == null)
        {
            instance = this;
        }
    }



    public void MainMenuClick()   //------------------------------------------------------------------------------------------------
    {
        PlayGround.instance.ClearPlayground();
        UIHandler.instance.gamePanel.SetActive(false);

        if (GameManager.instance.singlePlayerWithoutLogginIn)
        {
            UIHandler.instance.startGamePanel.SetActive(true);
        }
        else
        {
            GameManager.instance.GetCurrentPlayerPlaceInLeaderboard();
            UIHandler.instance.DisplayEnergy();
            UIHandler.instance.DisplayEnergyTimer();
            UIHandler.instance.mainPanel.SetActive(true);
        }


    }

    public void RestartClick()   //------------------------------------------------------------------------------------------------
    {
        if (GameManager.instance.singlePlayerWithoutLogginIn)
        {
            
            UIHandler.instance.gamePanel.SetActive(false);
            PlayGround.instance.ShowHidePlayegroundFieldObjects(false);
            UIHandler.instance.loadingPanelText.text = "If you don`t want to watch ads before each game, please register and log in";
            UIHandler.instance.loadingPanel.SetActive(true);
            AdManager.instance.ShowAd(true, AdManager.Panels.Game);
            //PlayGround.instance.InitializePlayGround();
        }
        else if (EnergyScript.currentEnergy > 0)
        {
            EnergyScript.currentEnergy--;
            GameManager.instance.EnergyDataUpdate(EnergyScript.currentEnergy, (int)EnergyScript.instance.energyTimer, false);
            PlayGround.instance.InitializePlayGround();
        }
        else
        {
            PlayGround.instance.ClearPlayground();
            UIHandler.instance.gamePanel.SetActive(false);
            GameManager.instance.GetCurrentPlayerPlaceInLeaderboard();
            UIHandler.instance.DisplayEnergy();
            UIHandler.instance.DisplayEnergyTimer();
            UIHandler.instance.mainPanel.SetActive(true);
        }
    }

    public void NewGame()  //------------------------------------------------------------------------------------------------
    {

        if (GameManager.instance.singlePlayerWithoutLogginIn)
        {
            //training game
           /* ///TESTING AD

            AdManager.instance.ShowTestBanner();
            
            ///TEST AD*/
            UIHandler.instance.startGamePanel.SetActive(false);
            UIHandler.instance.loadingPanelText.text = "If you don`t want to watch ads before each game, please register and log in";
            UIHandler.instance.loadingPanel.SetActive(true);
            AdManager.instance.ShowAd(true, AdManager.Panels.Start);
        }
        else if (EnergyScript.currentEnergy > 0)
        {
            EnergyScript.currentEnergy--;
            UIHandler.instance.DisplayEnergy();
            GameManager.instance.EnergyDataUpdate(EnergyScript.currentEnergy, (int)EnergyScript.instance.energyTimer, false);
            UIHandler.instance.mainPanel.SetActive(false);
            UIHandler.instance.gamePanel.SetActive(true);
            PlayGround.instance.InitializePlayGround();
        }
        else
        {
            UIHandler.instance.DisplayEnergy();
            StartCoroutine(UIHandler.instance.BlinkEnergyText2sec());
        }
    }

    public void GetEnergy()  //  //------------------------------------------------------------------------------------------------
    {
        //mainPanel.SetActive(false);
        UIHandler.instance.mainPanel.SetActive(false);
        UIHandler.instance.loadingPanelText.text = "If you want to get energy, you should watch this ads";
        UIHandler.instance.loadingPanel.SetActive(true);
        AdManager.instance.ShowAd(false, AdManager.Panels.Main);
    }

    public void ConfirmRegistration()    //------------------------------------------------------------------------------------------------
    {

        if (string.IsNullOrEmpty(inputRegisterNickname.text))
        { 
            registerStatusText.text = "Nickname field is empty";
            registerStatusText.color = Color.red;
        }
        else if (string.IsNullOrEmpty(inputRegisterEmail.text))
        {
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
            registerStatusText.text = "Password to short (must be 6 symbols minimum)";
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

    

    public void CloseErrorPanel()   //------------------------------------------------------------------------------------------------
    {
        if (GameManager.instance.needToSignOutAfterShowingErrorPanel)
        {
            Debug.Log("signing out after close errorpanel");
            GameManager.instance.needToSignOutAfterShowingErrorPanel = false;
            GameManager.instance.UserLogout(false);
        }
        errorStatusText.text = "";
        errorPanel.SetActive(false);
        startGamePanel.SetActive(true);
    }

    public void CancelRegistration()   //------------------------------------------------------------------------------------------------
    {
        //check if Auth workflow is running then abort Auth workflow
        registerGamePanel.SetActive(false);
        startGamePanel.SetActive(true);
    }

    public void GotoRegisterPanel()   //------------------------------------------------------------------------------------------------
    {
        startGamePanel.SetActive(false);
        registerStatusText.text = "";
        registerStatusText.color = Color.white;
        inputRegisterEmail.text = "";
        inputRegisterPassword.text = "";
        inputRegisterPasswordVerify.text =  "";
        registerGamePanel.SetActive(true);
    }

    public void GotologonPanel()   //------------------------------------------------------------------------------------------------
    {
        startGamePanel.SetActive(false);
        logonStatusText.text = "";
        logonStatusText.color = Color.white;
        inputLogonEmail.text = "";
        inputLogonPassword.text = "";
        logonPanel.SetActive(true);
    }

    public void ConfirmLogon()   //------------------------------------------------------------------------------------------------
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


            GameManager.instance.UserSingIn(inputLogonEmail.text.Trim(), inputLogonPassword.text.Trim());
        }
    }

    public void CancelLogon()   //------------------------------------------------------------------------------------------------
    {
            logonPanel.SetActive(false);
            startGamePanel.SetActive(true);
    }

    public void GotoAboutPanel(bool fromMain) //  //------------------------------------------------------------------------------------------------
    {
        GameManager.instance._fromMain = fromMain;
        if (fromMain)   mainPanel.SetActive(false);
        else    startGamePanel.SetActive(false);
        
        aboutPanel.SetActive(true);
    }

    public void BackFromAbout()   //------------------------------------------------------------------------------------------------
    {
        aboutPanel.SetActive(false);

        if (GameManager.instance._fromMain)
        {
            mainPanel.SetActive(true);
        }
        else
        {
            startGamePanel.SetActive(true);
        }
    }

    public void GotoSettingsPanel(bool fromMain)   //------------------------------------------------------------------------------------------------
    {
        GameManager.instance._fromMain = fromMain;
        if (fromMain)
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

    public void BackFromSettings()   //------------------------------------------------------------------------------------------------
    {
        
        settingsPanel.SetActive(false);

        if (GameManager.instance._fromMain)
        {
            mainPanel.SetActive(true);
        }
        else
        {
            startGamePanel.SetActive(true);
        }
        
    }

    public void SoundOnOff()   //------------------------------------------------------------------------------------------------
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

    public void SettingsLogout()   //------------------------------------------------------------------------------------------------
    {
        GameManager.instance.UserLogout(true);
        GameManager.instance._fromMain = false;
        BackFromSettings();
    }

    public void ResendVerificationMail()   //------------------------------------------------------------------------------------------------
    {
        errorStatusText.text = "Sending verification e-mail";
        errorStatusText.color = Color.green;
        errorResenEmailButton.gameObject.SetActive(false);
        GameManager.instance.ResendVerificationMail();
    }

    public void ResetUserPassword()   //------------------------------------------------------------------------------------------------
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
            GameManager.instance.ResetUserPassword(inputLogonEmail.text.Trim());
        }
    }

           public void QuitPanelCloseButtonHadler()   //------------------------------------------------------------------------------------------------
    {
        Application.Quit();
    }

    
}
