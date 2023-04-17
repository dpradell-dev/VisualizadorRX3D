using System;
using System.Collections;
using System.Collections.Generic;
using Pixelplacement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class ScreeningState: State, IPointerClickHandler
{
    public static Action OnPointerDoubleClick;
    public static int defaultDepth;

    protected ParentState _parentState;

    [Header("Cameras")]
    [SerializeField] protected Camera frontCamera;
    [SerializeField] protected Camera topCamera;
    
    [Header("Canvases")]
    [SerializeField] protected Canvas frontCanvas;
    [SerializeField] protected Canvas topCanvas;
    
    [Header("Images")]
    [SerializeField] protected RawImage frontImage;
    [SerializeField] protected RawImage topImage;

    protected virtual void Awake()
    {
        _parentState = transform.GetComponentInParent<ParentState>();

        if (_parentState == null)
        {
            Debug.LogError("Parent transform of " + this + " ScreeningState does not contain ParentState component. ScreeningState needs his parent!");
        }
    }

    protected virtual void OnEnable()
    {
        DisplayManager.OnTabPressed += SwitchDisplays;
    }

    protected virtual void OnDisable()
    {
        DisplayManager.OnTabPressed -= SwitchDisplays;
        
        frontCanvas.gameObject.SetActive(false);
        topCanvas.gameObject.SetActive(false);
    }
    
    private void SwitchDisplays()
    {
        if (frontCamera == null || topCamera == null)
        {
            Debug.LogError("Cameras are null");
            return;
        }
            
        //Switch Displays
        var frontCameraTarget = frontCamera.targetDisplay;
        var topCameraTarget = topCamera.targetDisplay;

        //Switch Canvas Order
        frontCamera.targetDisplay = topCameraTarget;
        topCamera.targetDisplay = frontCameraTarget;
        
        var frontCanvasOrder = frontCanvas.GetComponent<Canvas>().sortingOrder;
        var topCanvasOrder = topCanvas.GetComponent<Canvas>().sortingOrder;

        frontCanvas.GetComponent<Canvas>().sortingOrder = topCanvasOrder;
        topCanvas.GetComponent<Canvas>().sortingOrder = frontCanvasOrder;
    }
    
    public void OnBackButtonClicked()
    {
        Previous();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount == 2)
        {
            OnPointerDoubleClick?.Invoke();
        }
    }

    public int GetDefaultDepth()
    {
        return defaultDepth;
    }

    public void SetDefaultDepth(int newDepth)
    {
        defaultDepth = newDepth;
    }
}