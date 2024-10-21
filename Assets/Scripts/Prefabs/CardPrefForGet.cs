using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardPrefForGet : MonoBehaviour
{
    public CardGet cardGet;
    public GameObject selectEf;
    public Image image;
    public GameObject card;
    public int playerIndex;
    public int cardIndex;

    public Text cost;
    public Text name;
    public Text text;
    void Start()
    {
        StartCoroutine(Set());
    }
    IEnumerator Set()
    {
        float l = 0;
        float a = 0;
        int moveL = -1600 + Random.Range(-50, 51);
        while (a < 0.5f)
        {
            a += Time.deltaTime;
            l = Mathf.Sin(a * Mathf.PI);
            card.transform.localPosition = Vector3.up * moveL * l;
            yield return null;
        } 
    }
    bool isSelected = false;
    public void _OnMouseUpAsButton()
    {
        if (isSelected)
        {
            selectEf.SetActive(false);
            cardGet.selectedN[playerIndex]--;
            isSelected = false;
        }
        else
        {
            if (cardGet.getN > cardGet.selectedN[playerIndex])
            {
                selectEf.SetActive(true);
                cardGet.selectedN[playerIndex]++;
                isSelected = true;
            }
        }
    }
    public void Add()
    {
        if (isSelected)
        {
            DataSave.data.player_deck[playerIndex, DataSave.data.player_deck_N[playerIndex]] = cardIndex + 1;
            DataSave.data.player_deck_N[playerIndex]++;
        }
    }
}
