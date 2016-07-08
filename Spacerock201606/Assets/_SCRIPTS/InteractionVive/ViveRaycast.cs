using UnityEngine;
using UnityEngine.UI;
using System.Collections;
//using Leap;
//using Leap.Unity.PinchUtility;
//using Leap.Unity;

public class ViveRaycast : MonoBehaviour
{

    [Tooltip("The sceneGod should be placed here so ViveRaycast can find the reference to InteractionHandler.cs")]
    public GameObject sceneGod;
    private InteractionHandler myInteractionHandler;

    public GameObject controllerGameObjectLeft;
    private SteamVR_TestThrow wandLeft;
    private WandRay wandRayL;
    LineRenderer lineL;

    public GameObject controllerGameObjectRight;
    private SteamVR_TestThrow wandRight;
    private WandRay wandRayR;
    LineRenderer lineR;

    void Start()
    {
        wandLeft = controllerGameObjectLeft.GetComponent<SteamVR_TestThrow>();
        wandRayL = controllerGameObjectLeft.GetComponent<WandRay>();
        wandRight = controllerGameObjectRight.GetComponent<SteamVR_TestThrow>();
        wandRayR = controllerGameObjectLeft.GetComponent<WandRay>();

        myInteractionHandler = sceneGod.GetComponent<InteractionHandler>();
    }

    void Update()
    {
        ControllerUpdate(wandLeft, ConstantsSpacerock.LEFT);
        ControllerUpdate(wandRight, ConstantsSpacerock.RIGHT);

    }

    void ControllerUpdate(SteamVR_TestThrow wand, string handedness)
    {
        myInteractionHandler.HandleInteraction(wand.triggerPress, wand.gameObject.transform.position, handedness);
    }

}
