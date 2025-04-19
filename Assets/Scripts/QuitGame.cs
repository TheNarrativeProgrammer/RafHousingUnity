using UnityEngine;
using UnityEngine.UI;

public class QuitGame : MonoBehaviour
{

    public void OnLogOut()
    {
        SessionTracker.Instance.OnLogOut();
        Application.Quit();
        
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        
    }
}
