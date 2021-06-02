using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LODGroup))]
public class MapGenerator : MonoBehaviour
{
    public List<int> mapLevelSubdivisions;
    public GameObject mapTile = null;
    private LODGroup lodGroup;
    private MapTilesGenerator tileGenerator;
    private MapLODGenerator lODGenerator;

    void Start()
    {
        tileGenerator = ScriptableObject.CreateInstance<MapTilesGenerator>();
        lODGenerator = ScriptableObject.CreateInstance<MapLODGenerator>();
        lodGroup = GetComponent<LODGroup>();
        GenerateMap();
    }

    private void GenerateMap()
    {
        LOD[] lods;
        List<GameObject> mapTiles;

        mapTiles = GenerateMapTiles();
        lods = lODGenerator.GenerateLodsFromMapTiles(mapTiles);

        lodGroup.SetLODs(lods);
        lodGroup.RecalculateBounds();
    }

    private List<GameObject> GenerateMapTiles()
    {
        List<GameObject> mapTiles;
        MapTilesGenerator.TileGeneratorData genInitData;
        
        genInitData = MapTilesGenerator.GenerateTileGeneratorInitialisationData(mapTile, transform, mapLevelSubdivisions);
        tileGenerator.InitialiseTileGenerator(genInitData);
        if (tileGenerator.IsInitialised())
            mapTiles = tileGenerator.GenerateMultipleMapsLayers();
        else
            mapTiles = null;

        return mapTiles;
    }


}
