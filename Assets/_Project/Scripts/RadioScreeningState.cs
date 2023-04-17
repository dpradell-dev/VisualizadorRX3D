using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Pixelplacement;
using Radiography3D;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RadioScreeningState : ScreeningState
{
    public static Action OnFlip;
    
    #region Private Fields
    
    private readonly List<Texture2D> _allTextures = new List<Texture2D>();
    private Texture2D _frontTexture;
    private Texture2D _topTexture;
    
    private int _currentFileCount;
    
    #endregion

    protected override void OnEnable()
    {
        var files = _parentState.GetCurrentFiles();
        _currentFileCount = files.Length;

        if (_currentFileCount == 2) //Must be only 2 files. One for left eye, the other for right eye.
        {
            var naturalComparer = new NaturalFileInfoNameComparer();
            var sortedFilesInfo = files.OrderBy(x => x, naturalComparer);

            foreach (var fileInfo in sortedFilesInfo)
            {
                Texture2D newTexture = Tools.LoadImage(fileInfo.FullName);
                _allTextures.Add(newTexture);
            }
            
            PrepareTextures();
        }
        
        base.OnEnable();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            OnFlip?.Invoke();
        }
    }

    protected override void OnDisable()
    {
        _allTextures.Clear();
        _frontTexture = null;
        _topTexture = null;
        
        Debug.Log("Textures cleared");
        
        base.OnDisable();
    }

    private void PrepareTextures()
    {
        _frontTexture = _allTextures[0]; //First image loaded is for the left eye (frontCanvas).
        _topTexture = _allTextures[1]; //Second image loaded is for the right eye (topCanvas).

        frontImage.texture = _frontTexture;
        topImage.texture = _topTexture;
        
        Debug.Log("Textures are loaded. We can activate canvases");
        frontCanvas.gameObject.SetActive(true);
        topCanvas.gameObject.SetActive(true);
    }
}