using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public AllManager manager;

    public GameObject[] BaseMap;
    public GameObject Obstacle;
    
    public GameObject parent;

    [System.Serializable]
    public struct ObstacleMaps
    {
        public Vector2Int[] obstacleIndex;
    }

    [SerializeField]
    public ObstacleMaps[] obstacleMaps;

    public IEnumerator Initiate(int stage)
    {
        GameObject baseMap = Instantiate(BaseMap[stage]);
        baseMap.transform.SetParent(parent.transform);
        for(int i = 0; i < obstacleMaps[stage].obstacleIndex.Length; i++)
        {
            BattleTileFunction btf = manager.battleMap.tiles[obstacleMaps[stage].obstacleIndex[i].x, obstacleMaps[stage].obstacleIndex[i].y].GetComponent<BattleTileFunction>();
            btf.onTileObj = EffectManager.Target.OBSTACLE;
            btf.onObj = Instantiate(Obstacle, btf.transform.position, Obstacle.transform.rotation, btf.transform);
        }

        for (int i = 0; i < 60; i++)
        {
            parent.transform.position += Vector3.up * (31.24f / 60f);
            yield return null;
        }
    }
}
