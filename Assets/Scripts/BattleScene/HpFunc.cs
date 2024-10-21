using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class HpFunc : MonoBehaviour
{
    public int Hp;
    public int Dp;
    public GameObject DamagePrefab;
    public TextMeshPro HpText;
    public TextMeshPro DpText;
    public AllManager manager;
    public AudioSource[] SEs;

    public void FirstSetHp(int _Hp, int _Dp = 0)
    {
        Hp = _Hp;
        Dp = _Dp;
        SetUI();
    }

    public void SetDp(int delta)
    {
        Dp += delta;
        SetUI();
    }

    public void SetHp(int delta, bool toHp = false)
    {
        delta = HpCal(delta);
        if (toHp)
        {
            Hp += delta;
        }
        else
        {
            if (Dp < -delta)
            {
                Dp = 0;
                Hp += Dp + delta;
            }
            else
            {
                Dp += delta;
            }
        }
        //마우스로 하는 이동에서 ui 띄우는거 참고해서 데미지 띄우기
        GameObject temp = manager.InstantiateP(DamagePrefab);
        temp.transform.position = transform.position;
        if (delta < 0)
        {
            temp.GetComponent<TextMesh>().color = Color.red;
            if (delta > -4)
                SEs[0].Play();
            else
                SEs[1].Play();
        }
        else
            temp.GetComponent<TextMesh>().color = Color.green;
        temp.GetComponent<TextMesh>().text = delta.ToString();
        StartCoroutine(anime(temp));
        SetUI();
    }
    IEnumerator anime(GameObject temp)
    {
        for (int i = 0; i < 60; i++)
        {
            temp.transform.position += Vector3.up * 0.2f;
            Color tempC = temp.GetComponent<TextMesh>().color;
            tempC.a = 1 - Mathf.Pow(i / 60.0f, 2);
            temp.GetComponent<TextMesh>().color = tempC;
            yield return null;
        }
        Destroy(temp);
    }
    public void SetUI()
    {
        HpText.text = Hp.ToString();
        DpText.text = Dp.ToString();
    }
    public int DamageAdd = 0;
    public int DamageMlt = 1;
    int HpCal(int delta)
    {
        delta += DamageAdd;
        delta *= DamageMlt;
        return delta;
    }
}
