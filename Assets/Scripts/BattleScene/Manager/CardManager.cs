using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public AllManager manager;

    public int[] Deck;
    public int deckCount;
    public int[] Grave;
    public int graveCount;
    public int handCount;
    public Card[] hand;
    public int PlayerIndex;


    public List<Dictionary<string, object>>[] data = new List<Dictionary<string, object>>[3];

    /////////////////////////////////////////////////////////////////
    public IEnumerator Initiate()
    {
        data[0] = CSVReader.Read("Techno MageN");
        data[1] = CSVReader.Read("Body Guard");
        data[2] = CSVReader.Read("Techno MageN");

        Deck = new int[DataSave.data.player_deck_N[PlayerIndex]];
        deckCount = DataSave.data.player_deck_N[PlayerIndex];
        Grave = new int[DataSave.data.player_deck_N[PlayerIndex]];
        for (int i = 0; i < DataSave.data.player_deck_N[PlayerIndex]; i++)
        {
            Deck[i] = DataSave.data.player_deck[PlayerIndex, i];
        }
        //나중에 수정할 핸드 카드 정하는 부분

        Shuffle();
        //덱을 셔플

        for (int i = 0; i < 5; i++)
        {
            StartCoroutine(CardDraw());
            yield return new WaitForSeconds(0.1f);
        }
        //카드 5장 드로우로 시작
    }

    public IEnumerator CardDraw()
    {
        int myOrder = manager.flow.GetOrderNumber();
        GameObject OIC = manager.flow.CreateOrderIcon(myOrder, FlowManager.TYPE.DRAW, manager.character.selectedPlayer);
        yield return new WaitWhile(() => myOrder != manager.flow.nowOrder);
        if (deckCount == 0)
        {
            DeckReload();
        }
        //덱이 0장이면 묘지를 덱으로 돌린다.

        int drawIndex = deckCount - 1;
        //덱에서 드로우 할 위치를 정함.

        int drawCard = Deck[drawIndex];
        Deck[drawIndex] = 0;
        //그 위치의 카드 정보를 저장.

        deckCount--;
        //덱을 소트. 셔플 후 순서대로 드로우하기로 되어있기 때문에 소트는 빠짐

        CreateCard(handCount, drawCard);
        handCount++;
        //핸드에 카드 정보를 저장하고 핸드의 수를 1 늘린다.
        //여기서 카드 프리팹이 생성

        if (handCount >= 6)
        {
            int returnCard = hand[0].cardIndex;
            for (int i = deckCount - 1; i >= 0; i--)
            {
                Deck[i + 1] = Deck[i];
            }
            Deck[0] = returnCard;
            //이거 0번으로 옮기고 소트
            deckCount++;
            HandSort(0, Way.DECK);
            //핸드가 5장을 넘으면 맨 앞장을 덱으로 돌린다.
        }
        manager.character.SetUI();
        manager.flow.NextOrder();
        Destroy(OIC);
    }

    public void DeckReload()
    {
        for (int i = 0; i < graveCount; i++)
        {
            Deck[deckCount] = Grave[i];
            Grave[i] = 0;
            deckCount++;
        }
        Shuffle();
        graveCount = 0;
    }

    private void Shuffle()
    {
        for (int i = 0; i < 30; i++)
        {
            int index1 = Random.Range(0, deckCount);
            int index2 = Random.Range(0, deckCount);
            int a = Deck[index1];
            int b = Deck[index2];
            Deck[index1] = b;
            Deck[index2] = a;
        }
    }

    public enum Way { GRAVE, DECK}
    public void HandSort(int outIndex, Way way)//배열에서 카드 한장을 빼고, 나머지를 앞으로 당김, 개수-1
    {
        //여기서 카드 삭제하는 연산, 함수로 따로 만들어야 할지도?
        switch (way)
        {
            case Way.GRAVE:
                Grave[graveCount] = hand[outIndex].cardIndex;
                graveCount++;
                break;
            case Way.DECK:
                Destroy(hand[outIndex].gameObject);
                break;
        }
        for (int i = outIndex + 1; i < handCount; i++)
        {
            hand[i - 1] = hand[i];
            hand[i - 1].handpos = i - 1;
        }
        handCount--;
    }

    /////////////////////////////////////////////////////////////////
    ///위는 카드 정보 구현, 아래는 카드 애니메이션 구현
    /////////////////////////////////////////////////////////////////

    public ImageLibrary imageLibrary;
    public GameObject cardPreafab;

    public void CreateCard(int pos,int index)//드로우 애니메이션 함수
    {
        GameObject handCard = manager.InstantiateP(cardPreafab);
        Card card = handCard.GetComponent<Card>();
        hand[pos] = card;
        card.SetImage(imageLibrary.ShortImg[PlayerIndex].Img[index - 1], imageLibrary.LongImg[PlayerIndex].Img[index - 1]);
        card.handpos = pos;
        card.cardIndex = index;
        card.PlayerIndex = PlayerIndex;
        card.state = Card.State.hand;
        card.CostInHand.text = data[PlayerIndex][index - 1]["Cost"].ToString();
        card.CostInPlay.text = data[PlayerIndex][index - 1]["Cost"].ToString();
        card.NameinHand.text = data[PlayerIndex][index - 1]["Name"].ToString();
        card.NameInPlay.text = data[PlayerIndex][index - 1]["Name"].ToString();
        card.TextInPlay.text = data[PlayerIndex][index - 1]["Text"].ToString();
        card.manager = manager;
    }
}
