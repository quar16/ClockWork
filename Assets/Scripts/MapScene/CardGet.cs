using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardGet : MonoBehaviour
{
    public ImageLibrary imageLibrary;
    public Text text;
    public GameObject[] cardSet;
    GameObject[] cards;
    public GameObject CardPrefab;
    int showN;
    public int getN;
    public int[] selectedN;

    public List<Dictionary<string, object>>[] data = new List<Dictionary<string, object>>[3];


    void OnEnable()
    {
        data[0] = CSVReader.Read("Techno MageN");
        data[1] = CSVReader.Read("Body Guard");
        data[2] = CSVReader.Read("Techno MageN");
        showN = DataSave.reward.showN;
        getN = DataSave.reward.getN;
        text.text = getN.ToString();
        cards = new GameObject[showN * 3];
        cardSet[0].SetActive(true);
        cardSet[1].SetActive(false);
        cardSet[2].SetActive(false);
        for(int i = 0; i < 3; i++)
        {
            StartCoroutine(showCard(i));
        }
    }
    IEnumerator showCard(int index)
    {
        for (int i = 0; i < showN; i++)
        {
            GameObject temp = Instantiate(CardPrefab);
            cards[i + showN * index] = temp;
            CardPrefForGet CPFG = temp.GetComponent<CardPrefForGet>();
            temp.transform.SetParent(cardSet[index].transform);
            CPFG.cardGet = this;
            CPFG.playerIndex = index;
            CPFG.cardIndex = Random.Range(0, data[index].Count);
            CPFG.image.sprite = imageLibrary.ShortImg[index].Img[CPFG.cardIndex];
            CPFG.cost.text = data[index][CPFG.cardIndex]["Cost"].ToString();
            CPFG.name.text = data[index][CPFG.cardIndex]["Name"].ToString();
            CPFG.text.text = data[index][CPFG.cardIndex]["Text"].ToString();
            RectTransform rect = temp.GetComponent<RectTransform>();
            rect.localPosition = Vector3.zero;
            float angle = 8 * showN;
            rect.rotation = Quaternion.Euler(0, 0, angle / (showN - 1) * i - angle * 0.5f + Random.Range(-1, 1.0f));
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void ClickOK()
    {
        for(int i = 0; i < 3; i++)
        {
            cardSet[i].SetActive(true);
            selectedN[i] = 0;
        }
        for(int i = 0; i < cards.Length; i++)
        {
            cards[i].GetComponent<CardPrefForGet>().Add();
            Destroy(cards[i]);
        }
        DataSave.SaveData();
        gameObject.SetActive(false);
    }

    public void ChrClick(int index)
    {
        for (int i = 0; i < 3; i++)
        {
            if (index == i)
                cardSet[i].SetActive(true);
            else
                cardSet[i].SetActive(false);
        }
    }
}
