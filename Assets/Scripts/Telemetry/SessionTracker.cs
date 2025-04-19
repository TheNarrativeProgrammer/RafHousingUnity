using System.Collections.Generic;
using UnityEngine;

public class SessionTracker : MonoBehaviour
{
    private float sesssionStartTimeInSeconds = 0f;
    private string eventTimeDateLastGameLaunch = "";
    private string eventTimeDateLastGameEnd = "";
    private string eventName = "";

    public static SessionTracker Instance {  get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
    }

    public void OnLogin()
    {
        sesssionStartTimeInSeconds = Time.time;

        eventName = "session_start";
        eventTimeDateLastGameLaunch = System.DateTime.UtcNow.ToString("o");

        TelemetryManager.Instance.LogEvent(eventName, eventTimeDateLastGameLaunch, new Dictionary<string, object>
        {
            {"Heading", "LaunchGame" } //this is the contents of the Dictionary param 
        });

    }

    public void OnLogOut()
    {
        eventName = "session_end";
        eventTimeDateLastGameEnd = System.DateTime.UtcNow.ToString("o");

        float sessionDuration = Time.time - sesssionStartTimeInSeconds;
        TelemetryManager.Instance.LogEvent(eventName, eventTimeDateLastGameEnd, new Dictionary<string, object>
        {
            {"Heading", "EndGame" }, //this is the contents of the Dictionary param 
            {"duration_seconds", sessionDuration },
        });
    }

    public float GetStartTime()
    {
        return sesssionStartTimeInSeconds; 
    }
}
