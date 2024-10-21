using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StartButton : MonoBehaviour
{
    public MainSceneFunction MSF;

    private void OnMouseUpAsButton()
    {
        if (EventSystem.current.IsPointerOverGameObject() == false)
        {
            MSF.Play();
        }
    }
}
