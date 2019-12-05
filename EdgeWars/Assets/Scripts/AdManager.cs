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
    private bool testOnDevice = true;
    private bool RewardsAdded = false;


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

    // Start is called before the first frame update
    void Start()
    {
        //test
        if (testMode) 
        {
            appId = "ca-app-pub-3940256099942544~3347511713";
            adUnitId = "ca-app-pub-3940256099942544/5224354917";
        }
        //

        MobileAds.SetiOSAppPauseOnBackground(true);
        MobileAds.Initialize(appId);

        rewardBasedVideoAd = RewardBasedVideoAd.Instance;
        rewardBasedVideoAd.OnAdClosed += HandleOnAdClosed;
        rewardBasedVideoAd.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        rewardBasedVideoAd.OnAdStarted += HandleOnAdStarted;
        rewardBasedVideoAd.OnAdRewarded += HandleOnAdRewarded;
        rewardBasedVideoAd.OnAdLoaded += HandleOnAdLoaded;

    }


    public void ShowAd(bool loadGame)
    {
        if (testMode && !testOnDevice) UIHandler.instance.loadingPanel.SetActive(false);

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
        //try to reload
        if (testMode)
        {
            Debug.Log("Failed to load, TEST MODE !");
        }
        else
        {
            Debug.Log("Failed to load Ad");

            if (loadGameAfterAD)
            {
                StartGame();
            }
        }
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

        if (testMode)
        {
            Debug.Log("AdClosed test mode");
            if (!RewardsAdded)
            {
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
                }
            }
            else
            {
                Debug.Log("Rewards not added");
            }
            
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

}
