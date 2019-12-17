using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds;
using GoogleMobileAds.Api;
using System;
using UnityEngine.UI;

public class AdManager : MonoBehaviour
{


    public static AdManager instance;

    private bool loadGameAfterAD = false;

    private bool testMode = false;
    private bool testOnDevice = false;
    private bool RewardsAdded = false;

    private int tryCounter = 0;

    public enum Panels
    {
        Start,
        Game,
        Main
    }

    Panels returnPanel;

#if UNITY_ANDROID
    string appId = "ca-app-pub-9898645005884031~3330841357";
#elif UNITY_IPHONE
    string appId = "ca-app-pub-9898645005884031~8778609150";
#else
    string appId = "unexpected_platdorm";
#endif


#if UNITY_ANDROID
    string adUnitId = "ca-app-pub-9898645005884031/1634616301";
#elif UNITY_IPHONE
    string adUnitId = "ca-app-pub-9898645005884031/7714591886";
#else
    string adUnitId = "unexpected_platform";
#endif

    private RewardBasedVideoAd rewardBasedVideoAd;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        if (instance == null)
        {
            instance = this;
        }

        Debug.Log("appId = " + appId);
        Debug.Log("adUnitId = " + adUnitId);
    }

    void Start()
    {
        MobileAds.Initialize(appId);

        rewardBasedVideoAd = RewardBasedVideoAd.Instance;
        rewardBasedVideoAd.OnAdClosed += HandleOnAdClosed;
        rewardBasedVideoAd.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        rewardBasedVideoAd.OnAdStarted += HandleOnAdStarted;
        rewardBasedVideoAd.OnAdRewarded += HandleOnAdRewarded;
        rewardBasedVideoAd.OnAdLoaded += HandleOnAdLoaded;
        rewardBasedVideoAd.OnAdLeavingApplication += HandleOnAdLeavingApp;

    }

    public void ShowAd(bool loadGame, Panels panel)
    {
        tryCounter = 0;
        returnPanel = panel;
        loadGameAfterAD = loadGame;
        RewardsAdded = false;

        LoadRewardBasedAd();
    }


    private void LoadRewardBasedAd()
    {
        Debug.Log("Loading ad");
        rewardBasedVideoAd.LoadAd(new AdRequest.Builder().Build(), adUnitId);
    }

    private void HandleOnAdLoaded(object sender, EventArgs e)
    {
        Debug.Log("Ad Loaded");

        if (AudioManager.instance.IsSoundOn) AudioManager.instance.m_MyAudioSource.volume = 0f;

        UIHandler.instance.loadingPanelText.color = new Color(255, 255, 255, 0f);
        UIHandler.instance.loadingPanelButton.gameObject.SetActive(true);
    }


    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        tryCounter++;
        if (tryCounter > 5)
        {
            Debug.Log("Counter is above 5");
            StartCoroutine(LoadingScipt.instance.ShowFailedToLoadAdMessage(returnPanel));
            return;
        }
        //try to reload
        Debug.Log("Failed to load Ad");
        Debug.Log("try to load again");

        rewardBasedVideoAd.LoadAd(new AdRequest.Builder().Build(), adUnitId);
    }

    public void HandleOnAdRewarded(object sender, Reward args)
    {
        RewardsAdded = true;
        //reward user
        if (testMode)
        {
            Debug.Log("Test Mode rewards added");
        }
        else
        {
            Debug.Log("Ad Rewarded");
        }

    }

    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        if (AudioManager.instance.IsSoundOn) AudioManager.instance.m_MyAudioSource.volume = 1.0f;

            Debug.Log("AdClosed");
            Debug.Log("Rewards = " + RewardsAdded);

            if (RewardsAdded)
            {
                if (loadGameAfterAD)
                {
                    StartGame();
                }
                else
                {
                    if (EnergyScript.currentEnergy < EnergyScript.MAXENERGY) EnergyScript.currentEnergy++;
                }
            }
            else
            {
                switch (returnPanel)
                {
                    case Panels.Game:
                        PlayGround.instance.ShowHidePlayegroundFieldObjects(true);
                        UIHandler.instance.gamePanel.SetActive(true);
                        break;

                    case Panels.Main:
                        UIHandler.instance.mainPanel.SetActive(true);
                        break;

                    case Panels.Start:
                        UIHandler.instance.startGamePanel.SetActive(true);
                        break;
                }

                Debug.Log("Rewards not added");
            }
    }

    public void StartGame()
    {
        UIHandler.instance.startGamePanel.SetActive(false);
        UIHandler.instance.startGamePanel.SetActive(false);
        UIHandler.instance.gamePanel.SetActive(true);
        PlayGround.instance.InitializePlayGround();
    }
    public void HandleOnAdStarted(object sender, EventArgs args)
    {
        Debug.Log("AdStarted");
    }

    public void HandleOnAdLeavingApp(object sender, EventArgs args)
    {
        if (AudioManager.instance.IsSoundOn) AudioManager.instance.m_MyAudioSource.volume = 1.0f;
    }

    public void ButtonSplashShow()
    {
        UIHandler.instance.loadingPanelText.color = new Color(255, 255, 255, 1f);
        UIHandler.instance.loadingPanelButton.gameObject.SetActive(false);
        UIHandler.instance.loadingPanel.SetActive(false);
        rewardBasedVideoAd.Show();
    }
    

}
