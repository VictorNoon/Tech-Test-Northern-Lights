using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITileGenerator
{
    List<GameObject> GenerateTiles();
    void TileGenerationInitialisationSquence(GameObject mapTile);
    bool IsLayerSubdivisionInNPossible(int subdivisionNumber);
    void UpdateTileGenerationParameters(int numberOfSubdivisions, Vector3 parentPosition, float mapSize);
    void ThrowGenerationException();
}
