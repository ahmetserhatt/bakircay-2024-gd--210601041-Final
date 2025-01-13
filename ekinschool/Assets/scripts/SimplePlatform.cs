using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SimplePlatform : MonoBehaviour
{
    public DraggableObject CurrentFruit;
    public int Score = 0;
    public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI FruitsCountsText;
    public Transform Fruits;
    public GameObject ComplatePanel;
    public ParticleSystem matchEffect;
    private bool isDoubleScoreActive = false;
    private string buttonText = "2X";
    private Color buttonColor = Color.white;

    public static int TotalScore = 0;
    private int matchCount = 0;
    public int totalPairs = 7;

    private void Start()
    {
        // **Skoru geri yükle**
        Score = TotalScore;
        ScoreText.text = "Score: " + Score;
        FruitsCountsText.text = "Fruit Count: " + Fruits.childCount;
    }

    public void ResetGame()
    {
        // **Tüm değişkenleri sıfırla**
        Score = 0;
        TotalScore = 0; // **Global skoru da sıfırla**
        matchCount = 0; // **Eşleşme sayısını sıfırla**

        // **UI Güncelle**
        ScoreText.text = "Score: " + Score;

        // **Sahneyi tamamen sıfırla**
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


    public void PlayMatchEffect(Vector3 position)
    {
        if (matchEffect != null)
        {
            ParticleSystem effectInstance = Instantiate(matchEffect, position, Quaternion.identity);
            effectInstance.Play();
            Destroy(effectInstance.gameObject, 1f);
        }
    }

    public void UpdateScore(int amount)
    {
        if (isDoubleScoreActive)
        {
            amount *= 2;
            isDoubleScoreActive = false;
            buttonText = "2X";
            buttonColor = Color.white;
        }

        Score += amount;
        ScoreText.text = "Score: " + Score;
    }

    public void IncreaseMatchCount()
    {
        matchCount++;

        if (matchCount == totalPairs)
        {
            TotalScore = Score;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private void Update()
    {
        if (Fruits.childCount == 0)
        {
            TotalScore = Score;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private void OnGUI()
    {
        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.fontSize = 30;

        Color originalColor = GUI.backgroundColor;
        GUI.backgroundColor = buttonColor;

        if (GUI.Button(new Rect(50, 50, 200, 100), buttonText, buttonStyle))
        {
            isDoubleScoreActive = true;
            buttonText = "Active!";
            buttonColor = Color.green;
            Debug.Log("🔥 2X Skor Aktif!");
        }

        GUI.backgroundColor = originalColor;
    }
}
