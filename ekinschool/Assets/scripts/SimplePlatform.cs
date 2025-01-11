using System;
using TMPro;
using UnityEngine;

public class SimplePlatform : MonoBehaviour
{
   // public Transform anchorPoint; // Platform �zerindeki sabit nokta
    public DraggableObject CurrentFruit;
    public int Score = 0;
    public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI FruitsCountsText;
    public Transform Fruits;
    public GameObject ComplatePanel;

    private void Start()
    {
        FruitsCountsText.text ="Fruit Count: "+ Fruits.childCount;
    }
}
