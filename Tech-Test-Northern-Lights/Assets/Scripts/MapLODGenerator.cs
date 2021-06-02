using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLODGenerator : ScriptableObject
{
    public LOD[] GenerateLodsFromMapTiles(List<GameObject> lodsGameObjects)
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

        lod = new LOD(1f / (1 + i), GetRenderersInChildOf(gameObject));

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
}
