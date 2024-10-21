using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckUiForSub : MonoBehaviour
{
    public ImageLibrary imageLibrary;
    public int playerIndex;
    public CardSub cardSub;
    public GameObject[] cardParent;
    public GameObject cardPrefab;
    public Scrollbar scrollbar;
    public int[] scrollLength;
    // Start is called before the first frame update

    public List<Dictionary<string, object>>[] data = new List<Dictionary<string, object>>[3];
    int insertOrder;
    void OnEnable()
    {
        insertOrder = 0;
        data[0] = CSVReader.Read("Techno MageN");
        data[1] = CSVReader.Read("Body Guard");
        data[2] = CSVReader.Read("Techno MageN");
        cardSub.cards = new GameObject[DataSave.data.player_deck_N[0] + DataSave.data.player_deck_N[1] + DataSave.data.player_deck_N[2]];
        playerIndex = 0;
        scrollbar.value = 0;
        int tempI = ((DataSave.data.player_deck_N[0] - 1) / 5);
        if (tempI < 1)
            tempI = 1;
        scrollbar.size = 0.1f + 0.9f / tempI;
        for (int i = 0; i < 3; i++)
        {
            CardCreate(i);
        }
        cardParent[0].SetActive(true);
        cardParent[1].SetActive(false);
        cardParent[2].SetActive(false);
    }
    public void ScrollMoved()
    {
        cardParent[playerIndex].GetComponent<RectTransform>().anchoredPosition = new Vector3(0, scrollbar.value * scrollLength[playerIndex], 0);
    }

    public void Click(int index)
    {
        cardSub.Select(-1);
        scrollbar.value = 0;
        int tempI = ((DataSave.data.player_deck_N[index] - 1) / 5);
        if (tempI < 1)
            tempI = 1;
        scrollbar.size = 0.1f + 0.9f / tempI;
        for (int i = 0; i < 3; i++)
        {
            if (index == i)
                cardParent[i].SetActive(true);
            else
                cardParent[i].SetActive(false);
        }
        playerIndex = index;
    }
    public void Close()
    {
        cardSub.Select(-1);
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < cardParent[i].transform.childCount; j++)
            {

                Destroy(cardParent[i].transform.GetChild(j).gameObject);
            }
        }
        DataSave.SaveData();
        gameObject.SetActive(false);
    }

    private void CardCreate(int index)
    {
        for (int i = 0; i < DataSave.data.player_deck_N[index]; i++)
        {
            int cardIndex = DataSave.data.player_deck[index, i];
            GameObject temp = Instantiate(cardPrefab);
            cardSub.cards[insertOrder] = temp;
            temp.transform.SetParent(cardParent[index].transform);
            int x = i % 5;
            int y = i / 5;
            temp.GetComponent<RectTransform>().anchoredPosition = new Vector3(-500 + x * 250, 200 - y * 350, 0);

            CardPrefForSub cpfs = temp.GetComponent<CardPrefForSub>();
            cpfs.cardSub = cardSub;
            cpfs.playerIndex = index;
            cpfs.image.sprite = imageLibrary.ShortImg[index].Img[cardIndex - 1];
            cpfs.cost.text = data[index][cardIndex - 1]["Cost"].ToString();
            cpfs.name.text = data[index][cardIndex - 1]["Name"].ToString();
            cpfs.text.text = data[index][cardIndex - 1]["Text"].ToString();
            cpfs.orderIndex = insertOrder;
            insertOrder++;
        }
        scrollLength[index] = ((DataSave.data.player_deck_N[index] - 1) / 5 - 1) * 350;
        if (scrollLength[index] < 0)
            scrollLength[index] = 0;
    }
}
