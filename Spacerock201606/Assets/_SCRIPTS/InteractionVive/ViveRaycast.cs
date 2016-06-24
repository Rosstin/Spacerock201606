using UnityEngine;
using UnityEngine.UI;
using System.Collections;
//using Leap;
//using Leap.Unity.PinchUtility;
//using Leap.Unity;

public class ViveRaycast : MonoBehaviour {

    public GameObject controllerGameObjectLeft;
    private SteamVR_TestThrow wandLeft;

    public GameObject controllerGameObjectRight;
    private SteamVR_TestThrow wandRight;

    public GameObject toggle3DGameObject;
    Collider toggle3DCollider;
    DiageticToggle toggle3Dscript;

    public GameObject slider1;
    Collider slider1Collider;
    DiageticSlider slider1script;

    public GameObject sliderFollowers;
    Collider sliderFollowersCollider;
    DiageticSlider sliderFollowersScript;

    public GameObject PanelContainer;
    //todo: an object with an array of all buttons should be included 

    public GameObject infravisionQuad;

    public GameObject rightCapsuleHandObject;
    //CapsuleHand rightCapsuleHandScript;
    public GameObject leftCapsuleHandObject;
    //CapsuleHand leftCapsuleHandScript;

    float SLIDER_MOVE_SPEED = 0.004f;

    // VIEWPANEL
    float VIEWPANEL_EULER_X_LOWER_THRESHHOLD = 14.0f;
    float VIEWPANEL_EULER_X_UPPER_THRESHHOLD = 100.0f;

    int panelState = 1;
    static public int PANEL_ON = 0;
    float turnPanelOffTimer = 0.0f;
    static public int PANEL_OFF = 1;
    float turnPanelOnTimer = 0.0f;

    float PANEL_ON_TIMER_CONSTANT = 0.5f;
    float PANEL_OFF_TIMER_CONSTANT = 1.5f;

    public Camera playerCamera; // aka CenterEyeAnchor

    LineRenderer myLineRenderer;

    GenerateGraph graphGenerator;

    // handles two-handed action of palm proximity
    int palmState = 0;
    static public int PALM_STATE_NORMAL = 0;
    static public int PALM_STATE_GROUP_SELECTED = 501;
    int selectedKey;

    float palmSelectionTime = 0.0f;
    float palmDeselectionTime = 0.0f;
    public static float PALM_SELECTION_TIME_THRESHHOLD = 1.0f;
    public static float PALM_DESELECTION_TIME_THRESHHOLD = 2.0f;

    public static float TWO_HAND_PROXIMITY_CONSTANT = 0.10f;

    //Node[] nodes;

    ControllerState controllerStateR = new ControllerState();
    ControllerState controllerStateL = new ControllerState();

    Vector3 nodeContainerStartPosition;
    Vector3 zoomPinchStartPositionL;
    Vector3 zoomPinchStartPositionR;
    float zoomPinchStartDistance;
    float lastZoomPinchDistance;

    void Start () {
        GameObject prefabLineToRender = Resources.Load("Line") as GameObject;
        GameObject lineToRender = Instantiate (prefabLineToRender, new Vector3 (0, 0, 0), Quaternion.identity) as GameObject;
        myLineRenderer = lineToRender.GetComponent<LineRenderer> ();
        myLineRenderer.enabled = false;

        graphGenerator = this.gameObject.GetComponent<GenerateGraph> ();

        wandLeft = controllerGameObjectLeft.GetComponent<SteamVR_TestThrow>();
        wandRight = controllerGameObjectRight.GetComponent<SteamVR_TestThrow>();

        toggle3DCollider = toggle3DGameObject.GetComponent<Collider> ();
        toggle3Dscript = toggle3DGameObject.GetComponent<DiageticToggle>();

        slider1Collider = slider1.GetComponent<Collider> ();
        slider1script = slider1.GetComponent<DiageticSlider> ();

        sliderFollowersCollider = sliderFollowers.GetComponent<Collider>();
        sliderFollowersScript = sliderFollowers.GetComponent<DiageticSlider>();

    }

