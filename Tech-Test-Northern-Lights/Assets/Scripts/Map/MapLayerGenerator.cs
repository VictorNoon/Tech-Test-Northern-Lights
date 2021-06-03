﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NLTechTest.Map
{
    public class MapLayerGenerator : ScriptableObject
    {

        public struct LayerGeneratorData
        {
            public string parentNamePrefix;
            public float mapSize;
            public Transform mapTilesParent;
            public GameObject mapTile;
            public Vector3 tileSize;
            public List<int> mapLayerSubdivisionsAmount;
        }

        [System.Serializable]
        public class GeneratorInitializationIncorrect : System.Exception
        {
            public GeneratorInitializationIncorrect() { }
            public GeneratorInitializationIncorrect(string message) : base(message) { }
            public GeneratorInitializationIncorrect(string message, System.Exception inner) : base(message, inner) { }
            protected GeneratorInitializationIncorrect(
                System.Runtime.Serialization.SerializationInfo info,
                System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        }

        MapTileGenerator tileGenerator;
        private GameObject _mapTile = null;
        private bool _isInitialized = false;
        private string _parentNamePrefix;
        private float _mapSize;
        private List<int> _mapLayerSubdivisionsAmount;
        private Transform _mapTilesParent = null;

        void Awake()
        {
            tileGenerator = new MapTileGenerator();
        }

        public LayerGeneratorData GenerateInitialisationData(GameObject mapTile, Transform parentTransform, List<int> mapLevelSubdivisions)
        {
            LayerGeneratorData genData = new LayerGeneratorData();

            genData.parentNamePrefix = "MapLayer";
            genData.mapSize = 1f;
            genData.mapTile = mapTile;
            genData.tileSize = parentTransform.lossyScale;
            genData.mapLayerSubdivisionsAmount = mapLevelSubdivisions;
            genData.mapTilesParent = parentTransform;

            return genData;
        }
        public void InitialiseTileGenerator(LayerGeneratorData initialisationData)
        {
            _parentNamePrefix = initialisationData.parentNamePrefix;
            _mapSize = initialisationData.mapSize;
            _mapTile = initialisationData.mapTile;
            _mapLayerSubdivisionsAmount = initialisationData.mapLayerSubdivisionsAmount;
            _isInitialized = true;
            _mapTilesParent = initialisationData.mapTilesParent;
        }

        public bool IsInitialised()
        {
            return _isInitialized;
        }

        public bool CheckIfGenerable()
        {
            if (!IsGeneratorConfiguarationValid())
                return false;

            if (!AllLayersAreGenerable())
                return false;

            return true;
        }

        private bool SafeCheckIsGenerable()
        {
            if (!IsGeneratorConfiguarationValid())
                throw new GeneratorInitializationIncorrect();
            if (!AllLayersAreGenerable())
                throw new MapTileGenerator.GenerationNotPossible();

            return true;
        }

        public List<GameObject> Generate()
        {
            List<GameObject> tileLayers = new List<GameObject>();

            if (SafeCheckIsGenerable())
                GenerateAllLayers(tileLayers);


            return tileLayers;
        }

        private void GenerateAllLayers(List<GameObject> tileLayers)
        {
            tileGenerator.TileGenerationInitialisationSquence(_mapTile);
            for (int i = 0; i < _mapLayerSubdivisionsAmount.Count; i++)
                tileLayers.Add(GenerateTileLayer(i));

            tileLayers.Reverse(); //Reverse to return layers sorted from high number of tile to low number of tile.
        }

        private GameObject GenerateTileLayer(int level)
        {
            GameObject layer;
            List<GameObject> layerTiles;

            layer = GenerateLayerParentContainer(level);
            tileGenerator.UpdateTileGenerationParameters(_mapLayerSubdivisionsAmount[level], layer.transform.position, _mapSize);
            layerTiles = tileGenerator.GenerateTiles();
            SetLayerChildrens(layer, layerTiles);

            return (layer);
        }

        private bool AllLayersAreGenerable()
        {
            List<bool> layersGenerationPossible;

            layersGenerationPossible = GetGenerationPossibilityForEachLayers();
            return !IsAnyLayerNotGenerable(layersGenerationPossible);
        }

        private bool IsAnyLayerNotGenerable(List<bool> layersGenerationPossible)
        {
            foreach (bool layerGenerationPossible in layersGenerationPossible)
                if (!layerGenerationPossible)
                    return true;

            return false;
        }

        private List<bool> GetGenerationPossibilityForEachLayers()
        {
            List<bool> layersGenerationModePossible;

            layersGenerationModePossible = new List<bool>();

            for (int i = 0; i < _mapLayerSubdivisionsAmount.Count; i++)
                layersGenerationModePossible.Add(tileGenerator.IsLayerSubdivisionInNPossible(_mapLayerSubdivisionsAmount[i]));

            return layersGenerationModePossible;
        }

        private void SetLayerChildrens(GameObject lod, List<GameObject> lodTiles)
        {
            foreach (GameObject tile in lodTiles)
                tile.transform.SetParent(lod.transform);
        }

        
        private bool IsGeneratorConfiguarationValid()
        {
            if (_mapLayerSubdivisionsAmount == null)
                return false;
            return true;
        }

        private GameObject GenerateLayerParentContainer(int level)
        {
            GameObject layerParent;

            layerParent = new GameObject();

            layerParent.name = _parentNamePrefix + level;
            layerParent.transform.position = _mapTilesParent.position;
            layerParent.transform.rotation = _mapTilesParent.rotation;
            layerParent.transform.SetParent(_mapTilesParent);

            return layerParent;
        }

    }
}