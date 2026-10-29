
using UnityEngine;
using System.Collections;

[RequireComponent (typeof(MeshFilter))]
[RequireComponent (typeof(MeshRenderer))]
[RequireComponent (typeof(SphereCollider))]
class Resource : MonoBehaviour
{
    [SerializeField]
    ResourceType m_resource;

    [SerializeField] AnimationCurve m_animCurve;

    string m_name;

    public void Initialise(ResourceType ingredient)
    {
        (GetComponent<MeshFilter>()).sharedMesh = m_resource.mesh;
        m_name = m_resource.Name;
    }


    void OnTriggerEnter(Collider col)
    {
        // Could test for layer etc, but currently no other rigids but player so no point. 
        GetComponent<SphereCollider>().enabled = false;

        PlayerController player = col.gameObject.GetComponent<PlayerController>();
        
        // TODO
        StartCoroutine(SeekToCauldron(player.GetCauldronTransform()));
        //Inventory.AddResource(m_resource);
    }

    IEnumerator SeekToCauldron( Transform cauldronLocation )
    {
        // For dropping into cauldron from above
        const float yDropDistance = 0.6f;
        const float dropTime = 0.3f;
        const float travelTime = 0.8f;
        float timer = 0;

        Vector3 initialDifference = cauldronLocation.position - transform.position;
        float yOffset = 0;

        // Fly to position just above cauldron
        while ( timer < travelTime )
        {
            float s = timer/travelTime;
            Vector3 position = Vector3.Lerp(transform.position, cauldronLocation.position, s) - (Vector3.up * yOffset);
            Vector3 difference = cauldronLocation.position - transform.position;

            yOffset = m_animCurve.Evaluate(s);
            transform.position = position + (Vector3.up * yOffset);
            
            timer = Mathf.Min(timer + Time.deltaTime, travelTime);
            yield return new WaitForFixedUpdate();
        }
        timer = 0;
        Vector3 dropVector = Vector3.up * yDropDistance;

        // Drop into cauldron
        while ( timer < dropTime )
        {
            float s = timer/dropTime;
            Vector3 position = Vector3.Lerp(cauldronLocation.position, cauldronLocation.position - dropVector, s);

            timer = Mathf.Min(timer + Time.deltaTime, dropTime);
            yield return new WaitForFixedUpdate();
        }

        // Remove
        Destroy(gameObject, 0.01f);
    }
    
}
