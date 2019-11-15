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
    public bool needToSignOutAfterShowingErrorPanel = false;
    public bool newUserCreated = false;

    public static GameManager instance;
    
    public UnityEvent OnFirebaseInitialized = new UnityEvent();

    private FirebaseAuth _auth;
    private FirebaseUser _user, _lastSignedInUser;
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
            UIHandler.instance.ShowQuitPanel();
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
            Debug.Log(" Failed to initalize Firebase with " + dependencyResult);
            UIHandler.instance.ShowQuitPanel();
        }

        _auth = FirebaseAuth.DefaultInstance;
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
                _lastSignedInUser = _user;

                if (userAutoSignedIn)
                {
                    if (_auth.CurrentUser.IsEmailVerified)
                    {
                        singlePlayerWithoutLogginIn = false;
                        UIHandler.instance.UserSignedIn();
                        Debug.Log(" last signed in " + _auth.CurrentUser.Metadata.LastSignInTimestamp);
                    }
                    else
                    {
                        Debug.Log(" last signed in " + _auth.CurrentUser.Metadata.LastSignInTimestamp);
                        needToSignOutAfterShowingErrorPanel = true;
                        UIHandler.instance.ShowErrorPanel("E-mail " + _auth.CurrentUser.Email + " is not verified ! please verify your e-mail before loggin in", Color.red, true);
                    }

                }
                else
                {
                    if (_auth.CurrentUser.IsEmailVerified)
                    {
                        singlePlayerWithoutLogginIn = false;
                        UIHandler.instance.UserSignedIn();
                        Debug.Log(" last signed in " + _auth.CurrentUser.Metadata.LastSignInTimestamp);
                    }
                    else
                    {
                        if (newUserCreated)
                        {
                            _user.SendEmailVerificationAsync().ContinueWith(nexttask =>
                            {
                                if (nexttask.IsCanceled)
                                {
                                    Debug.Log("SendEmailVerificationAsync was canceled.");
                                }
                                if (nexttask.IsFaulted)
                                {
                                    Debug.Log("SendEmailVerificationAsync encountered an error: " + nexttask.Exception);
                                }
                                else if (nexttask.IsCompleted)
                                {
                                    Debug.Log("SendEmailVerificationAsync sent successfully.");
                                }
                                else
                                {
                                    Debug.Log("SendEmailVerificationAsync ends with unknown state");
                                }
                            });

                        }
                        else
                        {
                            Debug.Log(" last signed in " + _auth.CurrentUser.Metadata.LastSignInTimestamp);
                            needToSignOutAfterShowingErrorPanel = true;
                            UIHandler.instance.ShowErrorPanel("E-mail " + _auth.CurrentUser.Email + " is not verified ! please verify your e-mail  before loggin in", Color.red, true);
                        }


                    }
                }
                
            }
        }
    }

    public void CreateNewUser(string email, string password, string nickname)
    {
        bool userCreateSuccess = false;
        string errorText = "";
        newUserCreated = true;

        _auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.Log("CreateUserWithEmailAndPasswordAsync was canceled.");
                errorText = "Registration canceled";
            }
            else if (task.IsFaulted)
            {
                Debug.Log("CreateUserWithEmailAndPasswordAsync faulted with" + task.Exception);
                foreach (var e in task.Exception.InnerExceptions)
                {
                    errorText = e.InnerException.Message;
                }
            }
            else 
            {
                _user = task.Result;
                UserProfile userProfile = new UserProfile();
                userProfile.DisplayName = nickname;

                _user.UpdateUserProfileAsync(userProfile).ContinueWith(newtask =>
                  {
                      if (task.IsCanceled)
                      {
                          Debug.Log("UpdateUserProfileAsync was canceled.");
                      }
                      else if (task.IsFaulted)
                      {
                          Debug.Log("UpdateUserProfileAsync faulted with" + task.Exception);
                      }
                      else if (task.IsCompleted)
                      {
                          Debug.Log("UpdateUserProfileAsync completed");
                      }
                      else
                      {
                          Debug.Log("UpdateUserProfileAsync ends with unknown state");
                      }
                  });

                Debug.Log("Firebase user created successfully: " + _user.DisplayName + " " + _user.UserId);
                userCreateSuccess = true;
                newUserCreated = false;
            }

            newUserCreated = false;
            UnityMainThreadDispatcher.Instance().Enqueue(UIHandler.instance.UserRegisterCleanUp());
            if (!userCreateSuccess) UnityMainThreadDispatcher.Instance().Enqueue(UIHandler.instance.ShowErrorPanelIEnumrator(errorText, Color.red, false));
            else
            {
                needToSignOutAfterShowingErrorPanel = true;
                UnityMainThreadDispatcher.Instance().Enqueue(UIHandler.instance.ShowErrorPanelIEnumrator("E-mail " + _auth.CurrentUser.Email + " is not verified ! please verify your e-mail  before loggin in", Color.red, true));
            }
        });

        

    }

    public void UserSingIn(string email, string password)
    {
        bool userSingInstatus = false;
        string errorText = "E-mail and password pair mismatch";

        _auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.Log("SignInWithEmailAndPasswordAsync was canceled.");
                errorText = "Loggin in canceled";
            }
            else if (task.IsFaulted)
            {
                Debug.Log("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                foreach (var e in task.Exception.InnerExceptions)
                {
                    errorText = e.InnerException.Message;
                }
             
            }
            else if (task.IsCompleted)
            {
                Debug.Log("SignInWithEmailAndPasswordAsync task Completed");
                userSingInstatus = true;
            }
            else
            {
                Debug.Log("SignInWithEmailAndPasswordAsync unhadled status");
                errorText = "Something went wrong, please try again later";
            }

            UnityMainThreadDispatcher.Instance().Enqueue(UIHandler.instance.UserSingInCleanUp());
            if (!userSingInstatus) UnityMainThreadDispatcher.Instance().Enqueue(UIHandler.instance.UpdateLogonStatus(errorText, Color.red));
        });
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
                foreach (var e in task.Exception.InnerExceptions)
                {
                    errorText = e.InnerException.Message;
                }
                errorTextColor = Color.red;
                showResendEmailButton = true;
            }
            else if (task.IsCompleted)
            {
                Debug.Log("Verification email sent successfully.");
                errorText = "We send you a verification link on e-mail";
                errorTextColor = Color.white;
                showResendEmailButton = false;
            }
            else
            {
                errorText = "Something went wrong, please try again later";
                errorTextColor = Color.red;
                showResendEmailButton = true;
            }

           UnityMainThreadDispatcher.Instance().Enqueue(UIHandler.instance.UpdateVirificationEmailSendingStatus(errorText, errorTextColor, showResendEmailButton));

        });

        
        
    }

    public void ResetUserPassword(string email)
    {
        bool taskSuccess = false;
        string errorText = "?";
        
        _auth.SendPasswordResetEmailAsync(email).ContinueWith(task => {
            
            
            if (task.IsCanceled)
            {
                Debug.Log("SendPasswordResetEmailAsync was canceled.");
                errorText = "Sending e-mail canceled";
            }
            else if (task.IsFaulted)
            {
                Debug.Log("SendPasswordResetEmailAsync encountered an error: " + task.Exception);

                Debug.Log(task.Exception.InnerExceptions.Count);
                errorText = task.Exception.InnerExceptions[0].Message;

            }
            else if (task.IsCompleted)
            {
                Debug.Log("Reset email sent successfully.");
                errorText = "We send you a reset password link on e-mail";
                taskSuccess = true;
            }
            else
            {
                errorText = "Something went wrong, please try again later";
            }

            if (taskSuccess)
            {
                UnityMainThreadDispatcher.Instance().Enqueue(UIHandler.instance.UserSingInCleanUp());
                UnityMainThreadDispatcher.Instance().Enqueue(UIHandler.instance.ShowErrorPanelIEnumrator(errorText, Color.white, false));
            }
            else UnityMainThreadDispatcher.Instance().Enqueue(UIHandler.instance.UpdateLogonStatus(errorText, Color.red));

        });
    }
}
