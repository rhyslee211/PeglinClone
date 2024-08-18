using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombScript : MonoBehaviour
{

    int numHits = 0;

    public float explosionRadius = 0.5f;
    public float explosionForce = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        numHits++;

        if(numHits == 2){
            gameManagerScript.instance.addBomb();
            
            explode();

            Destroy(gameObject);
        }

        
    }

    void OnDrawGizmosSelected()
    {
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }

    private void explode(){

        Debug.Log("Exploding");

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        Debug.Log("Colliders found: " + colliders.Length);

        List<OrbScript> orbs = new List<OrbScript>();

        foreach(Collider2D collider in colliders){
            if(collider.GetComponent<OrbScript>() != null){
                orbs.Add(collider.GetComponent<OrbScript>());
            }
        }

        foreach(OrbScript orb in orbs){
            
            Rigidbody2D orbRigidbody = orb.GetComponent<Rigidbody2D>();

            if (orbRigidbody != null)
            {
                Debug.Log("Adding force to orb");

                Vector2 direction = orbRigidbody.position - (Vector2)transform.position;

                orbRigidbody.AddForce(direction.normalized * explosionForce, ForceMode2D.Impulse);
            }

        }
    }

}
