using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneMoveButton : MonoBehaviour
{
    public SceneManage sceneManage;
    public int SceneAheadN;
    public int SceneOut;

    private void Awake()
    {
        sceneManage = GameObject.FindGameObjectWithTag("SceneManager").GetComponent<SceneManage>();
    }

    public void Click()
    {
        sceneManage.Loading(SceneOut, SceneAheadN);
    }
}
