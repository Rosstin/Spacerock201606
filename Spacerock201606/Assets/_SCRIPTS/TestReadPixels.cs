using UnityEngine;
using System.Collections;

public class TestReadPixels : MonoBehaviour {

    public bool grab;
    public Renderer display;

    public Camera inputCamera;
    public Camera outputCamera;

    public RenderTexture myTexture;

    void OnPostRender()
    {
        myTexture = inputCamera.targetTexture;


        //Texture2D tex = new Texture2D(128, 128);
        //tex.ReadPixels(new Rect(0, 0, 128, 128), 0, 0);
        //tex.Apply();


        

        //outputCamera.targetTexture = tex;
        
            //display.material.mainTexture = tex;
            //grab = false;
        //}
    }




    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
