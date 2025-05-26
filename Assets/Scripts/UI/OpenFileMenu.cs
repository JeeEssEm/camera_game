
using UnityEngine;

public class OpenMenuButton : MonoBehaviour
{
    public GameObject HiddenPanel;

    public void OnButtonPressed()
    {
        HiddenPanel.SetActive(!HiddenPanel.activeSelf);

    }
}
