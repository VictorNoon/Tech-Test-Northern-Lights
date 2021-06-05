using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NLTechTest.Map
{
    [RequireComponent(typeof(LODGroup))]
    public class MapGenerator : MonoBehaviour
    {
        [SerializeField]
        public GameObject mapTile;
        public List<int> mapLevelSubdivisions;
        public List<float> lodScreenRelativeTransitionHeight;
        private LODGroup _lodGroup;
        private MapLayerGenerator _layerGenerator;
        private MapLODGenerator _lODGenerator;
        private IGenerateableTile _mapTileInterface;
        void Start()
        {
            _layerGenerator = ScriptableObject.CreateInstance<MapLayerGenerator>();
            _lODGenerator = ScriptableObject.CreateInstance<MapLODGenerator>();
            _lodGroup = GetComponent<LODGroup>();
            _mapTileInterface = mapTile.GetComponent<IGenerateableTile>();
            GenerateMap();
        }

        private void GenerateMap()
        {
            LOD[] lods;
            List<GameObject> mapTiles;

            mapTiles = GenerateMapTiles();
            lods = _lODGenerator.GenerateLodsFromGameObjects(mapTiles, lodScreenRelativeTransitionHeight);
            Debug.Log(lods.Length);

            _lodGroup.SetLODs(lods);
            _lodGroup.RecalculateBounds();
        }

        private List<GameObject> GenerateMapTiles()
        {
            List<GameObject> mapTiles;
            MapLayerGenerator.LayerGeneratorData genInitData;

            genInitData = _layerGenerator.GenerateInitialisationData(_mapTileInterface, transform, mapLevelSubdivisions);
            genInitData.mapSize = 2f;
            _layerGenerator.InitialiseTileGenerator(genInitData);
            if (_layerGenerator.IsInitialised())
                mapTiles = _layerGenerator.Generate();
            else
                mapTiles = null;

            return mapTiles;
        }
    }
}

