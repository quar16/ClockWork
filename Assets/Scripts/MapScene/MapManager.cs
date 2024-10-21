using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class MapManager : MonoBehaviour
{
    static int X = 7, Y = 7;

    public EventManager eventManager;

    public GameObject tilePrefab;
    public GameObject TileParent;
    [SerializeField]
    public GameObject[,] tiles = new GameObject[X,Y];//움직이는 범위

    public Vector3 BasePoint = new Vector3(0, 0, 0);
    public float BaseLength;  

    public GameObject Player;
    public Vector2Int playerCoordinate;

    public GameObject[] NodePrefab;

    // Start is called before the first frame update
    void Start()
    {
        Initiate();
        if (!DataSave.data.firstStory)
        {
            eventManager.StartEvent(EventManager.EventType.Start);
            DataSave.data.firstStory = true;
            DataSave.SaveData();
        }
        else if (DataSave.Difficulty != -1)
        {
            eventManager.StartEvent(EventManager.EventType.AfterBattle);
            DataSave.Difficulty = -1;
        }
        StartCoroutine(MakeStorm());
    }
    public GameObject[] storms;
    IEnumerator MakeStorm()
    {
        for(int i = 0; i < 5; i++)
        {
            GameObject temp = Instantiate(storms[Random.Range(0, 3)], transform);
            temp.transform.position = new Vector3(Random.Range(-25, -4.0f), 1.5f, Random.Range(-23, -5.0f));
        }
        while (true)
        {
            GameObject temp =  Instantiate(storms[Random.Range(0, 3)], transform);
            temp.transform.position = new Vector3(Random.Range(-25, -4.0f), 1.5f, Random.Range(-23, -5.0f));
            yield return new WaitForSeconds(0.4f);
        }
    }

    void Initiate()
    {
        for (int x = 0; x < X; x++)
        {
            for (int z = 0; z < Y; z++)
            {
                GameObject tile = Instantiate(tilePrefab);
                tiles[x, z] = tile;
                tile.transform.SetParent(TileParent.transform);
                tile.transform.localPosition = BasePoint + new Vector3(x * BaseLength, 0, z * BaseLength / Mathf.Sqrt(3) / 2f * 3);
                if (z % 2 != 0)
                {
                    tile.transform.localPosition += new Vector3(BaseLength / 2f, 0, 0);
                }
                tile.GetComponent<TileFunction>().mapManager = this;
                tile.GetComponent<TileFunction>().tilePoint = new Vector2Int(x, z);
            }
        }
        //타일 까는 부분
        playerCoordinate = new Vector2Int(DataSave.data.mapX, DataSave.data.mapZ);
        //플레이어 좌표 불러오고
        Player.transform.position = tiles[playerCoordinate.x, playerCoordinate.y].transform.position;
        //가져온 좌표 위치로 물리 위치 이동시키고
        DataSave.data.tileInfo[DataSave.data.BossX, DataSave.data.BossY] = 5;
        if (!DataSave.data.firstStory)
        {
            TileSet(DataSave.data.Event_N, 1);
            TileSet(DataSave.data.Enemy_easy_N, 2);
            TileSet(DataSave.data.Enemy_normal_N, 3);
            TileSet(DataSave.data.Enemy_hard_N, 4);
            DataSave.SaveData();
        }
        //첫 실행일 경우, 타일에 정보를 입력한다.
        for (int x = 0; x < X; x++)
        {
            for (int z = 0; z < Y; z++)
            {
                int index = DataSave.data.tileInfo[x, z];
                if (index!= 0)
                {
                    tiles[x, z].GetComponent<TileFunction>().SetIcon(NodePrefab[index - 1]);
                }
            }
        }//각 타일이 입력된 정보에 따라 이벤트 아이콘을 불러온다.
    }

    void TileSet(int roopN,int index)
    {
        for (int i = 0; i < roopN;)
        {
            Vector2Int tempV = Vector2Int.zero;
            tempV.x = Random.Range(0, X);
            tempV.y = Random.Range(0, Y);
            if (tempV == playerCoordinate)
                continue;
            if (DataSave.data.tileInfo[tempV.x, tempV.y] == 0)
            {
                DataSave.data.tileInfo[tempV.x, tempV.y] = index;
                i++;
            }
        }
    }
    

    #region MOVE
    Vector3 startPosition;
    float speed;
    float L = 0;
    bool isMoving = false;
    public GameObject area;
    public void MovePlayer(Vector3 target, Vector2Int tilePoint, GameObject eventIcon)
    {
        if (!isMoving)
        {
            startPosition = Player.transform.position;
            if (Vector3.Distance(target, startPosition) < 3)
            {
                area.SetActive(false);
                isMoving = true;
                speed = 0.15f / Vector3.Distance(target, startPosition);

                playerCoordinate = tilePoint;
                DataSave.data.mapX = playerCoordinate.x;
                DataSave.data.mapZ = playerCoordinate.y;
                StartCoroutine(Moving(target, eventIcon));
            }
        }
    }

    IEnumerator Moving(Vector3 target, GameObject eventIcon)
    {
        L = 0;
        while (L <= 1)
        {
            L += speed;
            Player.transform.position = Vector3.Lerp(startPosition, target, L);
            yield return null;
        }
        isMoving = false;
        eventManager.StartEvent((EventManager.EventType)DataSave.data.tileInfo[playerCoordinate.x, playerCoordinate.y]);
        DataSave.data.tileInfo[playerCoordinate.x, playerCoordinate.y] = 0;
        area.SetActive(true);
        if (eventIcon != null)
            eventIcon.SetActive(false);
    }

    #endregion
}
