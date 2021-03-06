using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGenerateableTile
{
    Vector3 GetTileSize();
    GameObject GetTileModel();
    void InitializeTile();
}
