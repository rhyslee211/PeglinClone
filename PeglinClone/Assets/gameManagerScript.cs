using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class gameManagerScript : MonoBehaviour
{

    private int reloadTime = 1;
    public bool isCrit = false;
    public int damageNum = 0;
    public int regScore = 2;
    public int critScore = 7;

    public int numResets = 2;

    public int numCrits = 3;

    public GameObject orb;

    public Text damageText;
    public Text critText;
    public Text regText;

    public static gameManagerScript instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        damageText.text = "Damage: " + damageNum;

        resetBoard();
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

    public void addDamage() {

        if(isCrit){
            damageNum = damageNum + critScore;
        }
        else{
            damageNum = damageNum + regScore;
        }

        damageText.text = "Damage: " + damageNum;

        Debug.Log("Damage: " + damageNum);
    }

    public void setCrit() {
        if(!isCrit){
            
            isCrit = true;
            damageNum = (critScore * damageNum)/regScore;
        }
    }

    public void resetBoard() {
        pegScript[] allPegs = FindObjectsOfType<pegScript>();

        foreach (pegScript peg in allPegs)
        {
            peg.myCircleCollider.isTrigger = false;
            peg.crit = false;
            peg.reset = false;

            peg.mySpriteRenderer.color = new Color(1f, 1f, 1f, 1f);
        }

        // Create a list to hold the indices of chosen pegs
        List<int> chosenCritIndices = new List<int>();

        // Assign crit to 3 random pegs
        while (chosenCritIndices.Count < numCrits)
        {
            int randomIndex = Random.Range(0, allPegs.Length);

            // Ensure no duplicate indices
            if (!chosenCritIndices.Contains(randomIndex))
            {
                chosenCritIndices.Add(randomIndex);
                allPegs[randomIndex].setCrit();
            }
        }

        // Assign reset to n random pegs

        List<int> chosenResetIndices = new List<int>();

        while (chosenResetIndices.Count < numResets)
        {
            int randomIndex = Random.Range(0, allPegs.Length);

            // Ensure no duplicate indices
            if (!chosenCritIndices.Contains(randomIndex) && !chosenResetIndices.Contains(randomIndex))
            {
                chosenResetIndices.Add(randomIndex);
                allPegs[randomIndex].setReset();
            }
        }

    }

    public void handleOrbDeath() {
        applyDamage();

        spawnOrb();
    }

    void spawnOrb()
    {
        Vector2 spawnPos = new Vector2(-1.5f, 0.5f);
        Instantiate(orb, spawnPos, Quaternion.identity);
    }

    void applyDamage()
    {
        damageNum = 0;
    }

}
