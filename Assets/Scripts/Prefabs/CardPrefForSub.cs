using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardPrefForSub : MonoBehaviour
{
    public CardSub cardSub;
    public GameObject selectEf;
    public Image image;
    public int playerIndex;
    public int orderIndex;

    public Text cost;
    public Text name;
    public Text text;
    
    public void _OnMouseUpAsButton()
    {
        cardSub.Select(orderIndex);
    }
}
