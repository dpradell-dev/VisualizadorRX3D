using System.Collections;
using System.Collections.Generic;
using Pixelplacement;
using UnityEngine;
using UnityEditor;

public class MenuState : State
{
    public void OnTcButtonClicked()
    {
        ChangeState(1);
    }

    public void OnRadioButtonClicked()
    {
        ChangeState(2);
    }

    public void OnSimulationButtonClicked()
    {
        ChangeState(3);
    }
    
    public void OnSecurityButtonClicked()
    {
        ChangeState(4);
    }
}
