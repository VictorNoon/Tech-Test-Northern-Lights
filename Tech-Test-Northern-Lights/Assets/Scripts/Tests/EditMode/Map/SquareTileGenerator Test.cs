using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class SquareTileGeneratorTest
    {
        // A Test behaves as an ordinary method
        [Test]
        public void SquareTileGenerator_Uninitialized_Generate()
        {
            NLTechTest.Map.SquareTileGenerator generator;
            generator = ScriptableObject.CreateInstance<NLTechTest.Map.SquareTileGenerator>();

            Assert.Throws<System.NullReferenceException>(() => generator.GenerateTiles());
        }

        [Test]
        public void SquareTileGenerator_Initialized_With_Null()
        {
            NLTechTest.Map.SquareTileGenerator generator;
            generator = ScriptableObject.CreateInstance<NLTechTest.Map.SquareTileGenerator>();


            Assert.Throws<System.NullReferenceException>(() => generator.SetTileToBegenerated(null));
        }

        [Test]
        public void SquareTileGenerator_Initialized_With_Valid_Gameobject_Generate_Without_Setting_GenerationRules()
        {
            NLTechTest.Map.SquareTileGenerator generator;
            List<GameObject> generatorOutput;
            IGenerateableTile tile;

            generator = ScriptableObject.CreateInstance<NLTechTest.Map.SquareTileGenerator>();
            tile = GetNewSquareTile();

            generator.SetTileToBegenerated(tile);
            generatorOutput = generator.GenerateTiles();

            Assert.AreEqual(1, generatorOutput.Count);
            Assert.AreEqual(tile.GetTileModel().name, generatorOutput[0].name);
            Assert.AreEqual(Vector3.zero, generatorOutput[0].transform.localScale);
        }

        [Test]
        public void SquareTileGenerator_Generate_Valid_OneTile_Map_Size_One()
        {
            NLTechTest.Map.SquareTileGenerator generator;
            List<GameObject> generatorOutput;
            IGenerateableTile tile;


            generator = ScriptableObject.CreateInstance<NLTechTest.Map.SquareTileGenerator>();
            tile = GetNewSquareTile();

            generator.SetTileToBegenerated(tile);
            generator.SetTileGenerationParameters(1, Vector3.zero, 1);
            generatorOutput = generator.GenerateTiles();

            Assert.AreEqual(1, generatorOutput.Count);
            Assert.AreEqual(tile.GetTileModel().name, generatorOutput[0].name);
            Assert.AreEqual(tile.GetTileModel().transform.localScale, generatorOutput[0].transform.localScale);
            Assert.AreEqual(Vector3.zero, generatorOutput[0].transform.position);
        }

        [Test]
        public void SquareTileGenerator_Generate_Valid_One_Tile_Map_Size_Two()
        {
            NLTechTest.Map.SquareTileGenerator generator;
            List<GameObject> generatorOutput;
            IGenerateableTile tile;


            generator = ScriptableObject.CreateInstance<NLTechTest.Map.SquareTileGenerator>();
            tile = GetNewSquareTile();

            generator.SetTileToBegenerated(tile);
            generator.SetTileGenerationParameters(1, Vector3.zero, 2);
            generatorOutput = generator.GenerateTiles();

            Assert.AreEqual(1, generatorOutput.Count);
            Assert.AreEqual(tile.GetTileModel().name, generatorOutput[0].name);
            Assert.AreEqual(tile.GetTileModel().transform.localScale * 2, generatorOutput[0].transform.localScale);
            Assert.AreEqual(Vector3.zero, generatorOutput[0].transform.position);
        }

        [Test]
        public void SquareTileGenerator_Generate_Valid_Four_Tile_Map_Size_One()
        {
            NLTechTest.Map.SquareTileGenerator generator;
            List<GameObject> generatorOutput;
            IGenerateableTile tile;
            Vector3 parentPositon;


            generator = ScriptableObject.CreateInstance<NLTechTest.Map.SquareTileGenerator>();
            tile = GetNewSquareTile();
            parentPositon = Vector3.zero;


            generator.SetTileToBegenerated(tile);
            generator.SetTileGenerationParameters(4, parentPositon, 1);
            generatorOutput = generator.GenerateTiles();

            Assert.AreEqual(4, generatorOutput.Count);

            List<Vector3> expectedPositions = new List<Vector3>();
            expectedPositions.Add(new Vector3(-0.25f, 0f, -0.25f));
            expectedPositions.Add(new Vector3(0.25f, 0f, -0.25f));
            expectedPositions.Add(new Vector3(-0.25f, 0f, 0.25f));
            expectedPositions.Add(new Vector3(0.25f, 0f, 0.25f));

            for (int i = 0; i < generatorOutput.Count; i++)
            {
                Assert.AreEqual(tile.GetTileModel().name, generatorOutput[i].name);
                Assert.AreEqual(tile.GetTileModel().transform.localScale / 2, generatorOutput[i].transform.localScale);
                Assert.AreEqual(expectedPositions[i], generatorOutput[i].transform.position);
            }
        }

        [Test]
        public void SquareTileGenerator_Generate_Valid_Four_Tile_Map_Size_Two()
        {
            NLTechTest.Map.SquareTileGenerator generator;
            List<GameObject> generatorOutput;
            IGenerateableTile tile;
            Vector3 parentPositon;


            generator = ScriptableObject.CreateInstance<NLTechTest.Map.SquareTileGenerator>();
            tile = GetNewSquareTile();
            parentPositon = Vector3.zero;


            generator.SetTileToBegenerated(tile);
            generator.SetTileGenerationParameters(4, parentPositon, 2);
            generatorOutput = generator.GenerateTiles();

            Assert.AreEqual(4, generatorOutput.Count);

            List<Vector3> expectedPositions = new List<Vector3>();
            expectedPositions.Add(new Vector3(-0.5f, 0f, -0.5f));
            expectedPositions.Add(new Vector3(0.5f, 0f, -0.5f));
            expectedPositions.Add(new Vector3(-0.5f, 0f, 0.5f));
            expectedPositions.Add(new Vector3(0.5f, 0f, 0.5f));

            for (int i = 0; i < generatorOutput.Count; i++)
            {
                Assert.AreEqual(tile.GetTileModel().name, generatorOutput[i].name);
                Assert.AreEqual(tile.GetTileModel().transform.localScale, generatorOutput[i].transform.localScale);
                Assert.AreEqual(expectedPositions[i], generatorOutput[i].transform.position);
            }
        }

        [Test]
        public void SquareTileGenerator_Generate_Valid_Nine_Tile_Map_Size_One()
        {
            NLTechTest.Map.SquareTileGenerator generator;
            List<GameObject> generatorOutput;
            IGenerateableTile tile;
            Vector3 parentPositon;


            generator = ScriptableObject.CreateInstance<NLTechTest.Map.SquareTileGenerator>();
            tile = GetNewSquareTile();
            parentPositon = Vector3.zero;


            generator.SetTileToBegenerated(tile);
            generator.SetTileGenerationParameters(9, parentPositon, 1);
            generatorOutput = generator.GenerateTiles();

            Assert.AreEqual(9, generatorOutput.Count);

            List<Vector3> expectedPositions = new List<Vector3>();
            expectedPositions.Add(new Vector3(-1f / 3f, 0f, -1f / 3f));
            expectedPositions.Add(new Vector3(0f, 0f, -1f / 3f));
            expectedPositions.Add(new Vector3(1f / 3f, 0f, -1f / 3f));
            expectedPositions.Add(new Vector3(-1f / 3f, 0f, 0f));
            expectedPositions.Add(new Vector3(0, 0f, 0f));
            expectedPositions.Add(new Vector3(1f / 3f, 0f, 0f));
            expectedPositions.Add(new Vector3(-1f / 3f, 0f, 1f / 3f));
            expectedPositions.Add(new Vector3(0, 0f, 1f / 3f));
            expectedPositions.Add(new Vector3(1f / 3f, 0f, 1f / 3f));

            for (int i = 0; i < generatorOutput.Count; i++)
            {
                Assert.AreEqual(tile.GetTileModel().name, generatorOutput[i].name);
                Assert.AreEqual(tile.GetTileModel().transform.localScale / 3f, generatorOutput[i].transform.localScale);
                Assert.AreEqual(expectedPositions[i], generatorOutput[i].transform.position);
            }
        }

        [Test]
        public void SquareTileGenerator_Generate_Valid_Nine_Tile_Map_Size_Two()
        {
            NLTechTest.Map.SquareTileGenerator generator;
            List<GameObject> generatorOutput;
            IGenerateableTile tile;
            Vector3 parentPositon;


            generator = ScriptableObject.CreateInstance<NLTechTest.Map.SquareTileGenerator>();
            tile = GetNewSquareTile();
            parentPositon = Vector3.zero;


            generator.SetTileToBegenerated(tile);
            generator.SetTileGenerationParameters(9, parentPositon, 2);
            generatorOutput = generator.GenerateTiles();

            Assert.AreEqual(9, generatorOutput.Count);

            List<Vector3> expectedPositions = new List<Vector3>();
            expectedPositions.Add(new Vector3(-2f / 3f, 0f, -2f / 3f));
            expectedPositions.Add(new Vector3(0f, 0f, -2f / 3f));
            expectedPositions.Add(new Vector3(2f / 3f, 0f, -2f / 3f));
            expectedPositions.Add(new Vector3(-2f / 3f, 0f, 0f));
            expectedPositions.Add(new Vector3(0, 0f, 0f));
            expectedPositions.Add(new Vector3(2f / 3f, 0f, 0f));
            expectedPositions.Add(new Vector3(-2f / 3f, 0f, 2f / 3f));
            expectedPositions.Add(new Vector3(0, 0f, 2f / 3f));
            expectedPositions.Add(new Vector3(2f / 3f, 0f, 2f / 3f));

            for (int i = 0; i < generatorOutput.Count; i++)
            {
                Assert.AreEqual(tile.GetTileModel().name, generatorOutput[i].name);
                Assert.AreEqual(tile.GetTileModel().transform.localScale * 2f / 3f, generatorOutput[i].transform.localScale);
                Assert.AreEqual(expectedPositions[i], generatorOutput[i].transform.position);
            }
        }

        //[TEST] Not Implemented Yet
        public void SquareTileGenerator_Generate_Valid_SevenTeen_Tile_Map_Size_1()
        {
            NLTechTest.Map.SquareTileGenerator generator;
            List<GameObject> generatorOutput;
            IGenerateableTile tile;
            Vector3 parentPositon;


            generator = ScriptableObject.CreateInstance<NLTechTest.Map.SquareTileGenerator>();
            tile = GetNewSquareTile();
            parentPositon = Vector3.zero;


            generator.SetTileToBegenerated(tile);
            generator.SetTileGenerationParameters(17, parentPositon, 1);
            generatorOutput = generator.GenerateTiles();

            Assert.AreEqual(17, generatorOutput.Count);

            List<Vector3> expectedPositions = new List<Vector3>();
            //Expected Border tiles
            expectedPositions.Add(new Vector3(-1f / 3f, 0f, -1f / 3f));
            expectedPositions.Add(new Vector3(0f, 0f, -1f / 3f));
            expectedPositions.Add(new Vector3(1f / 3f, 0f, -1f / 3f));
            expectedPositions.Add(new Vector3(-1f / 3f, 0f, 0f));
            expectedPositions.Add(new Vector3(1f / 3f, 0f, 0f));
            expectedPositions.Add(new Vector3(-1f / 3f, 0f, 1f / 3f));
            expectedPositions.Add(new Vector3(0, 0f, 1f / 3f));
            expectedPositions.Add(new Vector3(1f / 3f, 0f, 1f / 3f));
            //Expected Center tiles
            expectedPositions.Add(new Vector3(1f / 9f, 0f, 1f / 9f));
            expectedPositions.Add(new Vector3(0f, 0f, 1f / 9f));
            expectedPositions.Add(new Vector3(1f / 9f, 0f, 1f / 9f));
            expectedPositions.Add(new Vector3(1f / 9f, 0f, 0f));
            expectedPositions.Add(new Vector3(0, 0f, 0f));
            expectedPositions.Add(new Vector3(1f / 9f, 0f, 0f));
            expectedPositions.Add(new Vector3(1f / 9f, 0f, 1f / 9f));
            expectedPositions.Add(new Vector3(0, 0f, 1f / 9f));
            expectedPositions.Add(new Vector3(1f / 9f, 0f, 1f / 9f));

            for (int i = 0; i < generatorOutput.Count; i++)
            {
                Assert.AreEqual(tile.GetTileModel().name, generatorOutput[i].name);
                if (i < 8)
                    Assert.AreEqual(tile.GetTileModel().transform.localScale * 1f / 3f, generatorOutput[i].transform.localScale);
                else
                    Assert.AreEqual(tile.GetTileModel().transform.localScale * 1f / 9f, generatorOutput[i].transform.localScale);
                Assert.AreEqual(expectedPositions[i], generatorOutput[i].transform.position);
            }
        }

        private IGenerateableTile GetNewSquareTile()
        {
            GameObject tile = new GameObject();
            NLTechTest.Map.SquareTile tileInterface = tile.AddComponent<NLTechTest.Map.SquareTile>();
            tileInterface.tileSize = Vector3.one;

            return tileInterface;
        }
    }
}
