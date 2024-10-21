using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EndButton : MonoBehaviour
{
    public MainSceneFunction MSF;

    private void OnMouseUpAsButton()
    {
        if (EventSystem.current.IsPointerOverGameObject() == false)
        {
            MSF.Exit();
        }
    }
}
