using System;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] Button startGameButton;

    private void Start()
    {
        startGameButton.onClick.AddListener(ToggleStartGameButtonOff);
    }

    void ToggleStartGameButtonOn() => startGameButton.gameObject.SetActive(true);
    void ToggleStartGameButtonOff() => startGameButton.gameObject.SetActive(false);

    void OnEnable()
    {
        MapGenerator.onMapGenerated += ToggleStartGameButtonOn;
    }
    void OnDestroy()
    {
        MapGenerator.onMapGenerated -= ToggleStartGameButtonOn;

    }
}