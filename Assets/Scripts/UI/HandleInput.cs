using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HandleInput : MonoBehaviour
{
    [SerializeField] TMP_InputField TimeInput;
    [SerializeField] TMP_InputField NameInput;
    [SerializeField] TMP_Text ValidationText;
    [SerializeField] Button SaveButton;

    public event EventHandler<Tuple<string, float>> OnValidated;

    private void Start()
    {
        ValidationText.enabled = false;

        TimeInput.interactable = false;
        NameInput.interactable = false;
        SaveButton.interactable = false;
    }

    public void ValidateInput()
    {
        var timeText = TimeInput.text;
        var nameText = NameInput.text;

        if (!float.TryParse(timeText, out float time))
        {
            ValidationText.enabled = true;
            ValidationText.text = "Time field must be a float or int!";
            return;
        }
        if (time < 1)
        {
            ValidationText.enabled = true;
            ValidationText.text = "Time must be positive!";
            return;
        }

        if (nameText.Length == 0)
        {
            ValidationText.enabled = true;
            ValidationText.text = "Name of point can not be empty!";
            return;
        }
        ValidationText.enabled = false;

        OnValidated?.Invoke(this, Tuple.Create(nameText, time));
    }

    public void SetValues(string pointName, float pointTime)
    {
        ValidationText.enabled = false;

        TimeInput.interactable = true;
        NameInput.interactable = true;
        SaveButton.interactable = true;

        TimeInput.text = pointTime.ToString();
        NameInput.text = pointName;
    }

    public void SetNonInteractable()
    {
        TimeInput.interactable = false;
        NameInput.interactable = false;
        SaveButton.interactable = false;

        TimeInput.text = "";
        NameInput.text = "";
    }

}
