using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyManager : MonoBehaviour
{
    public AllManager manager;

    public GameObject []enemyPrefab;
    public GameObject enemyParent;
    
    public EnemyInterface[] EnemyF = new EnemyInterface[0];
    public Vector3Int[] EnemyPos;
    public int enemyN;
    DataManager.Enemy enemy;

    public IEnumerator Initiate()
    {
        enemy = DataSave.enemy;
        int length = enemy.EnemyIndex.Length;
        EnemyF = new EnemyInterface[length];
        for (int i = 0; i < length; i++)
        {
            GameObject temp = Instantiate(enemyPrefab[enemy.EnemyIndex[i] - 1]);
            temp.transform.SetParent(enemyParent.transform);
            temp.GetComponent<HpFunc>().manager = manager;
            EnemyF[i] = temp.GetComponent<EnemyInterface>();
            EnemyF[i].GetMono().transform.position = new Vector3(100, 5.5f, 5.5f);
            enemyN++;
        }
        for (int j = 0; j < 60; j++)
        {
            for (int i = 0; i < EnemyF.Length; i++)
            {
                EnemyF[i].GetMono().transform.position += (-EnemyF[i].GetMono().transform.position + manager.battleMap.tiles[enemy.EnemyPos[i].x, enemy.EnemyPos[i].y].transform.position + Vector3.up * 0.75f) / 6.0f;
                //에너미에서 초기 위치 받아오도록, 아니면 플로우나 에너미 그룹에서
            }
            yield return null;
        } 
        for (int i = 0; i < EnemyF.Length; i++)
        {
            EnemyF[i].GetMono().transform.position = manager.battleMap.tiles[enemy.EnemyPos[i].x, enemy.EnemyPos[i].y].transform.position + Vector3.up * 0.75f;
            EnemyF[i].GetManager() = manager;
            manager.battleMap.tiles[enemy.EnemyPos[i].x, enemy.EnemyPos[i].y].onTileObj = EffectManager.Target.ENEMY;
            manager.battleMap.tiles[enemy.EnemyPos[i].x, enemy.EnemyPos[i].y].onObj = EnemyF[i].GetMono().gameObject;
            EnemyF[i].GetEnemyXZIndex() = new Vector3Int(enemy.EnemyPos[i].x, 0, enemy.EnemyPos[i].y);
        }
        StartCoroutine(CheckStop());
        //아직 에너미 세팅이 완벽하지 못함, 배열
    }
    IEnumerator CheckStop()
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                DataSave.Stop = !DataSave.Stop;
            }
            yield return null;
        }
    }

    public void CreateEnemy(int index, Vector3Int v3)
    {
        int length = EnemyF.Length;
        EnemyInterface[] EI = new EnemyInterface[length + 1];
        for (int i = 0; i < length; i++)
        {
            EI[i] = EnemyF[i];
        }
        GameObject temp = Instantiate(enemyPrefab[index]);
        temp.transform.SetParent(enemyParent.transform);
        temp.GetComponent<HpFunc>().manager = manager;
        EI[length] = temp.GetComponent<EnemyInterface>();
        enemyN++;

        EI[length].GetMono().transform.position = manager.battleMap.tiles[v3.x, v3.z].transform.position + Vector3.up * 0.75f;
        EI[length].GetManager() = manager;
        manager.battleMap.tiles[v3.x, v3.z].onTileObj = EffectManager.Target.ENEMY;
        manager.battleMap.tiles[v3.x, v3.z].onObj = EI[length].GetMono().gameObject;
        EI[length].GetEnemyXZIndex() = v3;
        EnemyF = EI;
    }

    public Image WinImage;
    public void DeathCheck()
    {
        for (int i = 0; i < EnemyF.Length; i++)
        {
            if (EnemyF[i].GethpFunc().Hp <= 0 && EnemyF[i].GetAlive())
            {
                EnemyF[i].Death();
                enemyN--;
                if (enemyN == 0)
                {
                    StartCoroutine(Win());
                }
            }
        }
    }
    IEnumerator Win()
    {
        WinImage.gameObject.SetActive(true);
        Color color = WinImage.color;
        float l = 0;
        while (color.a < 1)
        {
            l += Time.deltaTime;
            color.a += l;
            WinImage.color = color;
            yield return null;
        }
        yield return new WaitForSeconds(1);
        WinImage.gameObject.SetActive(false);
        SceneManage sceneManage = GameObject.FindGameObjectWithTag("SceneManager").GetComponent<SceneManage>();

        sceneManage.Loading(3, 2);
    }
}
