using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds;
using GoogleMobileAds.Api;
using System;

public class AdManager : MonoBehaviour
{

    public static AdManager instance;

    private bool loadGameAfterAD = false;

    private bool testMode = true;
    private bool testOnDevice = false;
    private bool RewardsAdded = false;
    public bool ShowAdvertiseBeforeGame = false;
    public int ShowAdvertiseBeforeGameCounter = 0;

    private int tryCounter = 0;

    private BannerView bannerView;

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
        string adBannerUnitId = "ca-app-pub-9898645005884031/4608164317";

        if (testMode) 
        {
            appId = "ca-app-pub-3940256099942544~3347511713";
            adUnitId = "ca-app-pub-3940256099942544/5224354917";
            adBannerUnitId = "ca-app-pub-3940256099942544/6300978111";
        }


        MobileAds.Initialize(appId);

        bannerView = new BannerView(adBannerUnitId, AdSize.SmartBanner, AdPosition.Bottom);

        rewardBasedVideoAd = RewardBasedVideoAd.Instance;
        rewardBasedVideoAd.OnAdClosed += HandleOnAdClosed;
        rewardBasedVideoAd.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        rewardBasedVideoAd.OnAdStarted += HandleOnAdStarted;
        rewardBasedVideoAd.OnAdRewarded += HandleOnAdRewarded;
        rewardBasedVideoAd.OnAdLoaded += HandleOnAdLoaded;
        rewardBasedVideoAd.OnAdLeavingApplication += HandleOnAdLeavingApp;

        //start banner here
    }

    public void ShowBannerAd()
    {
        AdRequest bannerRequest = new AdRequest.Builder().Build();
        bannerView.LoadAd(bannerRequest);
    }

    public void ShowAd(bool loadGame, Panels panel)
    {
        Debug.Log("Start method show ad");

        tryCounter = 0;
        if (testMode && !testOnDevice) UIHandler.instance.loadingPanel.SetActive(false);

        returnPanel = panel;
        loadGameAfterAD = loadGame;
        RewardsAdded = false;


        if (testMode)
        {
            if (testOnDevice) LoadRewardBasedAd();
            else StartGame();
        }
        else
        {
            LoadRewardBasedAd();
        }
            

    }


    private void LoadRewardBasedAd()
    {
        //test device
        if (testMode)
        {
            rewardBasedVideoAd.LoadAd(new AdRequest.Builder().AddTestDevice("921BB65A29EBB28F").Build(), adUnitId);
        }
        else
        {
            Debug.Log("Loading ad");
            rewardBasedVideoAd.LoadAd(new AdRequest.Builder().Build(), adUnitId);
        }

    }

    private void HandleOnAdLoaded(object sender, EventArgs e)
    {
        if (AudioManager.instance.IsSoundOn) AudioManager.instance.m_MyAudioSource.volume = 0f;

        UIHandler.instance.loadingPanel.SetActive(false);
        if (testMode)
        {
            Debug.Log("Ad Loaded test mode");
            rewardBasedVideoAd.Show();
        }
        else
        {
            Debug.Log("Ad Loaded");
            rewardBasedVideoAd.Show();
        }
    }


    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        tryCounter++;
        if (tryCounter > 3)
        {
            Debug.Log("Counter is above 5");
            StartCoroutine(LoadingScipt.instance.ShowFailedToLoadAdMessage(returnPanel));
            return;
        }
        //try to reload
        if (testMode)
        {
            Debug.Log("Failed to load, TEST MODE !");
        }
        else
        {
            Debug.Log("Failed to load Ad");
        }

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

        if (testMode)
        {
            Debug.Log("AdClosed test mode");
            if (!RewardsAdded)
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

                Debug.Log("Test mode rewards not added");
                return;
            }
            if (loadGameAfterAD) StartGame();
        }
        else
        {
            Debug.Log("AdClosed");
            if (RewardsAdded)
            {
                if (loadGameAfterAD)
                {
                    StartGame();
                }
                else
                {
                    if (EnergyScript.currentEnergy < EnergyScript.MAXENERGY) EnergyScript.currentEnergy++;
                    if (EnergyScript.currentEnergy == EnergyScript.MAXENERGY) EnergyScript.instance.energyTimer = 0;
                    UIHandler.instance.mainPanel.SetActive(true);
                    UIHandler.instance.DisplayEnergy();
                    UIHandler.instance.DisplayEnergyTimer();
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
    }

    public void StartGame()
    {
        ShowAdvertiseBeforeGame = false;
        ShowAdvertiseBeforeGameCounter = 0;
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
    

}
