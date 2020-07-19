using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Object
{
    public GameObject _player;
    /// <summary>
    /// Enemy seeks as long as it is this distance from the target object or farther.
    /// </summary>
    private float _seekRadius;
    // Start is called before the first frame update
    void Start()
    {
        _seekRadius = 1.5f;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        this.Seek(_player.transform.position);
    }

    private void Seek(Vector2 seekPos)
    {
        float squaredDist = Mathf.Pow(this.transform.position.x - seekPos.x, 2) + Mathf.Pow(this.transform.position.y - seekPos.y, 2);
        if (squaredDist < Mathf.Pow(_seekRadius, 2))
        {
            StopHorizontalMotion();
            return;
        }
        Vector2 direction = (this.transform.position - (Vector3)seekPos).normalized;
        Vector2 seekForce = direction * Mathf.Pow((squaredDist - Mathf.Pow(_seekRadius, 2)), 2);
        ApplyForce(seekForce);
    }
}
