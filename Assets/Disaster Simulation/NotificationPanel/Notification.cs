using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class is used to simplify the process of displaying and modifying objective notifications
/// The static class included is untility intended to make this process easier
/// </summary>
public class Notification : MonoBehaviour
{
    public int severity;
    public Text text;
    public PlayerObjective objective;

    public RectTransform rectTransform;

    public PlayerControls manager;
    
    void Start()
    {
        objective.revealed = false;
        rectTransform = gameObject.GetComponent<RectTransform>();
        manager = FindObjectOfType<PlayerControls>();
    }

    public void Clicked()
    {
        manager.HighlightSelectedObjective(objective);
        manager.objectiveMessage.panel.SetActive(false);
        if (objective.revealed)
        {
            manager.currentObjectivePanel.panel.SetActive(false);
            manager.Display(objective);
        }
        else
        {
            objective.onMap = true;
            manager.currentObjectivePanel.panel.SetActive(true);
            manager.currentObjectivePanel.text.text = text.text;
            Vector2 objectivePos = USNGGrid.ToUSNG(objective.transform.position);
            manager.objectiveLocationPanel.text.text = "Located at: " + (int)objectivePos.x + ", " + (int)objectivePos.y;
            manager.FocusOn(new Vector2(0, 0), 60);
        }
    }

    public void Close()
    {
        if (objective.objectiveState != ObjectiveState.Resolved)
        {
            if (!objective.needsResponse)
            {
                manager.manager.score += objective.originalScore;
            }

            manager.objectiveMessage.panel.SetActive(false);

            if (objective.needsResponse && objective.status >= 1)
            {
                manager.manager.score += objective.score;
            }

            manager.ignoredObjectivesActual++;
        }

        manager.currentObjectivePanel.panel.SetActive(false);

        Destroy(objective.gameObject);
        Destroy(gameObject);
        Destroy(objective.icon);
    }

    public void FocusOnObjective()
    {
        gameObject.GetComponent<Image>().color = Color.grey;
        Vector3 objectivePos = objective.transform.position;
        manager.selectedObjective = objective;
        manager.FocusOn(new Vector2(objectivePos.x, objectivePos.z), 20);
    }
}

// http://answers.unity.com/answers/1610964/view.html
/// <summary>
/// Utility class to allow for easier manipulation of rectTransforms.
/// Found on unity forums.
/// Written by: https://answers.unity.com/users/546375/eldoir.html
/// </summary>
public static class RectTransformExtensions
{
    public static void SetLeft(this RectTransform rt, float left)
    {
        rt.offsetMin = new Vector2(left, rt.offsetMin.y);
    }

    public static void SetRight(this RectTransform rt, float right)
    {
        rt.offsetMax = new Vector2(-right, rt.offsetMax.y);
    }

    public static void SetTop(this RectTransform rt, float top)
    {
        rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
    }

    public static void SetBottom(this RectTransform rt, float bottom)
    {
        rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);
    }
}