﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    public GameObject mainPanel;
    public Text playerRank;

    public GameObject startGamePanel;

    public GameObject errorPanel;
    public Text errorStatusText;
    public Button errorResenEmailButton;


    public GameObject registerGamePanel;
    public Button registerConfirmButton;
    public Button registerCloseButton;
    public InputField registerNickname;
    public InputField registerEmail;
    public InputField registerPassword;
    public InputField registerPasswordVerify;

    public GameObject logonPanel;
    public Text logonStatusText;
    public Button logonCloseButton;
    public Button logonConfirmButton;
    public Button logonForgotPasswrodButton;

    public GameObject settingsPanel;
    public GameObject aboutPanel;
    public GameObject quitPanel;

    

    public Text EnergyLabel;
    public Text nextEnergyText;

    public GameObject gamePanel;

    public GameObject loadingPanel;
    public Text loadingPanelText;


    public static UIHandler instance;
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        if (instance == null)
        {
            instance = this;
        }
    }

    public void UserSignedInShowMainPanel()
    {
        startGamePanel.SetActive(false);
        logonPanel.SetActive(false);
        mainPanel.SetActive(true);
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
        logonStatusText.text = "";
        logonCloseButton.gameObject.SetActive(true);
        logonConfirmButton.gameObject.SetActive(true);
        logonForgotPasswrodButton.gameObject.SetActive(true);

        yield return null;
    }

    public IEnumerator UserRegisterCleanUp()
    {
        registerNickname.text = "";
        registerEmail.text = "";
        registerPassword.text = "";
        registerPasswordVerify.text = "";

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

    public IEnumerator UpdatePlayerRankUI(int currentPlace, int leaderboardPlayersCount)
    {
        Debug.Log(" Update player rank" + PlayerManager.instance.playerName + " rating is " + currentPlace);
        playerRank.text = PlayerManager.instance.playerName + " points : " + PlayerManager.instance.playerRank  + "\n\r place " + currentPlace + " of " + leaderboardPlayersCount ;
        GameManager.instance.taskCounter++;
        yield return null;
    }

    public void DisplayEnergy()
    {

        EnergyLabel.text = "ENERGY " + EnergyScript.currentEnergy + "/10";
        if (EnergyScript.currentEnergy == 0) EnergyLabel.color = Color.red;
        else EnergyLabel.color = new Color(0, 255, 244);

    }

    public void DisplayEnergyTimer()
    {
        int currentSecondsToNewEnergy = EnergyScript.SECONDSTONEWENERGY - (int)EnergyScript.instance.energyTimer;
        int hoursToNewEnergy = currentSecondsToNewEnergy / 3600;
        int minutesToNewEnergy = (currentSecondsToNewEnergy % 3600) / 60;
        if (EnergyScript.currentEnergy < EnergyScript.MAXENERGY)
            nextEnergyText.text = "You get energy in " + hoursToNewEnergy + " hours " + minutesToNewEnergy + " minutes";
        else
            nextEnergyText.text = "";
    }

    public IEnumerator UserSignedInShowMainPanelIEnumerator()
    {
        startGamePanel.SetActive(false);
        logonPanel.SetActive(false);
        mainPanel.SetActive(true);

        yield return null;
    }
 
    public IEnumerator BlinkEnergyText2sec()
    {
        EnergyLabel.color = Color.white;
        yield return new WaitForSeconds(0.15f);
        EnergyLabel.color = Color.red;
        yield return new WaitForSeconds(0.15f);
        EnergyLabel.color = Color.white;
        yield return new WaitForSeconds(0.15f);
        EnergyLabel.color = Color.red;
        yield return new WaitForSeconds(0.15f);
        EnergyLabel.color = Color.white;
        yield return new WaitForSeconds(0.15f);
        EnergyLabel.color = Color.red;
        yield return new WaitForSeconds(0.15f);
    }

    public void HideLoadingPanel()
    {
        loadingPanel.SetActive(false);
        GameManager.instance.gameIsLoading = false;
    }


}
