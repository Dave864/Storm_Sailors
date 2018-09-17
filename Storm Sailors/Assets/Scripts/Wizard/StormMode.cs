using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StormMode : MonoBehaviour
{
    // Reference to game objects
    private GameObject compassCenter;
    private GameObject cloudManager;
    private GameObject shipObject;

    // Storm level thresholds
    [SerializeField] private int stormLevelSustainable = 3;
    [SerializeField] private int stormLevelOverload = 5;
    public int StormLevelSustainable { get { return stormLevelSustainable; } }
    public int StormLevelOverload { get { return stormLevelOverload; } }

    // Use this for initialization
    private void Awake()
    {
        // Get reference to cloud manager
        cloudManager = GameObject.Find("Cloud Manager");
        if (!cloudManager)
        {
            Debug.LogError("Cloud Manager object not found", cloudManager);
        }

        // Get reference to compass center
        compassCenter = GameObject.Find("Compass Center");
        if (!compassCenter)
        {
            Debug.LogError("Compass Center object not found", compassCenter);
        }

        // Get reference to ship object
        shipObject = GameObject.Find("Ship Object");
        if (!shipObject)
        {
            Debug.LogError("Ship Object object not found", shipObject);
        }
    }

    // Update is called once per frame
    void Update ()
    {
        if (GetComponent<Wizard>().CurMode == Wizard.Mode.STORM)
        {
            // Execute various Storm mode actions
            WizardFaceMouse();
            // TODO: Charge thunderhead
            // TODO: Gather thunderheads
            // TODO: Launch thunderhead
        }
    }

    // Rotate the wizard to face the position of the mouse
    void WizardFaceMouse ()
    {
        if (Input.mousePresent)
        {
            // Translate the mouse position to in game world position
            Vector2 mousePos = Input.mousePosition;
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, Camera.main.nearClipPlane));

            // Raycast the mouse position find the apparent mouse position on the sea
            Vector3 mouseSeaPosition = new Vector3();
            RaycastHit mouseRayHit;
            if (Physics.Raycast(mouseWorldPosition, Camera.main.transform.forward, out mouseRayHit))
            {
                mouseSeaPosition = new Vector3(mouseRayHit.point.x, transform.position.y, mouseRayHit.point.z);
            }
            else
            {
                mouseSeaPosition = mouseWorldPosition;
            }

            // Rotate the wizard in the direction of the mouse point on the sea
            Vector3 direction = mouseSeaPosition - transform.position;
            transform.forward = direction;
        }
    }
}
