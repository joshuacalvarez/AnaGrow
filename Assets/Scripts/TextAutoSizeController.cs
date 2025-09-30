using UnityEngine;
using TMPro;

public class TextAutoSizeController : MonoBehaviour
{
    public TMP_Text[] TextObjects;

    /// <summary>
    /// Force each text object to shrink so its content fits inside its bounds.
    /// </summary>
    public void UpdateFontSizes()
    {
        if (TextObjects == null || TextObjects.Length == 0)
            return;

        foreach (TMP_Text tmp in TextObjects)
        {
            if (tmp == null) continue;

            // Let TMP calculate auto-size once
            tmp.enableAutoSizing = true;
            tmp.ForceMeshUpdate();

            // Lock in the size TMP chose
            float fittedSize = tmp.fontSize;
            tmp.enableAutoSizing = false;
            tmp.fontSize = fittedSize;

            // Shrink further if it’s still overflowing
            while (tmp.isTextOverflowing && tmp.fontSize > 1f)
            {
                tmp.fontSize -= 0.5f;
                tmp.ForceMeshUpdate();
            }
        }
    }
}
