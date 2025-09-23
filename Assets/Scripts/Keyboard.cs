using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Keyboard : MonoBehaviour
{
    [Header("Fields in order (4,5,6)")]
    [SerializeField] private TMP_InputField[] fields;

    [Header("Max lengths for each field")]
    [SerializeField] private int[] maxLens = { 4, 5, 6 };

    [Header("Options")]
    [SerializeField] private bool uppercase = true;

    [Header("Events")]
    public UnityEvent<string, string, string> onAllSubmitted; // (word4, word5, word6)

    private int idx = 0; // which field we're typing into

    void Awake()
    {
        // Make fields display-only but visually normal
        foreach (var f in fields)
        {
            if (!f) continue;
            f.readOnly = true;           // block manual typing
            f.interactable = true;       // keep normal tint (not grey)
            f.text = string.Empty;
        }
        Focus(idx);
    }

    public void PressChar(string c)
    {
        if (string.IsNullOrEmpty(c)) return;
        if (c == " ") return; // no spaces
        if (uppercase) c = c.ToUpper();

        var f = fields[idx];
        if (!f) return;

        int max = maxLens[idx];
        if (f.text.Length >= max) { TryAdvance(); return; }

        f.text += c;
        f.ForceLabelUpdate();

        if (f.text.Length >= max) TryAdvanceOrSubmit();
    }

    public void PressBackspace()
    {
        var f = fields[idx];
        if (!f) return;

        if (f.text.Length > 0)
        {
            f.text = f.text.Substring(0, f.text.Length - 1);
            f.ForceLabelUpdate();
            return;
        }
    }

    public void PressEnter()
    {
        // Only submit when all fields are full
        for (int i = 0; i < fields.Length; i++)
            if (fields[i].text.Length != maxLens[i]) return;

        onAllSubmitted?.Invoke(fields[0].text, fields[1].text, fields[2].text);
    }

    public void ClearAll()
    {
        foreach (var f in fields)
        {
            if (!f) continue;
            f.text = string.Empty;
            f.ForceLabelUpdate();
        }
        idx = 0;
        Focus(idx);
    }

    // --- helpers ---
    private void TryAdvance()
    {
        if (idx < fields.Length - 1) { idx++; Focus(idx); }
    }

    private void TryAdvanceOrSubmit()
    {
        if (idx < fields.Length - 1)
        {
            idx++;
            Focus(idx);
        }
        else
        {
            // last field filled — auto-submit or wait for Enter
            PressEnter();
        }
    }

    private void Focus(int i)
    {
        // Optional: visually indicate current field (e.g., by caret color or a highlight Image)
        // If your field has an Image background, you can tint it here.
        // Example: (assuming you added an Image next to the InputField)
        // var img = fields[i].GetComponent<UnityEngine.UI.Image>();
        // if (img) img.color = new Color32(255,255,255,255);
    }
}
