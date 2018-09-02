using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudManager : MonoBehaviour
{
    // Variables for managing spawned clouds
    public int maxCloudCnt = 3;
    private int curCloudCnt = 0;
    public float dipVal = 0.5f;
    public GameObject thunderheadPrefab;

    // Vector resulting from combined gale vectors of thunderheads
    [HideInInspector] public Vector3 CombinedGaleVector { get; set; }

    // Containers for keeping track of spawned thunderheads
    private Dictionary<Vector2, GameObject> thunderheadPos = new Dictionary<Vector2, GameObject>();
    private GameObject stormCloudRef;

    // List of keys for thunderheadPos container
    private List<Vector2> cardinalPos;

    // Reference to other game objects
    private GameObject compassCenter;
    private GameObject wizard;

    // Use this for initialization
    void Start()
    {
        // Initialize combined gale vector
        CombinedGaleVector = new Vector3(0, 0, 0);

        // Establish reference to compass center
        compassCenter = GameObject.Find("Compass Center");
        if (compassCenter == null)
        {
            Debug.LogError("Compass Center object not found", compassCenter);
        }

        // Establish reference to wizard
        wizard = GameObject.Find("Wizard Object");
        if (wizard == null)
        {
            Debug.LogError("Wizard Object object not found", wizard);
        }

        // Initialize thunderheadPos
        thunderheadPos.Add(new Vector2(0, 1), null);    // position N
        thunderheadPos.Add(new Vector2(1, 1), null);    // position NE
        thunderheadPos.Add(new Vector2(1, 0), null);    // position E
        thunderheadPos.Add(new Vector2(1, -1), null);   // position SE
        thunderheadPos.Add(new Vector2(0, -1), null);   // position S
        thunderheadPos.Add(new Vector2(-1, -1), null);  // position SW
        thunderheadPos.Add(new Vector2(-1, 0), null);   // position W
        thunderheadPos.Add(new Vector2(-1, 1), null);   // position NW

        // Initialize list of keys
        cardinalPos = new List<Vector2>(thunderheadPos.Keys);

        // Initialize the storm cloud reference
        stormCloudRef = null;
    }

    // Return whether a thunderhead is at pos
    public bool ThunderheadAtPos(Vector2 pos)
    {
        if (thunderheadPos.ContainsKey(pos))
        {
            return thunderheadPos[pos] != null;
        }
        return false;
    }

    // Summon a new thunderhead
    public void SpawnThunderhead(Vector2 cardinalPos = default(Vector2))
    {
        Vector3 wizardPos = wizard.transform.position;
        GameObject newThunderhead;
        switch (wizard.GetComponent<Wizard>().CurMode)
        {
            // Gale mode
            case 0:
                if (curCloudCnt < maxCloudCnt && thunderheadPos.ContainsKey(cardinalPos) && thunderheadPos[cardinalPos] == null)
                {
                    // Instantiate gale thunderhead at position of wizard
                    newThunderhead = Instantiate(thunderheadPrefab, wizardPos, Quaternion.identity, transform);

                    // Rotate thunderhead so it looks at the center of the cloud rail
                    newThunderhead.transform.LookAt(compassCenter.transform, Vector3.up);

                    // Lower the thunderhead by the dip value
                    newThunderhead.transform.position = new Vector3(wizardPos.x, wizardPos.y - dipVal, wizardPos.z);

                    // Add thunderhead to container
                    thunderheadPos[cardinalPos] = newThunderhead;
                    curCloudCnt++;

                    // Update combined gale vector
                    CombinedGaleVector += newThunderhead.GetComponent<Thunderhead>().GaleVector;
                }
                break;

            // Storm mode
            case 1:
                // Instantiate storm thunderhead at position of wizard
                stormCloudRef = Instantiate(thunderheadPrefab, wizardPos, Quaternion.identity, transform);
                break;

            default:
                break;
        }
    }

    // Dispel a thunderhead
    public void DispelThunderhead(Vector2 cardinalPos = default(Vector2))
    {
        switch (wizard.GetComponent<Wizard>().CurMode)
        {
            // Gale mode
            case 0:
                if (curCloudCnt > 0 && thunderheadPos.ContainsKey(cardinalPos))
                {
                    // Update combined gale vector
                    CombinedGaleVector -= thunderheadPos[cardinalPos].GetComponent<Thunderhead>().GaleVector;
                    // Dispel thunderhead
                    Destroy(thunderheadPos[cardinalPos]);
                    thunderheadPos[cardinalPos] = null;
                    curCloudCnt--;
                }
                break;

            // Storm mode
            case 1:
                // Dispel storm thunderhead
                Destroy(stormCloudRef);
                break;

            default:
                break;
        }
    }

    // Dispel all spawned thunderheads
    public void DispelAll()
    {
        CombinedGaleVector = Vector3.zero;
        curCloudCnt = 0;
        // Dispel all thunderclouds
        for (int i = 0; i < cardinalPos.Count; i++)
        {
            Destroy(thunderheadPos[cardinalPos[i]]);
            thunderheadPos[cardinalPos[i]] = null;
        }
    }
}
