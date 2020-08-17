/**
 * @Author - Sean Lynch
 * FlutterFly.cs
 * Date: 08/16/20
 */
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
        SolveQuadratic();
    }
    // Update is called once per frame
    protected override void Update()
    {
        timer += Time.deltaTime;

        // Enemy is currently traveling.
        if(timer < timeToTravel)
        {
            float xPos;
            // Moving left to right...
            if (movingForwards)
            {
                xPos = startPos.x + (timer / timeToTravel) * xLength;
            }
            // Movinf right to left...
            else
            {
                xPos = endPos.x - (timer / timeToTravel) * xLength;
            }
            // Update position.
            this.transform.position = new Vector2(xPos, a * Mathf.Pow(xPos, 2) + b * xPos + c);
        }
        else if(timer > timeToTravel + coolDownTime)
        {
            timer = 0;
            movingForwards = !movingForwards;
        }
    }
    private void SolveQuadratic()
    {
        // Solve system of three equations in form
        // | a |  |x1^2 x1 1|-1   | y1 |
        // | b |= |x2^2 x2 1|   * | y2 |
        // | c |  |x3^2 x3 1|     | y3 |
        float[,] inverse = new float[3, 3];
        // Calculate the determinate of the 3x3 matrix
        float det = 1 / ((Mathf.Pow(startPos.x, 2) * (midPos.x - endPos.x)) + (startPos.x * (Mathf.Pow(endPos.x, 2) - Mathf.Pow(midPos.x, 2))
            + ((Mathf.Pow(midPos.x, 2) * endPos.x) - (midPos.x * Mathf.Pow(endPos.x, 2)))));

        // Calculate the inverse matrix
        inverse[0, 0] = det * (midPos.x - endPos.x);
        inverse[0, 1] = det * (endPos.x - startPos.x);
        inverse[0, 2] = det * (startPos.x - midPos.x);

        inverse[1, 0] = det * (Mathf.Pow(endPos.x, 2) - Mathf.Pow(midPos.x, 2));
        inverse[1, 1] = det * (Mathf.Pow(startPos.x, 2) - Mathf.Pow(endPos.x, 2));
        inverse[1, 2] = det * (Mathf.Pow(midPos.x, 2) - Mathf.Pow(startPos.x, 2));

        inverse[2, 0] = det * ((Mathf.Pow(midPos.x, 2) * endPos.x) - (Mathf.Pow(endPos.x, 2) * midPos.x));
        inverse[2, 1] = det * ((Mathf.Pow(endPos.x, 2) * startPos.x) - (Mathf.Pow(startPos.x, 2) * endPos.x));
        inverse[2, 2] = det * ((Mathf.Pow(startPos.x, 2) * midPos.x) - (Mathf.Pow(midPos.x, 2) * startPos.x));

        // Solve for the coefficients.
        a = (inverse[0, 0] * startPos.y) + (inverse[0, 1] * midPos.y) + (inverse[0, 2] * endPos.y);
        b = (inverse[1, 0] * startPos.y) + (inverse[1, 1] * midPos.y) + (inverse[1, 2] * endPos.y);
        c = (inverse[2, 0] * startPos.y) + (inverse[2, 1] * midPos.y) + (inverse[2, 2] * endPos.y);
    }
}
