using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LODGroup))]
public class MapGenerator : MonoBehaviour
{
    public GameObject mapTile;
    public List<int> lodTileCount;
    public string lodName = "LOD";
    public float lodSize = 1f;
    private Vector3 platePosition;
    private Vector3 tileSize;
    private int numberOfTiles;
    private LODGroup lodGroup;

    void Start()
    {
        lodGroup = GetComponent<LODGroup>();
        if (!CanSquareMapBeGenerated())
            Debug.LogError("Map can't be generated.");
        GenerateMap();
    }

    private bool CanSquareMapBeGenerated()
    {
        if (canBeFilledWithIdenticalSizeSquares())
            return true;

        return false;
    }

    private bool canBeFilledWithIdenticalSizeSquares()
    {
        if (numberOfTiles == 1)
            return true;
        
        return Mathf.Sqrt(numberOfTiles) % 1 == 0;
    }

    private void GenerateMap()
    {
        LOD[] lods;

        lods = GenerateLods();

        lodGroup.SetLODs(lods);
        lodGroup.RecalculateBounds();
    }

    private LOD[] GenerateLods()
    {
        LOD[] lods;
        List<GameObject> lodsGameObjects;

        lodsGameObjects = GenerateLodsObjects();
        lods = GenerateLodsFromLodsObjects(lodsGameObjects);

        return lods;
    }

    private LOD[] GenerateLodsFromLodsObjects(List<GameObject> lodsGameObjects)
    {
        LOD[] lods = new LOD[lodsGameObjects.Count];
        for (int i = 0; i < lods.Length; i++)
        {
            lods[i] = GenerateLodFromLodGameObjects(lodsGameObjects[i], i);
        }

        return lods;
    }

    private LOD GenerateLodFromLodGameObjects(GameObject gameObject, int i)
    {
        LOD lod;
        
        
        lod = new LOD(1f/(1 + i), GetRenderersInChildOf(gameObject));
        return lod;
    }

    private static Renderer[] GetRenderersInChildOf(GameObject gameObject)
    {
        Renderer[] renderers;
        int lodRendererCout;

        lodRendererCout = gameObject.transform.childCount;
        renderers = new Renderer[lodRendererCout];

        for (int i = 0; i < lodRendererCout; i++)
            renderers[i] = gameObject.transform.GetChild(i).GetComponent<Renderer>();

        return renderers;
    }

    private List<GameObject> GenerateLodsObjects()
    {
        List<GameObject> lodsGameObjects = new List<GameObject>();
        
        for (int i = lodTileCount.Count - 1; i >= 0; i--)
        {
            InitializeLodObjectsGenerator(i);
            lodsGameObjects.Add(GenerateLodObjects(i));
        }

        return lodsGameObjects;
    }

    private void InitializeLodObjectsGenerator(int i)
    {
        numberOfTiles = lodTileCount[i];
        platePosition = transform.position;
        tileSize = mapTile.transform.lossyScale * 10 * lodSize / Mathf.Sqrt(numberOfTiles);
    }

    private GameObject GenerateLodObjects(int level)
    {
        GameObject lod;
        List<GameObject> lodTiles;

        lod = GenerateLodParentContainer(level);
        lodTiles = GenerateLodTiles();
        SetLodChildrens(lod, lodTiles);

        return (lod);
    }

    private void SetLodChildrens(GameObject lod, List<GameObject> lodTiles)
    {
        foreach (GameObject tile in lodTiles)
            tile.transform.SetParent(lod.transform);
    }

    private GameObject GenerateLodParentContainer(int level)
    {
        GameObject lodParent = new GameObject();

        lodParent.name = lodName + level;
        lodParent.transform.position = transform.position;
        lodParent.transform.rotation = transform.rotation;
        lodParent.transform.SetParent(transform);

        return lodParent;
    }

    private List<GameObject> GenerateLodTiles()
    {
        List<GameObject> tilePlates = new List<GameObject>();

        for (int i = 0; i < numberOfTiles; i++)
            tilePlates.Add(GenerateTileAtIndex(i));

        return tilePlates;
    }

    private GameObject GenerateTileAtIndex(int tileIndex)
    {
        GameObject newTile;
        Vector3 tilePosition;
        Quaternion tileOrientation;

        tilePosition = GetPositionOfTileAtIndex(tileIndex);
        tileOrientation = GetTileOrientationAtIndex(tileIndex);
        
        newTile = Instantiate(mapTile, tilePosition, tileOrientation);
        ScaleTile(newTile);

        return newTile;
    }

    private void ScaleTile(GameObject newTile)
    {
        Vector3 newTileScale;
        
        newTileScale = newTile.transform.localScale;

        newTile.transform.localScale = new Vector3(newTileScale.x * tileSize.x, newTileScale.y * tileSize.y, newTileScale.z * tileSize.z);
    }

    private Quaternion GetTileOrientationAtIndex(int tileIndex)
    {
        return Quaternion.Euler(0,0,0);
    }

    private Vector3 GetPositionOfTileAtIndex(int tileIndex)
    {
        Vector3 tilePosition;

        tilePosition = platePosition + GetTileOffsetAtIndex(tileIndex);

        return tilePosition;
    }

    private Vector3 GetTileOffsetAtIndex(int i)
    {
        Vector3 tileOffSet;
        int plateSideLen = (int)Mathf.Sqrt(numberOfTiles);

        tileOffSet = new Vector3((-plateSideLen/2 + (i % plateSideLen)) * tileSize.x, 0, (-plateSideLen/2 + (i / plateSideLen)) * tileSize.z);
        tileOffSet += new Vector3(tileSize.x,  0, tileSize.z) /2 * (1 - (plateSideLen % 2));
        tileOffSet *= 10;

        return tileOffSet;
    }
}
