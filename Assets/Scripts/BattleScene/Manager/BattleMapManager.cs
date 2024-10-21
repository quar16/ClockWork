using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleMapManager : MonoBehaviour
{
    public AllManager manager;

    public GameObject hexPrefab;
    public GameObject tileParent;

    public Vector3 BasePoint;
    public float BaseLength;
    
    //public Vector3Int mouseHexIndex;
    public Vector3Int mouseXZIndex;


    public BattleTileFunction[,] tiles = new BattleTileFunction[9, 7];//x,y


    public IEnumerator Initiate()
    {
        for (int z = 0; z < 7; z++)
        {
            for (int x = 0; x < 9; x++)
            {

                if (!((x == 0 && z == 6) || (x == 8 && z == 6) || (x == 8 && z == 1) || (x == 8 && z == 3) || (x == 8 && z == 5)))
                {
                    GameObject tile = Instantiate(hexPrefab);
                    tiles[x, z] = tile.GetComponent<BattleTileFunction>();
                    tile.transform.SetParent(tileParent.transform);

                    tile.transform.localPosition = BasePoint + new Vector3(x * BaseLength, 120f, z * BaseLength / Mathf.Sqrt(3) / 2f * -3);

                    if (z % 2 != 0)
                    {
                        tile.transform.localPosition += new Vector3(BaseLength / 2f, 0, 0);
                    }
                    tiles[x, z].tileXZIndex = new Vector3Int(x, 0, z);
                    tiles[x, z].tileHexIndex = XZtoHex(tiles[x, z].tileXZIndex);
                    tiles[x, z].manager = manager;
                    yield return null;
                    for (int i = 0; i < 2; i++)
                    {
                        tile.transform.localPosition += Vector3.down * 60;
                        yield return null;
                    }
                }
            }
        }
    }

    public Vector3Int XZtoHex(Vector3Int XZ)
    {
        return new Vector3Int(XZ.x - XZ.z / 2, -(XZ.x - XZ.z / 2) - XZ.z, XZ.z);
    }
    public Vector3Int HexToXZ(Vector3Int Hex)
    {
        return new Vector3Int(Hex.x + Hex.z / 2, 0, Hex.z);
    }

    public BattleTileFunction XZtoTile(Vector3Int XZ)
    {
        return tiles[XZ.x, XZ.z];
    }

    public int Distance(Vector3Int XZ1, Vector3Int XZ2)
    {
        Vector3Int XYZ1 = XZtoHex(XZ1);
        Vector3Int XYZ2 = XZtoHex(XZ2);
        return (Mathf.Abs(XYZ1.x - XYZ2.x) + Mathf.Abs(XYZ1.y - XYZ2.y) + Mathf.Abs(XYZ1.z - XYZ2.z)) / 2;
    }

    Vector3Int[] around =
    {
        new Vector3Int(1,0,-1),
        new Vector3Int(1,-1,0),
        new Vector3Int(-1,1,0),
        new Vector3Int(0,1,-1),
        new Vector3Int(-1,0,1),
        new Vector3Int(0,-1,1),
    };

    public GameObject FindTarget(Vector3Int center, EffectManager.Target target)
    {
        Vector3Int tempV = XZtoHex(center);
        for(int i = 0; i < 6; i++)
        {
            try
            {
                tempV = XZtoHex(center) + around[i];
                
                if (XZtoTile(HexToXZ(tempV)).onTileObj == target)
                {
                    if (target == EffectManager.Target.TILE)
                        return XZtoTile(HexToXZ(tempV)).gameObject;
                    else
                        return XZtoTile(HexToXZ(tempV)).onObj;
                }
            }
            catch { }
        }
        return null;
    }

    public int HexDistance(Vector3Int start, Vector3Int end)
    {
        //두 지점이 주어지고 계산 시작
        int[,] distances = new int[9, 7];
        Vector3Int[] NextCalHex = new Vector3Int[30];
        Vector3Int[] NowCalHex = new Vector3Int[30];
        int NextCalHex_n = 1;
        int NowCalHex_n = 0;
        NextCalHex[0] = end;
        //뉴 에리어와 올드 에리어를 나눔(9,7)
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                distances[i, j] = 999;
            }
        }
        distances[end.x, end.z] = 0;
        //종점을 0, 나머지 점을 999로 두고 시작

        while(NextCalHex_n != 0)
        {
            NowCalHex_n = NextCalHex_n;
            NextCalHex_n = 0;
            NowCalHex = NextCalHex;
            NextCalHex = new Vector3Int[30];
            for(int i = 0; i < NowCalHex_n; i++)
            {
                for (int r = 0; r < 6; r++)
                {
                    try
                    {
                        Vector3Int tempHex = XZtoHex(NowCalHex[i]) + around[r];
                        int tI = tempHex.x + tempHex.z / 2;
                        int tJ = tempHex.z;
                        //각각이 찾아낸 주변 헥스의 XZ
                        bool test = tiles[tI, tJ].isTargetOn;
                        if (tiles[tI, tJ].onTileObj != EffectManager.Target.OBSTACLE)
                        {
                            if (distances[tI, tJ] > distances[NowCalHex[i].x, NowCalHex[i].z] + 1)
                            {
                                distances[tI, tJ] = distances[NowCalHex[i].x, NowCalHex[i].z] + 1;
                                NextCalHex[NextCalHex_n] = new Vector3Int(tI, 0, tJ);
                                NextCalHex_n++;

                                if (start == new Vector3Int(tI, 0, tJ))
                                {
                                    return distances[tI, tJ];
                                }
                            }
                        }
                    }
                    catch { }
                }
            }
        }

       
        return -1;
    }
}
