using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterManager : MonoBehaviour
{
    public AllManager manager;

    public Text MoveUI;


    public CharacterFunc[] Playable;
    public int playerN;

    public int selectedPlayer;

    public IEnumerator Initiate()
    {
        for (int i = 0; i < Playable.Length; i++)
        {
            Playable[i].gameObject.SetActive(true);
            playerN++;
        }
        ChangeHighlight(0);
        for (int j = 0; j < 60; j++)
        {
            for (int i = 0; i < Playable.Length; i++)
            {
                Playable[i].transform.position += (-Playable[i].transform.position + manager.battleMap.tiles[1, 2 + 2 * i].transform.position + Vector3.up * 0.75f) / 6.0f;
            }
            yield return null;
        }
        for (int i = 0; i < Playable.Length; i++)
        {
            Playable[i].transform.position = manager.battleMap.tiles[1, 2 + 2 * i].transform.position + Vector3.up * 0.75f;
            manager.battleMap.tiles[1, 2 + 2 * i].onTileObj = EffectManager.Target.FRIENDLY;
            manager.battleMap.tiles[1, 2 + 2 * i].onObj = Playable[i].gameObject;
            Playable[i].manager = manager;
            Playable[i].MoveUI = MoveUI;
            Playable[i].ChrNumber = i;
            Playable[i].playerXZIndex = new Vector3Int(1, 0, 2 * (i + 1));
            if (i != 2)
                Playable[i].GetComponent<HpFunc>().FirstSetHp(12, 4);
            else
                Playable[i].GetComponent<HpFunc>().FirstSetHp(1, 0);
        }
    }

    public void LayerChange(int layerN)
    {
        for (int i = 0; i < Playable.Length; i++)
        {
            if (Playable[i].isAlive)
                Playable[i].gameObject.layer = layerN;
        }
    }

    public void ChangeHighlight(int index)
    {
        for (int i = 0; i < Playable.Length; i++)
        {
            bool tempB = i == index ? true : false;
            Playable[i].highlight.SetActive(tempB);
        }
        selectedPlayer = index;
        SetUI();
    }
    public Text DeckUI;
    public Text GraveUI;
    public void SetUI()
    {
        manager.flow.ApUI.GetComponent<TextMeshPro>().text = manager.flow.Ap[selectedPlayer].ToString();
        DeckUI.text = manager.card[selectedPlayer].deckCount.ToString();
        GraveUI.text = manager.card[selectedPlayer].graveCount.ToString();
    }

    public void DeathCheck()
    {
        for(int i = 0; i < Playable.Length; i++)
        {
            if (Playable[i].hpFunc.Hp <= 0 && Playable[i].isAlive)
            {
                Playable[i].Death();
                playerN--;
                if (playerN == 0)
                {
                    StartCoroutine(Lose());
                }
                if (i == selectedPlayer)
                {
                    for (int c = 2; c >= 0; c--)
                    {
                        if (Playable[c].isAlive)
                        {
                            ChangeHighlight(c);
                        }
                    }
                }
            }
        }
    }
    public Image LoseImg;
    IEnumerator Lose()
    {
        LoseImg.gameObject.SetActive(true);
        Color color = LoseImg.color;
        float l = 0;
        while (color.a < 1)
        {
            l += Time.deltaTime;
            color.a += l;
            LoseImg.color = color;
            yield return null;
        }
        yield return new WaitForSeconds(1);
        LoseImg.gameObject.SetActive(false);
        SceneManage sceneManage = GameObject.FindGameObjectWithTag("SceneManager").GetComponent<SceneManage>();

        sceneManage.Loading(3, 1);
    }
}
