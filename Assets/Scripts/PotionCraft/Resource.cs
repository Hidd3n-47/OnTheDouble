
using UnityEngine;
using System.Collections;
using UnityEditor.ShaderGraph.Internal;

[RequireComponent (typeof(MeshFilter))]
[RequireComponent (typeof(MeshRenderer))]
[RequireComponent (typeof(SphereCollider))]
public class Resource : MonoBehaviour
{
    ResourceType m_resourceType;

    [SerializeField] AnimationCurve m_animCurve;
    [SerializeField] AnimationCurve m_scaleCurve;

    [SerializeField] float m_travelTime = 0.8f;

    string m_name;

    // Responsibility of Spawner
    public void Initialise(ResourceType ingredient)
    {
        transform.localPosition = Vector3.zero;

        m_resourceType = ingredient;
        (GetComponent<MeshFilter>()).sharedMesh = m_resourceType.mesh;
        m_name = m_resourceType.Name;

        m_resourceType++;
    }


    void OnTriggerEnter(Collider col)
    {
        // Could test for layer etc, but currently no other rigids but player so no point. 
        GetComponent<SphereCollider>().enabled = false;

        PlayerController player = col.gameObject.GetComponentInParent<PlayerController>();
        player.AddResource(m_resourceType);

        transform.parent = player.transform;

        m_resourceType--;

        // TODO
        StartCoroutine(SeekToCauldron(player.GetCauldronTransform()));
        //Inventory.AddResource(m_resource);
    }

    IEnumerator SeekToCauldron( Transform cauldronLocation )
    {
        float timer = 0.0f;
        Vector3 initialPosition = transform.position;

        // Fly to position just above cauldron
        while (timer < m_travelTime)
        {
            float s = timer/ m_travelTime;
            Vector3 position = Vector3.Lerp(initialPosition, cauldronLocation.position, s);

            float scale   = m_scaleCurve.Evaluate(s);
            float yOffset = m_animCurve.Evaluate(s);

            transform.position   = position + (Vector3.up * yOffset);
            transform.localScale = new Vector3(scale, scale, scale);
            
            timer = Mathf.Min(timer + Time.fixedDeltaTime, m_travelTime);
            yield return new WaitForFixedUpdate();
        }

        // Remove
        Destroy(gameObject, 0.01f);
    }

    public ResourceType Type => m_resourceType;
    
}
