using TMPro;
using Unity.Netcode;
using UnityEngine;

public class UI : MonoBehaviour
{
    [SerializeField] private GameObject crossArrowGameObject;
    [SerializeField] private GameObject circleArrowGameObject;
    [SerializeField] private GameObject crossYouGameObject;
    [SerializeField] private GameObject circleYouGameObject;
    [SerializeField] private TextMeshProUGUI playerCrossScoreTextMesh;
    [SerializeField] private TextMeshProUGUI playerCircleScoreTextMesh;
    private void Awake()
    {
        crossArrowGameObject.SetActive(false);
        circleArrowGameObject.SetActive(false);
        crossYouGameObject.SetActive(false);
        circleYouGameObject.SetActive(false);
        playerCrossScoreTextMesh.text = "";
        playerCircleScoreTextMesh.text = "";
    }
    private void Start()
    {
        GameManager.Instance.OnGameStarted += GameManager_OnGameStarted;
        GameManager.Instance.OnCurrentPlayablePlayerTypeChanged += GameManager_OnCurrentPlayablePlayerTypeChanged;
        GameManager.Instance.OnGameWin += GameManager_OnGameWin;

    }

    private void GameManager_OnGameWin(object sender, GameManager.OnGameWinEventArgs e)
    {
        GameManager.Instance.GetScores(out int playerCircleScore, out int playerCrossScore);
        playerCrossScoreTextMesh.text = playerCrossScore.ToString();
        playerCircleScoreTextMesh.text = playerCircleScore.ToString();
    }

    private void GameManager_OnGameStarted(object sender, System.EventArgs e)//you标识的指引
    {
        if (GameManager.Instance.GetLocalPlayerType() == GameManager.PlayerType.Cross)
        {
           
            crossYouGameObject.SetActive(true);

        }
        else
        {
            
            circleYouGameObject.SetActive(true);
        }
        UpdateCurrentArrow();
    }
    private void GameManager_OnCurrentPlayablePlayerTypeChanged(object sender, System.EventArgs e)//箭头的指引
    {
        UpdateCurrentArrow();
    }
    
    private void UpdateCurrentArrow()
    {
        if(GameManager.Instance.GetCurrentPlayablePlayerType() == GameManager.PlayerType.Cross)
        {
            crossArrowGameObject.SetActive(true);
            circleArrowGameObject.SetActive(false);
        }
        else
        {
            
            crossArrowGameObject.SetActive(false);
            circleArrowGameObject.SetActive(true);
        }
    }
}
