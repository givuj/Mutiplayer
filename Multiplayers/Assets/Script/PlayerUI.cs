using Unity.Netcode;
using UnityEngine;

public class UI : MonoBehaviour
{
    [SerializeField] private GameObject crossArrowGameObject;
    [SerializeField] private GameObject circleArrowGameObject;
    [SerializeField] private GameObject crossYouGameObject;
    [SerializeField] private GameObject circleYouGameObject;
    private void Awake()
    {
        crossArrowGameObject.SetActive(false);
        circleArrowGameObject.SetActive(false);
        crossYouGameObject.SetActive(false);
        circleYouGameObject.SetActive(false);
    }
    private void Start()
    {
        GameManager.Instance.OnGameStarted += GameManager_OnGameStarted;
        GameManager.Instance.OnCurrentPlayablePlayerTypeChanged += GameManager_OnCurrentPlayablePlayerTypeChanged;

    }
    private void GameManager_OnGameStarted(object sender, System.EventArgs e)//you��ʶ��ָ��
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
    private void GameManager_OnCurrentPlayablePlayerTypeChanged(object sender, System.EventArgs e)//��ͷ��ָ��
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
