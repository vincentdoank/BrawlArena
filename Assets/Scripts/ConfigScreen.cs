using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ConfigScreen : MonoBehaviour
{
    public string nextSceneName = "Gameplay";
    public TMP_InputField ipInputField;
    public TMP_InputField portInputField;
    public Button connectButton;

    private void Start()
    {
        connectButton.onClick.AddListener(Connect);
        if (PlayerPrefs.HasKey("ip"))
        {
            ipInputField.text = PlayerPrefs.GetString("ip");
        }
        if (PlayerPrefs.HasKey("port"))
        {
            portInputField.text = PlayerPrefs.GetString("port");
        }
    }


    private void Connect()
    {
        PlayerPrefs.SetString("ip", ipInputField.text);
        PlayerPrefs.SetString("port", portInputField.text);
        PlayerPrefs.Save();
        SceneManager.LoadScene(nextSceneName);
    }
}
