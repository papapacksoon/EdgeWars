using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

using Firebase;

using Firebase.Auth;


public class GameManager : MonoBehaviour
{
    public bool singlePlayerWithoutLogginIn = true;
    public bool userAutoSignedIn = true;
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
                Debug.Log("Listener: Signed out " + _user.UserId);
            }
            _user = _auth.CurrentUser;
            if (signedIn)
            {
                Debug.Log("Listener: Signed in " + _user.UserId);
                if (userAutoSignedIn)
                {
                    if (_auth.CurrentUser.IsEmailVerified)
                    {
                        singlePlayerWithoutLogginIn = false;
                        ButtonHandler.instance.UserSignedIn();
                        Debug.Log("PhotoUrl " + _auth.CurrentUser.PhotoUrl);
                    }
                    else
                    {
                        singlePlayerWithoutLogginIn = true;
                        _auth.SignOut();
                        ButtonHandler.instance.ShowErrorPanel("E-mail is not verified ! please verify your e-mail", Color.red, true);
                    }

                }
                else
                {
                    if (_auth.CurrentUser.IsEmailVerified)
                    {
                        singlePlayerWithoutLogginIn = false;
                        ButtonHandler.instance.UserSignedIn();
                        Debug.Log("PhotoUrl " + _auth.CurrentUser.PhotoUrl);
                    }
                    else
                    {
                        singlePlayerWithoutLogginIn = true;
                        _auth.SignOut();
                        ButtonHandler.instance.ShowErrorPanel("E-mail is not verified ! please verify your e-mail", Color.red, true);
                    }
                }
                
            }
        }
    }

    public void CreateNewUser(string email, string password, string nickname)
    {
        bool userCreateSuccess = false;
        string errorText = "";

        _auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.Log("CreateUserWithEmailAndPasswordAsync was canceled.");
                errorText = "Registration canceled";
                return;
            }

            if (task.IsFaulted)
            {
                Debug.Log("CreateUserWithEmailAndPasswordAsync faulted with" + task.Exception);
                errorText = "Registration faulted, please try again later";
                return;
            }

            // Firebase user has been created.
            _user = task.Result;
            UserProfile userProfile = new UserProfile();
            userProfile.DisplayName = nickname;
            _user.UpdateUserProfileAsync(userProfile);
            Debug.Log("Firebase user created successfully: " + _user.DisplayName + " " + _user.UserId);
            userCreateSuccess = true;
            
            _user.SendEmailVerificationAsync().ContinueWith(nexttask => {
                if (nexttask.IsCanceled)
                {
                    Debug.Log("SendEmailVerificationAsync was canceled.");
                }
                if (nexttask.IsFaulted)
                {
                    Debug.Log("SendEmailVerificationAsync encountered an error: " + nexttask.Exception);
                }

                Debug.Log("Email sent successfully.");
            });

        });

        if (!userCreateSuccess) ButtonHandler.instance.ShowErrorPanel(errorText, Color.red, false);

    }

    public void UserSingIn(string email, string password)
    {
        bool userLoginSuccess = false;
        string errorText = "E-mail and password pair mismatch";

        _auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.Log("SignInWithEmailAndPasswordAsync was canceled.");
                errorText = "Loggin in canceled";
                return;
                
            }
            if (task.IsFaulted)
            {
                Debug.Log("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                errorText = "Loggin in faulted, please try again later";
                return;
            }
            
            _user = task.Result;
            Debug.Log("User signed in successfully: " + _user.DisplayName + " " + _user.UserId);
            singlePlayerWithoutLogginIn = false;
            userLoginSuccess = true;
            
        });

        if (!userLoginSuccess) ButtonHandler.instance.ShowErrorPanel(errorText, Color.red, false);
    }

    public void UserLogout()
    {
        _auth.SignOut();
        singlePlayerWithoutLogginIn = true;
        userAutoSignedIn = false;
    }

    public void ResendVerificationMail()
    {
        string errorText = "";
        Color errorTextColor = Color.red;
        bool showResendEmailButton = true;

        _user.SendEmailVerificationAsync().ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.Log("SendEmailVerificationAsync was canceled.");
                errorText = "Sending e-mail canceled";
                errorTextColor = Color.red;
                showResendEmailButton = true;

            }
            else if (task.IsFaulted)
            {
                Debug.Log("SendEmailVerificationAsync encountered an error: " + task.Exception);
                errorText = "Sending e-mail faulted, please try again later";
                errorTextColor = Color.red;
                showResendEmailButton = true;
            }
            else
            {
                Debug.Log("Verification email sent successfully.");
                errorText = "We send you a verification link on e-mail";
                errorTextColor = Color.white;
                showResendEmailButton = false;
            }

            ButtonHandler.instance.errorStatusText.text = errorText;
            ButtonHandler.instance.errorStatusText.color = errorTextColor;
            if (showResendEmailButton) ButtonHandler.instance.errorResenEmailButton.gameObject.SetActive(true);
            else ButtonHandler.instance.errorResenEmailButton.gameObject.SetActive(false);

        });
        
    }

    public void ResetUserPassword()
    {
        _auth.SendPasswordResetEmailAsync(_auth.CurrentUser.Email).ContinueWith(task => {
            string errorText = "";
            Color errorTextColor = Color.red;

            if (task.IsCanceled)
            {
                Debug.Log("SendPasswordResetEmailAsync was canceled.");
                errorText = "Sending e-mail canceled";
                errorTextColor = Color.red;
            }
            else if (task.IsFaulted)
            {
                Debug.Log("SendPasswordResetEmailAsync encountered an error: " + task.Exception);
                errorText = "Sending e-mail faulted, please try again later";
                errorTextColor = Color.red;
            }
            else
            {
                Debug.Log("Reset email sent successfully.");
                errorText = "We send you a reset password link on e-mail";
                errorTextColor = Color.white;
            }

            ButtonHandler.instance.ShowErrorPanel(errorText, errorTextColor, false);
            });
    }
}
