using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ProjectileLaucher : NetworkBehaviour
{

    [Header("References")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Transform projectileSpawPoint;
    [SerializeField] private GameObject serverProjectilePrefab;
    [SerializeField] private GameObject clientProjectilePrefab;
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private Collider2D playerCollider2D;

    [Header("Settings")]
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float fireRate;
    [SerializeField] private float muzzleFlashDuration;

    private bool shouldFire;
    private float previusFireTimer;
    private float muzzleFlashTimer;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            return;
        }

        inputReader.PrimaryFireEvent += HandlePrimaryFire;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner)
        {
            return;
        }

        inputReader.PrimaryFireEvent -= HandlePrimaryFire;
    }

    private void Update()
    {
        if (muzzleFlashTimer > 0f)
        {
            muzzleFlashTimer -= Time.deltaTime;

            if (muzzleFlashTimer <= 0f)
            {

            }
        }

        if (!IsOwner)
        {
            return;
        }

        if (!shouldFire)
        {
            return;
        }

        if(Time.time < (1/ fireRate) + previusFireTimer) 
        { 
            return; 
        }

        PrimaryFireServerRpc(projectileSpawPoint.position, projectileSpawPoint.up);
        SpawnDummyProjectile(projectileSpawPoint.position, projectileSpawPoint.up);
    
        previusFireTimer = Time.time;
    }

    [ServerRpc]
    private void PrimaryFireServerRpc(Vector3 spawnPos, Vector3 direction)
    {
        GameObject projectileInstance = Instantiate(serverProjectilePrefab, spawnPos, Quaternion.identity);
        projectileInstance.transform.up = direction;

        Physics2D.IgnoreCollision(playerCollider2D, projectileInstance.GetComponent<Collider2D>());

        if (projectileInstance.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb2D)) 
        {
            rb2D.velocity = rb2D.transform.up * projectileSpeed;
        }

        SpawnDummyProjectileClientRpc(spawnPos, direction);
    }

    [ClientRpc]
    private void SpawnDummyProjectileClientRpc(Vector3 spanwPos, Vector3 direction)
    {
        if (IsOwner)
        {
            return;
        }

        SpawnDummyProjectile(spanwPos, direction);
    }

    private void SpawnDummyProjectile(Vector3 spawnPos, Vector3 direction)
    {
        muzzleFlash.SetActive(true);

        muzzleFlashTimer = muzzleFlashDuration;
        
        GameObject projectileInstance = Instantiate(clientProjectilePrefab, spawnPos, Quaternion.identity);
        projectileInstance.transform.up = direction;

        Physics2D.IgnoreCollision(playerCollider2D, projectileInstance.GetComponent<Collider2D>());

        if (projectileInstance.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb2D))
        {
            rb2D.velocity = rb2D.transform.up * projectileSpeed;
        }
    }

    private void HandlePrimaryFire(bool shouldFire)
    {
        this.shouldFire = shouldFire;
    }
}
