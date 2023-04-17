using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class ScreenView : MonoBehaviour
{
    [SerializeField] private RawImage myImage;
    [SerializeField] private RectTransform myContainer;
    [SerializeField] private RectTransform otherContainer;

    private Canvas _parentCanvas;
    private RectTransform _scrollRectT;
    private Vector2 _imageInitSize;
    private Vector3 _containerInitScale;
    private const float _containerMaxScaleValue = 6f;
    private const float _containerMidScaleValue = 1.8f;
    private const float _zoomCoef = 0.1f;
    private bool _zoomedIn;
    
    private void Awake()
    {
        _parentCanvas = GetComponentInParent<Canvas>();
        if (_parentCanvas == null)
        {
            Debug.LogError("ScreenView must be child of a Canvas.");    
        }
        
        _scrollRectT = GetComponent<ScrollRect>().GetComponent<RectTransform>();
    }
    
    private void OnEnable()
    {
        ScreeningState.OnPointerDoubleClick += OnPointerDoubleClickHandler;
        RadioScreeningState.OnFlip += OnFlipHandler;
        TcScreeningState.OnStop += OnStopHandler;
        SimulationScreeningState.OnStop += OnStopHandler;
        
        //Calculem l'amplada relativa que ha de tenir la RawImage.
        if (Mathf.RoundToInt(myImage.texture.height) == Mathf.RoundToInt(_scrollRectT.rect.height))
        {
            myImage.rectTransform.sizeDelta = new Vector2(myImage.texture.width, _scrollRectT.rect.height);
        }
        else
        {
            var relativeCoeficient = _scrollRectT.rect.height / myImage.texture.height;
            var relativeWidth = myImage.texture.width * relativeCoeficient;
            
            myImage.rectTransform.sizeDelta = new Vector2(relativeWidth, _scrollRectT.rect.height);
        }

        _imageInitSize = myImage.rectTransform.sizeDelta;
        _scrollRectT.sizeDelta = _imageInitSize;
        myContainer.sizeDelta = _imageInitSize;
        _containerInitScale = myContainer.localScale;
    }

    private void Update()
    {
        if (Input.mouseScrollDelta.y != 0)
        {
            var currentScale = Math.Round(myContainer.localScale.x, 1);
            var newScale = Math.Round(currentScale + Input.mouseScrollDelta.y * _zoomCoef, 1);
            
            if (newScale < _containerInitScale.x)
            {
                _zoomedIn = false;
                return;
            }
            
            if (newScale > _containerMaxScaleValue)
            {
                return;
            }

            var scrollDelta = Input.mouseScrollDelta.y * _zoomCoef;
            myContainer.localScale += new Vector3(scrollDelta, scrollDelta, scrollDelta);
            
            //Check if we have returned to 0 zoom
            if (Mathf.Approximately((float)newScale, _containerInitScale.x))
            {
                myContainer.anchoredPosition = Vector2.zero;
                myContainer.localScale = _containerInitScale;
                _zoomedIn = false;
            }
            else
            {
                _zoomedIn = true;
            }
        }

        if (!_zoomedIn) return;
        if (_parentCanvas.sortingOrder != 1) return;
        
        var myAnchoredPosition = myContainer.anchoredPosition;
        otherContainer.anchoredPosition = myAnchoredPosition;
    }

    private void OnDisable()
    {
        ScreeningState.OnPointerDoubleClick -= OnPointerDoubleClickHandler;
        RadioScreeningState.OnFlip -= OnFlipHandler;
        TcScreeningState.OnStop -= OnStopHandler;
        SimulationScreeningState.OnStop -= OnStopHandler;

        Reset();
    }

    private void OnFlipHandler()
    {
        myContainer.Rotate(0, 180, 0);
    }
    
    private void OnStopHandler()
    {
        myContainer.anchoredPosition = Vector2.zero;
        myContainer.localScale = _containerInitScale;

        _zoomedIn = false;
    }

    private void OnPointerDoubleClickHandler()
    {
        if (!_zoomedIn)
        {
            Vector3 zoomedScale = myContainer.localScale;
            zoomedScale *= _containerMidScaleValue;

            myContainer.localScale = zoomedScale;
        }
        else
        {
            myContainer.anchoredPosition = Vector2.zero;
            myContainer.localScale = _containerInitScale;
        }
            
        _zoomedIn = !_zoomedIn;
    }

    private void Reset()
    {
        myContainer.anchoredPosition = Vector2.zero;
        myContainer.localScale = _containerInitScale;
        myContainer.eulerAngles = Vector3.zero;

        _zoomedIn = false;
    }
}
