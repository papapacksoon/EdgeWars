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
        appId = "ca-app-pub-3940256099942544~3347511713";
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

    

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowAd(bool loadGame)
    {

        // RequestBanner();
        /*
        this.LoadRewardBasedAd();
        if (rewardBasedVideoAd.IsLoaded())
        {
            rewardBasedVideoAd.Show();
        }
        else Debug.Log("Ad is not loaded");
        */
        adUnitId = "ca-app-pub-3940256099942544/5224354917";
        LoadRewardBasedAd();


    }

    private void LoadRewardBasedAd()
    {
        //test device
        
        rewardBasedVideoAd.LoadAd(new AdRequest.Builder().AddTestDevice("921BB65A29EBB28F").Build(), adUnitId);
        //rewardBasedVideoAd.LoadAd(new AdRequest.Builder().Build(), adUnitId);
    }

   public void HandleOnAdStarted(object sender, EventArgs args)
    {

        Debug.Log("AdStarted");

    }

    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        //try to reload
        Debug.Log("Failed to load, invoke reload sequnce");

        //rewardBasedVideoAd.LoadAd(new AdRequest.Builder().Build(), adUnitId);
    }

    public void HandleOnAdRewarded(object sender, Reward args)
    {
        //reward user
        Debug.Log("Rewarded " + args.Amount + " of type " + args.Type);
    }

    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        Debug.Log("AdClosed");
    }

    private void HandleOnAdLoaded(object sender, EventArgs e)
    {
        Debug.Log("Ad Loaded");
        rewardBasedVideoAd.Show();
    }

    public void RequestBanner()
    {
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-3940256099942544/6300978111";
#elif UNITY_IPHONE
      string adUnitId = "ca-app-pub-3940256099942544/2934735716";
#else
      string adUnitId = "unexpected_platform";
#endif

        // Create a 320x50 banner at the top of the screen.
        var bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Top);
        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the banner with the request.
        bannerView.LoadAd(request);
        
    }


}
