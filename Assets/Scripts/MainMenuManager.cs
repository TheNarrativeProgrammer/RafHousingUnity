using UnityEngine;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Sign UP Fields")]
    public TMP_InputField signUpUsernameField;
    public TMP_InputField signUpEmailField;
    public TMP_InputField signUpPasswordField;


    [Header("Sign In Fields")]
    public TMP_InputField signInUsernameField;
    public TMP_InputField signInPasswordField;


    [Header("Auth References")]
    public AuthService authService;

    //where to go once we log in
    [Header("login scene")]
    public string gameSceneName = "Game";


    //Events for buttons
    public void SignUpButton()
    {
        string username = signUpUsernameField.text.Trim();
        string email = signUpEmailField.text.Trim();
        string password = signUpPasswordField.text.Trim();

        //OnSignUpCompleted --> callback func that excutes once SignUp fun is done
        StartCoroutine(authService.SignUp(username, email, password, OnSignUpCompleted));
    }

    private void OnSignUpCompleted(bool success, string responseData)
    {
        if(success)
        {
            Debug.Log("Sign up successful: " + responseData);
        }
        else
        {
            Debug.Log("sign up failed" + responseData);
        }
    }

    public void SignInButton()
    {
        string username = signInUsernameField.text.Trim();
        string password = signInPasswordField.text;


        StartCoroutine(authService.SignIn(username, password, OnSignInCompleted));
    }

    private void OnSignInCompleted(bool success, string responseData)
    {
        if(success)
        {
            //if we signed in correctly, then get the Class signInResponse and parse to Json
            AuthService.SignInResponse signResp = JsonUtility.FromJson<AuthService.SignInResponse>(responseData);

            if (!string.IsNullOrEmpty(signResp.token))
            {
                //from response, then store the token from response
                SessionManager.Instance.SetAuthToken(signResp.token);
                Debug.Log("login successful" + signResp.token);

                //SceneManager.LoadScene(gameSceneName);

            }
            else
            {
                Debug.Log("no token in response " + responseData);
            }
            
        } else
        {
            Debug.LogError("login failed" + responseData);
        }

        //Note: this is overrideing the problem with logging in. Remove this line once problem is fixed
        SceneManager.LoadScene(gameSceneName);

    }
}
