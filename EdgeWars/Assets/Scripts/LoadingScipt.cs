﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScipt : MonoBehaviour
{
    // Start is called before the first frame update
    public Text loadingtext;
    private bool gotoStart = false;

        void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.Auth.CurrentUser == null && GameManager.instance.gameIsLoading && !gotoStart)
        {
            gotoStart = true;
            StartCoroutine(ShowStartGamePanelAfterLoad());
        }

        if (GameManager.instance.gameIsLoading)
        {
            loadingtext.color = new Color(255 ,255 ,255 , (Mathf.Sin(Time.time * 5) + 1.0f) / 2.0f);
        }

        if (GameManager.instance.taskCounter >= 2 && GameManager.instance.gameIsLoading)
        {
            
            UIHandler.instance.loadingPanel.SetActive(false);
            GameManager.instance.gameIsLoading = false;
            UIHandler.instance.mainPanel.SetActive(true);
        }
    }

    public IEnumerator ShowStartGamePanelAfterLoad()
    {
        yield return new WaitForSeconds(3.0f);
        loadingtext.color = new Color(255, 255, 255, 1f);
        UIHandler.instance.loadingPanel.SetActive(false);
        GameManager.instance.gameIsLoading = false;
        UIHandler.instance.startGamePanel.SetActive(true);
    }
    
}
