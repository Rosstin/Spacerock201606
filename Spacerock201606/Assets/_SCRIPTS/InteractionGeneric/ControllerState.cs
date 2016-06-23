using UnityEngine;
using System.Collections;

public class ControllerState {


    public int state;
    public static int STATE_NORMAL = 0;
    public static int STATE_DRAGGING = 1;

    public Node highlightedObject = null;
    public Vector3 originalControllerPosition;
    public Vector3 originalNodeHeading;
    public float originalNodeDistance;

    public Vector3 originalGraphGeneratorPosition;
    
    //public Vector3 originalGraphHeading;
    //public float originalGraphDistance;


}
