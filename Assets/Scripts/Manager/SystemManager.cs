/*
using Firebase;
using Firebase.Extensions;
using Firebase.Messaging;
using Gpm.LogViewer;*/
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_ANDROID
using UnityEngine.Android;
#endif
using UnityEngine.Rendering;

public class SystemManager : Singleton<SystemManager>
{    
    public void Initialize()
    {
        Debug.Log("SystemManager Initialize");

        CheckPermission();        

        Application.logMessageReceived += HandleLog;

        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        DG.Tweening.DOTween.SetTweensCapacity(3000, 200);
        DG.Tweening.DOTween.defaultAutoPlay = DG.Tweening.AutoPlay.All;

        ObjectPoolManager.Instance.Initialize();         
         
         Screen.sleepTimeout = SleepTimeout.NeverSleep;

         FrameRateManager.Instance.Initialize();         

         SetOption();

         /*FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
         {
             var dependencyStatus = task.Result;
             if (dependencyStatus == Firebase.DependencyStatus.Available)
             {
                 // Create and hold a reference to your FirebaseApp,
                 // where app is a Firebase.FirebaseApp property of your application class.
                 app = Firebase.FirebaseApp.DefaultInstance;

                 // Set a flag here to indicate whether Firebase is ready to use by your app.
                 FirebaseMessaging.TokenReceived += OnTokenReceived; // �Ʒ� ����
                 FirebaseMessaging.MessageReceived += OnMessageReceived; // �Ʒ� ����

                 FirebaseMessaging.RequestPermissionAsync().ContinueWithOnMainThread(task =>
                 {
                     Debug.Log("push permission: " + task.Status.ToString());
                 });
             }
             else
             {
                 UnityEngine.Debug.LogError(System.String.Format(
                   "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                 // Firebase Unity SDK is not safe to use here.
             }
         });

         SignAuthManager.Instance.InitializeSign();     */   
    }

    /*public Action OnUpdatePushToken = null;
    public void OnTokenReceived(object sender, TokenReceivedEventArgs token)
    {
        Debug.Log("OnTokenReceived: " + token.Token);

        GameManager.Instance.PushToken = token.Token;
        OnUpdatePushToken?.Invoke();
    }*/

   /* public void RegenerateToken()
    {
        FirebaseMessaging.DeleteTokenAsync().ContinueWith(deleteTask => {
            if (deleteTask.IsCompleted)
            {
                Debug.Log("Token deleted successfully.");
                app = Firebase.FirebaseApp.DefaultInstance;
                // ���ο� ��ū ��û
                FirebaseMessaging.GetTokenAsync().ContinueWithOnMainThread(tokenTask => {
                    if (tokenTask.IsCompleted)
                    {
                        var newToken = tokenTask.Result;
                        Debug.Log("New FCM Token: " + newToken);                        
                    }
                });
            }
            else
            {
                Debug.LogError("Failed to delete token.");
            }
        });
    }   */

    /*public void OnMessageReceived(object sender, MessageReceivedEventArgs e)
    {
        Debug.Log("OnMessageReceived");

        var message = e.Message;
        if (message != null)
        {
            Debug.Log("Message received from: " + message.From);

            if (!string.IsNullOrEmpty(message.Notification?.Title))
            {
                Debug.Log("Notification Title: " + message.Notification.Title);
            }

            if (!string.IsNullOrEmpty(message.Notification?.Body))
            {
                Debug.Log("Notification Body: " + message.Notification.Body);
            }

            // �����͸� ���� �޽����� �ִ� ��� ó��
            if (message.data != null)
            {
                foreach (var key in message.data.Keys)
                {
                    Debug.Log("data - Key: " + key + ", Value: " + message.data[key]);
                    if(key.Equals("message") == true)
                    {
                        GameManager.Instance.PushMessage = JsonUtility.FromJson<PushSubMessage>(message.data[key]);                        
                    }
                }
            }
        }
        else
        {
            Debug.Log("Received an empty message.");
        }
    }*/

    public static DateTime foregroundTime;
    public static DateTime backgroundTime;

    private void OnApplicationPause(bool pause)
    {
        if (pause == true)
        {
            foregroundTime = DateTime.Now;
            Debug.Log($"backgroundTime : {foregroundTime}");

            /*var gameserver = NetworkManager.Instance.GameServer;
            if (gameserver == null)
            {
                return;
            }

            if (gameserver.IsConnected() == false)
            {
                return;
            }*/
        }
        else
        {
            backgroundTime = DateTime.Now;
            Debug.Log($"foregroundTime : {backgroundTime}");                        

            AudioConfiguration config = AudioSettings.GetConfiguration();
            AudioSettings.Reset(config);            

            /*var gameserver = NetworkManager.Instance.GameServer;
            if (gameserver == null)
            {
                return;
            }*/

           /* if (GameManager.Instance.LoginComplate == true 
                && gameserver.IsConnected() == true)
            {
                NetworkManager.Send(new C2G.RQ_KEEP_ALIVE());
            }

            if (GameManager.Instance.LoginComplate == true)
            {
                CommonUtil.DelayedCall(1f, () =>
                {                    
                    GameManager.Instance.RequestRelogin();
                });
            }  */          
        }
    }    

