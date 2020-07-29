﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ProjectileType
{
    PLAYER_RIFLE
}
public class ProjectileManager : MonoBehaviour
{
    private List<GameObject> _playerProjectiles;
    public List<GameObject> PlayerProjectiles
    {
        get { return _playerProjectiles; }
    }
    private List<GameObject> _enemyProjectiles;
    private float _maxPlayerBullets;

    public GameObject _player;
    public GameObject _rifleBullet;

    // Will Bertiz
    public List<AudioClip> rifeGunshotSFX;          // List to hold the sfx for gunshots

    [SerializeField]
    private AudioManager audioManager;  // reference to playerAudioManager for methods

    // Start is called before the first frame update
    void Start()
    {
        _maxPlayerBullets = 15;
        _playerProjectiles = new List<GameObject>();
        _enemyProjectiles = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        DeleteBulletsPastLifeTime();
    }
    public void FirePlayerBullet(ProjectileType type, Vector2 direction)
    {
        if (_playerProjectiles.Count > _maxPlayerBullets) return;

        switch (type)
        {
            case ProjectileType.PLAYER_RIFLE:
                {
                    GameObject newProjectile = Instantiate(_rifleBullet, _player.transform.position, Quaternion.identity);
                    newProjectile.GetComponent<Projectile>().Velocity = newProjectile.GetComponent<Projectile>().InitialSpeed * direction;
                    newProjectile.GetComponent<Projectile>().ID = _playerProjectiles.Count;
                    _playerProjectiles.Add(newProjectile);
                    break;
                }
        }
    }
    public void DeleteBulletsPastLifeTime()
    {
        // Check to remove player bullets...
        for (int i = 0; i < _playerProjectiles.Count; i++)
        {
            if (_playerProjectiles[i].GetComponent<Projectile>().TimeAlive > 3f)
            {
                RemovePlayerProjectile(i);
            }
        }
    }
    public void RemovePlayerProjectile(int _id)
    {
        Destroy(_playerProjectiles[_id]);
        _playerProjectiles.RemoveAt(_id);
        // Decrement the id of all bullets shifted towards the front of the list.
        for(int i = _id; i < _playerProjectiles.Count; i++)
        {
            _playerProjectiles[i].GetComponent<Projectile>().ID--;
        }
    }

    public void RemoveEnemyProjectile(int _id)
    {
        Destroy(_enemyProjectiles[_id]);
        _enemyProjectiles.RemoveAt(_id);
        // Decrement the id of all bullets shifted towards the front of the list.
        for (int i = _id; i < _playerProjectiles.Count; i++)
        {
            _enemyProjectiles[i].GetComponent<Projectile>().ID--;
        }
    }
}