using AnagrowLoader.Models;
using Assets.Scripts.Business;
using System;
using System.Linq;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Keyboard : MonoBehaviour
{
    [Header("Fields in order (4,5,6)")]
    [SerializeField] private TMP_InputField[] fields;

    [Header("Max lengths for each field")]
    [SerializeField] private int[] maxLens = { 4, 5, 6 };

    [Header("Options")]
    [SerializeField] private bool uppercase = true;

    [Header("Events")]
    public UnityEvent<string, string, string> onAllSubmitted;

    public Button button;

    public TMP_Text hint1;
    public TMP_Text hint2;
    public TMP_Text hint3;

    [Header("Focus Styling")]
    [SerializeField] private Color activeBg = new Color(1f, 1f, 1f, 1f);
    [SerializeField] private Color inactiveBg = new Color(1f, 1f, 1f, 0.45f);
    [SerializeField] private Color activeText = new Color(0f, 0f, 0f, 1f);
    [SerializeField] private Color inactiveText = new Color(0f, 0f, 0f, 0.5f);
    [SerializeField, Range(0f, 1f)] private float placeholderAlpha = 0.6f;
    [SerializeField] private Color activeCaret = new Color(0f, 0f, 0f, 1f);
    [SerializeField] private Color hiddenCaret = new Color(0f, 0f, 0f, 0f);

    private SetHandler setHandler = new SetHandler();
    private int idx = 0;
    private int focusField = -1;

    void Awake()
    {
        for (int i = 0; i < fields.Length; i++)
        {
            var f = fields[i];
            if (!f) continue;

            f.readOnly = true;
            f.interactable = true;
            f.shouldHideMobileInput = true;
            f.onFocusSelectAll = false;

            var sel = f.selectionColor;
            sel.a = 0f;
            f.selectionColor = sel;

            f.text = string.Empty;

            int capture = i;
            f.onSelect.AddListener(_ => Focus(capture));
        }

        Focus(0);
    }

    private void UpdateHints(WordSet currentSet)
    {
        hint1.text = currentSet.Set.ElementAt(0).Hint;
        hint2.text = currentSet.Set.ElementAt(1).Hint;
        hint3.text = currentSet.Set.ElementAt(2).Hint;
    }

    public void PressChar(string c)
    {
        if (string.IsNullOrEmpty(c)) return;
        if (c == " ") return; // no spaces
        if (uppercase) c = c.ToUpper();

        var f = fields[idx];
        if (!f) return;

        int max = maxLens[idx];
        if (f.text.Length >= max) { return; }

        f.text += c;
        f.ForceLabelUpdate();

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
        var field = fields[focusField];
        //for (int i = 0; i < fields.Length; i++)
        //    if (fields[i].text.Length != maxLens[i]) return;

        ////onAllSubmitted?.Invoke(fields[0].text, fields[1].text, fields[2].text);

        if(setHandler.checkWord(fields[focusField].text, focusField))
        {
            field.textComponent.color = Color.blue;
            field.readOnly = true;
            Focus(++focusField);
        }


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


    private void CollapseSelectionToEnd(TMP_InputField f)
    {
        int end = f.text.Length;
        f.caretPosition = end;
        f.stringPosition = end;

        f.selectionStringAnchorPosition = end;
        f.selectionStringFocusPosition = end;

        f.ForceLabelUpdate();
    }
    private System.Collections.IEnumerator CollapseNextFrame(TMP_InputField f)
    {
        yield return null; // wait one frame
        if (!f) yield break;
        CollapseSelectionToEnd(f);
    }


    private void Focus(int i)
    {
        if (fields == null || fields.Length == 0) return;
        idx = Mathf.Clamp(i, 0, fields.Length - 1);

        for (int k = 0; k < fields.Length; k++)
        {
            var f = fields[k];
            if (!f) continue;

            bool isActive = (k == idx);

            var bg = f.GetComponent<UnityEngine.UI.Image>();
            if (bg) bg.color = isActive ? activeBg : inactiveBg;

            if (f.textComponent)
                f.textComponent.color = isActive ? activeText : inactiveText;

            if (f.placeholder is TMP_Text ph)
            {
                var baseCol = isActive ? activeText : inactiveText;
                ph.color = new Color(baseCol.r, baseCol.g, baseCol.b, placeholderAlpha);
            }

            f.caretColor = isActive ? activeCaret : hiddenCaret;

            if (isActive)
            {
                f.ActivateInputField();

                int end = f.text.Length;
                f.caretPosition = end;
                f.stringPosition = end;
                f.selectionStringAnchorPosition = end;
                f.selectionStringFocusPosition = end;
                f.ForceLabelUpdate();

                StartCoroutine(CollapseNextFrame(f));
                focusField = k;
            }
            else
            {
                f.ReleaseSelection();
                f.DeactivateInputField();
            }
        }
    }

    public void pressTempNextSetButton()
    {
        setHandler.getNextSet();
        WordSet newSet = setHandler.getCurrentWordSet();
        UpdateHints(newSet);

    }
}
