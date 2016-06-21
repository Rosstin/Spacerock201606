using UnityEngine;
using System.Collections;

public class LayerSelfBasedOnRelativeLocations : MonoBehaviour {

    public Camera eagleEyeCamera;
    public Camera headsetLocation;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
        // if the distance between you and the camera is less than the distance between the player and the camera, be foreground
        if(Vector3.Distance(this.transform.position, eagleEyeCamera.transform.position) < Vector3.Distance(headsetLocation.transform.position, eagleEyeCamera.transform.position))
        {
            this.gameObject.layer = ConstantsSpacerock.FOREGROUND_LAYER;
        }
        // else be background
        else
        {
            this.gameObject.layer = ConstantsSpacerock.DEFAULT_LAYER;
        }
    }
}
