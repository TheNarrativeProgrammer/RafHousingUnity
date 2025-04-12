using System.Collections.Generic;
using System.Net.Cache;
using UnityEngine;

public class TelemetryManager : MonoBehaviour
{
    //URL
    string serverURL = "http://localhost:3000/rafHousingTelemtry";

    //queue dictionary
        //key --> string, this will hold the name of the data being sent.
        //object --> objects can hold many different things. Here, we're passing strings as the objects, but it could be a class or any object.
    private Queue<Dictionary<string, object>> eventQueue;

    private bool isSending = false;

    //SINGLETON INSTANCE OF TELEMETRY MANAGER
    public static TelemetryManager Instance { get; private set; }

    private void Awake()
    {
        if(Instance == null) //if no Instance exist, then set the Instant to this TelemetryManager and initialize the Queue.
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            eventQueue = new Queue<Dictionary<string, object>>(); 
        }
    }

}