    void Update()
    {
        ControllerUpdate(wandLeft, ConstantsSpacerock.LEFT);
        ControllerUpdate(wandRight, ConstantsSpacerock.RIGHT);

        if (graphGenerator.interactionReady)
        {

            CheckDebugKeyboardActions();

            UpdateControlPanel();

            if (controllerStateL.state == ControllerState.STATE_DRAGGING)
            { // maybe do this if the user stops moving the node around, don't do it if the node is moving a lot
                graphGenerator.explodeSelectedNode(controllerStateL.highlightedObject);
            }

            if (controllerStateR.state == ControllerState.STATE_DRAGGING)
            {
                graphGenerator.explodeSelectedNode(controllerStateR.highlightedObject);
            }
        }

    }

    void ControllerUpdate(SteamVR_TestThrow wand, string handedness)
    {
        HandleInteraction(wand.triggerPress, wand.gameObject.transform.position, handedness);
    }

    void HandleInteraction(bool isActive, Vector3 positionOfInteractable, string handedness)
    {

        Vector3 p = positionOfInteractable;

        // camera to pinch vector
        Vector3 heading = Vector3.Normalize(p - playerCamera.transform.position);

        // camera to object vector
        Vector3 objectVector;
        float biggestDotProduct= 0.0f;
        int selectedNodeIndex = 0;
        int hoveredNodeIndex = 0;
        float dotProduct;

        ControllerState controllerState = null;

        int state = -1;
        if( handedness == ConstantsSpacerock.RIGHT){
            controllerState = controllerStateR;
        }
        else{
            controllerState = controllerStateL;
        }

        if (panelState == PANEL_ON) {

            // do panel actions
            graphGenerator.NodesAreDraggable (false);

            RaycastHit hit = new RaycastHit ();
            Vector3 endRayPosition = playerCamera.transform.position + (heading.normalized * 100.0f);

            Ray myRay = new Ray (playerCamera.transform.position, heading);

            UpdateToggleState(toggle3DCollider, toggle3Dscript, myRay, heading, p, isActive, handedness);

            UpdateSliderState (slider1Collider, slider1script, myRay, heading, p, isActive, handedness);
            UpdateSliderState(sliderFollowersCollider, sliderFollowersScript, myRay, heading, p, isActive, handedness);

        } else { // not looking at panel

            graphGenerator.NodesAreDraggable (true); // TODO only necessary if we have a LEAPRTS object for Leap Motion

            if (controllerState.state != ControllerState.STATE_DRAGGING && isActive) { // can start a drag
                controllerState.state = ControllerState.STATE_DRAGGING;

                for (int i = 0; i < graphGenerator.masterNodeList.Length; i++) {
                    if (graphGenerator.isLegalNode(graphGenerator.masterNodeList[i])) {
                        objectVector = Vector3.Normalize (graphGenerator.masterNodeList[i].gameObject.transform.position - playerCamera.transform.position);
                        dotProduct = Vector3.Dot (heading, objectVector);

                        if (dotProduct > biggestDotProduct) { // dont select nodes that are not visible
                            biggestDotProduct = dotProduct;
                            selectedNodeIndex = i;
                        }
                    }
                }

                // consider not interacting if the dotproduct is not large enough... below 0.9 or something

                //distanceOfGraph = (graphGenerator.nodeContainer.transform.position - p).magnitude; //TODO get the distance of graph and use that to move the container
                //Vector3 graphHeading = Vector3.Normalize(p - playerCamera.transform.position);

                // you could move it based on the selected node's delta position

                controllerState.originalNodeDistance = (graphGenerator.masterNodeList[selectedNodeIndex].gameObject.transform.position - p).magnitude;
                controllerState.originalNodeHeading = Vector3.Normalize(p - playerCamera.transform.position);

                controllerState.highlightedObject = graphGenerator.masterNodeList[selectedNodeIndex];
                controllerState.highlightedObject.nodeForce.Selected();

                controllerState.originalControllerPosition = positionOfInteractable;

                controllerState.originalGraphGeneratorPosition = graphGenerator.nodeContainer.transform.position;


            }
            else if (controllerState.state == ControllerState.STATE_DRAGGING) { // already dragging

                // get distance between us and the nodecontainer
                // map the delta angle from the node to the delta angle of the container

                if (controllerState.highlightedObject != null) {
                    controllerState.highlightedObject.nodeForce.timeSelected += Time.deltaTime;
                }

                Vector3 originalNodePosition = controllerState.originalControllerPosition + (controllerState.originalNodeHeading.normalized * controllerState.originalNodeDistance);
                Vector3 currentNodePosition = positionOfInteractable + (controllerState.originalNodeHeading.normalized * controllerState.originalNodeDistance); //then change it so that heading is not replicated

                Vector3 deltaNodePosition = originalNodePosition - currentNodePosition;

                graphGenerator.nodeContainer.transform.position = controllerState.originalGraphGeneratorPosition - deltaNodePosition*ConstantsSpacerock.MOVE_MULTIPLIER*Mathf.Log(controllerState.originalNodeDistance, ConstantsSpacerock.MOVE_DISTANCE_LOG_BASE); // you could make it so that this multiplier gets greater when its farther away

                //graphGenerator.nodeContainer.transform.position = controllerGameObjectLeft.transform.position + (graphHeadingL.normalized * distanceOfGraphL);

            }

            if (!isActive) { // if you let go you're not dragging
                controllerState.state = ControllerState.STATE_NORMAL;

                for (int i = 0; i < graphGenerator.masterNodeList.Length; i++)
                {
                    if (graphGenerator.isLegalNode(graphGenerator.masterNodeList[i]))
                    {
                        objectVector = Vector3.Normalize(graphGenerator.masterNodeList[i].gameObject.transform.position - playerCamera.transform.position);
                        dotProduct = Vector3.Dot(heading, objectVector);

                        if (dotProduct > biggestDotProduct) // should be "if visible" instead
                        { // dont select nodes that are not visible
                            biggestDotProduct = dotProduct;
                            hoveredNodeIndex = i;
                        }
                    }

                }

                //graphGenerator.masterNodeList[hoveredNodeIndex].nodeForce.Hovered();

                if (controllerState.highlightedObject != null) {
                    controllerState.highlightedObject.nodeForce.Unselected();
                    graphGenerator.unselectNode();
                    controllerState.highlightedObject.nodeForce.timeSelected = 0.0f;
                    controllerState.highlightedObject = null;
                }
            }

        }
    }

