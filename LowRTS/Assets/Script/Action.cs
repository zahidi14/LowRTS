using System;
using UnityEngine;
using System.Collections;

/// <summary>
/// An action handles some logic for entity interactions with the world
/// ie. Moving, Attacking, Gathering, Building
/// 
/// Only one action can be active at a time
/// </summary>
public class Action : MonoBehaviour {

    // The entity this action is running for
    public Entity Entity;
    // The request that began this action
    public ActionRequest Request { get; private set; }
    // Is this action complete?
    public virtual bool Complete { get { return Request == null; } }

    void OnEnable() {
        // Make sure we have an entity
        if (Entity == null) Entity = this.GetComponentInParents<Entity>();
    }

    // Return how best this action is able to serve the request
    public virtual float ScoreRequest(ActionRequest request) { return 0; }

    // Begin the request
    public virtual void Begin(ActionRequest request) {
        Request = request;
    }
    // Update the entity
    public virtual void Step(float dt) {
        if (Request == null) throw new Exception("Unable to step an action without a target!");
    }
    // End the request
    public virtual void End() {
        Request = null;
    }

}
