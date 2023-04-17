using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DepthButton : MonoBehaviour
{
    public static Action<int, int> OnDepthSelected;
    public int depth;
    [HideInInspector] public bool activated;

    private Image _image;
    private Color _imageInitColor;
    private TextMeshProUGUI _depthValueText;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _imageInitColor = _image.color;
        _depthValueText = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        _depthValueText.text = depth.ToString();
    }

    public void Deactivate()
    {
        _image.color = _imageInitColor;
        activated = false;
    }
    
    public void Activate()
    {
        _image.color = Color.white;
        activated = true;
    }

    public void ClickHandler()
    {
        if (activated) return;
        
        Activate();
        OnDepthSelected?.Invoke(GetInstanceID(), depth);
    }
}
