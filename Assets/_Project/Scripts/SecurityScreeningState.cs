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

public class SecurityScreeningState : ScreeningState
{
    public static Action OnStop;
    
    #region Private Fields
    
    private readonly List<Texture2D> _allTextures = new List<Texture2D>();
    private readonly List<Texture2D> _frontTextures = new List<Texture2D>();
    private readonly List<Texture2D> _topTextures = new List<Texture2D>();

    private int _currentFileCount;
    private int _currentTextureIndex;
    
    private bool _isPlaying;
    private bool _stopped = true;
    private float _waitTime = 0.25f;
    
    #endregion

    #region Private Serialized Fields

    [Header("Control Panel")]
    [SerializeField] private GameObject controlUI;
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject pauseButton;
    [SerializeField] private Toggle rewindToggle;

    #endregion

    protected override void Awake()
    {
        defaultDepth = 2; //VERY IMPORTANT
        
        base.Awake();
    }

    protected override void OnEnable()
    {
        DepthButton.OnDepthSelected += RecalculateTextures;
        
        var files = _parentState.GetCurrentFiles();
        _currentFileCount = files.Length;

        if (_currentFileCount > 0)
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

    protected override void OnDisable()
    {
        DepthButton.OnDepthSelected -= RecalculateTextures;
        
        Stop();
        _allTextures.Clear();
        _frontTextures.Clear();
        _topTextures.Clear();
        
        _currentTextureIndex = 0;
        rewindToggle.SetIsOnWithoutNotify(false);
        controlUI.SetActive(false);
        
        Debug.Log("Textures cleared");
        
        base.OnDisable();
    }

    private void Update()
    {
        //Speed
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            _waitTime -= 0.02f;
        }
        
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            _waitTime += 0.02f;
        }
        
        //Only 1 frame
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (_isPlaying) Pause();

            if (_currentTextureIndex == 0)
            {
                _currentTextureIndex = _frontTextures.Count - 1;
            }
            else
            {
                _currentTextureIndex--;   
            }

            frontImage.texture = _frontTextures[_currentTextureIndex];
            topImage.texture = _topTextures[_currentTextureIndex];
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (_isPlaying) Pause();

            if (_currentTextureIndex == _frontTextures.Count - 1)
            {
                _currentTextureIndex = 0;
            }
            else
            {
                _currentTextureIndex++;   
            }

            frontImage.texture = _frontTextures[_currentTextureIndex];
            topImage.texture = _topTextures[_currentTextureIndex];
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            controlUI.SetActive(!controlUI.activeSelf);
        }
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_isPlaying)
            {
                Pause();
            }
            else
            {
                Play();
            }
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            Stop();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            rewindToggle.isOn = !rewindToggle.isOn;
        }
    }

    public void Pause()
    {
        StopAllCoroutines();
        
        pauseButton.SetActive(false);
        playButton.SetActive(true);
        
        _isPlaying = false;
    }

    public void Play()
    {
        pauseButton.SetActive(true);
        playButton.SetActive(false);
        
        _isPlaying = true;
        _stopped = false;
        
        if (rewindToggle.isOn)
        {
            StartCoroutine(RewindImageSequence());
            return;
        }
        
        StartCoroutine(PlayImageSequence());
    }
    
    public void Stop()
    {
        StopAllCoroutines();
        
        playButton.SetActive(true);
        pauseButton.SetActive(false);

        if (rewindToggle.isOn)
        {
            _currentTextureIndex = _frontTextures.Count - 1;
            frontImage.texture = _frontTextures[_currentTextureIndex];
            topImage.texture = _topTextures[_currentTextureIndex];
        }
        else
        {
            _currentTextureIndex = 0;
            frontImage.texture = _frontTextures[_currentTextureIndex];
            topImage.texture = _topTextures[_currentTextureIndex];
        }

        _stopped = true;
        _isPlaying = false;
        OnStop?.Invoke();
    }

    public void HandleToggle()
    {
        Pause();

        if (_stopped)
        {
            if (rewindToggle.isOn)
            {
                _currentTextureIndex = _frontTextures.Count - 1;
                frontImage.texture = _frontTextures[_currentTextureIndex];
                topImage.texture = _topTextures[_currentTextureIndex];
            }
            else
            {
                _currentTextureIndex = 0;
                frontImage.texture = _frontTextures[_currentTextureIndex];
                topImage.texture = _topTextures[_currentTextureIndex];
            }
        }
    }

    IEnumerator PlayImageSequence()
    {
        CheckIndexIntegrity();

        while (_currentTextureIndex < _frontTextures.Count)
        {
             frontImage.texture = _frontTextures[_currentTextureIndex];
             topImage.texture = _topTextures[_currentTextureIndex];
             _currentTextureIndex++;

             yield return new WaitForSeconds(_waitTime);
        }

        _currentTextureIndex = 0;
        yield return PlayImageSequence();
    }
    
    IEnumerator RewindImageSequence()
    {
        CheckIndexIntegrity();
        
        while (_currentTextureIndex >= 0)
        {
            frontImage.texture = _frontTextures[_currentTextureIndex];
            topImage.texture = _topTextures[_currentTextureIndex];
            _currentTextureIndex--;

            yield return new WaitForSeconds(_waitTime);
        }

        _currentTextureIndex = _frontTextures.Count - 1;
        yield return RewindImageSequence();
    }

    private void CheckIndexIntegrity()
    {
        if (_currentTextureIndex >= _frontTextures.Count)
        {
            _currentTextureIndex = _frontTextures.Count - 1;
        }
        
        if (_currentTextureIndex < 0)
        {
            _currentTextureIndex = 0;
        }
    }

    private void PrepareTextures()
    {
        foreach (var tex in _allTextures)
        {
            _frontTextures.Add(tex);
        }

        List<Texture2D> tempList = new List<Texture2D>();
        List<Texture2D> tempRange = new List<Texture2D>();
        
        foreach (var tex in _allTextures)
        {
            tempList.Add(tex);    
        }

        tempRange = tempList.GetRange(0, defaultDepth); //TODO!!! Calcular d'alguna forma
        tempList.RemoveRange(0, defaultDepth);
        
        foreach (var tex in tempList)
        {
            _topTextures.Add(tex);
        }
        
        _topTextures.AddRange(tempRange);

        frontImage.texture = _frontTextures[0];
        topImage.texture = _topTextures[0];
        
        tempList.Clear();
        
        Debug.Log("Textures are loaded. We can activate canvases");
        frontCanvas.gameObject.SetActive(true);
        topCanvas.gameObject.SetActive(true);
    }
    
    private void RecalculateTextures(int instanceID, int selectedDepth)
    {
        _topTextures.Clear();
        
        List<Texture2D> tempList = new List<Texture2D>();
        List<Texture2D> tempRange = new List<Texture2D>();
        
        foreach (var tex in _allTextures)
        {
            tempList.Add(tex);    
        }
        
        tempRange = tempList.GetRange(0, selectedDepth);
        tempList.RemoveRange(0, selectedDepth);
        
        foreach (var tex in tempList)
        {
            _topTextures.Add(tex);
        }
        
        _topTextures.AddRange(tempRange);
        
        tempList.Clear();

        if (_isPlaying) return;
        frontImage.texture = _frontTextures[_currentTextureIndex];
        topImage.texture = _topTextures[_currentTextureIndex];
    }
}
