using System.Collections;
using System.Collections.Generic;
using System.Net.Cache;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using static AuthService;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class TelemetryManager : MonoBehaviour
{
    //URL
    string serverURL = "http://localhost:3000/rafHousingTelemtry";

    //QUEUE OBJECT--> ideal for buffering. Sends info as it comes and when it can. Puts info in queue sends it when it's appropriate.
    //Queue dictionary --> used to store info that will be sent.We're sending a JSON and saving it on server side. Backend POST matches localhost:3000/rafHousingTelemtry
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
        else
        {
            Destroy(gameObject);
        }
    }

    //LOGEVENT --> called when the session starts (login button) and when it ends (exit or new game).
    //eventName --> taken in as a param, then fed in as value in key/value pair. 

    public void LogEvent(string eventName, string eventTime, Dictionary<string, object> parametersDictionary = null)
    {
        if(parametersDictionary ==  null)                                                                 //if parameters are null, then make new dictionary
        {
            parametersDictionary = new Dictionary<string, object>();
        }

        //add key value pairs to dictionary & add to queue                                                  //key - name of data        //value - data
        parametersDictionary["eventName"] = eventName;
        parametersDictionary["sessionId"] = System.Guid.NewGuid().ToString();
        parametersDictionary["device"] = System.DateTime.UtcNow.ToString("o");                  //"o" --> format of time yyyymmddtime since 1970

        eventQueue.Enqueue(parametersDictionary);

        //Send - only send if we're not already sending.
        if(!isSending)
        {
            StartCoroutine(SendEvents(SessionManager.Instance.AuthToken));
        }
    }

    private IEnumerator SendEvents(string token)
    {
        isSending = true;

        //send events in the Queue
        //Note: can't pass Dictionary as Json. Must be a list or primative data. Use Function SerializationWrapper to turn dictionary into two list (keys & values)
        //call the constructor of the SerializationWrapper class and pass in dictionary. SerializationWrapper has keys as 1 list property and values as another 
        while (eventQueue.Count > 0)
        {
            Dictionary<string, object> currentEvent = eventQueue.Dequeue();
            string payload = JsonUtility.ToJson(new SerializeationWrapper(currentEvent));

            using (UnityWebRequest request = new UnityWebRequest(serverURL, "POST"))
            {
                //array of bytes
                byte[] bodyRaw = Encoding.UTF8.GetBytes(payload); //UTF8 -> most character have byte version which removes potential hazard for unknown characters
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();

                request.SetRequestHeader("Content-Type", "application/json");
                //TODO: add bearer token ---- "bearer adfadfadadsfadf"
                request.SetRequestHeader("Authorization", "Bearer " + token);

                //send the request
                yield return request.SendWebRequest();

                //check if the request comes back with error
                if (request.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.Log($"Error in Connection. Error {request.error}");
                    eventQueue.Enqueue(currentEvent); //if there is an error, then put it back into the queue and continue to next part of queue
                    break;
                }
                else
                {
                    Debug.Log("Request sent : " + payload);
                }
            }
            yield return new WaitForSeconds(0.1f); //throlte the request being sent and prevent too many request being sent at same time
        }
        isSending = false;
    }

    //need to convert to JSON, but can't do this with standard unity library. Here, we're definding a class that is used to wrap the data
    //turn the dictionarty into a list of keys, and a list of values. We're sending a JSON with a list of keys another with a list of values
    [System.Serializable]
    private class SerializeationWrapper
    {
        public List<string> keys = new List<string>();
        public List<string> values = new List<string>();

        //CONSTRUCTOR --> this is call when a new class is created. The paramaters passed in are the Dictionary that will be split into 2 lists
        public SerializeationWrapper(Dictionary<string, object> parameters)
        {
            foreach (var kvp in parameters)
            {
                keys.Add(kvp.Key);
                values.Add(kvp.Value.ToString());
            }
        }
    }
}
