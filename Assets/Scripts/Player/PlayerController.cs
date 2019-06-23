﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public StatusManager stats;
    public Rigidbody rb;
    private Vector3 forward;
    private Vector3 interactPosition;
    private const float interactRadius = 0.8f;
    private GrabableItem grabbedItem;

    /// For debugging purposes only.
    public void OnDrawGizmos()
    {
        // Draw 'Joystick' position (white sphere)
        Gizmos.DrawWireSphere(this.forward, 0.5f);

        // Draw 'Interactable Radius' position (red sphere)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.interactPosition, interactRadius);
    }

    void Awake()
    {
        this.stats.initialize();
    }
    private void Update()
    {
        // Update interactable radius position
        this.interactPosition = this.transform.GetChild(0).position;
    }

    /// <summary>
    /// Triggers an interaction in the interaction radius.
    /// </summary>
    public void interact()
    {
        // check for every object in interactable radius
        Collider[] interactable = Physics.OverlapSphere(this.interactPosition, interactRadius);
        foreach (Collider t in interactable)
        {
            // usable item  ??
            if (t.transform.tag == Common.usableTag)
            {
                // TODO == check for usable items
            }
            // Grabable Items
            if (t.transform.tag == Common.grabableTag)
            {
                GrabableItem i = t.GetComponent<GrabableItem>();
                // found an item! 
                if (i != null && i.isGrabable())
                {
                    // if already has a item, swap
                    if (this.hasGrabbedItem())
                    {
                        this.grabbedItem.drop(this.interactPosition + Common.dropPosition);
                        i.grab(this.transform);
                        this.grabbedItem = i;
                        return;
                    }
                    //if not, grab
                    else
                    {
                        i.grab(this.transform);
                        this.grabbedItem = i;
                        return;
                    }

                }
            }
        }
        // if no item is foun, drop the items
        if (this.hasGrabbedItem())
        {
            this.grabbedItem.drop(this.getThrowVector());
            this.grabbedItem = null;
        }
    }

    /// <summary>
    /// Move the character towards joystick position.
    /// </summary>
    /// <param name="speed">Speed and direction of the movement.</param>
    /// <param name="running">Is Running?.</param>
    public void moveCharacter(Vector3 speed, bool running)
    {
        float multiplier = stats.SPD * (running ? stats.runMultiplier : 1f);
        Vector3 finalSpeed = new Vector3(speed.x * multiplier, 0f, speed.z * multiplier);
        this.rb.velocity = finalSpeed;
    }

    /// <summary>
    /// Updates the character facing direction.
    /// </summary>
    public void updateDirection(Vector3 direction)
    {
        this.forward = (this.transform.position + direction);
        this.transform.LookAt(this.forward, Vector3.up);
    }

    /// <summary>
    /// Determines the direction and intensity to throw an item.
    /// </summary>
    /// <returns>
    /// Vector3 throw direction.
    /// </returns>
    public Vector3 getThrowVector()
    {
        return new Vector3(transform.forward.x * Common.throwForce, 0f, transform.forward.z * Common.throwForce);
    }

    /// <summary>
    /// Determines if the player is holding an item.
    /// </summary>
    /// <returns>
    /// bool true if holding something, otherwise else.
    /// </returns>
    public bool hasGrabbedItem()
    {
        return (this.grabbedItem != null);
    }

    // triggered when the component is reset
    private void Reset()
    {
        this.stats = GetComponent<StatusManager>();
        this.rb = GetComponent<Rigidbody>();
    }
}
