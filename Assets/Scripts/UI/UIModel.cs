using System.Collections.Generic;
using DTOs;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UIModel : ScriptableObject
{
    public ListManager listManager;
    public PointsManagerMenu pointsMenu;

    public Transform contentParent;
    public GameObject listItemPrefab;

    public GameObject SelectedItem { get; private set; }

    public HandleInput InputHandler;
    
    public Button playPauseButton;
    public Sprite playIcon;
    public Sprite pauseIcon;
    
    public void AddPoint(PointDTO data)
    {
        var inst = Instantiate(listItemPrefab, contentParent);

        var text = inst.GetComponentInChildren<TMP_Text>();
        text.text = data.Name;
        var button = inst.GetComponentInChildren<Button>();

        var listData = inst.GetComponent<ListItem>();
        listData.Data = data;

        if (button != null)
        {
            button.onClick.AddListener(() =>
            {
                var img = inst.transform.Find("Highlight");
                var state = img.gameObject.activeSelf;
                UnselectAll();
                img.gameObject.SetActive(!state);

                if (!state)
                {
                    SelectedItem = inst;
                    pointsMenu.PointSelected();
                    InputHandler.SetValues(data.Name, data.Time);
                }
                else
                {
                    SelectedItem = null;
                    pointsMenu.PointsUnselected();
                    InputHandler.SetNonInteractable();
                }
            });
        }
        inst.SetActive(true);
    }

    public void InitPointsList(List<PointDTO> data)
    {
        InputHandler.SetNonInteractable();
        pointsMenu.PointsUnselected();

        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);    
        }
        
        foreach (var item in data)
        {
            AddPoint(item);
        }
    }

    private void UnselectAll()
    {
        foreach (Transform child in contentParent)
        {
            child.Find("Highlight").gameObject.SetActive(false);
        }
    }

    public void RemovePoint(int id)
    {
        foreach (Transform child in contentParent)
        {
            if (child.gameObject.GetComponent<ListItem>().Data.Id == id)
            {
                pointsMenu.PointsUnselected();
                InputHandler.SetNonInteractable();
                Destroy(child.gameObject);
                UpdateLayout();
                return;
            }
        }
    }

    public void ChangePoint(int id, string name, float time)
    {
        foreach (Transform child in contentParent)
        {
            if (child.gameObject.GetComponent<ListItem>().Data.Id == id)
            {
                child.gameObject.GetComponentInChildren<TMP_Text>().text = name;
                return;
            }
        }
    }

    private void UpdateLayout()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentParent.GetComponent<RectTransform>());
    }

    public void MovePointUp()
    {
        SelectedItem.transform.SetSiblingIndex(SelectedItem.transform.GetSiblingIndex() - 1);
        UpdateLayout();
    }

    public void MovePointDown()
    {
        SelectedItem.transform.SetSiblingIndex(SelectedItem.transform.GetSiblingIndex() + 1);
        UpdateLayout();
    }

    public void SetPause()
    {
        playPauseButton.image.sprite = pauseIcon;
    }

    public void SetPlay()
    {
        playPauseButton.image.sprite = playIcon;
    }
}