    void NeutralizeButtonState() // neutralize state of all buttons in when you leave
    {
        NeutralizeSliderState(slider1script);
        NeutralizeSliderState(sliderFollowersScript);
        //todo neutralize the toggles
    }

    void UpdateControlPanel () {
        // looking at panel
        // not looking at panel

        //print("playerCamera.transform.eulerAngles.x: " + playerCamera.transform.eulerAngles.x);

        if (panelState == PANEL_ON) {
            PanelContainer.SetActive (true);

            if (!(playerCamera.transform.eulerAngles.x >= VIEWPANEL_EULER_X_LOWER_THRESHHOLD && playerCamera.transform.eulerAngles.x <= VIEWPANEL_EULER_X_UPPER_THRESHHOLD)) {
                turnPanelOnTimer = 0.0f;
                turnPanelOffTimer += Time.deltaTime;

                // if you're pinching, don't turn the panel off quite yet
                //if (!(rightPinchDetectorScript.IsPinching || leftPinchDetectorScript.IsPinching) && turnPanelOffTimer >= PANEL_OFF_TIMER_CONSTANT) {
                //    panelState = PANEL_OFF;
                //    NeutralizeButtonState();
                //}
            }

        } else if (panelState == PANEL_OFF) {

            PanelContainer.SetActive (false);

            if (playerCamera.transform.eulerAngles.x >= VIEWPANEL_EULER_X_LOWER_THRESHHOLD && playerCamera.transform.eulerAngles.x <= VIEWPANEL_EULER_X_UPPER_THRESHHOLD) {
                turnPanelOffTimer = 0.0f;
                turnPanelOnTimer += Time.deltaTime;

                if (turnPanelOnTimer >= PANEL_ON_TIMER_CONSTANT) {
                    PanelContainer.transform.eulerAngles = new Vector3( PanelContainer.transform.eulerAngles.x, playerCamera.transform.eulerAngles.y, PanelContainer.transform.eulerAngles.z ); 
                    panelState = PANEL_ON;
                }

            } 


        }



    }

