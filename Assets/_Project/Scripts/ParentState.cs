using System.IO;
using Pixelplacement;

public class ParentState : State
{
    private FileInfo[] _currentFiles;
    private float _currentAngle;

    public void SetCurrentFiles(FileInfo[] files)
    {
        _currentFiles = files;
    }

    public FileInfo[] GetCurrentFiles()
    {
        return _currentFiles;
    }
    
    public void SetCurrentAngle(float angle)
    {
        _currentAngle = angle;
    }

    public float GetCurrentAngle()
    {
        return _currentAngle;
    }

    public void BackToMenu()
    {
        ChangeState("MenuState");
    }
}
