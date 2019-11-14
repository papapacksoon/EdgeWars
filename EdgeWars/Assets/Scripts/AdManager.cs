using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds;
using GoogleMobileAds.Api;
using System;

public class AdManager : MonoBehaviour
{

    public static AdManager instance;

#if UNITY_EDITOR
    string appId = "unused";
#elif UNITY_ANDROID
    string appId = "ca-app-pub-9898645005884031~3330841357";
#elif UNITY_IPHONE
    string appId = "ca-app-pub-9898645005884031~8778609150";
#else
    string appId = "unexpected_platdorm";
#endif

#if UNITY_EDITOR
    string adUnitId = "unused";
#elif UNITY_ANDROID
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
    }

    // Start is called before the first frame update
    void Start()
    {
        MobileAds.SetiOSAppPauseOnBackground(true);
        MobileAds.Initialize(appId);

        rewardBasedVideoAd = RewardBasedVideoAd.Instance;
        rewardBasedVideoAd.OnAdClosed += HandleOnAdClosed;
        rewardBasedVideoAd.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        rewardBasedVideoAd.OnAdStarted += HandleOnAdStarted;
        rewardBasedVideoAd.OnAdRewarded += HandleOnAdRewarded;
        

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowAd()
    {
        this.LoadRewardBasedAd();
        if (rewardBasedVideoAd.IsLoaded())
        {
            rewardBasedVideoAd.Show();
        }
        else Debug.Log("Ad is not loaded");
    }

    private void LoadRewardBasedAd()
    {
        rewardBasedVideoAd.LoadAd(new AdRequest.Builder().Build(), adUnitId);
    }

   public void HandleOnAdStarted(object sender, EventArgs args)
    {
        
        if (AudioManager.instance.IsSoundOn)
        {
            //mute game sound
        }

    }

    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        //try to reload
        Debug.Log("Failed to load, invoke reload sequnce");

        rewardBasedVideoAd.LoadAd(new AdRequest.Builder().Build(), adUnitId);
    }

    public void HandleOnAdRewarded(object sender, Reward args)
    {
        //reward user
        Debug.Log("Rewarded " + args.Amount + " of type " + args.Type);
    }

    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        if (AudioManager.instance.IsSoundOn)
        {
            //volume up sounds
        }
    }


}
