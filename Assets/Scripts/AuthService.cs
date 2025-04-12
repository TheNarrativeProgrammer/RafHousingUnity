using System.Collections;
using UnityEngine;
using UnityEngine.Networking; //used for unity web connection
using System.Text; //needed for Encoding.UTF8.GetBytes(jsonData
public class AuthService : MonoBehaviour
{
    //get the Port URL from the scriptable object
    [Header("URL References")]
    public BackendConfig config;

    public IEnumerator SignUp(string username, string email, string password, System.Action<bool, string> callback)
    {
        //info to send to backend
        var payload = new SignUpRequet { username = username, email = email, password = password };
        //stringify payload
        string jsonData = JsonUtility.ToJson(payload);
        //define URL
        string url = config.baseUrl + "/api/auth/signup";

        using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
        {
            //convert the json we created above to bytes that are stored in handler
            www.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonData));
            www.downloadHandler = new DownloadHandlerBuffer();
            //"Content-Type" --> key
            //"application/json" --> value
            www.SetRequestHeader("Content-Type", "application/json");

            //ASYNC PART
            yield return www.SendWebRequest();

            //check what data we get back
            if(www.result == UnityWebRequest.Result.Success)
            {
                //if success, then call the callback/
                //downloadhandler contains the info
                callback(true, www.downloadHandler.text);
            }
            else
            {
                callback(false, www.downloadHandler.text);
            }
        }
    }

    //SIGN IN REQUEST

    public IEnumerator SignIn(string username, string password, System.Action<bool, string> callback)
    {
        var payload = new SignInRequest { username = username, password = password };
        string jsonData = JsonUtility.ToJson(payload);

        string url = config.baseUrl + "api/auth/signin";

        using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
        {
            www.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonData));
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                callback(true, www.downloadHandler.text);
            }
            else
            {
                callback(false, www.downloadHandler.text);
            }
        }
    }



    //serialization --> this data can be saved (usually in json or binary format)
    //this can be convertd in json
    [System.Serializable]
    public class SignUpRequet
    {
        public string username;
        public string email;
        public string password;
    }

    [System.Serializable]

    public class SignInRequest
    {
        public string username;
        public string password;
    }


    //we get a token back when we try to sign in.
    [System.Serializable]
    public class SignInResponse
    {
        public string token;
        public string message;
    }



}


