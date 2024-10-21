using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventManager : MonoBehaviour
{
    public DataManager dataManager;
    public GameObject PopUp;
    public Text text;
    
    public Text Buttontext;
    public int buttonType;

    string startText = "내 연구실에서 중요한 자료를 가지고 도망친 녀석, 절대로 용서할 수 없어.\n\n빨리 쫒아가자.";
    string endText = "이쪽 방향으로 가면 녀석을 쫒아갈 수 있을거같군. 계속 쫒아가자.";

    public enum EventType { None, Event, EasyEnemy, NormalEnemy, HardEnemy, Boss, AfterBattle, Start, End }
    int[] enemyN = { 0, 0, 0 };
    public void StartEvent(EventType eventType)
    {
        int temp = 0;
        switch (eventType)
        {
            case EventType.None://
                return;
            case EventType.AfterBattle://
                if (DataSave.Difficulty == 4)
                {
                    StartEvent(EventType.End);
                    return;
                }
                text.text = DataSave.reward.text;
                if (DataSave.reward.getN > 0)
                {
                    buttonType = 1;
                    Buttontext.text = "카드 받기";
                }
                else
                {
                    buttonType = 2;
                    Buttontext.text = "톱니 작동";
                }
                break;
            case EventType.Start://
                text.text = startText;
                buttonType = 0;
                Buttontext.text = "확인";
                break;
            case EventType.End://
                text.text = endText;
                buttonType = 4;
                Buttontext.text = "다음 챕터로";
                break;
            case EventType.Event://
                temp = Random.Range(0, dataManager.EventReward.Length);
                DataSave.reward = dataManager.EventReward[temp];
                DataSave.Difficulty = 0;
                text.text = DataSave.reward.text;
                if (DataSave.reward.getN > 0)
                {
                    buttonType = 1;
                    Buttontext.text = "카드 받기";
                }
                else
                {
                    buttonType = 2;
                    Buttontext.text = "톱니 작동";
                }
                break;
            case EventType.EasyEnemy://
                temp = Random.Range(0, dataManager.EasyEnemyEvent.Length);
                DataSave.enemy = dataManager.EasyEnemyEvent[temp];
                text.text = dataManager.EasyEnemyEvent[temp].text;
                for(int i = 0; i < DataSave.enemy.EnemyIndex.Length; i++)
                {
                    enemyN[DataSave.enemy.EnemyIndex[i]]++;
                }
                if (enemyN[2] != 0)
                {
                    text.text += "\n\n바르게스트×" + enemyN[2].ToString() + " : 전투를 위한 사냥개. 높은 내구력을 지니고 있으며 접근을 허용하면 점차 강한공격을 해온다.";
                }
                if (enemyN[1] != 0)
                {
                    text.text += "\n\n하운드×" + enemyN[1].ToString() + " : 적을 추격하기 위한 사냥개.";
                }
                DataSave.Difficulty = 1;
                temp = Random.Range(0, dataManager.EasyRewardEvent.Length);
                DataSave.reward = dataManager.EasyRewardEvent[temp];
                buttonType = 3;
                Buttontext.text = "전투 시작";
                break;
            case EventType.NormalEnemy://
                temp = Random.Range(0, dataManager.NormalEnemyEvent.Length);
                DataSave.enemy = dataManager.NormalEnemyEvent[temp];
                text.text = dataManager.NormalEnemyEvent[temp].text;
                for (int i = 0; i < DataSave.enemy.EnemyIndex.Length; i++)
                {
                    enemyN[DataSave.enemy.EnemyIndex[i]]++;
                }
                if (enemyN[2] != 0)
                {
                    text.text += "\n\n바르게스트×" + enemyN[2].ToString() + " : 전투를 위한 사냥개. 높은 내구력을 지니고 있으며 접근을 허용하면 점차 강한공격을 해온다.";
                }
                if (enemyN[1] != 0)
                {
                    text.text += "\n\n하운드×" + enemyN[1].ToString() + " : 적을 추격하기 위한 사냥개.";
                }
                DataSave.Difficulty = 2;
                temp = Random.Range(0, dataManager.NormalRewardEvent.Length);
                DataSave.reward = dataManager.NormalRewardEvent[temp];
                buttonType = 3;
                Buttontext.text = "전투 시작";
                break;
            case EventType.HardEnemy://
                temp = Random.Range(0, dataManager.HardEnemyEvent.Length);
                DataSave.enemy = dataManager.HardEnemyEvent[temp];
                text.text = dataManager.HardEnemyEvent[temp].text;
                for (int i = 0; i < DataSave.enemy.EnemyIndex.Length; i++)
                {
                    enemyN[DataSave.enemy.EnemyIndex[i]]++;
                }
                if (enemyN[2] != 0)
                {
                    text.text += "\n\n바르게스트×" + enemyN[2].ToString() + " : 전투를 위한 사냥개. 높은 내구력을 지니고 있으며 접근을 허용하면 점차 강한공격을 해온다.";
                }
                if (enemyN[1] != 0)
                {
                    text.text += "\n\n하운드×" + enemyN[1].ToString() + " : 적을 추격하기 위한 사냥개.";
                }
                DataSave.Difficulty = 3;
                temp = Random.Range(0, dataManager.HardRewardEvent.Length);
                DataSave.reward = dataManager.HardRewardEvent[temp];
                buttonType = 3;
                Buttontext.text = "전투 시작";
                break;
            case EventType.Boss://
                text.text = dataManager.Boss.text;
                text.text += "\n\n하운드 마스터 : 하운드와 바르게스트를 관리하는 관리인. 본체를 노리고 접근하면 무슨 일이 일어날 지 모른다.";
                DataSave.Difficulty = 4;
                DataSave.enemy = dataManager.Boss;
                buttonType = 3;
                Buttontext.text = "전투 시작";
                break;
        }
        PopUp.SetActive(true);
    }
    public GameObject GetCard;
    public GameObject SubCard;
    public void ButtonFunc()
    {
        SceneManage sm;
        sm = GameObject.FindGameObjectWithTag("SceneManager").GetComponent<SceneManage>();
        switch (buttonType)
        {
            case 0://ok
                break;
            case 1://get
                GetCard.SetActive(true);
                break;
            case 2://sub
                SubCard.SetActive(true);
                break;
            case 3://battle
                sm.Loading(2, 3);
                break;
            case 4://next
                sm.Loading(2, 1);
                break;
        }
        PopUp.SetActive(false);
    }
}
