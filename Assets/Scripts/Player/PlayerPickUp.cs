using System;
using System.Collections;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;

public class PlayerPickUp : MonoSingleton<PlayerPickUp>
{
    private Vector3 _position;
    
    private void OnEnable()
    {
        InputManager.interactStarted += PickupItem;
    }

    private void PickupItem(InputAction.CallbackContext context)
    {
        if (!PlayerInventory.instance.CheckHands()) // check if player is already holding an item
        {
            var hit = DetectItem();
        
            if (hit != default(RaycastHit2D))
                StartCoroutine(OnHit(hit));
        }
    }

    private RaycastHit2D DetectItem()
    {
        _position = transform.position;
        RaycastHit2D hit = default(RaycastHit2D);
        Vector3 raycastPos = new Vector3(_position.x, _position.y - 0.08f, _position.z);

        hit = PlayerAnimation.instance.CheckDirection() switch
        {
            0 => // up
                Physics2D.Raycast(raycastPos, Vector2.up, 0.16f, 1 << 9),
            1 => // down
                Physics2D.Raycast(raycastPos, Vector2.down, 0.16f, 1 << 9),
            2 => // left
                Physics2D.Raycast(raycastPos, Vector2.left, 0.16f, 1 << 9),
            3 => // right
                Physics2D.Raycast(raycastPos, Vector2.right, 0.16f, 1 << 9),
            _ => hit
        };
        
        return hit;
    }

    private IEnumerator OnHit(RaycastHit2D hit)
    {
        Debug.Log("Ran 2");
        SpriteRenderer hitSpriteRenderer = hit.transform.GetComponent<SpriteRenderer>();
        Vector3 target = new Vector3(_position.x, _position.y + 0.16f, _position.z);
        float time = 0f;
        float duration = 0.5f;
        
        hit.collider.enabled = false;
        GameManager.instance.DisablePlayerMovement();
        while (time < duration)
        {
            time += Time.deltaTime / duration;
            hit.transform.position = Vector3.Lerp(hit.transform.position, target, time);

            if (Math.Abs(time - duration / 2) < 0.001f)
            {
                hitSpriteRenderer.sortingOrder = 4;
                Debug.Log("Set sorting within layer to 4");
            }
            yield return null;
        }
        
        GameManager.instance.EnablePlayerMovement();
        PlayerInventory.instance.PutItemInHands(hit.transform.gameObject);
        hit.transform.parent = transform;
        
    }
    
    private void OnDrawGizmos()
    {
        Vector3 position = transform.position;
        Vector3 raycastPos = new Vector3(position.x, position.y - 0.08f, position.z);
        Vector3 raycastToPos = new Vector3(position.x, position.y - 0.16f, position.z);
            
        Gizmos.DrawLine(raycastPos, raycastToPos);
    }
}


    
