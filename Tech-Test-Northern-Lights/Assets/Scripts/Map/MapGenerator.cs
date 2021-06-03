using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NLTechTest.Map
{
    [RequireComponent(typeof(LODGroup))]
    public class MapGenerator : MonoBehaviour
    {
        public List<int> mapLevelSubdivisions;
        public List<float> lodScreenRelativeTransitionHeight;
        public GameObject mapTile = null;
        private LODGroup lodGroup;
        private MapLayerGenerator layerGenerator;
        private MapLODGenerator lODGenerator;

        void Start()
        {
            layerGenerator = ScriptableObject.CreateInstance<MapLayerGenerator>();
            lODGenerator = ScriptableObject.CreateInstance<MapLODGenerator>();
            lodGroup = GetComponent<LODGroup>();
            GenerateMap();
        }

        private void GenerateMap()
        {
            LOD[] lods;
            List<GameObject> mapTiles;

            mapTiles = GenerateMapTiles();
            lods = lODGenerator.GenerateLodsFromGameObjects(mapTiles, lodScreenRelativeTransitionHeight);

            lodGroup.SetLODs(lods);
            lodGroup.RecalculateBounds();
        }

        private List<GameObject> GenerateMapTiles()
        {
            List<GameObject> mapTiles;
            MapLayerGenerator.LayerGeneratorData genInitData;

            genInitData = layerGenerator.GenerateInitialisationData(mapTile, transform, mapLevelSubdivisions);
            layerGenerator.InitialiseTileGenerator(genInitData);
            if (layerGenerator.IsInitialised())
                mapTiles = layerGenerator.Generate();
            else
                mapTiles = null;

            return mapTiles;
        }
    }
}

