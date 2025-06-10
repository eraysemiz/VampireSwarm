using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class UIManager : MonoBehaviour
{
    public void QuitGame()
    {
        #if UNITY_EDITOR
                // Editörde Play modunu durdurmak için
                EditorApplication.isPlaying = false;
        #else
                // Build’da uygulamayý sonlandýrýr
                Application.Quit();
        #endif
    }
}