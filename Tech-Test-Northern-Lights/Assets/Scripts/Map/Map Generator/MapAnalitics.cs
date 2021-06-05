using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NLTechTest.Map
{
    public class MapAnalitics : MonoBehaviour
    {
        private static int _visibleTileCount = 0;

        public static void IncreaseVisibleTileCount()
        {
            _visibleTileCount += 1;
        }

        public static void DecreaseVisibleTileCount()
        {
            _visibleTileCount -= 1;
        }

        public static int GetVisibleTileCount()
        {
            return _visibleTileCount;
        }
    }
}

