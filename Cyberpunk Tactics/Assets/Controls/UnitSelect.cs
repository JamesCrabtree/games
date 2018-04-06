using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelect : MonoBehaviour {

    bool isSelecting = false;
    bool usingAbility = false;
    HashSet<GameObject> selectedObjects;
    Vector3 mousePosition1;
    Pathfinding search;
    GameObject UIUnit;

    void Start()
    {
        search = gameObject.GetComponent<Pathfinding>();
        selectedObjects = new HashSet<GameObject>();
    }

    void Update()
    {
        if (Input.GetKey("escape"))
            Application.Quit();
        if (Input.GetMouseButtonDown(0))
        {
            if (usingAbility)
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    foreach (GameObject obj in selectedObjects)
                    {
                        Controller controller = obj.GetComponent<Controller>();
                        if (controller.usingAbilityOne)
                        {
                            controller.abilityOneSelect(hit.point);
                            controller.usingAbilityOne = false;
                        }
                    }
                    usingAbility = false;
                }
            }
            else
            {
                isSelecting = true;
                if (!Input.GetKey(KeyCode.LeftShift))
                {
                    foreach (GameObject obj in selectedObjects)
                        unselectObject(obj);
                    if(UIUnit != null)
                        unshowUI(UIUnit);
                    UIUnit = null;
                    selectedObjects.Clear();
                    search.clearCurrentlySelected();
                }
                mousePosition1 = Input.mousePosition;
            }
        }
        if (Input.GetMouseButtonUp(0))
            isSelecting = false;

        if (isSelecting)
        {
            bool displayUI = true;
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Clickable"))
            {
                if (IsWithinSelectionBounds(obj) || MouseOver(obj))
                {
                    GameObject clickableObject = obj;
                    Controller controller = getController(clickableObject);
                    if (controller.isAttachment)
                        clickableObject = controller.getMasterObject();
                    selectObject(clickableObject);
                    if (displayUI)
                    {
                        UIUnit = clickableObject;
                        showUI(UIUnit);
                        displayUI = false;
                    }
                    selectedObjects.Add(clickableObject);
                    controller = getController(clickableObject);
                    if (controller.isUnit)
                        search.addUnitToCurrentlySelected(clickableObject.transform);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            foreach (GameObject unit in selectedObjects)
            {
                Controller controller = unit.GetComponent<Controller>();
                if (controller.abilityOneRequiresSelect)
                    usingAbility = true;
                controller.abilityOne();
                controller.usingAbilityOne = true;
            }
        }
    }

    void OnGUI()
    {
        if (isSelecting)
        {
            var rect = Utils.GetScreenRect(mousePosition1, Input.mousePosition);
            Utils.DrawScreenRect(rect, new Color(0.8f, 0.8f, 0.95f, 0.25f));
            Utils.DrawScreenRectBorder(rect, 2, new Color(0.8f, 0.8f, 0.95f));
        }
    }

    public bool IsWithinSelectionBounds(GameObject obj)
    {
        if (!isSelecting)
            return false;

        var camera = Camera.main;
        var viewportBounds =
            Utils.GetViewportBounds(camera, mousePosition1, Input.mousePosition);

        return viewportBounds.Contains(
            camera.WorldToViewportPoint(obj.transform.position));
    }

    public bool MouseOver(GameObject obj)
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            return obj == hit.transform.gameObject;
        }
        return false;
    }

    public void selectObject(GameObject obj)
    {
        obj.GetComponent<Controller>().select();
    }

    public void unselectObject(GameObject obj)
    {
        obj.GetComponent<Controller>().unselect();
    }

    public void showUI(GameObject obj)
    {
        obj.GetComponent<Controller>().displayUI();
    }

    public void unshowUI(GameObject obj)
    {
        obj.GetComponent<Controller>().destroyUI();
    }

    private Controller getController(GameObject obj)
    {
        return obj.GetComponent<Controller>();
    }
}

public static class Utils
{
    static Texture2D _whiteTexture;
    public static Texture2D WhiteTexture
    {
        get
        {
            if (_whiteTexture == null)
            {
                _whiteTexture = new Texture2D(1, 1);
                _whiteTexture.SetPixel(0, 0, Color.white);
                _whiteTexture.Apply();
            }

            return _whiteTexture;
        }
    }

    public static void DrawScreenRect(Rect rect, Color color)
    {
        GUI.color = color;
        GUI.DrawTexture(rect, WhiteTexture);
        GUI.color = Color.white;
    }

    public static void DrawScreenRectBorder(Rect rect, float thickness, Color color)
    {
        // Top
        Utils.DrawScreenRect(new Rect(rect.xMin, rect.yMin, rect.width, thickness), color);
        // Left
        Utils.DrawScreenRect(new Rect(rect.xMin, rect.yMin, thickness, rect.height), color);
        // Right
        Utils.DrawScreenRect(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), color);
        // Bottom
        Utils.DrawScreenRect(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), color);
    }

    public static Rect GetScreenRect(Vector3 screenPosition1, Vector3 screenPosition2)
    {
        // Move origin from bottom left to top left
        screenPosition1.y = Screen.height - screenPosition1.y;
        screenPosition2.y = Screen.height - screenPosition2.y;
        // Calculate corners
        var topLeft = Vector3.Min(screenPosition1, screenPosition2);
        var bottomRight = Vector3.Max(screenPosition1, screenPosition2);
        // Create Rect
        return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
    }

    public static Bounds GetViewportBounds(Camera camera, Vector3 screenPosition1, Vector3 screenPosition2)
    {
        var v1 = Camera.main.ScreenToViewportPoint(screenPosition1);
        var v2 = Camera.main.ScreenToViewportPoint(screenPosition2);
        var min = Vector3.Min(v1, v2);
        var max = Vector3.Max(v1, v2);
        min.z = camera.nearClipPlane;
        max.z = camera.farClipPlane;

        var bounds = new Bounds();
        bounds.SetMinMax(min, max);
        return bounds;
    }
}
