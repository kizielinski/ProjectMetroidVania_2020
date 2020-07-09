using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    private ProjectileManager _projMan;
  

    // Start is called before the first frame update
    void Start()
    {
        _projMan = GameObject.Find("GameManager").GetComponent<ProjectileManager>();   
    }

    // Update is called once per frame
    void Update()
    {
        if(_projMan.PlayerProjectiles.Count > 0)
            DetectProjectileCollisions();
    }

    private void DetectProjectileCollisions()
    {
        for(int i = 0; i < _projMan.PlayerProjectiles.Count; i++) 
        {
            GameObject g = _projMan.PlayerProjectiles[i];
            RaycastHit2D hit = Physics2D.Raycast(g.transform.position, g.GetComponent<Projectile>().Velocity.normalized, .2f);

            if (!hit.collider) continue;

            //TODO: Use collider information to trigger appropriate actions (i.e. enemie dying).

            _projMan.RemovePlayerProjectile(g.GetComponent<Projectile>().ID);
            i--;
        }
    }
}
