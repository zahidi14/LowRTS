using UnityEngine;
using System.Collections;

public static class ComponentEx {

    // Helper method to handle getting components in parents
    public static T GetComponentInParents<T>(this Component behaviour) where T : Component {
        var transform = behaviour.transform;
        while (transform != null) {
            var component = transform.GetComponent<T>();
            if (component != null) return component;
            transform = transform.parent;
        }
        return null;
    }

}
