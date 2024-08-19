using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;


public class gameManagerScript : MonoBehaviour
{
    public bool isCrit = false;
    public int damageNum = 0;
    public int bombNum = 0;

    
    public int bombScore = 50;
    public int regScore;
    public int critScore;
    public int playerHealth = 80;

    public Button restartButton;

    public int numResets = 2;

    public int numCrits = 3;

    public GameObject orbPrefab;
    public GameObject orbInstance;

    public Text damageText;
    public Text critText;
    public Text regText;
    public Text orbNameText;
    public Text playerHealthText;

    public Transform orbListContent; 
    public Text orbTextPrefab;

    public Image[] meleeSpots;
    public Text[] meleeHealthbars;

    public Image[] rangedSpots;
    public Text[] rangedHealthbars;

    public Sprite blueSlimeSprite;
    public Sprite greenSlimeSprite;
    public Sprite shooterPlantSprite;

    private Dictionary<string,Sprite> monsterSpriteDict = new Dictionary<string,Sprite>();

    public Sprite emptySprite;

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

        public string ToString(){
            return("Name: " + name + "; Health: " + health + "; Damage: " + attackDamge);
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

        rangedMonsterList.Add(new Monster("Bramball Plant", 50, 4));

        meleeMonsterList.Add(null);
        meleeMonsterList.Add(null);
        meleeMonsterList.Add(new Monster("Green Slime",60, 5));
        meleeMonsterList.Add(new Monster("Blue Slime", 80, 7));

        Debug.Log(listMonsters());

        monsterSpriteDict.Add("Blue Slime", blueSlimeSprite);
        monsterSpriteDict.Add("Green Slime", greenSlimeSprite);
        monsterSpriteDict.Add("Bramball Plant", shooterPlantSprite);

        damageText.text = "Total Damage: " + damageNum;
        playerHealthText.text = "Player Health: " + playerHealth;

        Button btn = restartButton.GetComponent<Button>();
        btn.onClick.AddListener(restartLevel);

        updateMonsterSprites();

        reload();

        resetBoard();
        getNextOrb();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void restartLevel(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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

        //Debug.Log("Damage: " + damageNum);
    }

    public void addBomb() {
        bombNum++;
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
        Vector2 spawnPos = new Vector2(0f, 1.0f);
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

        updateMonsterSprites();

        if(allMonstersDead()){
            gameWin();
            return;
        }
 
        enemyTurn();

        playerHealthText.text = "Player Health: " + playerHealth;

        if(isPlayerDead()){
            gameLose();
            return;
        }

        Debug.Log(listMonsters());
        Debug.Log("Player Health: " + playerHealth);

        getNextOrb();
    }

    void playerTurn() {

        attackClosestEnemy(damageNum);

        damageNum = 0;

        bombAttack();

        bombNum = 0;

    }

    void bombAttack() {
        for(int i = 0; i < bombNum; i++){
            throwBomb();
        }
    }

    void throwBomb(){
        for(int i = 0; i < meleeMonsterList.Count; i++){
            if(meleeMonsterList[i] != null){
                meleeMonsterList[i].health = meleeMonsterList[i].health - bombScore;

                if(meleeMonsterList[i].health <= 0){
                    meleeMonsterList[i]= null;
                }
            }
        }
        for(int i = 0; i < rangedMonsterList.Count; i++){
            if(rangedMonsterList[i] != null){
                rangedMonsterList[i].health = rangedMonsterList[i].health - bombScore;

                if(rangedMonsterList[i].health <= 0){
                    rangedMonsterList.RemoveAt(i);
                }
            }
        }
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
                    rangedMonsterList.RemoveAt(i);
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

        if(meleeMonsterList.Count == 0){
            return;
        }
        
        if(meleeMonsterList[0] == null){
            meleeMonsterList.RemoveAt(0);
        }
        else{
            applyDamage(meleeMonsterList[0].attackDamge);
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

    bool isPlayerDead(){
        if(playerHealth <= 0){
            return true;
        }
        else{
            return false;
        }
    }

    bool allMonstersDead() {
        foreach(Monster monster in meleeMonsterList){
            if(monster != null){
                return false;
            }
        }
        foreach(Monster monster in rangedMonsterList){
            if(monster != null){
                return false;
            }
        }

        return true;
    }

    void gameLose() {
        Debug.Log("Game Lost :(");
    }

    void gameWin() {
        Debug.Log("Game Won :)");
    }

    void updateMonsterSprites(){

        //Debug.Log(meleeMonsterList.Count);

        for(int i = 0; i < meleeSpots.Length; i++){
            if(i < meleeMonsterList.Count){
                if(meleeMonsterList[i] != null){
                    //Debug.Log(monsterSpriteDict[meleeMonsterList[i].name].sprite.name);
                    Sprite spriteToAssign = monsterSpriteDict[meleeMonsterList[i].name];
                    meleeSpots[i].sprite = spriteToAssign;
                    meleeSpots[i].enabled = true;

                    meleeHealthbars[i].text = "Health: " + meleeMonsterList[i].health;
                    meleeHealthbars[i].enabled = true;

                    Debug.Log("Assigned Sprite: " + spriteToAssign.name);
                }
                else{
                    meleeSpots[i].sprite = emptySprite;
                    meleeSpots[i].enabled = false;

                    meleeHealthbars[i].text = "";
                    meleeHealthbars[i].enabled = false;
                }
            }
            else{
                meleeSpots[i].sprite = emptySprite;
                meleeSpots[i].enabled = false;

                meleeHealthbars[i].text = "";
                meleeHealthbars[i].enabled = false;
            }
        }

        for(int i = 0; i < rangedSpots.Length; i++){
            if(i < rangedMonsterList.Count){
                if(rangedMonsterList[i] != null){
                    //Debug.Log(monsterSpriteDict[meleeMonsterList[i].name].sprite.name);
                    Sprite spriteToAssign = monsterSpriteDict[rangedMonsterList[i].name];
                    rangedSpots[i].sprite = spriteToAssign;
                    rangedSpots[i].enabled = true;

                    rangedHealthbars[i].text = "Health: " + rangedMonsterList[i].health;
                    rangedHealthbars[i].enabled = true;

                    Debug.Log("Assigned Sprite: " + spriteToAssign.name);
                }
                else{
                    rangedSpots[i].sprite = emptySprite;
                    rangedSpots[i].enabled = false;

                    rangedHealthbars[i].text = "";
                    rangedHealthbars[i].enabled = false;
                }
            }
            else{
                rangedSpots[i].sprite = emptySprite;
                rangedSpots[i].enabled = false;

                rangedHealthbars[i].text = "";
                rangedHealthbars[i].enabled = false;
            }
        }

    }

    string listMonsters(){

        string monsterString="";

        foreach(Monster monster in meleeMonsterList){
            if(monster != null){
                monsterString = monsterString + monster.ToString() + "||";
            }
            else{
                monsterString = monsterString + "Empty Space ||";
            }
        }
        foreach(Monster monster in rangedMonsterList){
            if(monster != null){
                monsterString = monsterString + "||" + monster.ToString();
            }
        }

        return(monsterString);
    }

}
