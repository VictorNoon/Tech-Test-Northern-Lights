using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NLTechTest.Map
{
    public class MapTileGenerator : ScriptableObject
    {

        public enum SubdivisionAlgorithmSelector
        {
            NoSubdivisionPossible = -2,
            NoKnownSubdivisionAlgorithmKnown = -1,
            SubdivionAlgorithNotEvaluatedYet = 0,
            EvenSubdivision = 1,

        }

        [System.Serializable]
        public class GenerationNotPossible : System.Exception
        {
            public GenerationNotPossible() { }
            public GenerationNotPossible(string message) : base(message) { }
            public GenerationNotPossible(string message, System.Exception inner) : base(message, inner) { }
            protected GenerationNotPossible(
                System.Runtime.Serialization.SerializationInfo info,
                System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        }

        private GameObject _mapTile = null;
        private List<SubdivisionAlgorithmSelector> _mapLayerSubdivisionAlgorithm;
        private int _numberOfTiles;
        private Vector3 _centerTilePosition;
        private Vector3 _tileSize;
        private int _numberOfSubdivisions;

        public SubdivisionAlgorithmSelector FindSubdivisionMethodForLayerWithNSubdivision(int subdivisionNumber)
        {
            if (CheckForEvenDistributionMethod(subdivisionNumber))
                return SubdivisionAlgorithmSelector.EvenSubdivision;

            return SubdivisionAlgorithmSelector.NoKnownSubdivisionAlgorithmKnown;
        }

        private bool CheckForEvenDistributionMethod(int subdivisionNumber)
        {
            return CanBeFilledWithIdenticalSizeSquares(subdivisionNumber);
        }

        private void SetLayerSubdivisionMethodTo(int layer, SubdivisionAlgorithmSelector algo)
        {
            _mapLayerSubdivisionAlgorithm[layer] = algo;
        }

        public bool IsLayerSubdivisionMethodValid(SubdivisionAlgorithmSelector layerAlgo)
        {
            return layerAlgo > 0;
        }

        public bool IsLayerSubdivisionInNPossible(int subdivisionNumber)
        {
            return IsLayerSubdivisionMethodValid(FindSubdivisionMethodForLayerWithNSubdivision(subdivisionNumber));
        }

        public void UpdateTileGenerationParameters(int numberOfSubdivisions, Vector3 parentPosition, float mapSize)
        {
            _numberOfTiles *= numberOfSubdivisions;
            _centerTilePosition = parentPosition;
            _tileSize = _mapTile.transform.lossyScale * 10 * mapSize / Mathf.Sqrt(_numberOfTiles);
            _numberOfSubdivisions = numberOfSubdivisions;
        }

        public List<GameObject> GenerateTiles()
        {
            List<GameObject> tilePlates;

            tilePlates = new List<GameObject>();

            if (FindSubdivisionMethodForLayerWithNSubdivision(_numberOfSubdivisions) == SubdivisionAlgorithmSelector.EvenSubdivision)
                GenerateEvenlySubdividedTiles(tilePlates);

            return tilePlates;
        }

        private void GenerateEvenlySubdividedTiles(List<GameObject> tilePlates)
        {
            for (int i = 0; i < _numberOfTiles; i++)
                tilePlates.Add(GenerateTileAtIndex(i));
        }

        private Vector3 GetPositionOfTileAtIndex(int tileIndex)
        {
            Vector3 tilePosition;

            tilePosition = _centerTilePosition + GetTileOffsetAtIndex(tileIndex);

            return tilePosition;
        }

        private Vector3 GetTileOffsetAtIndex(int i)
        {
            Vector3 tileOffSet;
            int plateSideLen = (int)Mathf.Sqrt(_numberOfTiles);

            tileOffSet = new Vector3((-plateSideLen / 2 + (i % plateSideLen)) * _tileSize.x, 0, (-plateSideLen / 2 + (i / plateSideLen)) * _tileSize.z);
            tileOffSet += new Vector3(_tileSize.x, 0, _tileSize.z) / 2 * (1 - (plateSideLen % 2));
            tileOffSet *= 10;

            return tileOffSet;
        }

        private void ScaleTile(GameObject newTile)
        {
            Vector3 newTileScale;

            newTileScale = newTile.transform.localScale;

            newTile.transform.localScale = new Vector3(newTileScale.x * _tileSize.x, newTileScale.y * _tileSize.y, newTileScale.z * _tileSize.z);
        }

        private Quaternion GetTileOrientationAtIndex(int tileIndex)
        {
            return Quaternion.Euler(0, 0, 0);
        }

        private GameObject GenerateTileAtIndex(int tileIndex)
        {
            GameObject newTile;
            Vector3 tilePosition;
            Quaternion tileOrientation;

            tilePosition = GetPositionOfTileAtIndex(tileIndex);
            tileOrientation = GetTileOrientationAtIndex(tileIndex);

            newTile = Instantiate(_mapTile, tilePosition, tileOrientation);
            ScaleTile(newTile);

            return newTile;
        }

        private bool CanBeFilledWithIdenticalSizeSquares(int number)
        {
            if (number == 1)
                return true;

            return Mathf.Sqrt(number) % 1 == 0;
        }

        public void TileGenerationInitialisationSquence(GameObject mapTile)
        {
            _numberOfTiles = 1;
            _numberOfSubdivisions = 1;
            _mapTile = mapTile;
        }
    }
}