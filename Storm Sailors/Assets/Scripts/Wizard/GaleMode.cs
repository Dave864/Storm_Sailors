using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GaleMode : MonoBehaviour
{
    // State variables to manage input events
    private bool spawnDispelAction = false;
    private bool positioning = false;
    public bool Positioning
    {
        get { return positioning; }
    }

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

    // Wizard timer variables
    [SerializeField] private float cloudSpawnTime = 0.5f;   // The time in seconds it takes to spawn a cloud
    [SerializeField] private float cloudDispelTime = 1f;    // The time in seconds it takes to dispel a cloud
    [SerializeField] private Slider cloudTimerSlider;       // UI Slider object to serve as timer

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
            cloudTimerSlider.gameObject.SetActive(false);
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
            // Execute various Gale mode actions
            PositionAction();
            CloudAction();
            // TODO: charge cloud

            // Position Cloud Timer UI at wizard transform
            if (cloudTimerSlider != null)
            {
                Vector3 cloudTimerWorldPosition = transform.position;
                cloudTimerWorldPosition.y -= cloudManager.GetComponent<CloudManager>().dipVal;
                cloudTimerSlider.transform.position = Camera.main.WorldToScreenPoint(cloudTimerWorldPosition);
            }

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

        // Set up the start and end positions if not already repositioning and not conducting a cloud action
        if (!spawnDispelAction && !positioning && posVect != new Vector2(0, 0))
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

    // Conduct various cloud actions
    private void CloudAction()
    {
        // Actions when the action button is tapped
        // Tapping picks up or places a held cloud
        /*if (Input.GetButtonDown("Cloud Action") && !spawnDispelAction && !positioning)
        {
            if (cloudManager.GetComponent<CloudManager>().ThunderheadAtPos(curCompassPos))
            {
                // Dispel cloud at position (TEMPORARY)
                cloudManager.GetComponent<CloudManager>().DispelThunderhead(curCompassPos);
                // Pick up cloud
                if (heldCloud != null)
                {

                }
                else
                {

                }
            }
            else
            {
                cloudManager.GetComponent<CloudManager>().SpawnThunderhead(curCompassPos);
                // Place cloud down
                if (heldCloud != null)
                {

                }
                else
                {

                }
            }
        }*/
        // Actions when the action button is held
        // Holding spawns or dispels a cloud
        if (Input.GetButton("Cloud Action") && !spawnDispelAction && !positioning)
        {
            if(heldCloud == null)
            {
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
        else if(Input.GetButtonUp("Cloud Action"))
        {
            spawnDispelAction = false;
        }
    }

    // Spawn or dispel cloud after a time
    IEnumerator SpawnDispelCloudTimer(bool spawn)
    {
        cloudTimerSlider.gameObject.SetActive(true);
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
            cloudTimerSlider.gameObject.SetActive(false);
            cloudTimerSlider.value = 0;
        }
        yield return null;
    }
}
