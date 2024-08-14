using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pegScript : MonoBehaviour
{
    public CircleCollider2D myCircleCollider;
    public SpriteRenderer mySpriteRenderer;

    public bool crit;
    public bool reset;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setCrit(){
        crit = true;
        mySpriteRenderer.color = new Color(1f, 0.5f, 0f, 1f);
    }

    public void setReset(){
        reset = true;
        mySpriteRenderer.color = new Color(0f, 1f, 0f, 1f);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        myCircleCollider.isTrigger = true;
        mySpriteRenderer.color = new Color(1f, 1f, 1f, 0.5f);
        
        if(crit && !gameManagerScript.instance.isCrit){
            gameManagerScript.instance.setCrit();
        }

        if(reset){
            gameManagerScript.instance.resetBoard();
        }
        
        gameManagerScript.instance.addDamage();

    }
   

}
