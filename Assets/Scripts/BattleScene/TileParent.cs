using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileParent : MonoBehaviour
{
    public void CallTiles()
    {
        BroadcastMessage("SetRange");
    }
    public void DestroyChild()
    {
        BroadcastMessage("DestroyRange");
    }
}
