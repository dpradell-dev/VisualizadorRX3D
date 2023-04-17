using System;
using System.Collections;
using System.Collections.Generic;
using Pixelplacement;
using UnityEngine;
using UnityEngine.Rendering;

public class MainStateMachine : StateMachine
{
    private void Start()
    {
        QualitySettings.SetQualityLevel(0);
        Application.targetFrameRate = 30;

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitApplication();
        }
    }

    public void ExitApplication()
    {
        Application.Quit();
    }
}
