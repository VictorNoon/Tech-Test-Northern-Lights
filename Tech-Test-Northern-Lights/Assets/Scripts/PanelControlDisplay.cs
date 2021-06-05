using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PanelControlDisplay : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private string labelText = "Number of visible tiles";

    [SerializeField]
    private Text textLabel;
    private Canvas canvas;
    private float timer;

    void Start()
    {
        canvas = GetComponent<Canvas>();
        canvas.enabled = false;
        enabled = false;
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer < 0) {
            canvas.enabled = false;
            enabled = false;
        }
            
    }

    private void UpdateText()
    {
        textLabel.text = labelText + NLTechTest.Map.MapAnalitics.GetVisibleTileCount();
    }

    public void DisplayForNSecond(float duration)
    {
        UpdateText();
        canvas.enabled = true;
        enabled = true;
        timer = duration;
    }
}
