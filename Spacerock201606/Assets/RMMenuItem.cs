using UnityEngine;
using System.Collections;
using System;

public class RMMenuItem : MonoBehaviour {

    public Material selectedMaterial;
    public Material defaultMaterial;
    public TextMesh titleTextMesh;

    public int index = 0;
    public Func<RMMenuItem, int> onClick;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetSelected ()
    {
        onClick(this);
    }

    public void SetTitle (string txt)
    {
        titleTextMesh.text = txt;
    }

    public string GetTitle ()
    {
        return titleTextMesh.text;
    }

    public void SetHighlighted (bool on)
    {
        gameObject.GetComponent<Renderer>().material = on ? selectedMaterial : defaultMaterial;
    }

}
