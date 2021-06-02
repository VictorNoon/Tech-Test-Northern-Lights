using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTilesGenerator : ScriptableObject
{
    public struct TileGeneratorData {
        public string parentNamePrefix;
        public float mapSize;
        public GameObject mapTile;
        public Transform mapTilesLayersParent;
        public Vector3 tileSize;
        public List<int> mapLayerSubdivisionsAmount;
    }

    private string _parentNamePrefix = "LOD";
    private float _mapSize;
    private GameObject _mapTile = null;
    private Transform _mapTilesLayersParent = null;
    private List<int> _mapLayerSubdivisionsAmount;
    private int _numberOfTiles;
    private Vector3 _platePosition;
    private Vector3 _tileSize;
    private bool _isInitialised = false;


    public static TileGeneratorData GenerateTileGeneratorInitialisationData(GameObject mapTile, Transform parentTransform, List<int> mapLevelSubdivisions)
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
        return _isInitialised;
    }

    public void InitialiseTileGenerator(TileGeneratorData generationInitialisationData)
    {
        _parentNamePrefix = generationInitialisationData.parentNamePrefix;
        _mapSize = generationInitialisationData.mapSize;
        _mapTile = generationInitialisationData.mapTile;
        _mapTilesLayersParent = generationInitialisationData.mapTilesLayersParent;
        _tileSize = generationInitialisationData.tileSize;
        _mapLayerSubdivisionsAmount = generationInitialisationData.mapLayerSubdivisionsAmount;

        _isInitialised = true;
    }

    public List<GameObject> GenerateMultipleMapsLayers()
    {
        List<GameObject> lodsGameObjects = new List<GameObject>();

        for (int i = _mapLayerSubdivisionsAmount.Count - 1; i >= 0; i--)
        {
            InitializeLodObjectsGenerator(i);
            lodsGameObjects.Add(GenerateMapLayer(i));
        }

        return lodsGameObjects;
    }
 
    private GameObject GenerateMapLayer(int level)
    {
        GameObject lod;
        List<GameObject> lodTiles;

        lod = GenerateLodParentContainer(level);
        lodTiles = GenerateMapTiles();
        SetLodChildrens(lod, lodTiles);

        return (lod);
    }

    private List<GameObject> GenerateMapTiles()
    {
        List<GameObject> tilePlates = new List<GameObject>();

        for (int i = 0; i < _numberOfTiles; i++)
            tilePlates.Add(GenerateTileAtIndex(i));

        return tilePlates;
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
    private bool CanSquareMapBeGenerated()
    {
        if (canBeFilledWithIdenticalSizeSquares())
            return true;

        return false;
    }

    private bool canBeFilledWithIdenticalSizeSquares()
    {
        if (_numberOfTiles == 1)
            return true;

        return Mathf.Sqrt(_numberOfTiles) % 1 == 0;
    }

    private void InitializeLodObjectsGenerator(int i)
    {
        _numberOfTiles = _mapLayerSubdivisionsAmount[i];
        _platePosition = _mapTilesLayersParent.position;
        _tileSize = _mapTile.transform.lossyScale * 10 * _mapSize / Mathf.Sqrt(_numberOfTiles);
    }
}
