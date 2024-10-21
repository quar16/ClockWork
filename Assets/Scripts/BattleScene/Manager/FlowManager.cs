using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FlowManager : MonoBehaviour
{
    public AllManager manager;
    

    public Image StartImg;
    public GameObject orderIconPref;
    public GameObject TouchPrevent;

    public int stageNumber;
    public int actNumber;
    public int indexNumber;

    #region 게임 시작 처리
    //////////////////////////////////////////////////////////////////////////////
    ///게임을 시작하고 모든 이니시에이트를 순서대로 작동시킨다.
    //////////////////////////////////////////////////////////////////////////////

    private void Start()
    {
        StartCoroutine(SetBattle());
    }
    IEnumerator SetBattle()
    {
        SceneManage sceneManage = GameObject.FindGameObjectWithTag("SceneManager").GetComponent<SceneManage>();
        yield return new WaitWhile(() => sceneManage.SceneProcessIsGoing);

        yield return StartCoroutine(manager.battleMap.Initiate());

        yield return StartCoroutine(manager.objectM.Initiate(stageNumber));

        StartCoroutine(manager.card[1].Initiate());
        StartCoroutine(manager.card[2].Initiate());
        yield return StartCoroutine(manager.card[0].Initiate());

        yield return StartCoroutine(manager.character.Initiate());

        yield return StartCoroutine(manager.enemy.Initiate());

        yield return StartCoroutine(StartTextFunc());
        for (int i = 0; i < manager.enemy.EnemyF.Length; i++)
        {
            manager.enemy.EnemyF[i].StartAI();
        }
        StartCoroutine(BattleLoop());
    }

    IEnumerator StartTextFunc()
    {
        StartImg.gameObject.SetActive(true);
        Color color = Color.white;
        yield return new WaitForSeconds(1);
        for (int i = 0; i < 60; i++)
        {
            color.a = (60 - i) / 60.0f;
            StartImg.color = color;
            yield return null;
        }
        StartImg.gameObject.SetActive(false);
        TouchPrevent.SetActive(false);
    }
    #endregion

    #region 시간,AP처리
    //////////////////////////////////////////////////////////////////////////////
    ///시간을 흐르게 하고, AP의 증가 감소를 처리한다.
    //////////////////////////////////////////////////////////////////////////////

    public GameObject timer;
    public GameObject ApUI;
    float time=5, second;
    public int []Ap;

    IEnumerator BattleLoop()
    {
        while (true)
        {
            while (second < 0.5f)
            {
                second += Time.deltaTime;
                timer.transform.rotation = Quaternion.Euler((Mathf.Pow(second, 2) * 4) * 90.0f, 0, 0);
                yield return null;
            }

            timer.transform.rotation = Quaternion.Euler(90.0f, 0, 0);
            time--;
            if (time == 0)
            {
                time = 15;
                APRecharge();
            }

            timer.GetComponent<Text>().text = (time).ToString();

            while (second < 1)
            {
                second += Time.deltaTime;
                timer.transform.rotation = Quaternion.Euler((Mathf.Pow((second - 1), 2) * 4) * 90.0f, 0, 0);
                yield return null;
            }
            second -= 1.0f;
        }
    }

    void APRecharge()
    {
        for (int i = Ap.Length-1; i >= 0; i--)
        {
            setAP(i,5);
        }
    }
    public void setAP(int chrIndex, int delta = 0)
    {
        if (Ap[chrIndex] + delta > 8 && delta > 0)
        {
            delta = 8 - Ap[chrIndex];
            if (delta < 0)
                delta = 0;
        }
        Ap[chrIndex] += delta;

        manager.character.SetUI();
    }
    #endregion
    
    #region 발동 순서 처리
    //////////////////////////////////////////////////////////////////////////////
    ///카드 효과, 이동, 드로우의 처리 순서를 관리하는 부분
    //////////////////////////////////////////////////////////////////////////////

    int orderNumber = 0;
    public int nowOrder = 0;
    public int GetOrderNumber()
    {
        return orderNumber++;
    }
    public void NextOrder()
    {
        nowOrder++;
        manager.enemy.DeathCheck();
        manager.character.DeathCheck();

    }
    public bool IsNoOrder()
    {
        if (orderNumber == nowOrder)
            return true;
        else
            return false;
    }
    public enum TYPE { DRAW,ACTIVE,MOVE,SKILL,ATTACK}
    public Canvas canvas;
    public Sprite[] Icons;
    public GameObject CreateOrderIcon(int myOrder, TYPE type, int index)
    {
        GameObject prefab = Instantiate(orderIconPref);
        prefab.transform.SetParent(canvas.transform);
        OrderIconFunc OIC = prefab.GetComponent<OrderIconFunc>();
        OIC.image.rectTransform.localPosition = new Vector2(600, -500);
        OIC.manager = manager;
        OIC.myOrder = myOrder;
        OIC.type = type;
        OIC.face.sprite = Icons[index];
        switch (type)
        {
            case TYPE.ACTIVE:
                OIC.text.text = "ACTIVE";
                break;
            case TYPE.DRAW:
                OIC.text.text = "DRAW";
                break;
            case TYPE.MOVE:
                OIC.text.text = "MOVE";
                break;
            case TYPE.SKILL:
                OIC.text.text = "SKILL";
                break;
            case TYPE.ATTACK:
                OIC.text.text = "ATTACK";
                break;
        }
        return prefab;
    }

    //이거 예제임
    public IEnumerator Name()
    {
        int myOrder = manager.flow.GetOrderNumber();//자신의 오더 순서를 받는다.
        GameObject OIC = manager.flow.CreateOrderIcon(myOrder, FlowManager.TYPE.MOVE, 0); //오더 UI를 소환하고 가져온다.
        yield return new WaitWhile(() => myOrder != manager.flow.nowOrder);//자신의 오더 순서까지 기다림

        //content

        manager.flow.NextOrder();//다음 순서의 오더를 부른다.
        Destroy(OIC);//자신의 처리가 끝나면 오더 ui를 파괴
    }


    #endregion
}
