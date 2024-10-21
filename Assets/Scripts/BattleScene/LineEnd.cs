using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineEnd : MonoBehaviour
{
    public AllManager manager;
    public CharacterFunc characterFunc;
    public EnemyInterface enemyFunc;
    public GameObject dotPref;
    public Vector3 start;
    public Vector3 end;
    

    private void Update()
    {
        if(enemyFunc == null)
        start = characterFunc.gameObject.transform.position;
        else
            start = enemyFunc.GetMono().gameObject.transform.position;
    }

    IEnumerator Create()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.2f);
            GameObject temp = Instantiate(dotPref);
            temp.transform.SetParent(transform);
            temp.GetComponent<LineDot>().lineEnd = this;
            temp.GetComponent<LineDot>().end = end;
        }
    }

    public void setStart()
    {
        for (int i = 0; i < 10; i++)
        {
            GameObject temp = Instantiate(dotPref);
            temp.transform.SetParent(transform);
            temp.GetComponent<LineDot>().lineEnd = this;
            temp.GetComponent<LineDot>().end = end;
            temp.GetComponent<LineDot>().i = i / 10.0f;
        }
        StartCoroutine(Create());
    }







    public void SetEnd()
    {
        BroadcastMessage("Destroy");
        Destroy(gameObject);
    }
}
