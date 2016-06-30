using UnityEngine;
using System.Collections;

public class LayerSelfBasedOnRelativeLocations : MonoBehaviour {

    public Camera eagleEyeCamera;
    public Camera headsetLocation;

    int state = 0;
    int STATE_IS_DEFAULT = 101;
    int STATE_IS_FOREGROUND = 102;

    int interstitial = 0;

    // Use this for initialization
    void Start () {
    
    }
    
    // Update is called once per frame
    void Update () {
    
        // if the distance between you and the camera is less than the distance between the player and the camera, be foreground
        if(Vector3.Distance(this.transform.position, eagleEyeCamera.transform.position) < Vector3.Distance(headsetLocation.transform.position, eagleEyeCamera.transform.position))
        {
            if(state != STATE_IS_FOREGROUND) { 

                if(interstitial == 0)
                {
                    SetLayerRecursively(this.gameObject, ConstantsSpacerock.INTERSTITIAL_LAYER);
                    interstitial++;
                }
                else if(interstitial < ConstantsSpacerock.INTERSTITIAL_FRAMES)
                {
                    interstitial++;
                }
                else
                {
                    interstitial = 0;
                    SetLayerRecursively(this.gameObject, ConstantsSpacerock.FOREGROUND_LAYER);
                    state = STATE_IS_FOREGROUND;
                }
            }

        }
        // else be background
        else
        {
            if (state != STATE_IS_DEFAULT)
            {

                if (interstitial == 0)
                {
                    SetLayerRecursively(this.gameObject, ConstantsSpacerock.INTERSTITIAL_LAYER);
                    interstitial++;
                }
                else if (interstitial < ConstantsSpacerock.INTERSTITIAL_FRAMES)
                {
                    interstitial++;
                }
                else
                {
                    interstitial = 0;
                    SetLayerRecursively(this.gameObject, ConstantsSpacerock.DEFAULT_LAYER);
                    state = STATE_IS_DEFAULT;
                }
            }
        }
    }

    void SetLayerRecursively(GameObject myGameObject, int newLayer)
    {
        myGameObject.layer = newLayer;

        foreach( Transform child in myGameObject.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }

    }

}
