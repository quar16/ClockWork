using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButton : MonoBehaviour
{
    public bool OnOff;
    public GameObject target;

    public void Click()
    {
        target.SetActive(OnOff);
    }
    public void ToggleClick()
    {
        target.SetActive(OnOff);
        OnOff = !OnOff;
    }
}
