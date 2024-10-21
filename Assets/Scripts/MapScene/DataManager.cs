using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    [System.Serializable]
    public struct Enemy
    {
        public int[] EnemyIndex;        //각 적의 종류, 숫자
        public Vector2Int[] EnemyPos;   //각 적의 시작 위치
        [Multiline(3)]                  
        public string text;             //전투에 앞선 대화 (적 설명은 따로)
    }

    public Enemy[] EasyEnemyEvent;
    public Enemy[] NormalEnemyEvent;
    public Enemy[] HardEnemyEvent;
    public Enemy Boss;


    [System.Serializable]
    public struct Reward
    {
        public int showN;           //보여주는 개수
        public int getN;            //받을 수 있는 개수, 이게 -일경우 빼기방향으로 감.

        [Multiline(3)]
        public string text;         //리워드를 받기 전에 나오는 대화
    }
    public Reward[] EasyRewardEvent;
    public Reward[] NormalRewardEvent;
    public Reward[] HardRewardEvent;
    public Reward[] EventReward;
    
}
