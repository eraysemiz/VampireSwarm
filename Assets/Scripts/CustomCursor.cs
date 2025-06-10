using UnityEngine;

public class CustomCursor : MonoBehaviour
{
    public Texture2D cursorTexture;

    // Ýmlecin týklama noktasýnýn (hotspot) offset’i
    public Vector2 hotspot = Vector2.zero;

    void Start()
    {
        // Özel imleci ayarla

        if (cursorTexture)
        {
            Cursor.SetCursor(cursorTexture, hotspot, CursorMode.Auto);

            // Ýmleci görünür kýl
            Cursor.visible = true;

            // Ýmleç konum kilidini kaldýr (fare ekranýn ortasýnda sabit kalmamasý için)
            Cursor.lockState = CursorLockMode.None;
        }
        
    }
}
