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

        [System.Serializable]
        public class NoRendererWithinAnyChild : System.Exception
        {
            public NoRendererWithinAnyChild() { }
            public NoRendererWithinAnyChild(string message) : base(message) { }
            public NoRendererWithinAnyChild(string message, System.Exception inner) : base(message, inner) { }
            protected NoRendererWithinAnyChild(
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
                lods[i] = GenerateLodFromLodGameObjects(lodsGameObjects[i], i);

            return lods;
        }

        private void CheckParametersAreValid(List<GameObject> lodsGameObjects, List<float> lodScreenRelativeTransitionHeight)
        {
            if (lodsGameObjects.Count != lodScreenRelativeTransitionHeight.Count)
                throw new ListSizeNotMatching("List<GameObject> lodsGameObjects.Count = " +  lodsGameObjects.Count + "List<float> lodScreenRelativeTransitionHeight.Count = " +  lodScreenRelativeTransitionHeight.Count);
        }

        private LOD GenerateLodFromLodGameObjects(GameObject gameObject, int i)
        {
            LOD lod;

            lod = new LOD(_lodScreenRelativeTransitionHeight[i], GetRenderers(gameObject));

            return lod;
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

            childRenderers = CreateChildRenderersList(gameObject, gameObject.transform.childCount);

            return childRenderers.ToArray();
        }

        private List<Renderer> CreateChildRenderersList(GameObject gameObject, int gameObjectChildCount)
        {
            List<Renderer> childRenderers = new List<Renderer>();

            for (int i = 0; i < gameObjectChildCount; i++)
                GetChildRendererIfAny(gameObject, childRenderers, i);

            return childRenderers;
        }

        private Renderer GetChildRendererIfAny(GameObject gameObject, List<Renderer> childRenderers, int i)
        {
            Renderer childRenderer;

            if ((childRenderer = gameObject.transform.GetChild(i).GetComponent<Renderer>()) != null)
                childRenderers.Add(childRenderer);

            return childRenderer;
        }
    }
}