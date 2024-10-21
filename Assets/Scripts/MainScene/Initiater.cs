using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initiater : MonoBehaviour
{
    public Light []light;
    public GameObject[] candle;
    public Camera camera;
    public GameObject chair;


    bool starting = false;
    public void Click()
    {
        SceneManage sceneManage = GameObject.FindGameObjectWithTag("SceneManager").GetComponent<SceneManage>();
        if (!sceneManage.SceneProcessIsGoing && !starting)
        {
            starting = true;
            StartCoroutine(Initiate());
        }

    }
    float l;
    IEnumerator Initiate()
    {
        l = 0;
        StartCoroutine(Lighting());
        StartCoroutine(CameraMoving());
        StartCoroutine(ChairMoving());
        while (l < 7)
        {
            l += Time.deltaTime;
            yield return null;
        }
        gameObject.SetActive(false);
    }

    IEnumerator Lighting()
    {
        Vector3 originalScale = candle[0].transform.localScale;
        while (l < 4)
        {
            for (int i = 0; i < 2; i++)
            {
                if (l < 2)
                    candle[i].transform.localScale = originalScale * (l * 0.25f + 1);
                light[i].range = 2 + l * 6.5f;
            }
            yield return null;
        }
        for(int i = 0; i < 2; i++)
        {
            light[i].GetComponent<LightBlink>().Blink(0.1f, 1, 3);
        }
    }

    IEnumerator CameraMoving()
    {
        Vector3 start = new Vector3(1, 23, -19);
        Vector3 end = new Vector3(1, 19, -7);
        Quaternion startAngle= Quaternion.Euler(26, -5, 0);
        Quaternion endAngle= Quaternion.Euler(50, -5, 0);
        yield return new WaitWhile(() => l < 1);
        while (l < 7)
        {
            float templ = (l - 1) / 5.5f;
            float templA = (l - 3) / 3.6f;
            camera.transform.position = Vector3.Lerp(start, end, templ);
            camera.transform.rotation = Quaternion.Lerp(startAngle, endAngle, templA);
            yield return null;
        }
    }
    IEnumerator ChairMoving()
    {
        Vector3 start = new Vector3(0,0,-5);
        Vector3 end = new Vector3(-3,0,-9);
        Quaternion angle = Quaternion.Euler(0, 50, 0);
        yield return new WaitWhile(() => l > 2.5f);

        while (l < 3)
        {
            float templ = (l - 2.5f) * 2;
            chair.transform.position = Vector3.Lerp(start, end, templ);
            chair.transform.rotation = Quaternion.Lerp(Quaternion.identity, angle, templ);
            yield return null;
        }
    }
}
