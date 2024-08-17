using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;


public class gameManagerScript : MonoBehaviour
{

    private int reloadTime = 1;
    public bool isCrit = false;
    public int damageNum = 0;
    public int regScore;
    public int critScore;
    public int playerHealth = 80;

    public int numResets = 2;

    public int numCrits = 3;

    public GameObject orbPrefab;
    public GameObject orbInstance;

    public Text damageText;
    public Text critText;
    public Text regText;
    public Text orbNameText;

    public Transform orbListContent; 
    public Text orbTextPrefab;


    public static gameManagerScript instance;

    public class Orb {

        public string name { get; set; }
        public int regDamage { get; set; }
        public int critDamage { get; set; }

        public Orb(string inputName, int inputRegDamage, int inputCritDamage){
            name = inputName;
            regDamage = inputRegDamage;
            critDamage = inputCritDamage;
        }

    }

    public class Monster {
        
        public string name { get; set; }
        public int health { get; set; }
        public int attackDamge { get; set; }

        public Monster(string inputName, int inputHealth, int inputAttackDamage){
            name = inputName;
            health = inputHealth;
            attackDamge = inputAttackDamage;
        }

    }

    public List<Orb> liveOrbMag = new List<Orb>();
    public List<Orb> orbMag = new List<Orb>();

    public List<Monster> meleeMonsterList = new List<Monster>();
    public List<Monster> rangedMonsterList = new List<Monster>();

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

        orbMag.Add(new Orb("Stone",3,4));
        orbMag.Add(new Orb("Dagger",2,7));
        orbMag.Add(new Orb("Stone",3,4));
        orbMag.Add(new Orb("Stone",3,4));
        orbMag.Add(new Orb("Dagger",2,7));

        rangedMonsterList.add(new Monster("Bramball Plant", 50, 4));

        meleeMonsterList.Add(null);
        meleeMonsterList.Add(null);
        meleeMonsterList.Add(new Monster("Green Slime",60, 5));
        meleeMonsterList.Add(new Monster("Blue Slime", 80, 7));

        damageText.text = "Total Damage: " + damageNum;


        reload();

        resetBoard();
        getNextOrb();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void reload(){
        liveOrbMag = orbMag.ToList();
    }

    void getNextOrb()
    {

        Orb cOrb = liveOrbMag[0];
        liveOrbMag.RemoveAt(0);

        spawnOrb(cOrb.regDamage,cOrb.critDamage);

        critText.text = "Crit Damage: " + critScore;
        regText.text = "Normal Damage: " + regScore;
        orbNameText.text = cOrb.name;

        if(liveOrbMag.Count == 0){
            reload();
        }

        populateOrbMagList();
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

        void spawnOrb(int regDamage, int critDamage)
    {
        Vector2 spawnPos = new Vector2(-1.5f, 0.5f);
        orbInstance = Instantiate(orbPrefab, spawnPos, Quaternion.identity);

        OrbScript orbScript = orbInstance.GetComponent<OrbScript>();

        orbScript.setOrbDamage(regDamage,critDamage);

        regScore = orbScript.regDamage;
        critScore = orbScript.critDamage;


    }

    void populateOrbMagList(){

        foreach (Transform child in orbListContent){
            Destroy(child.gameObject);
        }

        foreach (var item in liveOrbMag){

            Text itemText = Instantiate(orbTextPrefab, orbListContent);

            itemText.text = item.name;

        }

    }

    public void handleOrbDeath() {
        playerTurn();

        enemyTurn();

        getNextOrb();
    }

    void playerTurn() {

        attackClosestEnemy(damageNum);

        damageNum = 0;
    }

    void attackClosestEnemy(int damage){
        for(int i = 0; i < meleeMonsterList.Count; i++){
            if(meleeMonsterList[i] != null){
                meleeMonsterList[i].health = meleeMonsterList[i].health - damage;

                if(meleeMonsterList[i].health <= 0){
                    meleeMonsterList[i]= null;
                }

                return;
            }
        }
        for(int i = 0; i < rangedMonsterList.Count; i++){
            if(rangedMonsterList[i] != null){
                rangedMonsterList[i].health = rangedMonsterList[i].health - damage;

                if(rangedMonsterList[i].health <= 0){
                    rangedMonsterList[i]= null;
                }

                return;
            }
        }
    }

    void enemyTurn(){

        meleeEnemyTurn();

        rangedEnemyTurn();

    }

    void meleeEnemyTurn(){
        
        if(meleeMonsterList[0] == null){
            meleeMonsterList.RemoveAt(0);
        }
        else{
            applyDamge(meleeMonsterList[0].attackDamge);
        }

    }

    void rangedEnemyTurn(){
        foreach(Monster monster in rangedMonsterList){
            applyDamage(monster.attackDamge);
        }
    }

    void applyDamage(int damage)
    {
        playerHealth = playerHealth - damage;
    }

}
