using System;
using UnityEngine;
using UnityEngine.Events;
using Firebase;
using Firebase.Auth;
using Firebase.Unity.Editor;
using Firebase.Database;
using System.Collections.Generic;
using System.Globalization;

public class GameManager : MonoBehaviour
{
    public enum Panels {Main , Start, Null}

    public Panels showMenuPanel = Panels.Null;

    public bool singlePlayerWithoutLogginIn = true;
    public bool userAutoSignedIn = true;
    public bool isSoundsEnabled;
    public bool needToSignOutAfterShowingErrorPanel = false;
    public bool newUserCreated = false;
    public bool gameIsLoading = true;
    public bool isFirebaseLoaded = false;
    public bool isGameOver = true;
    public bool _fromMain = false;

    public static GameManager instance;


    
    public UnityEvent OnFirebaseInitialized = new UnityEvent();

    private FirebaseAuth _auth;
    private FirebaseUser _user;
    private DatabaseReference dataBaseReference;

    private DateTime serverDateTime;

    public string _displayedUserName;
    public int taskCounter = 0;

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
            isFirebaseLoaded = false;
            //UIHandler.instance.ShowQuitPanel();
            taskCounter = 2;

            throw new Exception($"Firebase not available with {FirebaseApp.CheckDependencies()}");
        }
        else
        {
            isFirebaseLoaded = true;
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
            isFirebaseLoaded = true;
            _app = FirebaseApp.DefaultInstance;
            OnFirebaseInitialized.Invoke();
            _auth = FirebaseAuth.DefaultInstance;
            _app.SetEditorDatabaseUrl("https://edge-wars.firebaseio.com/");
            dataBaseReference = FirebaseDatabase.DefaultInstance.RootReference;
            _auth.StateChanged += AuthStateChanged;
        }
        else
        {
            Debug.Log(" Failed to initalize Firebase with " + dependencyResult);
            //UIHandler.instance.ShowQuitPanel();
            isFirebaseLoaded = false;
            taskCounter = 2;
        }

       
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
                Debug.Log(" name = " + _auth.CurrentUser.DisplayName);
                Debug.Log(" email verified = " + _auth.CurrentUser.IsEmailVerified);

                if (userAutoSignedIn)
                {
                    Debug.Log("user auto sign in = true");

                    if (_auth.CurrentUser.IsEmailVerified)
                    {
                        singlePlayerWithoutLogginIn = false;

                        //load all stuff and then show main panel and then hide game loading panel
                        Debug.Log(" e-mail verified = " + _auth.CurrentUser.IsEmailVerified);

                       // UIHandler.instance.UserSignedInShowMainPanel();
                        RetrieveUserDataFromDatabase(_auth.CurrentUser.UserId);

                    }
                    else
                    {
                        needToSignOutAfterShowingErrorPanel = true;

                        Debug.Log(" e-mail verified = " + _auth.CurrentUser.IsEmailVerified);
                        //hide loading panel show error panel
                        UIHandler.instance.HideLoadingPanel();
                        UIHandler.instance.ShowErrorPanel("E-mail " + _auth.CurrentUser.Email + " is not verified ! please verify your e-mail before loggin in", Color.red, true);
                    }

                }
                else
                {
                    Debug.Log("user auto sign in = false");

                    if (_auth.CurrentUser.IsEmailVerified)
                    {
                        singlePlayerWithoutLogginIn = false;

                        //load all stuff and then show main panel and then hide game loading panel
                        Debug.Log(" e-mail verified = " + _auth.CurrentUser.IsEmailVerified);

                      //  UIHandler.instance.UserSignedInShowMainPanel();
                        RetrieveUserDataFromDatabase(_auth.CurrentUser.UserId);
                    }
                    else
                    {
                        if (newUserCreated)
                        {
                            Debug.Log(" new user ");

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
                            Debug.Log(" e-mail verified = " + _auth.CurrentUser.IsEmailVerified);

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
                PlayerManager.instance.playerName = nickname;
                PlayerManager.instance.playerRank = 1000;
                SaveNewUserToDatabase();
            }

            newUserCreated = false;
            UnityMainThreadDispatcher.Instance().Enqueue(UIHandler.instance.UserRegisterCleanUp());
            if (!userCreateSuccess)
            {
                UnityMainThreadDispatcher.Instance().Enqueue(UIHandler.instance.ShowErrorPanelIEnumrator(errorText, Color.red, false));
            }
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

            if (userSingInstatus)
            {
                if (_auth.CurrentUser.IsEmailVerified) UnityMainThreadDispatcher.Instance().Enqueue(UIHandler.instance.UserSignedInShowMainPanelIEnumerator());
            } 
            else UnityMainThreadDispatcher.Instance().Enqueue(UIHandler.instance.UpdateLogonStatus(errorText, Color.red));


        });
    }

    public void UserLogout(bool saveEnergyData)
    {
        if (saveEnergyData) EnergyDataUpdate(EnergyScript.currentEnergy, (int)EnergyScript.instance.energyTimer, true);
        else
        {
            _auth.SignOut();
            taskCounter = 0;
        } 
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

    public void SaveNewUserToDatabase()
    {
        if (_user != null)
        {
            PlayerManager.instance.playerEnergy = 10;
            
            string json = JsonUtility.ToJson(PlayerManager.instance);

            Debug.Log("json = " + json);
            Debug.Log(DateTime.Now.ToString());


            dataBaseReference.Child("users").Child(_user.UserId).SetRawJsonValueAsync(json).ContinueWith(task => {
                if (task.IsCanceled)
                {
                    Debug.Log("SetRawJsonValueAsync was canceled.");
                }
                else if (task.IsFaulted)
                {
                    Debug.Log("SetRawJsonValueAsync encountered an error: " + task.Exception);
                }
                else if (task.IsCompleted)
                {
                    Debug.Log("SetRawJsonValueAsync completed successfully.");
                }
                else
                {
                    Debug.Log("SetRawJsonValueAsync returns unhandled result");
                }
            });
        }
        else
        {
            Debug.Log("SaveNewUserToDatabase failed, _user is null");
        }

    }
    public void UpdatePlayerRank()
    {
    
        if (_user != null)
        {
            dataBaseReference.Child("users").Child(_user.UserId).Child("playerRank").SetValueAsync(PlayerManager.instance.playerRank).ContinueWith(task => {
                if (task.IsCanceled)
                {
                    Debug.Log("SetValueAsync was canceled.");
                }
                else if (task.IsFaulted)
                {
                    Debug.Log("SetValueAsync encountered an error: " + task.Exception);
                }
                else if (task.IsCompleted)
                {
                    Debug.Log("SetValueAsync completed successfully.");
                }
                else
                {
                    Debug.Log("SetValueAsync returns unhandled result");
                }
            });
        }
        else
        {
            Debug.Log("SetValueAsync failed, _user is null");
        }
    }
    public void RetrieveUserDataFromDatabase(string userID)
    {
        Debug.Log("RetrieveUserDataFromDatabase userID = " + userID);

       dataBaseReference.Child("users").Child(userID).GetValueAsync().ContinueWith( task => {
           if (task.IsCanceled)
           {
               Debug.Log("GetValueAsync was canceled.");
           }
           else if (task.IsFaulted)
           {
               Debug.Log("GetValueAsync encountered an error: " + task.Exception);
           }
           else if (task.IsCompleted)
           {
               Debug.Log("GetValueAsync completed successfully.");
               DataSnapshot dataSnapshot = task.Result;
               PlayerManager.instance.playerName = dataSnapshot.Child("playerName").Value.ToString();
               PlayerManager.instance.playerRank = int.Parse(dataSnapshot.Child("playerRank").Value.ToString());
               PlayerManager.instance.playerEnergy = int.Parse(dataSnapshot.Child("playerEnergy").Value.ToString());
               PlayerManager.instance.playerEnergyTimer = int.Parse(dataSnapshot.Child("playerEnergyTimer").Value.ToString());
               PlayerManager.instance.playerLogoutDateTime = dataSnapshot.Child("playerLogoutDateTime").Value.ToString();

               taskCounter++;

               GetCurrentPlayerPlaceInLeaderboard();
               OnLogonEnergyCounterUpdate();

           }
           else
           {
               Debug.Log("GetValueAsync returns unhandled result");
           }
       });
    }
    public void UpdateLeaderboard()
    {
        if (_user != null)
        {
            if (_user != null)
            {
                dataBaseReference.Child("leaderboard").Child(_user.UserId).SetValueAsync(PlayerManager.instance.playerRank).ContinueWith(task => {
                    if (task.IsCanceled)
                    {
                        Debug.Log("UpdateLeaderboard was canceled.");
                    }
                    else if (task.IsFaulted)
                    {
                        Debug.Log("UpdateLeaderboard encountered an error: " + task.Exception);
                    }
                    else if (task.IsCompleted)
                    {
                        Debug.Log("UpdateLeaderboard completed successfully.");
                    }
                    else
                    {
                        Debug.Log("UpdateLeaderboard returns unhandled result");
                    }
                });
            }
            else
            {
                Debug.Log("UpdateLeaderboard failed, _user is null");
            }
        }

    }

    public void GetCurrentPlayerPlaceInLeaderboard()
    {
        if (_user == null)
        {
            Debug.Log("GetCurrentPlayerRank aborted, CurrentUser is null");
            return;
        }

        int playerRank = 1;
        FirebaseDatabase.DefaultInstance.GetReference("leaderboard").OrderByValue().GetValueAsync().ContinueWith( task => 
        { 
            if (task.IsCanceled)
            {
                Debug.Log("GetCurrentPlayerRank was canceled.");
            }
            else if (task.IsFaulted)
            {
                Debug.Log("GetCurrentPlayerRank encountered an error: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                Debug.Log("GetCurrentPlayerRank completed successfully.");
                DataSnapshot dataSnapshot = task.Result;
                playerRank = (int)dataSnapshot.ChildrenCount;

                if (!dataSnapshot.HasChild(_auth.CurrentUser.UserId.ToString()) && _auth.CurrentUser.IsEmailVerified)
                {
                       UpdateLeaderboard();
                       GetCurrentPlayerPlaceInLeaderboard();
                }
                else
                {
                    foreach (var data in dataSnapshot.Children)
                    {
                        if (_user.UserId == data.Key) break;
                        playerRank --;
                    }
                    
                    UnityMainThreadDispatcher.Instance().Enqueue(UIHandler.instance.UpdatePlayerRankUI(playerRank, (int)dataSnapshot.ChildrenCount));
                }
                
            }
            else
            {
                Debug.Log("GetCurrentPlayerRank returns unhandled result");
            }
        });
    }

    public void OnLogonEnergyCounterUpdate()
    {
        DateTime serverDateTime = DateTime.Now;

        FirebaseDatabase.DefaultInstance.GetReference("/.info/serverTimeOffset").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.Log("GetDatabaseServerTimeStamp was canceled.");
            }
            else if (task.IsFaulted)
            {
                Debug.Log("GetDatabaseServerTimeStamp encountered an error: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                Debug.Log("GetDatabaseServerTimeStamp completed successfully.");

                double serverTimeOffset = double.Parse(task.Result.Value.ToString());
                serverDateTime = DateTime.Now.AddMilliseconds(serverTimeOffset);

                UnityMainThreadDispatcher.Instance().Enqueue(EnergyScript.instance.OnLogonEnergyCount(serverDateTime));
            }
            else
            {
                Debug.Log("GetDatabaseServerTimeStamp returns unhandled result");
            }
        });
    }

    public void EnergyDataUpdate(int energy, int timer, bool singOut)
    {
        if (_user == null)
        {
            Debug.Log("EnergyDataUpdate aborted, CurrentUser is null");
            return;
        }

        dataBaseReference.Child("users").Child(_user.UserId).Child("playerEnergy").SetValueAsync(energy).ContinueWith( task => 
        {
            if (task.IsCompleted)
            {
                dataBaseReference.Child("users").Child(_user.UserId).Child("playerEnergyTimer").SetValueAsync(timer).ContinueWith( task2 =>
                {
                    if (task2.IsCompleted)
                    {
                        if (singOut)
                        {
                            dataBaseReference.Child("users").Child(_user.UserId).Child("playerLogoutDateTime").SetValueAsync(DateTime.Now.ToString("g", DateTimeFormatInfo.InvariantInfo)).ContinueWith(task3 =>
                            {
                                if (task3.IsCompleted)
                                {
                                    _auth.SignOut();
                                    taskCounter = 0;
                                    Debug.Log(" Energy data updated and user signed out");
                                }
                            });
                        }
                        else
                        {
                            dataBaseReference.Child("users").Child(_user.UserId).Child("playerLogoutDateTime").SetValueAsync(DateTime.Now.ToString("g", DateTimeFormatInfo.InvariantInfo));
                            Debug.Log("Energy Data updated");
                        }
                            
                    }
                });
            }
        });
    }

    public void ApplicationQuitDataUpdate(int energy, int timer, bool isGameOver, bool isFirebaseLoaded)
    {
        if (_user == null)
        {
            Debug.Log("ApplicationQuitDataUpdate aborted, CurrentUser is null");
            return;
        }

        if (isFirebaseLoaded == false)
        {
            Debug.Log("ApplicationQuitDataUpdate aborted, isFirebaseLoaded is false");
            return;
        }

        if (!isGameOver)
        {
            Debug.Log("ApplicationQuitDataUpdate rank calculated ing game");
            int randomElement = UnityEngine.Random.Range(0, 10); //rank between 920 and 1080
            int gameEnd = 0; //lose
            int enemyRank = 1000;

            if (randomElement < 5) enemyRank -= randomElement * 20;
            else enemyRank += (randomElement - 5) * 20;

            Debug.Log("Player rank = " + PlayerManager.instance.playerRank);
            Debug.Log("Enemy rank = " + enemyRank);

            double expectedPlayerPoints = 1f / (1f + Math.Pow(10f, (enemyRank - PlayerManager.instance.playerRank) / 400f));

            PlayerManager.instance.playerRank += (int)(16 * (gameEnd - expectedPlayerPoints));
            Debug.Log("Claculated Player rank = " + PlayerManager.instance.playerRank);
        }

        dataBaseReference.Child("users").Child(_user.UserId).Child("playerEnergy").SetValueAsync(energy).ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                dataBaseReference.Child("users").Child(_user.UserId).Child("playerEnergyTimer").SetValueAsync(timer).ContinueWith(task2 =>
                {
                    if (task2.IsCompleted)
                    {
                        dataBaseReference.Child("users").Child(_user.UserId).Child("playerLogoutDateTime").SetValueAsync(DateTime.Now.ToString("g", DateTimeFormatInfo.InvariantInfo)).ContinueWith(task3 =>
                        {
                            if (task3.IsCompleted)
                            {
                                if (!isGameOver)
                                {
                                    //application terminated while in game = auto loose
                                    dataBaseReference.Child("leaderboard").Child(_user.UserId).SetValueAsync(PlayerManager.instance.playerRank).ContinueWith(task4 =>
                                    {
                                        if (task4.IsCompleted)
                                        {
                                            dataBaseReference.Child("users").Child(_user.UserId).Child("playerRank").SetValueAsync(PlayerManager.instance.playerRank).ContinueWith(task5 =>
                                            {
                                                if (task5.IsCompleted) Debug.Log("ApplicationQuitDataUpdate all data updated");
                                            });
                                        }
                                    });
                                }
                                else
                                {
                                    Debug.Log("ApplicationQuitDataUpdate only Energy data updated");
                                }
                            }
                        });
                    }
                });
            }
        });
    }


}

