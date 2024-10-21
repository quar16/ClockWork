using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardSub : MonoBehaviour
{
    public DeckUiForSub deckUi;
    public GameObject Gear;
    float gearSpeed = 2;
    float angle = 0;
    public Text text;
    int deleteN = 0;
    public GameObject[] cards;
    private void OnEnable()
    {
        deleteN = -1 * DataSave.reward.getN;
        text.text = deleteN.ToString();
    }
    private void Update()
    {
        angle -= gearSpeed;
        Gear.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    public int selected = -1;
    public void Select(int index)
    {
        selected = -1;

        for (int i = 0; i < cards.Length; i++)
        {
            CardPrefForSub cpfs = cards[i].GetComponent<CardPrefForSub>();
            if (i != index)
            {
                cpfs.selectEf.SetActive(false);
            }
            else
            {
                cpfs.selectEf.SetActive(true);
                selected = i;
            }
        }
    }
    public void Delete()
    {
        if (selected != -1)
        {

            CardPrefForSub cpfs = cards[selected].GetComponent<CardPrefForSub>();
            int chr = cpfs.playerIndex;
            int index = cpfs.orderIndex;
            for (int i = index; i < DataSave.data.player_deck_N[chr] - 1; i++)
            {
                DataSave.data.player_deck[chr, i] = DataSave.data.player_deck[chr, i + 1];
            }
            DataSave.data.player_deck[chr, DataSave.data.player_deck_N[chr] - 1] = 0;
            DataSave.data.player_deck_N[chr]--;
            deleteN--;
            text.text = deleteN.ToString();
            StartCoroutine(SpeedChange());
        }
        deckUi.Close();
    }
    public void OpenDeckUI(GameObject UI)
    {
        if (deleteN != 0)
            UI.SetActive(true);
    }

    IEnumerator SpeedChange()
    {
        float t = 0;
        Color color = Color.white;
        Image image= Gear.GetComponent<Image>();
        while (t < 1)
        {
            t += Time.deltaTime * 0.8f;
            gearSpeed += t * 0.5f;
            color.g = 1 - t;
            color.b = 1 - t;
            image.color = color;
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * 0.8f;
            gearSpeed -= t * 0.5f;
            color.g = t;
            color.b = t;
            image.color = color;
            yield return null;
        }
        gearSpeed = 2;
    }
}
