using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapTrigger : MonoBehaviour
{
    public GameObject ps;
    public float force = 200f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            //Lanceer
            Rigidbody rb = other.GetComponent<Rigidbody>();
            Transform t = other.transform;
            rb.constraints = RigidbodyConstraints.None;
           
            
            GameObject p = Instantiate(ps, transform);
            p.transform.position = t.position;
            
            rb.AddExplosionForce(force, new Vector3(t.position.x, t.position.y, t.position.z), 0f);
            rb.AddExplosionForce(force, new Vector3(t.position.x, t.position.y, t.position.z + 1f), 0f);
            Destroy(p, 5f);
            //Controls uit
            
            
            Movement mbScript = other.GetComponentInChildren<Movement>();
            mbScript.enabled = false;
            
        }
    }
}
