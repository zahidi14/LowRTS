using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneManager : MonoBehaviour {

    // A list of all entities in the scene
    public List<Entity> Entities = new List<Entity>();

    // List of commands to be invoked
    public List<Command> Commands = new List<Command>();

    void Update() {
        // Invoke any commands for this frame
        // TODO: JUST for this frame.
        for (int c = 0; c < Commands.Count; ++c) {
            Commands[c].Invoke(this);
        }
        Commands.Clear();
        // Update all entities in the scene
        for (int e = 0; e < Entities.Count; ++e) {
            var entity = Entities[e];
            entity.Step(Time.deltaTime);
        }
    }


    // A singleton instance of this class
    private static SceneManager _instance;
    public static SceneManager Instance {
        get {
            if (_instance == null) _instance = GameObject.FindObjectOfType<SceneManager>();
            return _instance;
        }
    }

    // Keep track of which entities exist in the scene
    public static void RegisterEntity(Entity entity) {
        if (Instance != null) Instance.Entities.Add(entity);
    }
    public static void DeregisterEntity(Entity entity) {
        if (Instance != null) Instance.Entities.Remove(entity);
    }

    // Queue a command to be invoked as soon as possible
    public static void QueueCommand(CommandAction command) {
        if (Instance != null) Instance.Commands.Add(command);
    }
}
