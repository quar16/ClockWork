using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class Card : MonoBehaviour
{
    public AllManager manager;


    public TextMeshPro CostInHand;
    public TextMeshPro NameinHand;
                   
    public TextMeshPro CostInPlay;
    public TextMeshPro NameInPlay;
    public TextMeshPro TextInPlay;

    public BoxCollider boxCollider;

    public enum State {deck,hand,field,ready,notMy,set };
    public Vector3[] pos;

    public int handpos;
    public int cardIndex;
    public int PlayerIndex;

    public State state;

    public GameObject InHand;
    public GameObject InPlay;
    public SpriteRenderer shortSpr;
    public SpriteRenderer longSpr;
    int readyN;
    // Update is called once per frame
    public void SetImage(Sprite shortImg, Sprite longImg)
    {
        shortSpr.sprite = shortImg;
        longSpr.sprite = longImg;
    }
    void Update()
    {
        switch (state)
        {
            case State.deck:
                break;
            case State.hand:
                if (PlayerIndex == manager.character.selectedPlayer)
                    transform.position += (-transform.position + pos[(int)State.hand] + new Vector3(0, 0.01f, -6f) * handpos) / 4.0f;
                else
                    transform.position += (-transform.position + pos[(int)State.notMy] + new Vector3(0, 0.01f, -6f) * handpos) / 4.0f;
                break;
            case State.field:
                transform.position += (-transform.position + pos[(int)State.field]) / 4.0f;
                break;
            case State.ready:
                transform.position += (-transform.position + pos[(int)State.ready]) / 10.0f;
                readyN++;
                if (readyN == 60)
                {
                    state = State.set;
                    Destroy(gameObject);
                }
                break;
            case State.set:
                break;
        }
    }
    bool isSelected = false;
    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        isSelected = true;
        InHand.SetActive(false);
        InPlay.SetActive(true);
        manager.effect.SetRange(cardIndex);
        manager.character.LayerChange(2);
        state = State.field;
        boxCollider.size = Vector3.zero;
    }

    Vector3 originalSize = new Vector3(4.94f, 0.3f, 2.98f);
    private void OnMouseUp()
    {
        if (!isSelected)
            return;
        boxCollider.size = originalSize;
        manager.character.LayerChange(0);
        if (manager.effect.CheckRange())
        {
            state = State.ready;
            manager.card[manager.character.selectedPlayer].HandSort(handpos, CardManager.Way.GRAVE);
        }
        else
        {
            state = State.hand;
            InHand.SetActive(true);
            InPlay.SetActive(false);
        }
        isSelected = false;
    }
}
