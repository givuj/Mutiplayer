using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI resultTextMesh;
    [SerializeField] private Color winColor;
    [SerializeField] private Color loseColor;
    [SerializeField] private Color tieColor;
    [SerializeField] private Button rematchButton;
    private void Awake()
    {
        rematchButton.onClick.AddListener(() => {
            GameManager.Instance.RematchRpc();
        });
    }
    private void Start()
    {
        GameManager.Instance.OnGameWin += GameManager_OnGameWin;
        GameManager.Instance.OnRematch += GameManager_OnRematch;
        GameManager.Instance.OnGameTied += Instance_OnGameTied;
        Hide();
        
    }

    private void Instance_OnGameTied(object sender, EventArgs e)
    {
        resultTextMesh.text = "TIE";
        resultTextMesh.color = tieColor;
        Show();
    }

    private void GameManager_OnRematch(object sender, EventArgs e)
    {
        Hide();
    }

    private void GameManager_OnGameWin(object sender, GameManager.OnGameWinEventArgs e)
    {
        if (e.winPlayerType == GameManager.Instance.GetLocalPlayerType())
        {
            resultTextMesh.text = "YOU WIN!";
            resultTextMesh.color = winColor;

        }
        else
        {
            resultTextMesh.text = "YOU LOSE!";
            resultTextMesh.color = loseColor;
        }
        Show();
        Debug.Log(resultTextMesh.text);
    }
    private void Show()
    {
        gameObject.SetActive(true);
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
