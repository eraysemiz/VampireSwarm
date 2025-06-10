using UnityEngine;

public class CustomCursor : MonoBehaviour
{
    public Texture2D cursorTexture;

    // �mlecin t�klama noktas�n�n (hotspot) offset�i
    public Vector2 hotspot = Vector2.zero;

    void Start()
    {
        // �zel imleci ayarla

        if (cursorTexture)
        {
            Cursor.SetCursor(cursorTexture, hotspot, CursorMode.Auto);

            // �mleci g�r�n�r k�l
            Cursor.visible = true;

            // �mle� konum kilidini kald�r (fare ekran�n ortas�nda sabit kalmamas� i�in)
            Cursor.lockState = CursorLockMode.None;
        }
        
    }
}
