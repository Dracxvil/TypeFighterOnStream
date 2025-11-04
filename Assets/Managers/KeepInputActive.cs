using TMPro;
using UnityEngine;

public class KeepInputActive : MonoBehaviour
{
    public TMP_InputField inputField;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!inputField.isFocused)
        {
            inputField.ActivateInputField();
        }
    }
}
