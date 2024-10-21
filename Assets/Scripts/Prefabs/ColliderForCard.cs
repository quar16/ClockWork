using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderForCard : MonoBehaviour
{
    public CardPrefForGet cpfg;
    private void OnMouseUpAsButton()
    {
        cpfg._OnMouseUpAsButton();
    }
}
