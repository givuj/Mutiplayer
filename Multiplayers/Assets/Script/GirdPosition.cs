using UnityEngine;
using UnityEngine.EventSystems;

public class GirdPosition : MonoBehaviour
{
    [SerializeField] private int x;
    [SerializeField] private int y;
    public static bool isGridInteractable = false;//�赲UI�������Ϸ�����߼�����
    private void OnMouseDown()
    {
        if (!isGridInteractable) return;
        GameManager.Instance.ClickedOnGridPositionRpc(x,y,GameManager.Instance.GetLocalPlayerType());
    }
    
}
