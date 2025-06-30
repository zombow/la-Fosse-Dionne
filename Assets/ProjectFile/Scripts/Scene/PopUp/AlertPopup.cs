using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class AlertPopup : MonoBehaviour
{
    public TextMeshProUGUI AlertTextBox;
    public Button ExiteButton;
    void Start()
    {
        ExiteButton.onClick.AddListener(OnExit);
        
    }
    public void SetText(string text)
    {
        AlertTextBox.text = text;
    }
    
    private void OnExit()
    {
        gameObject.SetActive(false);
    }
}
