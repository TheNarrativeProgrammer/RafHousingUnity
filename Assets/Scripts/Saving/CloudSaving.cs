using UnityEngine;
using System.Collections;
using UnityEngine.Networking; //used for making calls 


public class CloudSaving : MonoBehaviour
{
    
    public LocalSaveManager localSaveManager;                                       //Get instance of local save manager

    private string serverUrl = "http://localhost:3000/syncLocalClientWithCloud";    //url that connects to POST in backend

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            SyncWithCloud();
        }
    }

    public void SyncWithCloud()
    {
        StartCoroutine(SyncRoutine());
    }

    private IEnumerator SyncRoutine()
    {
        
        PlayerData localData = localSaveManager.playerData;             //Get local data. This is stored in the LocalSaveManager class's playerData object
        string localJson = JsonUtility.ToJson(localData);               //Convert localData TO Json, stringifying it, and save it to var.

        
        WWWForm form = new WWWForm();                                   //Create new WWWForm object --> used to make the call

                                                                        //You can add Fields OR add headers.
                                                                        //Headers --> are NOT part of the body. They are a key/value pair outside the body
        form.AddField("plainJson", localJson);                          //AddField --> are inside the body. This is store in req body. - req.body.plainJson 
                                                                        //Field --> a key / value pair within the payload. 

        using (UnityWebRequest www = UnityWebRequest.Post(serverUrl, form))//make call
        {
            yield return www.SendWebRequest();                              //wait here until we get a request back
            
            if (www.result != UnityWebRequest.Result.Success)               //check --> if the result from the request is not successful
            {
                Debug.Log($"syncError : {www.error} ");
            }
            else
            {
                string serverResponse = www.downloadHandler.text;           //downloadHandler--> gets the response from the web request. This stringifies the JSON
                                                                            //Convert the stringified JSON to an object.
                                                                            //the repsonse, var serverResponse, and casting it into a PlayerData object
                PlayerData serverData = JsonUtility.FromJson<PlayerData>(serverResponse);
                Debug.Log(serverData);
                localSaveManager.playerData = serverData;                   //SAVE --> change local data to the data received from server
                localSaveManager.SaveToLocal();                             //take playerData we just overwrote and saves the data to the .js file

                Debug.Log("save successul");
            }
        }
    }
}
