using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbScript : MonoBehaviour
{
    public Rigidbody2D myRigidbody;
    public CircleCollider2D myCircleCollider;
    [SerializeField] private float orbSpeed = 5;
    public LineRenderer myLineRenderer;
    private int xVelocity;
    private int yVelocity;
    private bool shotOrb = false;

    public int regDamage = 2;
    public int critDamage = 7;

    // Start is called before the first frame update
    void Start()
    {
        myLineRenderer.startColor = Color.white;
    }

    // Update is called once per frame
    void Update()
    {
        if (!(shotOrb) && Input.GetMouseButtonDown(0))
        {
            shootOrb();
        }
        if (myRigidbody.position.y < -6)
        {
            //killOrb();
        }

        if (shotOrb == false)
        {
            drawTrajectory();
        }
        else
        {
            myLineRenderer.positionCount = 0;
        }

    }

    private void shootOrb()
    {
        myRigidbody.bodyType = RigidbodyType2D.Dynamic;
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        float directionVectorLength = Mathf.Sqrt(Mathf.Pow((mousePos.x - myRigidbody.position.x),2) + Mathf.Pow(mousePos.y - myRigidbody.position.y, 2));

        float vectorNormalizer = orbSpeed / directionVectorLength;

        myRigidbody.velocity = new Vector2(vectorNormalizer * (mousePos.x - myRigidbody.position.x), vectorNormalizer * (mousePos.y - myRigidbody.position.y));

        shotOrb = true;
    }


    private List<Vector2> renderTrajPoints()
    {

        List<Vector2> trajectoryPoints = new List<Vector2>();

        float maxDur = 3.5f;
        float stepInterval = 0.02f;
        int maxSteps = (int)(maxDur / stepInterval);

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 initialPosition = myRigidbody.position;
        float directionVectorLength = Mathf.Sqrt(Mathf.Pow((mousePos.x - myRigidbody.position.x), 2) + Mathf.Pow(mousePos.y - myRigidbody.position.y, 2));

        float vectorNormalizer = orbSpeed / directionVectorLength;

        Vector2 directionVector = new Vector2(vectorNormalizer * (mousePos.x - myRigidbody.position.x), vectorNormalizer * (mousePos.y - myRigidbody.position.y));


        for (int i = 0; i < maxSteps; i++)
        {
            Vector2 calculatedPos = initialPosition + directionVector * i * stepInterval;
            calculatedPos.y += Physics2D.gravity.y / 2 * Mathf.Pow((i * stepInterval), 2);

            trajectoryPoints.Add(calculatedPos);

            if (CheckForCollision(calculatedPos) && i > 1)
            {
                break;
            }


        }
        return (trajectoryPoints);
    }

    private void drawTrajectory()
    {
        myLineRenderer.positionCount = renderTrajPoints().Count;
        for(int i = 0; i < myLineRenderer.positionCount; i++)
        {
            myLineRenderer.SetPosition(i, renderTrajPoints()[i]);
        }
    }

    private bool CheckForCollision(Vector2 position)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(position, 0.1f); //Measure collision via a small circle at the latest position, dont continue simulating Arc if hit
        if (hits.Length > 0) //Return true if something is hit, stopping Arc simulation
        {
            return true;
        }
        return false;
    }
}
