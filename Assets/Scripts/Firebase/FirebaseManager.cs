using UnityEngine;
using Firebase;
using Firebase.Messaging;
using Firebase.Analytics;

public class FirebaseManager : MonoBehaviour
{
    FirebaseApp _app;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                _app = FirebaseApp.DefaultInstance;
                FirebaseMessaging.TokenReceived += OnTokenReceived;
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + task.Result);
            }
        });

    }

    private void OnTokenReceived(object sender, TokenReceivedEventArgs e)
    {
        Debug.LogFormat("Token : {0}", e.Token);
    }

    private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
    {
        Debug.LogFormat("From: {0},Title: {1}, Text: {2}",
            e.Message.From,
            e.Message.Notification.TitleLocalizationArgs,
            e.Message.Notification.Body);
    }

    public static void LogEvent(string eventName, string paramName, int prameValue)
    {
        FirebaseAnalytics.LogEvent(eventName, paramName, prameValue);
    }
}
