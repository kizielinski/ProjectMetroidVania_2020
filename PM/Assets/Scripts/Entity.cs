using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : Object
{
    protected void Start()
    {
        base.Start();
    }
    // Update is called once per frame

    //TODO: implement shared properties and functionality between enemies, collectibles and npcs.
    /// <summary>
    /// 
    /// </summary>
    /// <param name="seekPos"></param>
    /// <param name="tooClose"> Do not seek if this entity is this distance or closer to the target position.</param>
    protected bool Seek(Vector2 seekPos, float tooClose)
    {
        float squaredDist = Mathf.Pow(this.transform.position.x - seekPos.x, 2) + Mathf.Pow(this.transform.position.y - seekPos.y, 2);
        if (squaredDist < Mathf.Pow(tooClose, 2))
        {
            return false;
        }
        Vector2 direction = ((Vector3)seekPos - this.transform.position).normalized;
        Vector2 seekForce = direction *  Time.deltaTime * 1 * Mathf.Pow((squaredDist - Mathf.Pow(tooClose, 2)), 2);
        if (this._leftColliding && seekForce.x < 0) seekForce.x = 0;
        if (this._rightColliding && seekForce.x > 0) seekForce.x = 0;
        seekForce.y = 0;
        ApplyForce(seekForce);
        Debug.Log("Performing Seek");
        return true;
    }
}
