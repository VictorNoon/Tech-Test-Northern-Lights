using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareTile : MonoBehaviour, IGenerateableTile
{
    public Vector3 tileSize;
    public GameObject GetTileModel()
    {
        return gameObject;
    }

    public Vector3 GetTileSize()
    {
        return tileSize;
    }

    public SquareTile(Vector3 size)
    {
        tileSize = size;
    }
    public SquareTile()
    {
        tileSize = Vector3.zero;
    }
}
