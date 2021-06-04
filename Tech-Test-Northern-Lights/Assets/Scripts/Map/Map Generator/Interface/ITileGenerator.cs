using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NLTechTest.Map
{
    public interface ITileGenerator
    {
        List<GameObject> GenerateTiles();
        void SetTileToBegenerated(IGenerateableTile tile);
        bool IsLayerSubdivisionInNPossible(int subdivisionNumber);
        void SetTileGenerationParameters(int numberOfSubdivisions, Vector3 parentPosition, float mapSize);
        void ThrowGenerationException();
    }
}