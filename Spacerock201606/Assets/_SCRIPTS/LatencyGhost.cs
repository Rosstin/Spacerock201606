using UnityEngine;
using System.Collections;

public class LatencyGhost : MonoBehaviour {

    public GameObject ghost;

    Vector3[] positions = new Vector3[ConstantsSpacerock.FIXED_UPDATE_FRAMES_DELAY];
	int currentIndex = 0;
    int latentIndex = 1;

    int frameskipIndex = 0;


	// Use this for initialization
	void Start () {
	
	}
	
	void FixedUpdate () {
        frameskipIndex += 1;
        if(frameskipIndex == ConstantsSpacerock.FIXED_UPDATE_PERCENT_FRAMES_SEEN) {
            frameskipIndex = 0;
		    positions[currentIndex] = new Vector3( this.gameObject.transform.position.x, this.gameObject.transform.position.y, this.gameObject.transform.position.z);

            //Debug.Log("positions[currentIndex]: " + positions[currentIndex]);

            if (positions[latentIndex] != null)
            {
                //Debug.Log("!= null... latentIndex: " + latentIndex + "... positions[latentIndex]: " + positions[latentIndex]);
                ghost.transform.position = positions[latentIndex];
            }
            else
            {
                //Debug.Log("null... " + "this.gameObject.transform.position: " + this.gameObject.transform.position);
            }

            currentIndex = incrementIndex(currentIndex);
            latentIndex = incrementIndex(latentIndex);
        }
    }

    int incrementIndex(int myIndex)
    {
        myIndex += 1;
        if(myIndex >= positions.Length)
        {
            return 0;
        }
        else
        {
            return myIndex;
        }
    }


}
