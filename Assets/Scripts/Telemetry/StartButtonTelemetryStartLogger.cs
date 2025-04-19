using System;
using UnityEngine;
using UnityEngine.UI;

public class StartButtonTelemetryStartLogger : MonoBehaviour
{

    private string eventTimeDate;
    [SerializeField] private float secondsSinceGameLaunch = 0.0f;



    private void OnStartButtonClicked()
    {
        Debug.Log("start session");
        string EventName = "UserInput";
        eventTimeDate = System.DateTime.UtcNow.ToString("o");

        secondsSinceGameLaunch = Time.time - SessionTracker.Instance.GetStartTime();

        TelemetryManager.Instance.LogEvent(EventName, eventTimeDate, new System.Collections.Generic.Dictionary<string, object>
        {
            {"Heading", "StartButtonClicked" }, //this is the contents of the Dictionary param
            {"TimeSinceLaunch", secondsSinceGameLaunch}
        });

    }
}
