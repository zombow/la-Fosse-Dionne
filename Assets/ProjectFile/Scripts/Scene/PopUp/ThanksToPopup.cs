using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThanksToPopup : MonoBehaviour
{
    public Button exitButton;
    // Start is called before the first frame update
    void Start()
    {
        exitButton.onClick.AddListener(OnExit);
    }

    private void OnExit()
    {
        gameObject.SetActive(false);
    }
    
}
