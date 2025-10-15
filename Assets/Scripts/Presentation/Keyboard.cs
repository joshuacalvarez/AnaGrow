using AnagrowLoader.Models;
using Assets.Scripts.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;

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
    [SerializeField] private Color solvedBg = new Color(0.88f, 0.96f, 1f, 1f);

    [SerializeField] private Color wrongText = new Color(0.90f, 0.10f, 0.10f, 1f);
    [SerializeField] private float shakeDuration = 0.4f;
    [SerializeField] private float shakeMagnitude = 8f;

    [SerializeField] private CanvasGroup lengthPopup;
    [SerializeField] private TMP_Text lengthPopupText;
    [SerializeField] private float fadeDuration = 0.3f;
    [SerializeField] private float popupStaySeconds = 1f;


    private SetHandler setHandler = new SetHandler();
    private int idx = 0;
    private int focusField = -1;
    private bool[] solved;

    private Dictionary<char, Button> keyButtons = new Dictionary<char, Button>();
    [SerializeField] private Button enterKey;
    [SerializeField] private Button backKey;

    void Awake()
    {

        if (lengthPopup)
        {
            lengthPopup.alpha = 0f;
            lengthPopup.gameObject.SetActive(false);
        }

        foreach (var button in GetComponentsInChildren<Button>(true))
        {
            var textComp = button.GetComponentInChildren<TMP_Text>();
            if (!textComp) continue;

            string label = textComp.text.Trim().ToUpper();

            // Skip non-letter buttons (like Enter/Backspace)
            if (label.Length != 1 || !char.IsLetter(label[0])) continue;

            char letter = label[0];
            if (!keyButtons.ContainsKey(letter))
                keyButtons.Add(letter, button);
        }

        solved = new bool[fields.Length];
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

        //Incorrect Word Length
        int requiredLength = maxLens[focusField];
        if (field.text.Length != requiredLength)
        {
            StartCoroutine(ShowLengthPopup($"Enter {requiredLength} letters"));
            StartCoroutine(WrongGuessFeedback(field));
            return;
        }

        //Word is Right
        if (setHandler.checkWord(fields[focusField].text, focusField))
        {
            //Make buttons blue
            string g = field.text.ToUpperInvariant();
            foreach (char letter in g)
            {
                SetKeyColor(letter);
            }


            //Make the textfield solved
            solved[focusField] = true;
            field.textComponent.color = Color.blue;
            field.readOnly = true;
            field.DeactivateInputField();


            if (CheckWinCondition())
            {
                DisableKeyboard();
                Focus(++focusField);
                //TriggerWin();
            }
            else
            {
                if(focusField == 2)
                {
                    focusField = (solved[0] == true) ? 1 : 0;
                    Focus(focusField);
                }
                else
                {
                    Focus(++focusField);
                }
            }
        }
        else
        {
            StartCoroutine(WrongGuessFeedback(field));
        }
    }

    private void DisableKeyboard()
    {
        foreach(KeyValuePair<char, Button> key in keyButtons){
            key.Value.enabled = false;
        }
        enterKey.enabled = false;
        backKey.enabled = false;
    }

    private bool CheckWinCondition()
    {
        foreach(bool isSolved in solved)
        {
            if(isSolved == false)
            {
                return false;
            }
        }
            return true;
    }

    private void SetKeyColor(char letter)
    {
        letter = Char.ToUpper(letter);
        if (keyButtons.TryGetValue(letter, out var button))
        {
            var image = button.GetComponent<Image>();
            if (image)
                image.color = new Color(0f / 255f, 138f / 255f, 213f / 255f); ;

            var text = button.GetComponentInChildren<TMP_Text>();
            if (text)
                text.color = Color.white;
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
        yield return null;
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

            if (solved[k])
            {
                f.textComponent.color = new Color(6f / 255f, 133f / 255f, 196f / 255f);
                f.readOnly = true;
                f.interactable = false;
                f.ReleaseSelection();

                continue;
            }

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

    private System.Collections.IEnumerator WrongGuessFeedback(TMP_InputField f)
    {
        if (!f) yield break;

        // cache
        var rt = f.GetComponent<RectTransform>();
        if (!rt) yield break;

        Color originalText = f.textComponent.color;
        Vector2 originalPos = rt.anchoredPosition;

        // flash red
        f.textComponent.color = wrongText;
        f.caretColor = wrongText;

        // optional: temporarily block typing during shake
        bool wasInteractable = f.interactable;
        f.interactable = false;

        float t = 0f;
        while (t < shakeDuration)
        {
            // random shake
            float offsetX = UnityEngine.Random.Range(-1f, 1f) * shakeMagnitude;
            float offsetY = UnityEngine.Random.Range(-1f, 1f) * (shakeMagnitude * 0.6f);
            rt.anchoredPosition = originalPos + new Vector2(offsetX, offsetY);

            t += Time.unscaledDeltaTime;
            yield return null;
        }

        // restore
        rt.anchoredPosition = originalPos;
        f.textComponent.color = originalText;
        f.caretColor = activeCaret;
        f.interactable = wasInteractable;

        f.ActivateInputField();
        CollapseSelectionToEnd(f);
    }

    private IEnumerator FadeOut(CanvasGroup cg)
    {
        if (!cg) yield break;
        cg.interactable = false;
        cg.blocksRaycasts = false;

        float t = 0f;
        float start = cg.alpha, end = 0f;
        while (t < fadeDuration)
        {
            cg.alpha = Mathf.Lerp(start, end, t / fadeDuration);
            t += Time.unscaledDeltaTime;
            yield return null;
        }
        cg.alpha = 0f;
        cg.gameObject.SetActive(false);
    }

    private IEnumerator FadeIn(CanvasGroup cg)
    {
        if (!cg) yield break;
        cg.gameObject.SetActive(true);
        cg.blocksRaycasts = true;
        cg.interactable = true;

        float t = 0f;
        float start = cg.alpha, end = 1f;
        while (t < fadeDuration)
        {
            cg.alpha = Mathf.Lerp(start, end, t / fadeDuration);
            t += Time.unscaledDeltaTime;
            yield return null;
        }
        cg.alpha = 1f;
    }

    private IEnumerator ShowLengthPopup(string msg)
    {
        if (!lengthPopup) yield break;

        if (lengthPopupText) lengthPopupText.text = msg;

        yield return StartCoroutine(FadeIn(lengthPopup));
        yield return new WaitForSecondsRealtime(popupStaySeconds);
        yield return StartCoroutine(FadeOut(lengthPopup));
    }



    private void HideButton()
    {
        lengthPopup.alpha = 0f;
        lengthPopup.interactable = false;
        lengthPopup.blocksRaycasts = false;
    }

    private void ShowButton()
    {
        lengthPopup.alpha = 1f;
        lengthPopup.interactable = true;
        lengthPopup.blocksRaycasts = true;
    }


}