using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NLTechTest.Map
{
    public class MapLODGenerator : ScriptableObject
    {
        
        [System.Serializable]
        public class ListSizeNotMatching : System.Exception
        {
            public ListSizeNotMatching() { }
            public ListSizeNotMatching(string message) : base(message) { }
            public ListSizeNotMatching(string message, System.Exception inner) : base(message, inner) { }
            protected ListSizeNotMatching(
                System.Runtime.Serialization.SerializationInfo info,
                System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        }


        private List<float> _lodScreenRelativeTransitionHeight;
        
        public LOD[] GenerateLodsFromGameObjects(List<GameObject> lodsGameObjects, List<float> lodScreenRelativeTransitionHeight)
        {
            LOD[] lods;
            
            CheckParametersAreValid(lodsGameObjects, lodScreenRelativeTransitionHeight);
            lods = new LOD[lodsGameObjects.Count];
            _lodScreenRelativeTransitionHeight = lodScreenRelativeTransitionHeight;
            for (int i = 0; i < lods.Length; i++) 
                lods[i] = GenerateLodFromLodGameObject(lodsGameObjects[i], i);

            return lods;
        }

        private void CheckParametersAreValid(List<GameObject> lodsGameObjects, List<float> lodScreenRelativeTransitionHeight)
        {
            if (lodsGameObjects.Count != lodScreenRelativeTransitionHeight.Count)
                throw new ListSizeNotMatching("List<GameObject> lodsGameObjects.Count = " +  lodsGameObjects.Count + "List<float> lodScreenRelativeTransitionHeight.Count = " +  lodScreenRelativeTransitionHeight.Count);
        }

        private LOD GenerateLodFromLodGameObject(GameObject gameObject, int level)
        {
            LOD lod;
            Renderer[] renderers;

            renderers = GetRenderers(gameObject);
            CheckIfRendererArrayIsValid(renderers);
            lod = new LOD(_lodScreenRelativeTransitionHeight[level], renderers);

            return lod;
        }

        private static void CheckIfRendererArrayIsValid(Renderer[] renderers)
        {
            if (renderers.Length == 0) {
                Debug.LogWarning("No Renderer were found in childs");                
            }        
        }

        private Renderer[] GetRenderers(GameObject gameObject)
        {
            Renderer[] renderers;

            renderers = GetRendererOfChildWithRenderer(gameObject);

            return renderers;
        }

        private Renderer[] GetRendererOfChildWithRenderer(GameObject gameObject)
        {
            List<Renderer> childRenderers;
            Renderer[]  renderers;

            childRenderers = CreateChildsRenderersList(gameObject, gameObject.transform.childCount);
            renderers = new Renderer[childRenderers.Count];
            renderers = childRenderers.ToArray();

            return renderers;
        }

        private List<Renderer> CreateChildsRenderersList(GameObject gameObject, int gameObjectChildCount)
        {
            List<Renderer> childRenderers = new List<Renderer>();

            for (int i = 0; i < gameObjectChildCount; i++)
                GetRendererIfAny(gameObject.transform.GetChild(i), childRenderers);

            return childRenderers;
        }

        private void GetRendererIfAny(Transform transform, List<Renderer> childRenderers)
        {
            Renderer renderer;

            renderer = transform.GetComponent<Renderer>();

            if (renderer != null)
                childRenderers.Add(renderer);
        }
    }
}