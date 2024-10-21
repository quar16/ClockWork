using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Break : MonoBehaviour
{
    int childN;
    GameObject[] child;
    public AudioSource deathSE;
    public void StartBreak()
    {
        childN = transform.childCount;
        child = new GameObject[childN];
        for (int i = 0; i < childN; i++)
        {
            child[i] = transform.GetChild(i).gameObject;
        }
        StartCoroutine(Breaking());
    }
    IEnumerator Breaking()
    {

        float l = 0;
        Vector3[] start = new Vector3[childN];
        Vector3[] end = new Vector3[childN];

        for (int i = 0; i < childN; i++)
        {
            start[i] = child[i].transform.localPosition;
            end[i] = (start[i] * 5);
        }
        l += Time.deltaTime;
        for (int i = 0; i < childN; i++)
        {
            child[i].transform.localPosition = Vector3.Lerp(start[i], end[i], l);
        }
        yield return new WaitForSeconds(0.3f);
        l += Time.deltaTime;
        for (int i = 0; i < childN; i++)
        {
            child[i].transform.localPosition = Vector3.Lerp(start[i], end[i], l);
        }
        yield return new WaitForSeconds(0.3f);
        for (int j = 0; j < 3; j++)
        {
            l += Time.deltaTime;
            for (int i = 0; i < childN; i++)
            {
                child[i].transform.localPosition = Vector3.Lerp(start[i], end[i], l);
            }
            yield return new WaitForSeconds(0.1f);
        }
        deathSE.Play();
        while (l < 0.9f)
        {
            l += Time.deltaTime;
            for (int i = 0; i < childN; i++)
            {
                child[i].transform.localPosition = Vector3.Lerp(start[i], end[i], l);
                child[i].transform.localPosition += new Vector3(0, (1 - Mathf.Pow(((l * 2) - 1), 2)) * 3, 0);
                child[i].transform.localRotation = Quaternion.Euler(l * 500 * (i + 1), l * 300 * (i + 1), 0);
            }
            yield return null;
        }
        yield return new WaitForSeconds(0.1f);
        l = 0;
        while (l < 3)
        {
            l += Time.deltaTime;
            for (int i = 0; i < childN; i++)
            {
                child[i].transform.localPosition += new Vector3(0, -0.03f * (i + 1), 0);
            }
            yield return null;
        }
        Destroy(gameObject);
    }
}
