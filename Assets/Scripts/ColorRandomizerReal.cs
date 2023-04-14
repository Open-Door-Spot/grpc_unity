using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorRandomizerReal : MonoBehaviour
{
    public Material material;
    public Button randomizeButton;
    private readonly Color _defaultColor = Color.red;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Script is triggered!");
        Button btn = randomizeButton.GetComponent<Button>();
        btn.onClick.AddListener(ChangeColor);
    }

    public void ChangeColor()
    {
        material.color = GetColor(material.color);
    }

    private Color GetColor(Color currentColor)
    {
        UnityEngine.Debug.Log("Got a request to change the color");
        return _defaultColor;
    }
}