    void NeutralizeSliderState(DiageticSlider slider)
    {
        slider.state = slider.NORMAL;
        performSliderAction(slider.sliderType, slider.currentValue);
        slider.UnGrab();
    }

    void performSliderAction(string sliderType, int value)
    {
        if(sliderType == "degree") // todo should be a constant string stored elsewhere
        {
            graphGenerator.NodeDegree = value;
            graphGenerator.showLegalNodesBasedOnFilterSettings();
        }
        else if(sliderType == "followers")
        {
            graphGenerator.FollowerCount = value;
            graphGenerator.showLegalNodesBasedOnFilterSettings();
        }

    }

    void performToggleAction(DiageticToggle toggle)
    {
        if(toggle.toggleType == "dimensionality")
        {
            bool newDimensionality = toggle.switchState();
            if (newDimensionality)
            {
                graphGenerator.changeNodeDimensionality(GenerateGraph.GRAPH_3D);
            }
            else
            {
                graphGenerator.changeNodeDimensionality(GenerateGraph.GRAPH_2D);
            }
        }

    }

    void UpdateToggleState(Collider collider, DiageticToggle toggle, Ray ray, Vector3 heading, Vector3 p, bool isActive, string handedness)
    {
        RaycastHit hit = new RaycastHit();

        if (toggle.state != toggle.DRAGGING && isActive && collider.Raycast(ray, out hit, 200.0f))
        { // start a drag
            toggle.state = toggle.DRAGGING;
            toggle.OnGrab();
            toggle.handUsed = handedness;

            // do things related to starting a drag
        }

        if (toggle.state == toggle.DRAGGING && toggle.handUsed == handedness)
        {

            if (!isActive)
            { // no longer dragging
                toggle.state = toggle.NORMAL;
                performToggleAction(toggle);
            }

        }
    }

    void UpdateSliderState(Collider collider, DiageticSlider slider, Ray ray, Vector3 heading, Vector3 p, bool isActive, string handedness){ // updating for both hands is screwing it up

        RaycastHit hit = new RaycastHit ();

        if (slider.state != slider.DRAGGING && isActive && collider.Raycast (ray, out hit, 200.0f)) { // start a drag
            slider.state = slider.DRAGGING;
            slider.OnGrab ();
            slider.handUsed = handedness;

            // do things related to starting a drag
        }

        if (slider.state == slider.DRAGGING && slider.handUsed == handedness) {

            Vector3 perp = Vector3.Cross (heading, (playerCamera.transform.position - slider.transform.position));
            float dir = Vector3.Dot (perp, playerCamera.transform.up);

            if (dir > 0f) {
                slider.transform.localPosition = new Vector3( slider.transform.localPosition.x + SLIDER_MOVE_SPEED, slider.transform.localPosition.y, slider.transform.localPosition.z);
            } else if (dir < 0f) {
                slider.transform.localPosition = new Vector3( slider.transform.localPosition.x - SLIDER_MOVE_SPEED, slider.transform.localPosition.y, slider.transform.localPosition.z);
            } else {
                //Debug.Log("ontarget");
            }
            if (slider.UpdateBarValue ()) { // value changed
                performSliderAction(slider.sliderType, slider.currentValue);
            }
            if (!isActive) { // no longer dragging
                NeutralizeSliderState(slider);
            }

        }
    }

