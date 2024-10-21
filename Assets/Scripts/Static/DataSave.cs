using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//아래부터 저장을 위한 추가 using
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

static class DataSave
{
    [Serializable]
    public struct Data
    {
        public int[,] player_deck;      //플레이어의 덱
        public int []player_deck_N;     //플레이어의 덱의 매수
        public int mapX, mapZ;          //플레이어의 맵에서의 위치
        public bool firstStory;         //튜토리얼을 봤는지 여부
        public int Enemy_easy_N;        //맵에 남은 쉬운 적의 수
        public int Enemy_normal_N;      //맵에 남은 보통 적의 수
        public int Enemy_hard_N;        //맵에 남은 어려운 적의 수
        public int Event_N;             //맵에 남은 이벤트의 수
        public int BossX, BossY;        //보스의 위치
        public int[,] tileInfo;         //타일의 이벤트 정보
        public float sound;
    }
    public static Data data = new Data
    {
        player_deck = new int[,] {
            {1,2,3,4,5,6,7,8,9,10,11,12,13,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
            {1,2,3,4,5,6,7,8,9,10,11,12,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
            {1,2,3,4,5,1,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
        },
        tileInfo = new int[7, 7],
        player_deck_N = new int[3] { 13, 12, 7 },
        mapX = 5,
        mapZ = 5,
        BossX = 1,
        BossY = 1,
        Enemy_easy_N = 14,
        Enemy_normal_N = 10,
        Enemy_hard_N = 7,
        Event_N = 6,
        firstStory = false,
        sound = 0.5f
    };

    public static void LoadData()
    {
        BinaryFormatter bf = new BinaryFormatter();

        FileStream file = File.Open(Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);

        if (file != null && file.Length > 0)
        {
            Data dataT = (Data)bf.Deserialize(file);



            data = dataT;

            //할당 파트 할당 한것들?

            //할당한 것들 디버깅?
        }

        file.Close();
    }

    public static void SaveData()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/playerInfo.dat");

        Data dataT = new Data();

        //할당할 것들
        dataT = data;

        bf.Serialize(file, data);
        file.Close();
    }
    public static DataManager.Enemy enemy = new DataManager.Enemy { };
    public static DataManager.Reward reward = new DataManager.Reward { };
    public static int Difficulty = -1;
    public static bool Stop = false;
}
