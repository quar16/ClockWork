using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collider : MonoBehaviour
{
    public BattleTileFunction BTF;
    public TileFunction TF;
    private void OnMouseEnter()
    {
        if (BTF != null)
            BTF._OnMouseEnter();
    }
    private void OnMouseUpAsButton()
    {
        if (TF != null)
            TF._OnMouseUpAsButton();
    }
    private void OnMouseExit()
    {
        if (BTF != null)
            BTF._OnMouseExit();
    }
}