    public float _doubleEscape = 0.25f;
    private bool _isOneClick = false;
    private double _timer = 0;
    // Update is called once per frame
    void Update()
    {
        if (_isOneClick && ((Time.time - _timer) > _doubleEscape))
        {
            _isOneClick = false;
        }

      /*  if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!_isOneClick)
            {
                _timer = Time.time;
                _isOneClick = true;

                if(GameSceneManager.Instance.currentSCENE_TYPE == SCENE_TYPE.SceneGame)
                {
                    do
                    {
                        UIPopupQueueData popupData = PopupManager.Instance.GetCurrentVisibleQueuePopup();
                        if (popupData != null)
                        {
                            if (popupData.popup._bgClickHide == false)
                            {
                                break;
                            }

                            popupData.popup.Hide();
                            break;
                        }
                    } while (false);
                    
                }                
            }
            else if (_isOneClick && ((Time.time - _timer) < _doubleEscape))
            {
                _isOneClick = false;                
                if(GameManager.Instance.IsInitialize() == true)
                {  
                   if(PopupManager.Instance.isShowPopup() == false)
                    {
                        if(GameManager.Instance.playGame == GAME_TYPE.NONE)
                        {
                            PopupManager.Instance.ShowPopup("UIPopupExit", action: (popup) =>
                            {
                                
                            });
                        }
                        else if(GameManager.Instance.playGame == GAME_TYPE.WATERMELON)
                        {
                            WatermelonMgr watermelonMgr = GameManager.Instance.GetMiniGameManager(GAME_TYPE.WATERMELON) as WatermelonMgr;
                            if (watermelonMgr != null)
                            {
                                if (watermelonMgr.State == WatermelonState.PlayAync)
                                {
                                    CommonPopup.PopupMessage("Leave", StringManager.Get("UI_GAME"), StringManager.Get("UI_LEAVE_GAME"), POPUP_TYPE.OK_CANCEL, () =>
                                    {
                                        NetworkManager.Send(new C2G.RQ_GAME_LEAVE());
                                    });
                                }
                                else
                                {
                                    NetworkManager.Send(new C2G.RQ_GAME_LEAVE());
                                }
                            }                            
                        }                        
                    }                    
                }                
            }
        }      */  
    }

    public void SetOption()
    {
           
    }

    public void CheckPermission()
    {
#if !UNITY_EDITOR && UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
        {
            Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
        }
        else 
        {
            Debug.Log("Permission already granted 1");
        }

        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead) && AndroidVersionCheck() < 30)
        {
            Permission.RequestUserPermission(Permission.ExternalStorageRead);
        }
        else
        {
            Debug.Log("Permission already granted 2");
        }

        // Android 10 ���Ͽ����� ���� ������ ��û
        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite) && AndroidVersionCheck() < 30)
        {
            Permission.RequestUserPermission(Permission.ExternalStorageWrite);
        }
        else
        {
            Debug.Log("Permission already granted 3");
        }

        // �ȵ���̵� 11 �̻󿡼��� ���� ���� ��û
        if (AndroidVersionCheck() >= 30)
        {
            if (Permission.HasUserAuthorizedPermission("android.permission.MANAGE_EXTERNAL_STORAGE"))
            {
                Debug.Log("Permission already granted 4");
            }
            else
            {                
                Permission.RequestUserPermission("android.permission.MANAGE_EXTERNAL_STORAGE");
            }
        }

        /*if (AndroidVersionCheck() >= 29)
        {
            if (!Permission.HasUserAuthorizedPermission("android.permission.ACCESS_MEDIA_LOCATION"))
            {
                if (ShouldShowRequestPermissionRationale("android.permission.ACCESS_MEDIA_LOCATION"))
                {
                    // ������ �ź�������, �ٽ� ���� ������ ���õ��� �ʾ��� ���
                    // ���� ��û�� �� �� ����
                    Debug.Log("Requesting permission with rationale.");
                    Permission.RequestUserPermission("android.permission.ACCESS_MEDIA_LOCATION");
                }
                else
                {
                    // "�ٽ� ���� ����"�� ���õǾ��ų� ó�� ��û�ϴ� ���
                    Debug.Log("Directing user to settings.");
                    OpenAppSettings();
                }          
            }
        }*/
#endif
    }




#if !UNITY_EDITOR && UNITY_ANDROID
    bool ShouldShowRequestPermissionRationale(string permission)
    {
        using (var activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            var context = activity.GetStatic<AndroidJavaObject>("currentActivity");
            return context.Call<bool>("shouldShowRequestPermissionRationale", permission);
        }
    }

    public int AndroidVersionCheck()
    {
        AndroidJavaClass version = new AndroidJavaClass("android.os.Build$VERSION");
        int sdk = version.GetStatic<int>("SDK_INT");
        return sdk;
    }

    void OpenAppSettings()
    {
        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent", "android.settings.APPLICATION_DETAILS_SETTINGS");
            string packageName = currentActivity.Call<string>("getPackageName");
            AndroidJavaObject uri = new AndroidJavaClass("android.net.Uri").CallStatic<AndroidJavaObject>("parse", "package:" + packageName);
            intent.Call<AndroidJavaObject>("setData", uri);
            currentActivity.Call("startActivity", intent);
        }
    }
#endif   

    void HandleLog(string logString, string stackTrace, LogType type)
    {        
        if (type == LogType.Exception || type == LogType.Error)
        {
            Debug.Log($"[LogType_{type} : {logString}");
            // ���� �� ���� �α� ó��
            Debug.LogError(stackTrace);
        }
    }

    public void OnDestroy()
    {
        DOTween.KillAll();
        Application.logMessageReceived -= HandleLog;
    }
}


