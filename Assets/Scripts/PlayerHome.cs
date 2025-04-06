using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerHome : MonoBehaviour
{
    public List<Vector2> healthyArea = new List<Vector2>();
    public List<Vector2> bombedAreasToBeRemoved = new List<Vector2>();
    [SerializeField] float areaResolution = 0.2f;
    BoxCollider2D boxCollider2D;
    public Vector2 homeTopLeft;
    public Vector2 homeBottomRight;
    public Vector2 homeTopRight;
    public Vector2 homeBottomLeft;
    public List<GameObject> homeowners = new List<GameObject>();
    List<GameObject> craters = new List<GameObject>();

    bool isRightSide;

    [SerializeField] ScoreKeeper scoreKeeper;
    float initialHealthyAreaCount;
    float healthyAreaDiscountFactor =2f;
    void Start()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
        SetPlayerHomeCorners();
        InitiateHealthyArea();
        isRightSide = homeowners.ElementAt(0).GetComponentInChildren<PlayerMovement>().isRightSide;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void InitiateHealthyArea(){
        for (float x = homeTopLeft.x; x <= homeTopRight.x; x += areaResolution) {
            for (float y = homeBottomLeft.y; y <= homeTopLeft.y; y += areaResolution) {
                healthyArea.Add(new Vector2(x, y));
            }
        }
        initialHealthyAreaCount = healthyArea.Count;
        scoreKeeper.LeftPlayerHomeSliderValue = 1f;
        scoreKeeper.RightPlayerHomeSliderValue = 1f;
        
        Debug.Log("Healthy Area: " + healthyArea.Count); 
    }
    public void RemovedBombedArea(GameObject crater)
    {
        SpriteRenderer craterRenderer = crater.GetComponentInChildren<SpriteRenderer>();
        Vector2 craterPos = crater.transform.position;
        float craterWidth = craterRenderer.bounds.size.x;
        float craterHeight = craterRenderer.bounds.size.y;

        Vector2 topLeft = new Vector2(craterPos.x - craterWidth/2, craterPos.y + craterHeight/2);
        Vector2 bottomRight = new Vector2(craterPos.x + craterWidth/2, craterPos.y - craterHeight/2);

        for (int i = healthyArea.Count - 1; i >= 0; i--)
        {
            Vector2 point = healthyArea[i];
            if (point.x >= topLeft.x && point.x <= bottomRight.x &&
                point.y <= topLeft.y && point.y >= bottomRight.y)
            {
                healthyArea.RemoveAt(i);
                bombedAreasToBeRemoved.Add(point);
            }
        }
        craters.Add(crater);
        if(isRightSide){
            scoreKeeper.RightPlayerHomeSliderValue = (float)healthyArea.Count / initialHealthyAreaCount;
        }else{
            scoreKeeper.LeftPlayerHomeSliderValue = (float)healthyArea.Count / initialHealthyAreaCount;
        }
        Debug.Log("==> RemovedBombedArea Healthy Area: " + healthyArea.Count); 
    }
    private void SetPlayerHomeCorners () {
        float width = boxCollider2D.bounds.size.x;
        float height = boxCollider2D.bounds.size.y;
        // float width = go.GetComponent<BoxCollider2D> ().bounds.size.x;
        // float height = go.GetComponent<BoxCollider2D> ().bounds.size.y;

        Vector2 topRight = transform.position, topLeft = transform.position, bottomRight = transform.position, bottomLeft = transform.position;
        //Vector2 topRight = go.transform.position, topLeft = go.transform.position, bottomRight = go.transform.position, bottomLeft = go.transform.position;

        topRight.x += width / 2;
        topRight.y += height / 2;

        topLeft.x -= width / 2;
        topLeft.y += height / 2;

        bottomRight.x += width / 2;
        bottomRight.y -= height / 2;

        bottomLeft.x -= width / 2;
        bottomLeft.y -= height / 2;
        homeTopLeft = new Vector2(topLeft.x, topLeft.y);
        homeBottomRight = new Vector2(bottomRight.x, bottomRight.y);
        homeTopRight = new Vector2(topRight.x, topRight.y);
        homeBottomLeft = new Vector2(bottomLeft.x, bottomLeft.y);

    }

    private void OnDrawGizmos()
    {
        // //drawHome bounds
        // Gizmos.color = new Color(Random.value, Random.value, Random.value, 1f);
        // Gizmos.DrawLine(homeTopLeft, new Vector2(homeTopLeft.x, homeBottomRight.y));
        // Gizmos.DrawLine(homeTopLeft, new Vector2(homeBottomRight.x, homeTopLeft.y));
        // Gizmos.DrawLine(homeBottomRight, new Vector2(homeTopLeft.x, homeBottomRight.y));
        // Gizmos.DrawLine(homeBottomRight, new Vector2(homeBottomRight.x, homeTopLeft.y));

        //draw all the bombed areas
        Gizmos.color = new Color(1f, 0f, 0f, 0.5f); // Red color for bombed areas       
        foreach (Vector2 point in bombedAreasToBeRemoved)
        {
            Gizmos.DrawSphere(point, 0.1f); // Draw a sphere at each point in the list
        }
        //draw all the healthy areas
        // Gizmos.color = new Color(0f, 1f, 0f, 0.5f); // Green color for healthy areas    
        // foreach (Vector2 point in healthyArea)
        // {
        //     Gizmos.DrawSphere(point, 0.1f); // Draw a sphere at each point in the list
        // }

        //draw all the craters
        // Gizmos.color = new Color(0f, 0f, 1f, 0.5f); // Blue color for craters
        // foreach (GameObject crater in craters)
        // {
        //     // Debug.Log($"Crater position: {crater.transform.position}");
        //     SpriteRenderer craterRenderer = crater.GetComponentInChildren<SpriteRenderer>();
        //     Vector2 craterPos = crater.transform.position;
        //     float craterWidth = craterRenderer.bounds.size.x;
        //     float craterHeight = craterRenderer.bounds.size.y;

        //     Vector2 topLeft = new Vector2(craterPos.x - craterWidth / 2, craterPos.y + craterHeight / 2);
        //     Vector2 bottomRight = new Vector2(craterPos.x + craterWidth / 2, craterPos.y - craterHeight / 2);

        //     Gizmos.DrawLine(topLeft, new Vector2(topLeft.x, bottomRight.y));
        //     Gizmos.DrawLine(topLeft, new Vector2(bottomRight.x, topLeft.y));
        //     Gizmos.DrawLine(bottomRight, new Vector2(topLeft.x, bottomRight.y));
        //     Gizmos.DrawLine(bottomRight, new Vector2(bottomRight.x, topLeft.y));
        // }
    }
}
