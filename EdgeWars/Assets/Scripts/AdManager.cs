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

    private bool testMode = false;
    private bool testOnDevice = false;
    private bool RewardsAdded = false;

/*    //TESTING AD
    BannerView bannerView;
    AdRequest request;
       //TESTING AD
       */


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
        rewardBasedVideoAd.OnAdLeavingApplication += HandleOnAdLeavingApp;

/*        //testing AD
        adUnitId = "ca-app-pub-9898645005884031/2134052986";
        request = new AdRequest.Builder().Build();
        AdSize adSize = new AdSize(250, 250);
        bannerView = new BannerView(adUnitId, adSize, AdPosition.Center);
        bannerView.OnAdLoaded += TestBannerLoad;
        bannerView.OnAdFailedToLoad += TestBannerFailedToLoad;
        //testing ad 
        */
    }

    /*//-----------------------------------TESTING AD
    public void ShowTestBanner()
    {
        Debug.Log("Test banner LOAD AD started");
        bannerView.LoadAd(request);
        
    }

    private void TestBannerLoad(object sender, EventArgs e)
    {
        Debug.Log("Test banner loaded");
        bannerView.Show();
    }

    private void TestBannerFailedToLoad(object sender, EventArgs e)
    {
        Debug.Log("Test banner Failed to loaded. Reloading ...");
        ShowTestBanner();
    }

    //-----------------------------------TESTING AD
    */
    public void ShowAd(bool loadGame, Panels panel)
    {
        Debug.Log("Start method show ad");

        if (testMode && !testOnDevice) UIHandler.instance.loadingPanel.SetActive(false);

        returnPanel = panel;
        loadGameAfterAD = loadGame;
        RewardsAdded = false;

        Debug.Log("Before Load ad");

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

            /*if (loadGameAfterAD)
            {
                StartGame();
            }*/
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
        Debug.Log("Ad Leaving Application");
    }
    

}
