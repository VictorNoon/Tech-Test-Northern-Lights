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
            NLTechTest.Map.MapLODGenerator lODGenerator;
            lODGenerator = ScriptableObject.CreateInstance<NLTechTest.Map.MapLODGenerator>();

            List<float> lodScreenRelativeTransitionHeightList = new List<float>();
            lodScreenRelativeTransitionHeightList.Add(0.5f);

            Assert.Throws<System.NullReferenceException>(() => lODGenerator.GenerateLodsFromGameObjects(null, lodScreenRelativeTransitionHeightList));
        }

        [Test]
        public void LODGeneratorTestNullLODScreenRelativeTransitionHeightList()
        {
            NLTechTest.Map.MapLODGenerator lODGenerator;
            lODGenerator = ScriptableObject.CreateInstance<NLTechTest.Map.MapLODGenerator>();

            List<GameObject> lodsGameObjects = new List<GameObject>();
            lodsGameObjects.Add(new GameObject());

            Assert.Throws<System.NullReferenceException>(() => lODGenerator.GenerateLodsFromGameObjects(lodsGameObjects, null));
        }

        [Test]
        public void LOD_Generator_Test_lodsGameObjects()
        {
            NLTechTest.Map.MapLODGenerator lODGenerator;
            lODGenerator = ScriptableObject.CreateInstance<NLTechTest.Map.MapLODGenerator>();

            List<float> lodScreenRelativeTransitionHeightList = new List<float>();
            lodScreenRelativeTransitionHeightList.Add(0.5f);

            List<GameObject> lodsGameObjects = new List<GameObject>();

            Assert.Throws<NLTechTest.Map.MapLODGenerator.ListSizeNotMatching>(() => lODGenerator.GenerateLodsFromGameObjects(lodsGameObjects, lodScreenRelativeTransitionHeightList));
        }

        [Test]
        public void LOD_Generator_Test_lodScreenRelativeTransitionHeightList()
        {
            NLTechTest.Map.MapLODGenerator lODGenerator;
            lODGenerator = ScriptableObject.CreateInstance<NLTechTest.Map.MapLODGenerator>();

            List<float> lodScreenRelativeTransitionHeightList = new List<float>();

            List<GameObject> lodsGameObjects = new List<GameObject>();
            lodsGameObjects.Add(new GameObject());

            Assert.Throws<NLTechTest.Map.MapLODGenerator.ListSizeNotMatching>(() => lODGenerator.GenerateLodsFromGameObjects(lodsGameObjects, lodScreenRelativeTransitionHeightList));
        }

        [Test]
        public void LOD_Generator_Test_Both_lODScreenRelativeTransitionHeightList_And_lodsGameObjects_Empty()
        {
            NLTechTest.Map.MapLODGenerator lODGenerator;
            lODGenerator = ScriptableObject.CreateInstance<NLTechTest.Map.MapLODGenerator>();

            List<float> lodScreenRelativeTransitionHeightList = new List<float>();

            List<GameObject> lodsGameObjects = new List<GameObject>();

            LOD[] expectedLod = new LOD[0];
            LOD[] newLod = lODGenerator.GenerateLodsFromGameObjects(lodsGameObjects, lodScreenRelativeTransitionHeightList);

            Assert.True(expectedLod.Length == newLod.Length);
        }

        [Test]
        public void LOD_Generator_Test_Both_lODScreenRelativeTransitionHeightList_And_lodsGameObjects_SizeOne_LodsGameObject_NoChild()
        {
            NLTechTest.Map.MapLODGenerator lODGenerator;
            lODGenerator = ScriptableObject.CreateInstance<NLTechTest.Map.MapLODGenerator>();

            List<float> lodScreenRelativeTransitionHeightList = new List<float>();
            lodScreenRelativeTransitionHeightList.Add(0.5f);

            List<GameObject> lodsGameObjects = new List<GameObject>();
            lodsGameObjects.Add(new GameObject());

            LOD[] newLod = lODGenerator.GenerateLodsFromGameObjects(lodsGameObjects, lodScreenRelativeTransitionHeightList);

            Assert.True(newLod[0].renderers.Length == 0);
        }

        [Test]
        public void LOD_Generator_Test_Both_lODScreenRelativeTransitionHeightList_And_lodsGameObjects_SizeOne_LodsGameObject_OneChild_With_No_Renderer()
        {
            NLTechTest.Map.MapLODGenerator lODGenerator;
            lODGenerator = ScriptableObject.CreateInstance<NLTechTest.Map.MapLODGenerator>();

            List<float> lodScreenRelativeTransitionHeightList = new List<float>();
            lodScreenRelativeTransitionHeightList.Add(0.5f);

            List<GameObject> lodsGameObjects = new List<GameObject>();
            lodsGameObjects.Add(new GameObject());
            GameObject child = new GameObject();
            child.transform.SetParent(lodsGameObjects[0].transform);

            LOD[] newLod = lODGenerator.GenerateLodsFromGameObjects(lodsGameObjects, lodScreenRelativeTransitionHeightList);

            Assert.True(newLod[0].renderers.Length == 0);
        }

        [Test]
        public void LOD_Generator_Test_Both_lODScreenRelativeTransitionHeightList_And_lodsGameObjects_SizeOne_LodsGameObject_One_Child_With_One_Renderer()
        {
            NLTechTest.Map.MapLODGenerator lODGenerator;
            lODGenerator = ScriptableObject.CreateInstance<NLTechTest.Map.MapLODGenerator>();

            List<float> lodScreenRelativeTransitionHeightList = new List<float>();
            lodScreenRelativeTransitionHeightList.Add(0.5f);

            List<GameObject> lodsGameObjects = new List<GameObject>();
            lodsGameObjects.Add(new GameObject());
            GameObject child = new GameObject();
            child.AddComponent<MeshFilter>();
            child.AddComponent<MeshRenderer>();
            child.transform.SetParent(lodsGameObjects[0].transform);

            LOD[] newLod = lODGenerator.GenerateLodsFromGameObjects(lodsGameObjects, lodScreenRelativeTransitionHeightList);

            Assert.True(newLod[0].renderers.Length == 1);
        }

        [Test]
        public void LOD_Generator_Test_Both_lODScreenRelativeTransitionHeightList_And_lodsGameObjects_SizeOne_LodsGameObject_Two_Child_With_One_Renderer_Each()
        {
            NLTechTest.Map.MapLODGenerator lODGenerator;
            lODGenerator = ScriptableObject.CreateInstance<NLTechTest.Map.MapLODGenerator>();

            List<float> lodScreenRelativeTransitionHeightList = new List<float>();
            lodScreenRelativeTransitionHeightList.Add(0.5f);

            List<GameObject> lodsGameObjects = new List<GameObject>();
            lodsGameObjects.Add(new GameObject());

            for (int i = 0; i < 2; i++)
            {
                GameObject child = new GameObject();
                child.AddComponent<MeshFilter>();
                child.AddComponent<MeshRenderer>();
                child.transform.SetParent(lodsGameObjects[0].transform);
            }


            LOD[] newLod = lODGenerator.GenerateLodsFromGameObjects(lodsGameObjects, lodScreenRelativeTransitionHeightList);

            Assert.True(newLod[0].renderers.Length == 2);
        }

        [Test]
        public void LOD_Generator_Test_Both_lODScreenRelativeTransitionHeightList_And_lodsGameObjects_SizeOne_LodsGameObject_Two_Child_With_One_With_Renderer_One_Without_Renderer()
        {
            NLTechTest.Map.MapLODGenerator lODGenerator;
            lODGenerator = ScriptableObject.CreateInstance<NLTechTest.Map.MapLODGenerator>();

            List<float> lodScreenRelativeTransitionHeightList = new List<float>();
            lodScreenRelativeTransitionHeightList.Add(0.5f);

            List<GameObject> lodsGameObjects = new List<GameObject>();
            lodsGameObjects.Add(new GameObject());

            for (int i = 0; i < 2; i++)
            {
                GameObject child = new GameObject();
                if (i == 0)
                {
                    child.AddComponent<MeshFilter>();
                    child.AddComponent<MeshRenderer>();
                }

                child.transform.SetParent(lodsGameObjects[0].transform);
            }

            LOD[] newLod = lODGenerator.GenerateLodsFromGameObjects(lodsGameObjects, lodScreenRelativeTransitionHeightList);

            Assert.True(newLod[0].renderers.Length == 1);
        }

        [Test]
        public void LOD_Generator_Test_Both_lODScreenRelativeTransitionHeightList_And_lodsGameObjects_SizeTwo_LodsGameObject_One_Child_Each_Parent_With_One_With_Renderer()
        {
            NLTechTest.Map.MapLODGenerator lODGenerator;
            lODGenerator = ScriptableObject.CreateInstance<NLTechTest.Map.MapLODGenerator>();

            List<float> lodScreenRelativeTransitionHeightList = new List<float>();

            lodScreenRelativeTransitionHeightList.Add(0.5f);
            lodScreenRelativeTransitionHeightList.Add(0.75f);

            List<GameObject> lodsGameObjects = new List<GameObject>();


            for (int i = 0; i < 2; i++)
            {
                lodsGameObjects.Add(new GameObject());
                GameObject child = new GameObject();
                child.AddComponent<MeshFilter>();
                child.AddComponent<MeshRenderer>();
                child.transform.SetParent(lodsGameObjects[i].transform);
            }

            LOD[] newLod = lODGenerator.GenerateLodsFromGameObjects(lodsGameObjects, lodScreenRelativeTransitionHeightList);

            foreach (LOD lod in newLod)
                Assert.True(lod.renderers.Length == 1);
        }

        [Test]
        public void LOD_Generator_Test_Both_lODScreenRelativeTransitionHeightList_And_lodsGameObjects_SizeTwo_LodsGameObject_One_Parent_With_One_With_Child_One_With_Two_Child_All_Child_With_Renderer()
        {
            NLTechTest.Map.MapLODGenerator lODGenerator;
            lODGenerator = ScriptableObject.CreateInstance<NLTechTest.Map.MapLODGenerator>();

            List<float> lodScreenRelativeTransitionHeightList = new List<float>();

            lodScreenRelativeTransitionHeightList.Add(0.5f);
            lodScreenRelativeTransitionHeightList.Add(0.75f);

            List<GameObject> lodsGameObjects = new List<GameObject>();


            for (int i = 0; i < 2; i++)
            {
                lodsGameObjects.Add(new GameObject());
                for (int j = 0; j < i + 1; j++)
                {
                    GameObject child = new GameObject();
                    child.AddComponent<MeshFilter>();
                    child.AddComponent<MeshRenderer>();
                    child.transform.SetParent(lodsGameObjects[i].transform);
                }
            }

            LOD[] newLod = lODGenerator.GenerateLodsFromGameObjects(lodsGameObjects, lodScreenRelativeTransitionHeightList);

            for (int i = 0; i < 2; i++)
            {
                Assert.True(newLod[i].renderers.Length == i + 1);
            }
            
        }

    }
}
