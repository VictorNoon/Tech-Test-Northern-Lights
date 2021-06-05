using System;
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
            public IGenerateableTile mapTile;
            public Vector3 tileSize;
            public List<int> mapLayerSubdivisionsAmount;
            public Transform mapTilesParent;
            public ITileGenerator tileGenerator;
            public IColoredTilesGenerator colorGenerator;
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

        private ITileGenerator _tileGenerator;
        private IColoredTilesGenerator _colorGenerator;
        private IGenerateableTile _mapTile = null;
        private bool _isInitialized = false;
        private string _parentNamePrefix;
        private float _mapSize;
        private List<int> _mapLayerSubdivisionsAmount;
        private Transform _mapTilesParent = null;

        public LayerGeneratorData GenerateInitialisationData(IGenerateableTile mapTile, Transform parentTransform, List<int> mapLayerSubdivisions)
        {
            LayerGeneratorData genData = new LayerGeneratorData();

            genData.parentNamePrefix = "MapLayer";
            genData.mapSize = 1f;
            genData.mapTile = mapTile;
            genData.tileSize = parentTransform.lossyScale;
            genData.mapLayerSubdivisionsAmount = mapLayerSubdivisions;
            genData.mapTilesParent = parentTransform;
            genData.tileGenerator = ScriptableObject.CreateInstance<SquareTileGenerator>();
            genData.colorGenerator = (IColoredTilesGenerator)genData.tileGenerator;
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
            _tileGenerator = initialisationData.tileGenerator;
            _colorGenerator = initialisationData.colorGenerator;
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
                _tileGenerator.ThrowGenerationException();

            return true;
        }

        public List<GameObject> Generate()
        {
            List<GameObject> tileLayers;

            if (SafeCheckIsGenerable())
                tileLayers = GenerateAllLayers();
            else
                tileLayers = new List<GameObject>();


            return tileLayers;
        }

        private List<GameObject> GenerateAllLayers()
        {
            List<GameObject> tileLayers;

            tileLayers = new List<GameObject>();

            _tileGenerator.SetTileToBegenerated(_mapTile);
            for (int i = 0; i < _mapLayerSubdivisionsAmount.Count; i++)
                tileLayers.Add(GenerateTileLayer(i, tileLayers));

            tileLayers.Reverse(); //Reverse to return layers sorted from high number of tile to low number of tile.

            return tileLayers;
        }

        private GameObject GenerateTileLayer(int level, List<GameObject> tileLayers)
        {
            GameObject layer;
            List<GameObject> layerTiles;

            layer = GenerateLayerParentContainer(level);

            if (tileLayers.Count == 0)
            {
                _tileGenerator.SetTileGenerationParameters(_mapLayerSubdivisionsAmount[level], layer.transform.position, _mapSize);
                layerTiles = _tileGenerator.GenerateTiles();
            }
            else
            {
                if (_colorGenerator != null)
                    _colorGenerator.SetColorPaternRuleShade();
                layerTiles = new List<GameObject>();
                for (int i = 0; i < tileLayers[tileLayers.Count - 1].transform.childCount; i++)
                {

                    if (_colorGenerator != null)
                    {
                        if (tileLayers.Count == 1)
                        {
                            _colorGenerator.SetBaseTilesColor(Color.white);
                            _colorGenerator.SetColorPaternRulePrism();
                        }
                        else
                        {
                            Renderer parentRenderer = tileLayers[tileLayers.Count - 1].transform.GetChild(i).GetComponent<Renderer>();
                            MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
                            parentRenderer.GetPropertyBlock(propertyBlock);
                            _colorGenerator.SetBaseTilesColor(propertyBlock.GetColor("_Color"));
                        }
                    }

                    _tileGenerator.SetTileGenerationParameters(_mapLayerSubdivisionsAmount[level], GetChildrenPosition(tileLayers, i), GetSubTileSize(level, tileLayers, i));
                    layerTiles.AddRange(_tileGenerator.GenerateTiles());
                }
            }

            SetLayerChildrens(layer, layerTiles);

            return (layer);
        }

        private static Vector3 GetChildrenPosition(List<GameObject> tileLayers, int childIndex)
        {
            return tileLayers[tileLayers.Count - 1].transform.GetChild(childIndex).transform.position;
        }

        private float GetSubTileSize(int level, List<GameObject> tileLayers, int i)
        {
            float subTileSize = tileLayers[tileLayers.Count - 1].transform.GetChild(i).transform.lossyScale.x;

            return subTileSize;
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
                layersGenerationModePossible.Add(_tileGenerator.IsLayerSubdivisionInNPossible(_mapLayerSubdivisionsAmount[i]));

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