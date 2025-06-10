using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class UIManager : MonoBehaviour
{
    public void QuitGame()
    {
        #if UNITY_EDITOR
                // Edit�rde Play modunu durdurmak i�in
                EditorApplication.isPlaying = false;
        #else
                // Build�da uygulamay� sonland�r�r
                Application.Quit();
        #endif
    }
}