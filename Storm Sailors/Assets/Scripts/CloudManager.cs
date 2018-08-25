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

    // Container for keeping track of spawned thunderheads
    private Dictionary<Vector2, GameObject> thunderheadPos = new Dictionary<Vector2, GameObject>();

    // List of keys for thunderhead container
    private List<Vector2> cardinalPos;

    // Reference to other game objects
    private GameObject compassCenter;
    private GameObject wizard;

    // Use this for initialization
    void Start()
    {
        // Keep the cloud max count from exceeding the number of positions
        // TODO: placeholder until editor scripts are made (after jam)
        if (maxCloudCnt > 8)
        {
            maxCloudCnt = 8;
        }

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

    // Summon a new thunderhead at pos
    public void SpawnThunderhead(Vector2 cardinalPos)
    {
        if (curCloudCnt < maxCloudCnt)
        {
            if (thunderheadPos.ContainsKey(cardinalPos))
            {
                if (thunderheadPos[cardinalPos] == null)
                {
                    Vector3 wizardPos = wizard.transform.position;

                    // Instantiate thunderhead at position of wizard
                    GameObject newThunderhead = Instantiate(thunderheadPrefab, wizard.transform.position, Quaternion.identity, transform);

                    // Rotate thunderhead so it looks at the center of the cloud rail
                    newThunderhead.transform.LookAt(compassCenter.transform, Vector3.up);

                    // Lower the thunderhead by the dip value
                    newThunderhead.transform.position = new Vector3(wizard.transform.position.x, wizard.transform.position.y - dipVal, wizard.transform.position.z);

                    // Add thunderhead to container
                    thunderheadPos[cardinalPos] = newThunderhead;
                    curCloudCnt++;

                    // Update combined gale vector
                    CombinedGaleVector += newThunderhead.GetComponent<Thunderhead>().GaleVector;
                }
            }
        }
    }

    // Dispel a thunderhead at pos
    public void DispelThunderhead(Vector2 cardinalPos)
    {
        if (curCloudCnt > 0)
        {
            if (thunderheadPos.ContainsKey(cardinalPos))
            {
                // Update combined gale vector
                CombinedGaleVector -= thunderheadPos[cardinalPos].GetComponent<Thunderhead>().GaleVector;
                // Dispel thunderhead
                Destroy(thunderheadPos[cardinalPos]);
                thunderheadPos[cardinalPos] = null;
                curCloudCnt--;
            }
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
