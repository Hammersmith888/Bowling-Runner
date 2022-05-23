using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Text pinsText;
    private void Start() {
        int pins = PlayerPrefs.GetInt("Pins");
        pinsText.text = pins.ToString();
    }
    
    public void PlayGame()
    {
        SceneManager.LoadScene(0);
        
    }
}
