using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WandController : MonoBehaviour
{

    private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObj.index); } } //get the newest object index // index of device doesn't change

    [Tooltip("Don't put anything here in-editor.")]
    public SteamVR_TrackedObject trackedObj;

    [Tooltip("Don't put anything here in-editor.")]
    public bool triggerActive = false;

    void Start()
    {
    }

    void Update()
    {
        if (controller == null)
        {
            return;
        }

        //TODO: this should be instantaneous, not a frame later
        //triggerActive = controller.GetPress(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger);
    }
}