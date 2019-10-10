using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/// <summary>
/// Begin an action on the provided entities
/// </summary>
[Serializable]
public class CommandAction : Command {

    public Entity[] Entities;
    public ActionRequest Request;

    public override void Invoke(SceneManager scene) {
        foreach (var entity in Entities) entity.BeginAction(Request);
        base.Invoke(scene);
    }

}
