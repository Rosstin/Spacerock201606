using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SteamVR_TrackedObject))]
public class SteamVR_TestThrow : MonoBehaviour
{

    public GameObject menuBox;

    public GameObject item1;
    public GameObject item2;
    public GameObject item3;
    public GameObject sceneGod;
    private GenerateGraph graphGenerator;

    RMMenu menu;
    bool menuUp = true;
    double lastY;
    double stepSize = 0.5;

    public GameObject prefab;
    public Rigidbody attachPoint;

    SteamVR_TrackedObject trackedObj;
    FixedJoint joint;

    public bool triggerPress;

    int onClick(RMMenuItem item)
    {
        print("Clicked: " + item.index);
        item.SetTitle("Hi");
        return 1;
    }

    int clicked2D3D(RMMenuItem item)
    {
        if (item.GetTitle().Equals(" ->2D  3D", System.StringComparison.Ordinal))
        {
            // Change to 3D
            item.SetTitle("  2D ->3D");
            graphGenerator.changeNodeDimensionality(GenerateGraph.GRAPH_3D);
        } else
        {
            // To 2D
            item.SetTitle(" ->2D  3D");
            graphGenerator.changeNodeDimensionality(GenerateGraph.GRAPH_2D);
        }
        return 1;
    }

    void Start()
    {
        graphGenerator = sceneGod.GetComponent<GenerateGraph>();


        RMMenuItem[] items = new RMMenuItem[3];
        items[0] = item1.GetComponent<RMMenuItem>();
        items[0].onClick = clicked2D3D;
        items[1] = item2.GetComponent<RMMenuItem>();
        items[1].onClick = onClick;
        items[2] = item3.GetComponent<RMMenuItem>();
        items[2].onClick = onClick;

        menu = menuBox.GetComponent<RMMenu>();
        menu.SetMenuItems(items);


        /*
                items[0] = item1;
                items[1] = item2;
                items[2] = item3;

                /*
                foreach (string item in items)
                {
                    GameObject menuItem = Instantiate(Resources.Load("MenuItem") as GameObject, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity) as GameObject;
                    menuBox.gameObject.transform.parent = menuItem.transform;
                }
                */
    }

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    void FixedUpdate()
    {
        /*
        RMMenuItem mi = item1.GetComponent<RMMenuItem>();
        print("index: " + mi.index);

        mi.SetSelected();
        */
        
        var device = SteamVR_Controller.Input((int)trackedObj.index);

        triggerPress = device.GetPress(SteamVR_Controller.ButtonMask.Trigger);

        var menuButtonPressed = device.GetPressDown(SteamVR_Controller.ButtonMask.ApplicationMenu);

        // print("Menu Pressed: " + menuButtonPressed);

        if (menuButtonPressed)
        {
            menuUp = !menuUp;
            menuBox.SetActive(menuUp);
        }

        var directionSwiped = device.GetAxis();
        //print("Direction: " + directionSwiped);

        //print("y: " + directionSwiped.y);

        double newY = directionSwiped.y;
        if (newY == 0.0)
        {
            lastY = newY;
        }
        else if (lastY - newY > stepSize ||
            newY - lastY > stepSize)
        {
            bool moveDown = lastY > newY;
            menu.HighlightNextItem(moveDown);
            lastY = newY;
        }


        var directionPressed = device.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad);
        //print("Pressed: " + directionPressed);

        if (directionPressed)
        {
            menu.SelectHighlightedItem();
        }
        




        /*
        // make an object to throw
        if (joint == null && device.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger))
        {
            var go = GameObject.Instantiate(prefab);
            go.transform.position = attachPoint.transform.position;

            joint = go.AddComponent<FixedJoint>();
            joint.connectedBody = attachPoint;
        }
        else if (joint != null && device.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger))
        {
            var go = joint.gameObject;
            var rigidbody = go.GetComponent<Rigidbody>();
            Object.DestroyImmediate(joint);
            joint = null;
            Object.Destroy(go, 15.0f);

            // We should probably apply the offset between trackedObj.transform.position
            // and device.transform.pos to insert into the physics sim at the correct
            // location, however, we would then want to predict ahead the visual representation
            // by the same amount we are predicting our render poses.

            var origin = trackedObj.origin ? trackedObj.origin : trackedObj.transform.parent;
            if (origin != null)
            {
                rigidbody.velocity = origin.TransformVector(device.velocity);
                rigidbody.angularVelocity = origin.TransformVector(device.angularVelocity);
            }
            else
            {
                rigidbody.velocity = device.velocity;
                rigidbody.angularVelocity = device.angularVelocity;
            }

            rigidbody.maxAngularVelocity = rigidbody.angularVelocity.magnitude;
        }
        */
    }
}
