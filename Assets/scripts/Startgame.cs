using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class StartGame : MonoBehaviour
{
    public AudioSource audioSource;
    public Button startButton;   
    public Button infoButton;   
    public Button X;   
    public Image infom;
    void Start()
    {
        Time.timeScale = 0f;
        startButton.onClick.AddListener(startButtonClick);
        infoButton.onClick.AddListener(infoButtonClick);
        X.onClick.AddListener (XButtonClick);
    }

    void startButtonClick()
    {
        audioSource.Play();
        Time.timeScale = 1f;
        gameObject.SetActive(false);
    }
    void infoButtonClick()
    { 
        infom.gameObject.SetActive(true);
        X.gameObject.SetActive(true);
    }
    void XButtonClick()
    {
        infom.gameObject.SetActive(false);
        X.gameObject.SetActive(false);
    }
    void Update()
    {
        
    }
}

// Update is called once per frame

