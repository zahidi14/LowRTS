using UnityEngine;
using System.Linq;
using System.Collections;

public class Entity : MonoBehaviour {

    public Player Player;
    public Vector3 Position { get { return transform.position; } set { transform.position = value; } }
    public Action ActiveAction { get; private set; }

    void Awake() {
        // Make sure this object belongs to a player
        if (Player == null) {
            var gaia = GameObject.Find("Gaia");
            if (gaia != null) Player = gaia.GetComponent<Player>();
        }
        if (Player == null) Player = GameObject.FindObjectOfType<Player>();
    }

    // Notify the TimeManager that a new entity is available
    void OnEnable() {
        SceneManager.RegisterEntity(this);
    }
    // Notify the TimeManager that the entity was removed
    void OnDisable() {
        SceneManager.DeregisterEntity(this);
    }

    // Request an action be performed
    public void BeginAction(ActionRequest request) {
        var action = GetComponents<Action>().OrderByDescending(a => a.ScoreRequest(request)).FirstOrDefault();
        if (action != null) BeginAction(action, request);
        else Debug.Log("Unable to find action for request " + request);
    }
    // Request a specific action be performed
    public void BeginAction(Action action, ActionRequest request) {
        if (ActiveAction != null) ActiveAction.End();
        ActiveAction = action;
        if (ActiveAction != null) ActiveAction.Begin(request);
    }
    // End the current action
    public void EndAction() {
        BeginAction(null, null);
    }

    // Controlled update of the object
    public void Step(float dt) {
        if (ActiveAction != null) {
            ActiveAction.Step(dt);
            if (ActiveAction.Complete) EndAction();
        }
    }

    // Kill the object
    public void Die() {
        Destroy(gameObject);
    }

}
