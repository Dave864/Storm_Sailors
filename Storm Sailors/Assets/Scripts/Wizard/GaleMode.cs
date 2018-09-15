using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GaleMode : MonoBehaviour
{
    // State variables to manage input events
    private bool grabAction = false;
    private bool spawnDispelAction = false;
    private bool positionAction = false;
    public bool Positioning
    {
        get { return positionAction; }
    }

    // Wizard timer variables
    [SerializeField] private float cloudGrabTime = 0.1f;    // The time to confirm a grab action
    private float curGrabTime = 0;                          // The timer for confirming a grab action
    [SerializeField] private float cloudSpawnTime = 0.5f;   // The time in seconds it takes to spawn a cloud
    [SerializeField] private float cloudDispelTime = 1f;    // The time in seconds it takes to dispel a cloud
    [SerializeField] private Slider cloudTimerSlider;       // UI Slider object to serve as timer
    [SerializeField] private AnimationCurve dispelAllMult;  // Curve for time for the dispel all action

    // Reference to held cloud
    private GameObject heldCloud;

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

        // Initialize the wizard to not be holding any clouds
        heldCloud = null;

        // Initialize the cloud timer to not be seen
        if (cloudTimerSlider != null)
        {
            cloudTimerSlider.GetComponent<CanvasGroup>().alpha = 0;
        }
        else
        {
            Debug.LogError("No slider UI assigned for cloud timer slider", cloudTimerSlider);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Execute Gale mode actions
        if (GetComponent<Wizard>().CurMode == Wizard.Mode.GALE)
        {
            // Dispel all clouds
            if (Input.GetButton("Dispel All") && !positionAction && !spawnDispelAction && !grabAction)
            {
                //cloudManager.GetComponent<CloudManager>().DispelAll();
                StartCoroutine(DispelAllAction());
            }
            else
            {
                // Execute various Gale mode actions
                PositionAction();
                CloudAction();
            }
        }
    }

    // Dispel all clouds after a time
    IEnumerator DispelAllAction()
    {
        float curTime = 0;
        float dispelAllTime = cloudDispelTime;
        bool actionActivated = false;

        // Check if dispel all multiplier curve accounts for possibility of eight spawned clouds
        if (dispelAllMult == null)
        {
            Debug.LogError("No curve created for the Dispel All multiplier");
        }
        else if (dispelAllMult[dispelAllMult.length - 1].time >= 8f)
        {
            dispelAllTime *= dispelAllMult.Evaluate(cloudManager.GetComponent<CloudManager>().CurCloudCnt);
        }

        // See if player holds dispel button long enough to execute action
        cloudTimerSlider.GetComponent<CanvasGroup>().alpha = 1;
        while (Input.GetButton("Dispel All") && !actionActivated)
        {
            cloudTimerSlider.value = curTime / dispelAllTime;
            curTime += Time.deltaTime;
            if (curTime >= dispelAllTime)
            {
                actionActivated = true;
            }
            yield return null;
        }
        cloudTimerSlider.GetComponent<CanvasGroup>().alpha = 0;

        // Execute the dispel all action
        if (actionActivated)
        {
            cloudManager.GetComponent<CloudManager>().DispelAll();
        }
        yield return null;
    }

    // Move wizard to new compass position
    private void PositionAction()
    {
        // Construct position vector from input
        float hInput = Input.GetAxisRaw("Horizontal");
        float vInput = Input.GetAxisRaw("Vertical");
        Vector2 posVect = new Vector2(hInput, vInput);

        // Set up the start and end positions if not already repositioning and not conducting a cloud action
        if (!spawnDispelAction && !positionAction && posVect != new Vector2(0, 0))
        {
            destinationCompassPos = new Vector2(posVect.x, posVect.y);
            positionAction = true;
        }

        // Reposition wizard
        if (positionAction)
        {
            StartCoroutine(Position(destinationCompassPos));
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
        positionAction = false;
        yield return null;
    }

    // Conduct various cloud actions
    private void CloudAction()
    {
        // Actions when the action button is pressed or held
        // Holding spawns or dispels a cloud
        // Pressing grabs and places a cloud
        if (Input.GetButton("Cloud Action") && !spawnDispelAction && !positionAction)
        {
            // Detect button press
            if (Input.GetButtonDown("Cloud Action"))
            {
                grabAction = true;
                curGrabTime = 0;
            }
            // Timer to confirm grab action
            else if (grabAction)
            {
                curGrabTime += Time.deltaTime;
                grabAction = (curGrabTime <= cloudGrabTime);
            }
            // Execute spawn or dispel action
            else if (heldCloud == null)
            {
                grabAction = false;
                spawnDispelAction = true;
                // Start dispelling cloud at current position
                if (cloudManager.GetComponent<CloudManager>().ThunderheadAtPos(curCompassPos))
                {
                    StartCoroutine(SpawnDispelCloudTimer(false));
                }
                // Start spawning cloud at current position
                else
                {
                    StartCoroutine(SpawnDispelCloudTimer(true));
                }
            }
        }

        // Actions when the button is released
        else if(Input.GetButtonUp("Cloud Action"))
        {
            // Execute the grab action
            if (grabAction)
            {
                // Holding cloud
                if (heldCloud != null)
                {
                    // Merge held cloud into cloud at position
                    if (cloudManager.GetComponent<CloudManager>().ThunderheadAtPos(curCompassPos))
                    {
                        cloudManager.GetComponent<CloudManager>().MoveThunderHead(curCompassPos, ref heldCloud);
                    }
                    // Place cloud into empty position
                    else
                    {
                        cloudManager.GetComponent<CloudManager>().MoveThunderHead(curCompassPos, ref heldCloud);
                    }
                    grabAction = false;
                }
                // Pick up cloud if not holding a cloud
                else if (cloudManager.GetComponent<CloudManager>().ThunderheadAtPos(curCompassPos))
                {
                    heldCloud = cloudManager.GetComponent<CloudManager>().MoveThunderHead(curCompassPos, ref heldCloud);
                }
            }

            // Enables spawn and dispel actions to be executed again
            // This accounts for when the action button is not released after the action has finished
            spawnDispelAction = false;
        }
    }

    // Spawn or dispel cloud after a time
    IEnumerator SpawnDispelCloudTimer(bool spawn)
    {
        cloudTimerSlider.GetComponent<CanvasGroup>().alpha = 1;
        float curTime = 0;
        bool actionActivated = false;

        // See if the player holds the action button long enough to execute a cloud action
        while(Input.GetButton("Cloud Action") && !actionActivated)
        {
            // Check for the spawn action
            if (spawn)
            {
                cloudTimerSlider.value = curTime / cloudSpawnTime;
                curTime += Time.deltaTime;
                if (curTime >= cloudSpawnTime)
                {
                    actionActivated = true;
                }
                yield return null;
            }

            // Check for the dispel action
            else
            {
                cloudTimerSlider.value = curTime / cloudDispelTime;
                curTime += Time.deltaTime;
                if (curTime >= cloudDispelTime)
                {
                    actionActivated = true;
                }
                yield return null;
            }
        }

        // Execute the activated action
        if (actionActivated)
        {
            if (spawn)
            {
                cloudManager.GetComponent<CloudManager>().SpawnThunderhead(curCompassPos);
            }
            else
            {
                cloudManager.GetComponent<CloudManager>().DispelThunderhead(curCompassPos);
            }
        }

        // Turn off the timer when either the action fails to activate or succeeds
        bool releasedActionButton = Input.GetButtonUp("Cloud Action");
        if (releasedActionButton || actionActivated)
        {
            spawnDispelAction = !releasedActionButton;
            cloudTimerSlider.GetComponent<CanvasGroup>().alpha = 0;
            cloudTimerSlider.value = 0;
        }
        yield return null;
    }
}