    /*
    void HandleTwoHandedActions(CapsuleHand lHand, LeapPinchDetector lDetector, CapsuleHand rHand, LeapPinchDetector rDetector)
    {
        float palmDistance = lHand.hand_.PalmPosition.DistanceTo(rHand.hand_.PalmPosition);
        if (palmState == PALM_STATE_GROUP_SELECTED)
        {

            if (palmDistance > TWO_HAND_PROXIMITY_CONSTANT * 2.0f)
            {
                palmDeselectionTime += Time.deltaTime;
                if(palmDeselectionTime > PALM_DESELECTION_TIME_THRESHHOLD)
                {
                    print("palmDeselectionTime > PALM_DESELECTION_TIME_THRESHHOLD");

                    //graphGenerator.hideCentroids();

                    print("infravision toggled off");
                    infravisionQuad.SetActive(false);

                    palmState = PALM_STATE_NORMAL;
                    palmSelectionTime = 0.0f;
                }
            }
        }

        else if(palmState == PALM_STATE_NORMAL)
        {
            if (palmDistance < TWO_HAND_PROXIMITY_CONSTANT)
            {
                palmSelectionTime += Time.deltaTime;
                if(palmSelectionTime > PALM_SELECTION_TIME_THRESHHOLD)
                {
                    print("palmSelectionTime > PALM_SELECTION_TIME_THRESHHOLD");

                    //graphGenerator.showCentroids();

                    // do a raycast against the centroids
                    // first, find the point between the two palms
                    Vector3 p = Vector3.Lerp(lHand.hand_.PalmPosition.ToVector3(), rHand.hand_.PalmPosition.ToVector3(), 0.5f);

                    Vector3 heading = Vector3.Normalize(p - playerCamera.transform.position);
                    Vector3 objectVector;
                    float dotProduct;
                    float biggestDotProduct = 0.0f;

                    // now raycast versus the face

                    foreach (int key in graphGenerator.nodeGroups.Keys) // todo should check legality once that is implemented
                    {
                        objectVector = Vector3.Normalize(graphGenerator.nodeGroups[key].nodeGroupContainerScript.centroid.gameObject.transform.position - playerCamera.transform.position);

                        dotProduct = Vector3.Dot(heading, objectVector);

                        //print("dotProduct: " + dotProduct);

                        if (dotProduct > biggestDotProduct) // should be "if visible" instead
                        { // dont select nodes that are not visible
                            biggestDotProduct = dotProduct;
                            selectedKey = key;
                        }
                    }

                    //graphGenerator.nodeGroups[selectedKey].nodeGroupContainerScript.centroid.groupCentroidScript.Selected();

                    print("infravision toggled on");
                    infravisionQuad.SetActive(true);

                    palmState = PALM_STATE_GROUP_SELECTED;
                    palmDeselectionTime = 0.0f;
                }
            }
        }
    }
    */


    void CheckDebugKeyboardActions()
    {

        if (Input.GetKeyDown("m"))
        {
            graphGenerator.loadMNistGraph();
        }

        if (Input.GetKeyDown("t"))
        {
            graphGenerator.loadTwitterGraph();
        }

        if (Input.GetKeyDown("space"))
        {
            print("infravision toggled on");
            infravisionQuad.SetActive(true);
        }

        if (Input.GetKeyDown("1"))
        {
            print("infravision toggled off");
            infravisionQuad.SetActive(false);
        }


        if (Input.GetKeyDown("c"))
        {
            print("show centroids");
            graphGenerator.showCentroids();
        }

        if (Input.GetKeyDown("x"))
        {
            print("hide centroids");
            graphGenerator.hideCentroids();
        }

        if (Input.GetKeyDown("d"))
        {
            print("graphGenerator.detailingMode = true");
            graphGenerator.detailingMode = true;
        }

        if (Input.GetKeyDown("f"))
        {
            print("graphGenerator.detailingMode = false");
            graphGenerator.detailingMode = false;
        }


        if (Input.GetKeyDown("2"))
        {
            print("graphGenerator.changeNodeDimensionality(GenerateGraph.GRAPH_2D)");
            graphGenerator.changeNodeDimensionality(GenerateGraph.GRAPH_2D);
        }

        if (Input.GetKeyDown("3"))
        {
            print("graphGenerator.changeNodeDimensionality(GenerateGraph.GRAPH_3D)");
            graphGenerator.changeNodeDimensionality(GenerateGraph.GRAPH_3D);
        }

    }



}
