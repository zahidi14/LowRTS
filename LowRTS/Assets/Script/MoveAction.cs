using UnityEngine;
using System.Collections;

/// <summary>
/// The MoveAction provides logic to move an object through
/// the world
/// </summary>
public class MoveAction : Action {

    // The speed at which to move
    public float Speed = 1;

    // The action is complete when it has reached its destination
    public override bool Complete {
        get { return Vector3.Distance(Request.GetTargetLocation(), Entity.Position) < 0.5f; }
    }

    // A requst always contains a location, thus this action is always valid
    public override float ScoreRequest(ActionRequest request) { return 1; }

    // Move the owning entity toward the target
    public override void Step(float dt) {
        var target = Request.GetTargetLocation();
        Entity.Position = Vector3.MoveTowards(Entity.Position, target, dt * Speed);
        base.Step(dt);
    }

}
