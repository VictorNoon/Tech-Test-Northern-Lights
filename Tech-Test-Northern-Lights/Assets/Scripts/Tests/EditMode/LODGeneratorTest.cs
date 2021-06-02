using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class LODGeneratorTest
    {
        [Test]
        public void LODGeneratorTestNullGameObjectList()
        {
            NLTechTest.Map.MapLODGenerator lODGenerator = new NLTechTest.Map.MapLODGenerator();
            List<float> lodScreenRelativeTransitionHeightList = new List<float>();
            lodScreenRelativeTransitionHeightList.Add(0.5f);

            Assert.Throws<System.NullReferenceException> (() => lODGenerator.GenerateLodsFromGameObjects(null, lodScreenRelativeTransitionHeightList));
        }

        [Test]
        public void LODGeneratorTestNullLODScreenRelativeTransitionHeightList()
        {
            NLTechTest.Map.MapLODGenerator lODGenerator = new NLTechTest.Map.MapLODGenerator();
            List<GameObject> lodsGameObjects = new List<GameObject>();
            lodsGameObjects.Add(new GameObject());

            Assert.Throws<System.NullReferenceException> (() => lODGenerator.GenerateLodsFromGameObjects(lodsGameObjects, null));
        }

        [Test]
        public void LOD_Generator_Test_lodsGameObjects()
        {
            NLTechTest.Map.MapLODGenerator lODGenerator = new NLTechTest.Map.MapLODGenerator();
            List<float> lodScreenRelativeTransitionHeightList = new List<float>();
            lodScreenRelativeTransitionHeightList.Add(0.5f);

            List<GameObject> lodsGameObjects = new List<GameObject>();

            Assert.Throws<NLTechTest.Map.MapLODGenerator.ListSizeNotMatching> (() => lODGenerator.GenerateLodsFromGameObjects(lodsGameObjects, lodScreenRelativeTransitionHeightList));
        }

        [Test]
        public void LOD_Generator_Test_lodScreenRelativeTransitionHeightList()
        {
            NLTechTest.Map.MapLODGenerator lODGenerator = new NLTechTest.Map.MapLODGenerator();
            List<float> lodScreenRelativeTransitionHeightList = new List<float>();

            List<GameObject> lodsGameObjects = new List<GameObject>();
            lodsGameObjects.Add(new GameObject());

            Assert.Throws<NLTechTest.Map.MapLODGenerator.ListSizeNotMatching> (() => lODGenerator.GenerateLodsFromGameObjects(lodsGameObjects, lodScreenRelativeTransitionHeightList));
        }

        [Test]
        public void LOD_Generator_Test_Both_lODScreenRelativeTransitionHeightList_And_lodsGameObjects_Empty()
        {
            NLTechTest.Map.MapLODGenerator lODGenerator = new NLTechTest.Map.MapLODGenerator();

            List<float> lodScreenRelativeTransitionHeightList = new List<float>();

            List<GameObject> lodsGameObjects = new List<GameObject>();

            LOD[] expectedLod = new LOD[0];
            LOD[] newLod = lODGenerator.GenerateLodsFromGameObjects(lodsGameObjects, lodScreenRelativeTransitionHeightList);

            Assert.True(expectedLod.Length == newLod.Length); 
        }

        [Test]
        public void LOD_Generator_Test_Both_lODScreenRelativeTransitionHeightList_And_lodsGameObjects_SizeOne_LodsGameObject_NoChild()
        {
            NLTechTest.Map.MapLODGenerator lODGenerator = new NLTechTest.Map.MapLODGenerator();

            List<float> lodScreenRelativeTransitionHeightList = new List<float>();
            lodScreenRelativeTransitionHeightList.Add(0.5f);

            List<GameObject> lodsGameObjects = new List<GameObject>();
            lodsGameObjects.Add(new GameObject());

            LOD[] expectedLod = new LOD[0];
            LOD[] newLod = lODGenerator.GenerateLodsFromGameObjects(lodsGameObjects, lodScreenRelativeTransitionHeightList);

            Assert.True(expectedLod.Length == newLod.Length); 
        }

    }
}
