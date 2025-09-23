using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Key : MonoBehaviour
{

    [Header(" Elements ")]
    [SerializeField] private Text keyText;
    private char key;

    public void SetKey(char key)
    {
        this.key = key;
        keyText.text = key.ToString();
    }

    public Button GetButton()
    {
        return GetComponent<Button>();
    }
}
