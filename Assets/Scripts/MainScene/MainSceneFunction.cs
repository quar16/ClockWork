using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//메인화면의 기능, SceneManage를 통해 씬을 연다.
public class MainSceneFunction : MonoBehaviour
{
    
    public SceneManage sceneManage;

    private void Start()
    {
        sceneManage = GameObject.FindGameObjectWithTag("SceneManager").GetComponent<SceneManage>();
    }

    public void Play()
    {
        DataSave.LoadData();
        sceneManage.Loading(1, 2);
    }

    public void Credit()
    {
        DataSave.SaveData();
    }

    public void Exit()
    {
        Application.Quit();
        Debug.Log("Quit");
    }
}
