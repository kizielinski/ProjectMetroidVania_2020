using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlutterFly : Enemy
{
    public Vector2 startPos;
    public Vector2 midPos;
    public Vector2 endPos;

    public float timeToTravel;
    public float coolDownTime;

    private float xLength;
    [SerializeField]
    private float timer;
    private bool movingForwards;


    public float a;
    public float b;
    public float c;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        xLength = Mathf.Abs(endPos.x - startPos.x);
        timer = 0;
        movingForwards = true;
        // Solve system of three equations:
        float[,] inverse = new float[3, 3];
        float det = 1 / ((Mathf.Pow(startPos.x, 2) * (midPos.x - endPos.x)) + (startPos.x * (Mathf.Pow(endPos.x, 2) - Mathf.Pow(midPos.x, 2)) 
            + ((Mathf.Pow(midPos.x, 2) * endPos.x) - (midPos.x * Mathf.Pow(endPos.x, 2)))));

        inverse[0, 0] = det * (midPos.x - endPos.x);
        inverse[0, 1] = det * (endPos.x - startPos.x);
        inverse[0, 2] = det * (startPos.x - midPos.x);

        inverse[1, 0] = det * (Mathf.Pow(endPos.x, 2) - Mathf.Pow(midPos.x, 2));
        inverse[1, 1] = det * (Mathf.Pow(startPos.x, 2) - Mathf.Pow(endPos.x, 2));
        inverse[1, 2] = det * (Mathf.Pow(midPos.x, 2) - Mathf.Pow(startPos.x, 2));

        inverse[2, 0] = det * ((Mathf.Pow(midPos.x, 2) * endPos.x) - (Mathf.Pow(endPos.x, 2) * midPos.x));
        inverse[2, 1] = det * ((Mathf.Pow(endPos.x, 2) * startPos.x) - (Mathf.Pow(startPos.x, 2) * endPos.x));
        inverse[2, 2] = det * ((Mathf.Pow(startPos.x, 2) * midPos.x) - (Mathf.Pow(midPos.x, 2) * startPos.x));

        a = (inverse[0, 0] * startPos.y) + (inverse[0, 1] * midPos.y) + (inverse[0, 2] * endPos.y);
        b = (inverse[1, 0] * startPos.y) + (inverse[1, 1] * midPos.y) + (inverse[1, 2] * endPos.y);
        c = (inverse[2, 0] * startPos.y) + (inverse[2, 1] * midPos.y) + (inverse[2, 2] * endPos.y);
    }
    // Update is called once per frame
    protected override void Update()
    {
        timer += Time.deltaTime;

        if(timer < timeToTravel)
        {
            float xPos;
            if (movingForwards)
            {
                xPos = startPos.x + (timer / timeToTravel) * xLength;
            }
            else
            {
                xPos = endPos.x - (timer / timeToTravel) * xLength;
            }
            this.transform.position = new Vector2(xPos, a * Mathf.Pow(xPos, 2) + b * xPos + c);
        }
        else if(timer > timeToTravel + coolDownTime)
        {
            timer = 0;
            movingForwards = !movingForwards;
        }
    }
}
