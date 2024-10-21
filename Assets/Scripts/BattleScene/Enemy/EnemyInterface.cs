using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface EnemyInterface 
{
    void StartAI();
    void Death();

    ref bool GetAlive();
    ref AllManager GetManager();
    ref HpFunc GethpFunc();
    ref Vector3Int GetEnemyXZIndex();
    MonoBehaviour GetMono();
    void SetMove(bool B);
    IEnumerator MoveAct(Vector3 startPosition, Vector3 target, Vector3Int targetXZ);
    int GetType();
}
