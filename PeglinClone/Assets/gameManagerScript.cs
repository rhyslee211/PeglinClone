using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class gameManagerScript : MonoBehaviour
{

    private int reloadTime = 1;
    public int regScore = 0;
    public int critScore = 0;
    public bool isCrit = false;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            reload();
        }
    }

    void reload()
    {

    }

    void spawnOrb()
    {
        //Instantiate
    }

}
