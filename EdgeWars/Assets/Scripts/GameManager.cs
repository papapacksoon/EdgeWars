using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System;
using Firebase;
using Firebase.Analytics;
using Firebase.Auth;
using UnityEngine.Events;

public class GameManager : MonoBehaviourPunCallbacks
{
    public bool singlePlayerWithoutLogginIn = true;
  
    public bool isSoundsEnabled;


    public static GameManager instance;

    //private bool isPlayerLoggedIn; ??


    public UnityEvent OnFirebaseInitialized = new UnityEvent();

    private FirebaseAuth _auth;

    public FirebaseAuth Auth
    {
        get
        {
            if (_auth == null)
            {
                _auth = FirebaseAuth.GetAuth(App);
            }

            return _auth;
        }
    }

    private FirebaseApp _app;

    public FirebaseApp App
    {
        get
        {
            if (_app == null)
            {
                _app = GetAppSynchronous();
            }

            return _app;
        }
    }

    private FirebaseApp GetAppSynchronous()
    {
     //   Debug.LogWarning("You are getting the FirebaseApp synchronously. You cannot resolve dependencies this way");
        if (FirebaseApp.CheckDependencies() != DependencyStatus.Available)
        {
            throw new Exception($"Firebase not available with {FirebaseApp.CheckDependencies()}");
        }

        return FirebaseApp.DefaultInstance;
    }

    private async void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        if (instance == null)
        {
            instance = this;
        }

        var dependencyResult = await FirebaseApp.CheckAndFixDependenciesAsync();
        if (dependencyResult == DependencyStatus.Available)
        {
            _app = FirebaseApp.DefaultInstance;
            OnFirebaseInitialized.Invoke();
        }
        else
        {
            Debug.Log(" Fialed to initalize Firebase with " + dependencyResult);
        }


    }
       
    
       
}
