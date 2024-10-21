using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//씬을 다루는 스크립트, 이전에 사용하던대로, 딱히 손볼 것 없음
public class SceneManage : MonoBehaviour
{
    public Image BGg;
    public Image LImage;
    public AudioSource[] audioSources;
    
    public void Awake()
    {
        Screen.SetResolution(1600, 800, true);
        Loading(0, 1);
    }

    public void Loading(int CloseNumber, int OpenNumber)//씬을 변경하는 함수
    {
        StartCoroutine(LoadScene(CloseNumber, OpenNumber));
    }
    public bool SceneProcessIsGoing = false;
    IEnumerator LoadScene(int CloseNumber, int OpenNumber)
    {
        SceneProcessIsGoing = true;
        if(CloseNumber != 0)
        {
            float l = 0;
            BGg.color = new Color(0, 0, 0, 1);
            LImage.color = new Color(1, 1, 1, 1);
            BGg.gameObject.SetActive(true);
            while (l < 1)
            {
                l += Time.deltaTime;
                audioSources[CloseNumber-1].volume = DataSave.data.sound * (1 - l);
                BGg.color = new Color(0, 0, 0, l);
                LImage.color = new Color(1, 1, 1, l);
                yield return null;
            }
            audioSources[CloseNumber - 1].Stop();
            yield return StartCoroutine(SceneProcess(false, CloseNumber));
        }
        
        if (OpenNumber != 0)
        {
            yield return StartCoroutine(SceneProcess(true, OpenNumber));

            float l = 1;
            BGg.color = new Color(0, 0, 0, 0);
            LImage.color = new Color(1, 1, 1, 0);
            audioSources[OpenNumber - 1].Play();
            while (l > 0)
            {
                l -= Time.deltaTime;
                audioSources[OpenNumber - 1].volume = DataSave.data.sound * (1 - l);
                BGg.color = new Color(0, 0, 0, l);
                LImage.color = new Color(1, 1, 1, l);
                yield return null;
            }
            BGg.gameObject.SetActive(false);
        }
        SceneProcessIsGoing = false;
    }

    IEnumerator SceneProcess(bool open, int sceneNumber)
    {
        string SceneName = "";
        switch (sceneNumber)
        {
            case 1:
                DataSave.Difficulty = -1;
                SceneName = "MainScene";
                break;
            case 2:
                SceneName = "MapScene";
                break;
            case 3:
                SceneName = "BattleScene";
                break;
        }

        AsyncOperation asyncLoad;

        if (open)
        {
            asyncLoad = SceneManager.LoadSceneAsync(SceneName, LoadSceneMode.Additive);
        }
        else
        {
            asyncLoad = SceneManager.UnloadSceneAsync(SceneName);
        }

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    
}