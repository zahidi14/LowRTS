using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SelectionManager : MonoBehaviour {

    // The camera to use for picking
    public Camera Camera;
    // The marker to use to show selections
    public SelectionMarker SelectionMarker;

    // Parameters for drag selection
    private Vector2? dragStart;
    private Vector2 dragEnd;
    private Rect dragRect {
        get {
            return Rect.MinMaxRect(
                Mathf.Min(dragStart.Value.x, dragEnd.x),
                Mathf.Min(dragStart.Value.y, dragEnd.y),
                Mathf.Max(dragStart.Value.x, dragEnd.x),
                Mathf.Max(dragStart.Value.y, dragEnd.y)
            );
        }
    }
    private bool IsDragging { get { return dragStart != null && dragRect.width >= 4 && dragRect.height >= 4; } }

    // The currently selected objects
    public List<Entity> Selected = new List<Entity>();
    // Markers for the currently selected objects
    public List<SelectionMarker> Markers = new List<SelectionMarker>();

    void Awake() {
        // Make sure we have a valid camera
        if (Camera == null) Camera = GetComponent<Camera>();
        if (Camera == null) Camera = Camera.main;
    }

    void Update() {
        // Begin dragging if they user clicks
        if (dragStart == null && Input.GetMouseButton(0)) dragStart = (Input.mousePosition);
        // Update the drag if they are still clicking
        if (dragStart != null) {
            if (Input.GetMouseButton(0)) dragEnd = (Input.mousePosition);
            UpdateSelection(Input.mousePosition);
            if (Input.GetMouseButtonUp(0)) {
                EndSelection(Input.mousePosition);
            }
        }
        // Give units orders
        if (Input.GetMouseButtonDown(1)) {
            CommandAtRay(Input.mousePosition);
        }

        // Delete the selection if the user presses delete
        if (Input.GetKeyDown(KeyCode.Delete)) {
            for (int s = Selected.Count - 1; s >= 0; --s) {
                var selected = Selected[s];
                if (selected != null) {
                    var entity = selected;
                    if (entity != null) entity.Die();
                }
            }
            ClearSelected();
        }
    }

    // Clear the selection based on user input
    public void PrepareSelect() {
        if (!Input.GetKey(KeyCode.LeftShift)) ClearSelected();
    }
    // Update the drag-selection
    public void UpdateSelection(Vector2 position) {
        if (dragStart != null) {
            dragEnd = position;
        }
        if (IsDragging) {
            PrepareSelect();
            SelectInRange(dragRect);
        }
    }
    // Perform the drag-selection
    public void EndSelection(Vector2 position) {
        if (!IsDragging) {
            PrepareSelect();
            SelectAtRay(Input.mousePosition);
        }
        dragStart = null;
    }

    // Various methods to select objects
    public void SelectInRange(Rect rect) {
        foreach (Entity entity in GameObject.FindObjectsOfType(typeof(Entity))) {
            var spos = Camera.WorldToScreenPoint(entity.transform.position);
            if (rect.Contains(spos)) AddSelectd(entity);
        }
    }
    public Entity GetAtRay(Vector2 mousePos) {
        var ray = Camera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100)) {
            return hit.collider.GetComponentInParents<Entity>();
        }
        return null;
    }
    public void SelectAtRay(Vector2 mousePos) {
        var entity = GetAtRay(mousePos);
        if (entity != null)
            AddSelectd(entity);
    }

    // Send an order to the selected objects
    public void CommandAtRay(Vector2 mousePos) {
        var ray = Camera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100)) {
            var request = new ActionRequest() {
                TargetEntity = null,
                TargetLocation = hit.point,
            };
            var target = hit.collider.GetComponentInParents<Entity>();
            if (target != null) {
                request.TargetEntity = target;
            }
            Target(request);
        }
    }

    // Clear the selection
    public void ClearSelected() {
        Selected.Clear();
        for (int m = 0; m < Markers.Count; ++m) Destroy(Markers[m].gameObject);
        Markers.Clear();
    }
    // Add a single object to the selection
    public void AddSelectd(Entity selectable) {
        if (Selected.Contains(selectable)) return;
        Selected.Add(selectable);
        if (SelectionMarker != null) {
            var marker = (SelectionMarker)GameObject.Instantiate(SelectionMarker);
            marker.Entity = selectable;
            Markers.Add(marker);
        }
    }

    // Draw the selection box
    void OnGUI() {
        if (IsDragging) {
            GUI.Box(Transform(dragRect), Selected.Count + " units");
        }
    }

    // Change from Screen Space to GUI Space
    private Rect Transform(Rect rect) {
        return new Rect(rect.x, Screen.height - rect.yMax - 1, rect.width, rect.height);
    }
    private Vector2 Transform(Vector2 vec) {
        return new Vector2(vec.x, Screen.height - vec.y - 1);
    }

    // Notify of a new action request
    private void Target(ActionRequest request) {
        var command = new CommandAction() {
            Entities = Selected.ToArray(),
            Request = request,
        };
        SceneManager.QueueCommand(command);
    }

}
