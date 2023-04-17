using System;
using System.IO;
using UnityEngine;
using Pixelplacement;
using SFB;
using TMPro;

public class ExplorerState : State
{
    public enum Type {Tac, Radio, Simulation, Security};
    public Type myType;
    
    private ParentState _parentState;
    private int _fileCount;
    
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI debugText;

    private void Awake()
    {
        _parentState = transform.GetComponentInParent<ParentState>();

        if (_parentState == null)
        {
            Debug.LogError("We can't find ParentState component in parent.");
            return;
        }

        switch (myType)
        {
            case Type.Tac:
                titleText.text = "Modo TC";
                descriptionText.text = "Seleccione una carpeta que contenga entre 40 y 720 imágenes de TC:";
                break;
        
            case Type.Radio:
                titleText.text = "Modo Radiografía";
                descriptionText.text = "Seleccione una carpeta que contenga 2 radiografías:";
                break;
        
            case Type.Simulation:
                titleText.text = "Modo Simulación TC";
                descriptionText.text = "Seleccione una carpeta que contenga entre 8 y 16 radiografías:";
                break;
                
            case Type.Security:
                titleText.text = "Modo Cinta Seguridad";
                descriptionText.text = "Seleccione una carpeta que contenga almenos 30 imágenes:";
                break;
        
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void Explore()
    {
        string[] paths = StandaloneFileBrowser.OpenFolderPanel(null, null, false);
        
        //No folder selected
        if (paths.Length == 0)
        {
            //TODO Debug text
            Debug.Log("Path not selected");
            debugText.text = "Debe seleccionar una carpeta";
            return;
        }

        if (paths[0] == string.Empty)
        {
            //TODO Debuf text
            Debug.Log("Empty selected path");
            return;
        }

        //Folder selected. Let's see what's inside the folder
        debugText.text = "";
        DirectoryInfo dir = new DirectoryInfo(paths[0]);
        FileInfo[] files = dir.GetFiles("*.jpg"); //TODO Create system to choose extension.

        _fileCount = files.Length;
        Debug.Log("Number of jpg elements found = " + _fileCount);

        switch (myType)
        {
            case Type.Tac:

                const int minCount = 40;
                const int maxCount = 720;
                
                if (_fileCount == 0 || _fileCount < minCount || _fileCount > maxCount)
                {
                    debugText.text = $"La carpeta seleccionada debe contener entre {minCount} y {maxCount} imágenes TC";
                    return;
                }

                float angle;

                switch (_fileCount)
                {
                    case <= 360:
                        angle = 360f / _fileCount;
                        break;
                    case > 360:
                        angle = (float) maxCount / _fileCount;
                        break;
                }
                
                _parentState.SetCurrentAngle(angle);
                
                break;
            
            case Type.Radio:
                if (_fileCount != 2) //There must be ONLY 2 files inside the folder.
                {
                    debugText.text = "La carpeta seleccionada debe contener 2 radiografías";
                    return;
                }
                break;
            
            case Type.Simulation:
                if (_fileCount == 0 || _fileCount < 8 || _fileCount > 18)
                {
                    debugText.text = "La carpeta seleccionada debe contener entre 8 y 16 radiografías";
                    return;
                }
                
                if (_fileCount % 2 != 0)
                {
                    debugText.text = "La carpeta seleccionada debe contener un cantidad par de radiografías";
                    return;
                }
                break;
            
            case Type.Security:
                if (_fileCount < 30) //There must be ONLY 2 files inside the folder.
                {
                    debugText.text = "La carpeta seleccionada debe contener almenos 30 imágenes";
                    return;
                }
                break;
            
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        debugText.text = "";
        _parentState.SetCurrentFiles(files);
        
        Next();
    }

    private void OnDisable()
    {
        debugText.text = "";
    }

    public void OnBackButtonClicked()
    {
        _parentState.BackToMenu();
    }
}