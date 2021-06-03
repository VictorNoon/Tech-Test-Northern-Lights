using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NLTechTest.Map
{
    public class MapTilesGenerator : ScriptableObject
    {
        public struct TileGeneratorData
        {
            public string parentNamePrefix;
            public float mapSize;
            public GameObject mapTile;
            public Transform mapTilesLayersParent;
            public Vector3 tileSize;
            public List<int> mapLayerSubdivisionsAmount;
        }

        private enum SubdivisionAlgorithmSelector
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

        private string _parentNamePrefix;
        private float _mapSize;
        private GameObject _mapTile = null;
        private Transform _mapTilesLayersParent = null;
        private List<int> _mapLayerSubdivisionsAmount;
        private List<SubdivisionAlgorithmSelector> _mapLayerSubdivisionAlgorithm;

        private int _numberOfTiles;
        private Vector3 _platePosition;
        private Vector3 _tileSize;
        private bool _isInitialized = false;


        public TileGeneratorData GenerateTileGeneratorInitialisationData(GameObject mapTile, Transform parentTransform, List<int> mapLevelSubdivisions)
        {
            TileGeneratorData genData = new TileGeneratorData();

            genData.parentNamePrefix = "MapLayer";
            genData.mapSize = 1f;
            genData.mapTile = mapTile;
            genData.mapTilesLayersParent = parentTransform;
            genData.tileSize = parentTransform.lossyScale;
            genData.mapLayerSubdivisionsAmount = mapLevelSubdivisions;

            return genData;
        }
        public bool IsInitialised()
        {
            return _isInitialized;
        }

        public void InitialiseTileGenerator(TileGeneratorData generationInitialisationData)
        {
            _parentNamePrefix = generationInitialisationData.parentNamePrefix;
            _mapSize = generationInitialisationData.mapSize;
            _mapTile = generationInitialisationData.mapTile;
            _mapTilesLayersParent = generationInitialisationData.mapTilesLayersParent;
            _tileSize = generationInitialisationData.tileSize;
            _mapLayerSubdivisionsAmount = generationInitialisationData.mapLayerSubdivisionsAmount;
            _mapLayerSubdivisionAlgorithm = new List<SubdivisionAlgorithmSelector>();
            _mapLayerSubdivisionAlgorithm.AddRange(new SubdivisionAlgorithmSelector[_mapLayerSubdivisionsAmount.Count]);

            _isInitialized = true;
        }

        public List<GameObject> GenerateAllMapsTileLayers()
        {
            List<GameObject> tileLayers = new List<GameObject>();

            FindAndSetSubDivisionAlgorithmForLayers();
            TileGenerationInitialisationSquence();
            for (int i = 0; i < _mapLayerSubdivisionsAmount.Count; i++)
                tileLayers.Add(GenerateTileLayer(i));
            tileLayers.Reverse(); //Reverse to return layers sorted from high number of tile to low number of tile.

            return tileLayers;
        }

        private void FindAndSetSubDivisionAlgorithmForLayers()
        {
            for (int i = 0; i < _mapLayerSubdivisionsAmount.Count; i++)
                FindAndSetSubdivisionMethodForLayer(i);
        }

        private void FindAndSetSubdivisionMethodForLayer(int layerIndex)
        {
            CheckForEvenDistributionMethod(layerIndex);

            if (!IsLayerSubdivisionMethodValid(layerIndex))
                throw new GenerationNotPossible("No Generation method found for subdivision in " + _mapLayerSubdivisionsAmount[layerIndex] + " squares.");
        }

        private void CheckForEvenDistributionMethod(int layerIndex)
        {
            if (CanBeFilledWithIdenticalSizeSquares(_mapLayerSubdivisionsAmount[layerIndex]))
                SetLayerSubdivisionMethodTo(layerIndex, SubdivisionAlgorithmSelector.EvenSubdivision);
        }

        private void SetLayerSubdivisionMethodTo(int layer, SubdivisionAlgorithmSelector algo)
        {
            _mapLayerSubdivisionAlgorithm[layer] = algo;
        }

        private bool IsLayerSubdivisionMethodValid(int layer)
        {
            return _mapLayerSubdivisionAlgorithm[layer] > 0;
        }

        private GameObject GenerateTileLayer(int level)
        {
            GameObject layer;
            List<GameObject> layerTiles;

            UpdateLayerGenerationParameters(level);
            layer = GenerateLodParentContainer(level);
            layerTiles = GenerateMapTiles(_mapLayerSubdivisionAlgorithm[level]);
            SetLodChildrens(layer, layerTiles);

            return (layer);
        }

        private void UpdateLayerGenerationParameters(int level)
        {
            _numberOfTiles *= _mapLayerSubdivisionsAmount[level];
            _platePosition = _mapTilesLayersParent.position;
            _tileSize = _mapTile.transform.lossyScale * 10 * _mapSize / Mathf.Sqrt(_numberOfTiles);
        }

        private List<GameObject> GenerateMapTiles(SubdivisionAlgorithmSelector algo)
        {
            List<GameObject> tilePlates = new List<GameObject>();

            if (algo == SubdivisionAlgorithmSelector.EvenSubdivision)
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

            tilePosition = _platePosition + GetTileOffsetAtIndex(tileIndex);

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

        private GameObject GenerateLodParentContainer(int level)
        {
            GameObject lodParent = new GameObject();

            lodParent.name = _parentNamePrefix + level;
            lodParent.transform.position = _mapTilesLayersParent.position;
            lodParent.transform.rotation = _mapTilesLayersParent.rotation;
            lodParent.transform.SetParent(_mapTilesLayersParent);

            return lodParent;
        }

        private void SetLodChildrens(GameObject lod, List<GameObject> lodTiles)
        {
            foreach (GameObject tile in lodTiles)
                tile.transform.SetParent(lod.transform);
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

        private void TileGenerationInitialisationSquence()
        {
            _numberOfTiles = 1;
        }
    }
}