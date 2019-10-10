using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// An action request defines the parameters used by actions
/// to score and begin acting
/// </summary>
[Serializable]
public class ActionRequest {

    // The target entity and position
    public Entity TargetEntity;
    public Vector3 TargetLocation;

    // Get the target entities position, or the target position
    public Vector3 GetTargetLocation() {
        if (TargetEntity != null) return TargetEntity.Position;
        return TargetLocation;
    }

    // So that the debug print looks nicer
    public override string ToString() {
        return (TargetEntity != null ? TargetEntity.name : "-none-") + " at (" + TargetLocation + ")";
    }

}
