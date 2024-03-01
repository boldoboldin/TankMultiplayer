using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerAiming : NetworkBehaviour
{
    [SerializeField] private Transform turretTransform;
    [SerializeField] private InputReader inputReader;

    private void LateUpdate()
    {
        if (!IsOwner)
        {
            return;
        }

        Vector2 aimScreenPos = inputReader.aimPos;
        Vector2 aimWorldPos = Camera.main.ScreenToWorldPoint(aimScreenPos);

        turretTransform.up = new Vector2(aimWorldPos.x - turretTransform.position.x, 
            aimWorldPos.y - turretTransform.position.y);
    }
}