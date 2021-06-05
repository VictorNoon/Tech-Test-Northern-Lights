using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NLTechTest.Map
{
    public class SquareTileGenerator : ScriptableObject, ITileGenerator, IColoredTilesGenerator
    {
        private struct TileGenerationData
        {
            public GameObject mapTile;
            public int numberOfTiles;
            public Vector3 centerTilePosition;
            public Vector3 tileScale;
            public int numberOfSubdivisions;
            public Vector3 tileBaseDimensions;
            public Color tilesBaseColor;
        }

        public enum SubdivisionAlgorithmSelector
        {
            NoSubdivisionPossible = -2,
            NoKnownSubdivisionAlgorithmKnown = -1,
            SubdivionAlgorithNotEvaluatedYet = 0,
            EvenSubdivision = 1,
            BorderSubdivision = 2
        }

        [System.Serializable]
        public class GenerationNotPossible : System.Exception
        {
            public GenerationNotPossible() { }
            public GenerationNotPossible(string message) : base(message) { }
            public GenerationNotPossible(string message, System.Exception inner) : base(message, inner) { }
            protected GenerationNotPossible(
                System.Runtime.Serialization.SerializationInfo info,
                System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        }

        private GameObject _mapTile = null;
        private List<SubdivisionAlgorithmSelector> _mapLayerSubdivisionAlgorithm;
        private int _numberOfTiles = 1;
        private Vector3 _centerTilePosition;
        private Vector3 _tileScale;
        private float _mapSize;
        private int _numberOfSubdivisions = 1;
        private Vector3 _tileBaseDimensions;
        private Color _baseColor;
        private bool _baseColorSet = false;
        private bool _colorPaternPrism = false;

        public void SetTileToBegenerated(IGenerateableTile tile)
        {
            _numberOfTiles = 1;
            _numberOfSubdivisions = 1;
            _tileBaseDimensions = tile.GetTileSize();
            _mapTile = tile.GetTileModel();
        }

        public void SetTileGenerationParameters(int numberOfSubdivisions, Vector3 parentPosition, float mapSize)
        {
            _numberOfTiles = numberOfSubdivisions;
            _centerTilePosition = parentPosition;
            _mapSize = mapSize;
            _tileScale = _mapTile.transform.lossyScale * mapSize / Mathf.Sqrt(_numberOfTiles);
            _numberOfSubdivisions = numberOfSubdivisions;
        }

        public List<GameObject> GenerateTiles()
        {
            List<GameObject> tilePlates;

            SubdivisionAlgorithmSelector subdivisionMethod = FindSubdivisionMethodForLayerWithNSubdivision(_numberOfSubdivisions);
            if (subdivisionMethod == SubdivisionAlgorithmSelector.EvenSubdivision)
                tilePlates = GenerateEvenlySubdividedTiles();
            else if (subdivisionMethod == SubdivisionAlgorithmSelector.BorderSubdivision)
                tilePlates = GenerateBorderSubdividedTiles();
            else
                tilePlates = new List<GameObject>();

            return tilePlates;
        }

        public bool IsLayerSubdivisionInNPossible(int subdivisionNumber)
        {
            return IsLayerSubdivisionMethodValid(FindSubdivisionMethodForLayerWithNSubdivision(subdivisionNumber));
        }

        public void ThrowGenerationException()
        {
            throw new GenerationNotPossible();
        }


        private SubdivisionAlgorithmSelector FindSubdivisionMethodForLayerWithNSubdivision(int subdivisionNumber)
        {
            if (CheckForEvenSubdivisionMethod(subdivisionNumber))
                return SubdivisionAlgorithmSelector.EvenSubdivision;
            if (CheckForBorderDistributionMethod(subdivisionNumber))
                return SubdivisionAlgorithmSelector.BorderSubdivision;

            return SubdivisionAlgorithmSelector.NoKnownSubdivisionAlgorithmKnown;
        }

        private List<GameObject> GenerateEvenlySubdividedTiles()
        {
            List<GameObject> tiles;

            tiles = new List<GameObject>();
            for (int i = 0; i < _numberOfTiles; i++)
                tiles.Add(GenerateTileAtIndex(i));

            return tiles;
        }

        private List<GameObject> GenerateBorderSubdividedTiles()
        {
            List<GameObject> tiles;
            TileGenerationData[] genDatas;

            genDatas = GenerateGenerationDatas();

            tiles = new List<GameObject>();
            tiles.AddRange(GenerateBorderTiles(genDatas[0]));
            tiles.AddRange(GenerateCenterTiles(genDatas[1]));
            return tiles;
        }
        private List<GameObject> GenerateBorderTiles(TileGenerationData genData)
        {
            List<GameObject> tiles;
            int trueIndex;

            UpdateGenData(genData);
            tiles = new List<GameObject>();

            trueIndex = CalculateBorderIndexFromTileIndex(0);
            for (int i = 0; trueIndex < _numberOfTiles;)
            {
                tiles.Add(GenerateTileAtIndex(trueIndex));
                trueIndex = CalculateBorderIndexFromTileIndex(++i);
            }


            return tiles;
        }

        private void UpdateGenData(TileGenerationData genData)
        {
            _centerTilePosition = genData.centerTilePosition;
            _mapTile = genData.mapTile;
            _numberOfSubdivisions = genData.numberOfSubdivisions;
            _numberOfTiles = genData.numberOfTiles;
            _tileBaseDimensions = genData.tileBaseDimensions;
            _tileScale = genData.tileScale;
        }

        private int CalculateBorderIndexFromTileIndex(int i)
        {
            int offsetIndex;

            offsetIndex = BorderNavigationOffset(i);

            return i + offsetIndex;
        }

        private int BorderNavigationOffset(int i)
        {
            int offsetIndex;
            int squareSideLenInSquares;
            int centerSquareSideLenInSqures;

            centerSquareSideLenInSqures = CalculateCenterSquareSideLenInSquares();
            squareSideLenInSquares = CalculateCurentSquareSideSizeInTile();

            offsetIndex = ((1 + i - squareSideLenInSquares) / 2) * centerSquareSideLenInSqures;
            offsetIndex = Mathf.Clamp(offsetIndex, 0, centerSquareSideLenInSqures * centerSquareSideLenInSqures);

            return offsetIndex;
        }

        private int CalculateNumberOfCenterTiles()
        {
            return _numberOfSubdivisions - (int)Mathf.Pow(CalculateCenterSquareSideLenInSquares(), 2);
        }

        private int CalculateCenterSquareSideLenInSquares()
        {
            return (int)Mathf.Sqrt(_numberOfSubdivisions) - 2;
        }

        private int CalculateCurentSquareSideSizeInTile()
        {
            return (int)Mathf.Sqrt(_numberOfSubdivisions);
        }

        public int GetMiddleLineIndex(int i)
        {
            int lineLenInSquares;
            int numberOfEmptySquaresOnLine;

            numberOfEmptySquaresOnLine = CalculateCenterSquareSideLenInSquares();
            lineLenInSquares = CalculateCurentSquareSideSizeInTile();

            return i + ((i - lineLenInSquares) / 2) * numberOfEmptySquaresOnLine;
        }

        private IEnumerable<GameObject> GenerateCenterTiles(TileGenerationData genData)
        {
            List<GameObject> tiles;

            UpdateGenData(genData);
            tiles = new List<GameObject>();
            for (int i = 0; i < _numberOfTiles; i++)
                tiles.Add(GenerateTileAtIndex(i));

            return tiles;
        }



        private TileGenerationData[] GenerateGenerationDatas()
        {
            TileGenerationData[] genDatas;

            genDatas = new TileGenerationData[2];

            genDatas[0] = GenerateBorderGenerationData();
            genDatas[1] = GenerateCenterGenerationData(genDatas[0]);

            return genDatas;
        }

        private TileGenerationData GenerateCenterGenerationData(TileGenerationData borderGenData)
        {
            TileGenerationData centerTileGenerationData;

            centerTileGenerationData.numberOfTiles = _numberOfTiles - CalculateNumberOfTilesOfBorderTiles(_numberOfSubdivisions);
            centerTileGenerationData.numberOfSubdivisions = centerTileGenerationData.numberOfTiles;
            centerTileGenerationData.centerTilePosition = _centerTilePosition;
            centerTileGenerationData.mapTile = _mapTile;
            centerTileGenerationData.tileBaseDimensions = _tileBaseDimensions;
            centerTileGenerationData.tileScale = GetAdjustedTileScale(borderGenData, centerTileGenerationData.numberOfTiles);
            centerTileGenerationData.tilesBaseColor = _baseColor;

            return centerTileGenerationData;
        }

        private Vector3 GetAdjustedTileScale(TileGenerationData borderGenData, int numberOfTiles)
        {
            float scaleMultiplier;
            float borderSquareSideLenInSquares;
            float centerSquareSideLenInSquares;

            borderSquareSideLenInSquares = (Mathf.Sqrt(borderGenData.numberOfSubdivisions));
            centerSquareSideLenInSquares = (Mathf.Sqrt(numberOfTiles));

            /*Debug.Log("BSL = " + borderSquareSideLenInSquares);
            Debug.Log("CSL = " + centerSquareSideLenInSquares);*/

            scaleMultiplier = 1f - (2f / borderSquareSideLenInSquares);

            /* Adjustment for even center plate not working Yet, Need to find actuctal formula. Here hard value for 128 */
            if (centerSquareSideLenInSquares % 2 == 0)
                scaleMultiplier -= 0.15f;

            //Debug.Log("scaleMultiplier = " + scaleMultiplier);

            return borderGenData.tileScale * scaleMultiplier;
        }

        private TileGenerationData GenerateBorderGenerationData()
        {
            TileGenerationData borderTileGenerationData;

            borderTileGenerationData.numberOfTiles = CalculateNumberOfBorderSubdivision(CalculateNumberOfTilesOfBorderTiles(_numberOfSubdivisions));
            borderTileGenerationData.numberOfSubdivisions = borderTileGenerationData.numberOfTiles;
            borderTileGenerationData.centerTilePosition = _centerTilePosition;
            borderTileGenerationData.mapTile = _mapTile;
            borderTileGenerationData.tileBaseDimensions = _tileBaseDimensions;
            borderTileGenerationData.tileScale = _mapTile.transform.lossyScale * _mapSize / Mathf.Sqrt(borderTileGenerationData.numberOfTiles);
            borderTileGenerationData.tilesBaseColor = _baseColor;

            return borderTileGenerationData;
        }

        private int CalculateNumberOfBorderSubdivision(int numberOfTiles)
        {
            return (int)Mathf.Pow(((numberOfTiles / 4) + 1), 2);
        }

        private int CalculateNumberOfTilesOfBorderTiles(int subdivisionNumber)
        {
            int numberOfBorderTile = 8;

            while (numberOfBorderTile < subdivisionNumber)
            {
                if (CanBeFilledWithIdenticalSizeSquares(subdivisionNumber - numberOfBorderTile))
                    return numberOfBorderTile;

                numberOfBorderTile += 4;
            }

            return 0;
        }

        private bool IsLayerSubdivisionMethodValid(SubdivisionAlgorithmSelector layerAlgo)
        {
            return layerAlgo > 0;
        }

        private bool CheckForEvenSubdivisionMethod(int subdivisionNumber)
        {
            return CanBeFilledWithIdenticalSizeSquares(subdivisionNumber);
        }

        private void SetLayerSubdivisionMethodTo(int layer, SubdivisionAlgorithmSelector algo)
        {
            _mapLayerSubdivisionAlgorithm[layer] = algo;
        }

        private Vector3 GetPositionOfTileAtIndex(int tileIndex)
        {
            Vector3 tilePosition;

            tilePosition = _centerTilePosition + GetTileOffsetAtIndex(tileIndex);

            return tilePosition;
        }

        private Vector3 GetTileOffsetAtIndex(int i)
        {
            Vector3 tileOffSet;

            tileOffSet = CalculateTileOffsetAtPosition(i);
            tileOffSet = GetOffSetAdjustedByTileBaseDimensions(tileOffSet);

            return tileOffSet;
        }

        private Vector3 CalculateTileOffsetAtPosition(int i)
        {
            int squareSideLenInTiles = (int)Mathf.Sqrt(_numberOfTiles);

            Vector3 tileOffSet = new Vector3((-squareSideLenInTiles / 2 + (i % squareSideLenInTiles)) * _tileScale.x, 0, (-squareSideLenInTiles / 2 + (i / squareSideLenInTiles)) * _tileScale.z);

            tileOffSet += new Vector3(_tileScale.x, 0, _tileScale.z) / 2 * (1 - (squareSideLenInTiles % 2)); //Adjust placement on even numbers of divisions

            return tileOffSet;
        }

        private Vector3 GetOffSetAdjustedByTileBaseDimensions(Vector3 tileOffSet)
        {
            tileOffSet = new Vector3(tileOffSet.x * _tileBaseDimensions.x, tileOffSet.y * _tileBaseDimensions.y, tileOffSet.z * _tileBaseDimensions.z);

            return tileOffSet;
        }

        private void ScaleTile(GameObject newTile)
        {
            Vector3 newTileScale;

            newTileScale = newTile.transform.localScale;

            newTile.transform.localScale = new Vector3(newTileScale.x * _tileScale.x, newTileScale.y * _tileScale.y, newTileScale.z * _tileScale.z);
        }

        private Quaternion GetTileOrientationAtIndex(int tileIndex)
        {
            return _mapTile.transform.rotation;
        }

        private GameObject GenerateTileAtIndex(int tileIndex)
        {
            GameObject newTile;
            Vector3 tilePosition;
            Quaternion tileOrientation;

            tilePosition = GetPositionOfTileAtIndex(tileIndex);
            tileOrientation = GetTileOrientationAtIndex(tileIndex);

            newTile = Instantiate(_mapTile, tilePosition, tileOrientation);
            newTile.GetComponent<IGenerateableTile>().InitializeTile();

            ApplyColorOnTileIfAble(newTile, tileIndex);

            newTile.name = _mapTile.name;
            ScaleTile(newTile);

            return newTile;
        }

        private void ApplyColorOnTileIfAble(GameObject newTile, int tileIndex)
        {
            IPaintableTile tile;
            Color color;

            tile = newTile.GetComponent<IPaintableTile>();
            if (tile == null)
                return ;
                
            color = CalculateTileColor(tileIndex);
            tile.SetTileColor(color);
        }

        private Color CalculateTileColor(int tileIndex)
        {
            if (_colorPaternPrism)
            {
                if (_baseColorSet)
                    switch (tileIndex % 4)
                    {
                        case 0:
                            return new Color (_baseColor.r, 0, 0);
                        case 1:
                            return new Color (0, _baseColor.g, 0);
                        case 2:
                            return new Color (0, 0, _baseColor.b);
                        default:
                            return _baseColor;
                    }
                else
                {
                    switch (tileIndex % 4)
                    {
                        case 0:
                            return Color.red;
                        case 1:
                            return Color.green;
                        case 2:
                            return Color.blue;
                        default:
                            return Color.white;
                    }

                }

            }
            else
            {
                if (_baseColorSet)
                    return _baseColor * (1 - ((float)tileIndex / (float)_numberOfSubdivisions));
                else
                    return Color.white * (1 - ((float)tileIndex / (float)_numberOfSubdivisions));
            }

        }

        private bool CanBeFilledWithIdenticalSizeSquares(int number)
        {
            if (number < 1)
                return false;

            return Mathf.Sqrt(number) % 1 == 0;
        }

        private bool CheckForBorderDistributionMethod(int subdivisionNumber)
        {
            int borderSize = 8;

            while (borderSize < subdivisionNumber)
            {
                if (CanBeFilledWithIdenticalSizeSquares(subdivisionNumber - borderSize))
                    return true;
                borderSize += 4;
            }

            return false;
        }

        public void SetBaseTilesColor(Color baseTileColor)
        {
            _baseColor = baseTileColor;
            _baseColorSet = true;
        }

        public void SetColorPaternRulePrism()
        {
            _colorPaternPrism = true;
        }

        public void SetColorPaternRuleShade()
        {
            _colorPaternPrism = false;
        }
    }
}