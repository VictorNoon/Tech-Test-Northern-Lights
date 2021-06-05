using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareTile : MonoBehaviour, IGenerateableTile, IPaintableTile
{
    public Vector3 tileSize;


    MaterialPropertyBlock _probBloc;
    public GameObject GetTileModel()
    {
        return gameObject;
    }

    public Vector3 GetTileSize()
    {
        return tileSize;
    }

    public void InitializeTile()
    {
    }

    public SquareTile(Vector3 size)
    {
        tileSize = size;
    }
    public SquareTile()
    {
        tileSize = Vector3.zero;
    }

    public void SetTileColor(Color color)
    {
        Renderer rd;

        rd = GetComponent<Renderer>();
        if (rd == null)
            return;
            
        _probBloc = new MaterialPropertyBlock();

        rd.GetPropertyBlock(_probBloc);
        _probBloc.SetColor("_Color", color);
        rd.SetPropertyBlock(_probBloc);
    }

    public Color GetTileColr()
    {
        throw new System.NotImplementedException();
    }
}
