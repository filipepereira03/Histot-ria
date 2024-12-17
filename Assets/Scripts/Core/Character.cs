using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character
{
    public string name;
    public Color color;
    public string sideImage;

    public Character(string name, Color color, string sideImage)
    {
        this.name = name;
        this.color = color;
        this.sideImage = sideImage;
        check();
    }

    public Character(string name, Color color)
    {
        this.name = name;
        this.color = color;
        this.sideImage = null;
        check();
    }

    public Character(string name)
    {
        this.name = name;
        this.color = Color.blue;
        this.sideImage = null;
        check();
    }

    private void check()
    {
        if (string.IsNullOrEmpty(this.name))
        {
            Debug.LogWarning("Name must contain a valid string");
        }
    }
}
