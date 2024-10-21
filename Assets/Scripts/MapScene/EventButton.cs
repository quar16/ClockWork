using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventButton : MonoBehaviour
{
    public GameObject PopUp;
    public GameObject RewardUI;
    public void Click()
    {
        RewardUI.SetActive(true);

    }
}
