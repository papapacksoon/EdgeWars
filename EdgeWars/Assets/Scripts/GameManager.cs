using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

using Firebase;

using Firebase.Auth;


public class GameManager : MonoBehaviourPunCallbacks
{
    public bool singlePlayerWithoutLogginIn = true;
    public bool isSoundsEnabled;
    public static GameManager instance;

    


    public UnityEvent OnFirebaseInitialized = new UnityEvent();

    private FirebaseAuth _auth;
    private FirebaseUser _user;
    public string _displayedUserName;

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

        _auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        _auth.StateChanged += AuthStateChanged;

    }

    void OnDestroy()
    {
        if (_auth != null)
        {
            _auth.StateChanged -= AuthStateChanged;
            _auth = null;
        }
    }

    private void AuthStateChanged(object sender, EventArgs e)
    {
        if (_auth.CurrentUser != _user)
        {
            bool signedIn = _user != _auth.CurrentUser && _auth.CurrentUser != null;
            if (!signedIn && _user != null)
            {
                Debug.Log("Signed out " + _user.UserId);
            }
            _user = _auth.CurrentUser;
            if (signedIn)
            {
                Debug.Log("Signed in " + _user.UserId);
            }
        }
    }

    public void CreateNewUser(string email, string password)
    {
        _auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.Log("CreateUserWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.Log("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }

            // Firebase user has been created.
            _user = task.Result;
            Debug.Log("Firebase user created successfully: " + _user.DisplayName + " " + _user.UserId);
           // UserSingIn(email, password);
        }
        );
    }

    public void UserSingIn(string email, string password)
    {
        _auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.Log("SignInWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.Log("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }

            _user = task.Result;
            Debug.Log("User signed in successfully: " + _user.DisplayName + " " + _user.UserId);
            singlePlayerWithoutLogginIn = false;
        });
    }
}
