using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DepthBar : MonoBehaviour
{
    [SerializeField] private ScreeningState myScreeningState;
    private DepthButton[] _depthButtons;

    private void Awake()
    {
        _depthButtons = transform.GetComponentsInChildren<DepthButton>();
    }

    private void OnEnable()
    {
        DepthButton.OnDepthSelected += OnDepthSelectedHandler;
    }

    private void Start()
    {
        _depthButtons[1].depth = myScreeningState.GetDefaultDepth(); //We always want to start with the second one activated
        _depthButtons[1].Activate();
    }

    private void OnDisable()
    {
        DepthButton.OnDepthSelected -= OnDepthSelectedHandler;
        
        foreach (var button in _depthButtons)
        {
            if (button.activated)
            {
                myScreeningState.SetDefaultDepth(button.depth);
            }
        }
    }

    private void OnDepthSelectedHandler(int selectedInstanceID, int selectedDepth)
    {
        if (_depthButtons.Length == 0) return;
        
        foreach (var button in _depthButtons)
        {
            if (button.GetInstanceID() != selectedInstanceID)
            {
                button.Deactivate();
            }
        }
    }
}
