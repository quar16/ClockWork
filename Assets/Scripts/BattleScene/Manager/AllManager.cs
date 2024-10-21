using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllManager : MonoBehaviour
{
    public BattleMapManager battleMap;
    public CardManager[] card;
    public CharacterManager character;
    public EnemyManager enemy;
    public FlowManager flow;
    public ObjectManager objectM;
    public EffectManager effect;
    public CameraManager camera;
    public Transform prefabParent;

    public GameObject InstantiateP(GameObject prefab)
    {
        GameObject temp = Instantiate(prefab);
        temp.transform.SetParent(prefabParent);
        return temp;
    }
}
