using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GaleMode : MonoBehaviour
{
    // State variables to manage input events
    private readonly int GaleModeVal = 0;
    private bool positioning = false;
    public bool Positioning
    {
        get { return positioning; }
    }

    // Reference to game objects
    private GameObject compassCenter;
    private GameObject cloudManager;
    private GameObject shipObject;

    // Wizard movement variables
    private Vector2 curCompassPos = new Vector2(0, 0);
    private Vector2 destinationCompassPos = new Vector2(0, 0);
    private float rotRate;

    // Use this for initialization
    private void Awake()
    {
        // Get reference to cloud manager
        cloudManager = GameObject.Find("Cloud Manager");
        if (cloudManager == null)
        {
            Debug.LogError("Cloud Manager object not found", cloudManager);
        }

        // Get reference to compass center
        compassCenter = GameObject.Find("Compass Center");
        if (compassCenter == null)
        {
            Debug.LogError("Compass Center object not found", compassCenter);
        }

        // Get reference to ship object
        shipObject = GameObject.Find("Ship Object");
        if (shipObject == null)
        {
            Debug.LogError("Ship Object object not found", shipObject);
        }
    }

    // Use this for initialization
    private void Start()
    {
        // Intialize current compass position and compass rotation rate of wizard
        curCompassPos = compassCenter.GetComponent<CompassCenter>().strtPos;
        rotRate = compassCenter.GetComponent<CompassCenter>().rotRate;
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<Wizard>().CurMode == GaleModeVal)
        {
            // Execute Gale mode actions
            PositionAction();
            SummonAction();

            // Dispel all clouds
            if (Input.GetButton("Dispel All"))
            {
                cloudManager.GetComponent<CloudManager>().DispelAll();
            }
        }
    }

    // Move wizard to new compass position
    private void PositionAction()
    {
        // Construct position vector from input
        float hInput = Input.GetAxisRaw("Horizontal");
        float vInput = Input.GetAxisRaw("Vertical");
        Vector2 posVect = new Vector2(hInput, vInput);

        // Set up the start and end positions if not already repositioning
        if (!positioning && posVect != new Vector2(0, 0))
        {
            destinationCompassPos = new Vector2(posVect.x, posVect.y);
            positioning = true;
        }

        // Reposition wizard
        if (positioning)
        {
            StartCoroutine(Position(destinationCompassPos));
        }
    }

    // Summon or dispel cloud at wizard position
    private void SummonAction()
    {
        if (Input.GetButtonDown("Summon") && !positioning)
        {
            // Dispel cloud at position
            if (cloudManager.GetComponent<CloudManager>().ThunderheadAtPos(curCompassPos))
            {
                cloudManager.GetComponent<CloudManager>().DispelThunderhead(curCompassPos);
            }

            // Summon a cloud at position
            else
            {
                cloudManager.GetComponent<CloudManager>().SpawnThunderhead(curCompassPos);
            }
        }
    }

    // Reposition wizard
    IEnumerator Position(Vector2 endPos)
    {
        // Set up the start and end rotations
        Quaternion strtRot = compassCenter.GetComponent<CompassCenter>().PositionRot(curCompassPos);
        Quaternion endRot = compassCenter.GetComponent<CompassCenter>().PositionRot(endPos);

        for (float posTime = 0; posTime < rotRate; posTime += Time.deltaTime)
        {
            // Rotate the center of the rail to position the wizard
            if (compassCenter != null)
            {
                compassCenter.transform.rotation = Quaternion.Slerp(strtRot, endRot, posTime / rotRate);
            }
            yield return null;
        }

        // Finalize rotation to account for for loop not quite rotating all the way
        compassCenter.transform.rotation = endRot;

        // update the current wizard position
        curCompassPos = endPos;

        // Reset timer and state flags
        positioning = false;
        yield return null;
    }
}
