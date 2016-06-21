﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Nodelike : MonoBehaviour {
    protected float scale = 0.10f;

    public Color color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    public Color selectedColor = new Color(0.0f, 1.0f, 0.3f, 1.0f);
    public Color hoveredColor = Color.green;

    public int degree;
    public int followerCount;
    public int group;

    public float timeSelected = 0.0f;

    bool selected = false;

    protected MeshRenderer myRenderer;

    public GameObject myTextMeshGameObject;
    public TextMesh myTextMesh;

    public float x_3d;
    public float y_3d;
    public float z_3d;
    public Vector3 endPosition_3d;

    public float x_2d;
    public float y_2d;
    public Vector3 endPosition_2d;

    int crawlstate = 0;
    static int NORMAL = 0;
    static int CRAWL_TOWARDS_3D = 3;
    static int CRAWL_TOWARDS_2D = 2;

    static float CRAWL_SPEED = 4.0f;

    List<Color> colors;

    public float[,] image = new float[GenerateGraph.MNIST_IMAGE_SIZE, GenerateGraph.MNIST_IMAGE_SIZE];

    void Start()
    {
        myRenderer = this.GetComponent<MeshRenderer>();
        myTextMesh = myTextMeshGameObject.GetComponent<TextMesh>();

        AddColorsToColorList();

        SetScale(scale);

        SetColor(color);

        DeactivateText();

    }

    // Update is called once per frame
    void Update()
    {
        if (crawlstate != NORMAL)
        {
            Crawl();
        }
    }

    void Crawl()
    {
        if (crawlstate == CRAWL_TOWARDS_3D)
        {
            Vector3 local_endPosition_3d = this.transform.parent.transform.position - endPosition_3d;
            float step = CRAWL_SPEED * Time.deltaTime;
            gameObject.transform.position = Vector3.MoveTowards(transform.position, local_endPosition_3d, step);

            if (local_endPosition_3d == gameObject.transform.position)
            {
                crawlstate = NORMAL;
            }

        }
        else if (crawlstate == CRAWL_TOWARDS_2D)
        {
            Vector3 local_endPosition_2d = this.transform.parent.transform.position - endPosition_2d;
            float step = CRAWL_SPEED * Time.deltaTime;
            gameObject.transform.position = Vector3.MoveTowards(transform.position, local_endPosition_2d, step);
            if (local_endPosition_2d == gameObject.transform.position)
            {
                crawlstate = NORMAL;
            }

        }

    }

    void FixedUpdate()
    {
    }

    public virtual void crawlTowardsNewPosition(int dimensionality)
    {

        // recalculate edges
        // initial position and final position are calculated the same way
        // wizardry with parents will need to be observed at some point

        if (dimensionality == GenerateGraph.GRAPH_3D)
        {
            endPosition_3d = new Vector3(
                    x_3d,
                    y_3d,
                    z_3d
                );
            crawlstate = CRAWL_TOWARDS_3D;

        }
        else
        {
            endPosition_2d = new Vector3(
                    x_2d,
                    y_2d,
                    0.0f
                );
            crawlstate = CRAWL_TOWARDS_2D;
        }
    }

    public void SetScale(float newScale)
    {
        scale = newScale;
        this.transform.localScale = new Vector3(scale, scale, scale);
    }

    public void SetScaleFromDegree(int degree)
    {
        SetScale(Mathf.Log(degree) / 10.0f);
    }

    public void SetColor(Color newColor)
    {
        color = newColor;
        this.myRenderer.material.color = newColor;
    }

    public void SetColorByGroup(int group)
    {

        if (colors == null || colors.Count == 0)
        {
            AddColorsToColorList();
        }

        Color newColor = new Color(Random.value, Random.value, Random.value, 1.0f);

        //Debug.Log ("group: " + group + "... r: " + r + "... g: " + g + "... b: " + b);

        newColor = colors[group % colors.Count];

        color = newColor;
    }

    public void RevertColor()
    {
        this.myRenderer.material.color = color;
    }

    public void Selected()
    {
        this.myRenderer.material.color = selectedColor;
    }

    public void Hovered()
    {
        this.myRenderer.material.color = hoveredColor;
    }

    public void Unselected()
    {
        this.myRenderer.material.color = color;
    }


    public void RandomText()
    {
        float r = Random.value;
        if (r < 0.20f)
        {
            myTextMesh.text = "synergy";
        }
        else if (r < 0.40f)
        {
            myTextMesh.text = "robots";
        }
        else if (r < 0.60f)
        {
            myTextMesh.text = "cats";
        }
        else if (r < 0.80f)
        {
            myTextMesh.text = "love";
        }
        else
        {
            myTextMesh.text = "value";
        }
    }

    public void SetText(string newText)
    {
        myTextMesh = myTextMeshGameObject.GetComponent<TextMesh>();
        myTextMesh.text = newText;
    }

    public void DeactivateText()
    {
        myTextMeshGameObject.SetActive(false);
    }

    public void ActivateText()
    {
        myTextMeshGameObject.SetActive(true);
    }

    public void TextFaceCamera(Transform cameraPosition)
    {
        myTextMesh.transform.rotation = cameraPosition.rotation;
    }

    protected void AddColorsToColorList()
    {
        colors = new List<Color>();

        colors.Add(new Color(1.0f, 0.0f, 0.0f, 1.0f));
        colors.Add(new Color(1.0f, 128.0f / 255.0f, 0.0f, 1.0f));
        //colors.Add (new Color(1.0f, 1.0f, 0.0f, 1.0f));
        colors.Add(new Color(1.0f, 1.0f, 1.0f, 1.0f));

        colors.Add(new Color(0.0f, 0.0f, 0.0f, 1.0f));

        colors.Add(new Color(1.0f, 204.0f / 255.0f, 204.0f / 255.0f, 1.0f));

        colors.Add (new Color(128.0f/255.0f, 1.0f, 0.0f, 1.0f));
        //colors.Add (new Color(0.0f, 1.0f, 0.0f, 1.0f));
        colors.Add (new Color(0.0f, 1.0f, 128.0f/255.0f, 1.0f));

        //colors.Add (new Color(0.0f, 1.0f, 1.0f, 1.0f));
        //colors.Add (new Color(0.0f, 0.5f, 1.0f, 1.0f));
        colors.Add(new Color(0.0f, 0.0f, 1.0f, 1.0f));
        colors.Add(new Color(0.5f, 0.0f, 1.0f, 1.0f));
        colors.Add(new Color(1.0f, 0.0f, 1.0f, 1.0f));
        colors.Add(new Color(1.0f, 0.0f, 0.5f, 1.0f));





        //{ 2,63,165},{ 125,135,185},{ 190,193,212},{ 214,188,192},{ 187,119,132},{ 142,6,59},{ 74,111,227},{ 133,149,225},{ 181,187,227},{ 230,175,185},{ 224,123,145},{ 211,63,106},{ 17,198,56},{ 141,213,147},{ 198,222,199},{ 234,211,198},{ 240,185,141},{ 239,151,8},{ 15,207,192},{ 156,222,214},{ 213,234,231},{ 243,225,235},{ 246,196,225},{ 247,156,212}





    }
}
