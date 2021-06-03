using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class MapTileGeneratorTest
    {
        // A Test behaves as an ordinary method
        [Test]
        public void IsTileGeneratorInitialized_Pre_initializationCall_Pre_initializationData_Creation()
        {
            NLTechTest.Map.MapLayerGenerator mapTilesGenerator;
            mapTilesGenerator = ScriptableObject.CreateInstance<NLTechTest.Map.MapLayerGenerator>();

            Assert.AreEqual(false, mapTilesGenerator.IsInitialised());
        }

        [Test]
        public void IsTileGeneratorInitialized_Pre_initializationCall_Post_initializationData_Creation()
        {
            NLTechTest.Map.MapLayerGenerator mapTilesGenerator;
            NLTechTest.Map.MapLayerGenerator.LayerGeneratorData initData;
            GameObject mapParent;
            List<int> mapLevelSubdivisions;

            mapTilesGenerator = ScriptableObject.CreateInstance<NLTechTest.Map.MapLayerGenerator>();
            mapParent = new GameObject();
            mapLevelSubdivisions = GenerateMockUpLevelSubdivisions_type_Incremental_List_Start_One(5);
            initData = mapTilesGenerator.GenerateInitialisationData(new GameObject(), mapParent.transform, mapLevelSubdivisions);

            Assert.AreEqual(false, mapTilesGenerator.IsInitialised());
        }

        [Test]
        public void IsTileGeneratorInitialized_Post_initializationCall_Custom_Tile_Generator_Data_mapLayerSubdivisionsAmount_Is_Null()
        {
            NLTechTest.Map.MapLayerGenerator mapTilesGenerator;

            mapTilesGenerator = ScriptableObject.CreateInstance<NLTechTest.Map.MapLayerGenerator>();

            Assert.Throws<NLTechTest.Map.MapLayerGenerator.GeneratorInitializationIncorrect>(() => mapTilesGenerator.InitialiseTileGenerator(new NLTechTest.Map.MapLayerGenerator.LayerGeneratorData()));
        }

        [Test]
        public void IsTileGeneratorInitialized_Post_initializationCall_Custom_Tile_Generator_Data_mapLayerSubdivisionsAmount_Is_Empty()
        {
            NLTechTest.Map.MapLayerGenerator mapTilesGenerator;
            NLTechTest.Map.MapLayerGenerator.LayerGeneratorData initializationData;

            mapTilesGenerator = ScriptableObject.CreateInstance<NLTechTest.Map.MapLayerGenerator>();
            initializationData = new NLTechTest.Map.MapLayerGenerator.LayerGeneratorData();
            initializationData.mapLayerSubdivisionsAmount = new List<int>();

            mapTilesGenerator.InitialiseTileGenerator(initializationData);

            Assert.AreEqual(true, mapTilesGenerator.IsInitialised());
        }

        [Test]
        public void IsTileGeneratorInitialized_Post_initializationCall_Post_initializationData_Creation_With_Automatic_Data()
        {
            NLTechTest.Map.MapLayerGenerator mapTilesGenerator;
            NLTechTest.Map.MapLayerGenerator.LayerGeneratorData initData;
            GameObject mapParent;
            List<int> mapLevelSubdivisions;

            mapTilesGenerator = ScriptableObject.CreateInstance<NLTechTest.Map.MapLayerGenerator>();
            mapParent = new GameObject();
            mapLevelSubdivisions = GenerateMockUpLevelSubdivisions_type_Incremental_List_Start_One(5);

            initData = mapTilesGenerator.GenerateInitialisationData(new GameObject(), mapParent.transform, mapLevelSubdivisions);
            mapTilesGenerator.InitialiseTileGenerator(initData);

            Assert.AreEqual(true, mapTilesGenerator.IsInitialised());
        }

        [Test]
        public void TileGeneratorInitialized_Generate_Empty_Subdivisions_List()
        {
            NLTechTest.Map.MapLayerGenerator mapTilesGenerator;
            List<GameObject> mapLayers;

            mapTilesGenerator = GetInitializedGeneratorLinearSubdivisions(0);
            mapLayers = mapTilesGenerator.Generate();

            Assert.AreEqual(0, mapLayers.Count);
        }

        [Test]
        public void TileGeneratorInitialized_Generate_Subdivisions_Layer_List_One_Subdivision_CheckNumber_Of_Layers()
        {
            NLTechTest.Map.MapLayerGenerator mapTilesGenerator;
            List<GameObject> mapLayers;

            mapTilesGenerator = GetInitializedGeneratorLinearSubdivisions(1);
            mapLayers = mapTilesGenerator.Generate();

            Assert.AreEqual(1, mapLayers.Count);
            Assert.AreEqual(1, mapLayers[0].transform.childCount);
        }

        [Test]
        public void TileGeneratorInitialized_Generate_Subdivisions_Layer_List_One_Four_Nine_Subdivision()
        {
            NLTechTest.Map.MapLayerGenerator mapTilesGenerator;
            List<GameObject> mapLayers;
            int totalSubdivisionCount;

            mapTilesGenerator = GetInitializedGeneratorSquareSubdivisions(3);
            mapLayers = mapTilesGenerator.Generate();
            mapLayers.Reverse();

            totalSubdivisionCount = 1;
            for (int i = 0; i < mapLayers.Count; i++)
            {
                int layerSubdivisions = (i + 1) * (i + 1);
                totalSubdivisionCount *= layerSubdivisions;
                Assert.AreEqual(totalSubdivisionCount, mapLayers[i].transform.childCount);
            }

        }


        [Test]
        public void TileGeneratorInitialized_Generate_Subdivisions_List_Zero_To_Ten_Subdivision_CheckNumber_Of_Layers()
        {
            NLTechTest.Map.MapLayerGenerator mapTilesGenerator;
            List<GameObject> mapLayers;

            for (int i = 0; i < 10; i++)
            {
                mapTilesGenerator = GetInitializedGeneratorUniqueSubdivision(i);
                if (Mathf.Sqrt(i) % 1 == 0)
                {
                    mapLayers = mapTilesGenerator.Generate();
                    Assert.AreEqual(1, mapLayers.Count);
                }
                else
                {
                    Assert.Throws<NLTechTest.Map.MapTileGenerator.GenerationNotPossible>(() => mapTilesGenerator.Generate());
                }

            }

        }


        private List<int> GenerateMockUpLevelSubdivisions_type_unique(int numberOfSubdivisions)
        {
            List<int> list;

            list = new List<int>();
            list.Add(numberOfSubdivisions);

            return list;
        }
        private List<int> GenerateMockUpLevelSubdivisions_type_Incremental_List_Start_One(int numberOfLines)
        {
            List<int> list;

            list = new List<int>();
            for (int i = 1; i <= numberOfLines; i++)
                list.Add(i);

            return list;
        }

        private List<int> GenerateMockUpLevelSubdivisions_type_Square_List_Start_One(int numberOfLines)
        {
            List<int> list;

            list = new List<int>();
            for (int i = 1; i <= numberOfLines; i++)
                list.Add(i * i);

            return list;
        }

        private NLTechTest.Map.MapLayerGenerator GetInitializedGeneratorUniqueSubdivision(int numberOfSubdivisions)
        {
            NLTechTest.Map.MapLayerGenerator mapTilesGenerator;
            NLTechTest.Map.MapLayerGenerator.LayerGeneratorData initData;
            GameObject mapParent;
            List<int> mapLevelSubdivisions;

            mapTilesGenerator = ScriptableObject.CreateInstance<NLTechTest.Map.MapLayerGenerator>();
            mapParent = new GameObject();
            mapLevelSubdivisions = GenerateMockUpLevelSubdivisions_type_unique(numberOfSubdivisions);

            initData = mapTilesGenerator.GenerateInitialisationData(new GameObject(), mapParent.transform, mapLevelSubdivisions);
            mapTilesGenerator.InitialiseTileGenerator(initData);

            return mapTilesGenerator;
        }

        private NLTechTest.Map.MapLayerGenerator GetInitializedGeneratorLinearSubdivisions(int numberOfSubdivisions)
        {
            NLTechTest.Map.MapLayerGenerator mapTilesGenerator;
            NLTechTest.Map.MapLayerGenerator.LayerGeneratorData initData;
            GameObject mapParent;
            List<int> mapLevelSubdivisions;

            mapTilesGenerator = ScriptableObject.CreateInstance<NLTechTest.Map.MapLayerGenerator>();
            mapParent = new GameObject();
            mapLevelSubdivisions = GenerateMockUpLevelSubdivisions_type_Incremental_List_Start_One(numberOfSubdivisions);

            initData = mapTilesGenerator.GenerateInitialisationData(new GameObject(), mapParent.transform, mapLevelSubdivisions);
            mapTilesGenerator.InitialiseTileGenerator(initData);

            return mapTilesGenerator;
        }

        private NLTechTest.Map.MapLayerGenerator GetInitializedGeneratorSquareSubdivisions(int numberOfSubdivisions)
        {
            NLTechTest.Map.MapLayerGenerator mapTilesGenerator;
            NLTechTest.Map.MapLayerGenerator.LayerGeneratorData initData;
            GameObject mapParent;
            List<int> mapLevelSubdivisions;

            mapTilesGenerator = ScriptableObject.CreateInstance<NLTechTest.Map.MapLayerGenerator>();
            mapParent = new GameObject();
            mapLevelSubdivisions = GenerateMockUpLevelSubdivisions_type_Square_List_Start_One(numberOfSubdivisions);

            initData = mapTilesGenerator.GenerateInitialisationData(new GameObject(), mapParent.transform, mapLevelSubdivisions);
            mapTilesGenerator.InitialiseTileGenerator(initData);

            return mapTilesGenerator;
        }
    }
}
