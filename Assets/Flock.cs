using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
    public FlockManager myManager;
    float speed;

    // Start is called before the first frame update
    void Start()
    {
        speed = Random.Range(myManager.minSpeed, myManager.maxSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        ApplyFlockingRules();
        transform.Translate(0, 0, Time.deltaTime * speed); 
    }

    void ApplyFlockingRules()
    {
        // check our neighbors within neighborDistance
        GameObject[] all = myManager.allFish;

        Vector3 nbCenter = Vector3.zero;     // Rule #1
        float nbSpeed = 0.0f;                // Rule #2
        Vector3 nbAvoid = Vector3.zero;      // Rule #3
        int nbSize = 0;

        foreach (GameObject fish in all)
        {
            if (fish == this.gameObject)
                continue;

            float nDistance = Vector3.Distance(fish.transform.position, this.transform.position);
            if (nDistance > myManager.neighborDistance)
                continue;

            nbCenter += fish.transform.position;
            nbSize++;

            // Rule#2 : moving along the flock
            nbSpeed += fish.GetComponent<Flock>().speed;

            // Rule#3 : moving away when too close
            if (nDistance < 1.0f)
            {
                nbAvoid += this.transform.position - fish.transform.position;
            }


        }

        if (Physics.Raycast(transform.position, transform.forward,out RaycastHit hit, 1, LayerMask.GetMask("Obstacle")))
        {
            print("EVADE");
            
            Vector3 evadeDir = Vector3.Reflect(transform.forward, hit.normal + hit.point);
            transform.rotation = Quaternion.Slerp(this.transform.rotation,
                                       Quaternion.LookRotation(evadeDir),
                                       (1.0f - hit.distance) * 1.0f );

        }
        
        else if (nbSize > 0)
        {
            nbCenter = nbCenter / nbSize;
            nbSpeed = nbSpeed / nbSize;

            // computer target direction
            Vector3 targetDir = (nbCenter + nbAvoid ) - this.transform.position;

            // turning toward target direction
            transform.rotation = Quaternion.Slerp( this.transform.rotation,
                                                   Quaternion.LookRotation(targetDir),
                                                   myManager.rotationSpeed * Time.deltaTime);
        }
    }
}
