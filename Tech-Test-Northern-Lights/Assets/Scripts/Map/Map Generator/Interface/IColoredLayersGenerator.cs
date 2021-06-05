using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IColoredTilesGenerator
{
    void SetBaseTilesColor(Color baseTileColor);
    void SetColorPaternRulePrism();
    void SetColorPaternRuleShade();
}
