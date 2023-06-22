using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pegScript : MonoBehaviour
{
    public CircleCollider2D myCircleCollider;
    public SpriteRenderer mySpriteRenderer;
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
        myCircleCollider.isTrigger = true;
        mySpriteRenderer.color = new Color(1f, 1f, 1f, 0.5f);
    }
}
