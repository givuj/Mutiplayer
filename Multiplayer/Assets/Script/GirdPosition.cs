using UnityEngine;

public class GirdPosition : MonoBehaviour
{
    [SerializeField] private int x;
    [SerializeField] private int y;
    private void OnMouseDown()
    {
        Debug.Log("click");
        GameManager.Instance.ClickedOnGridPosition(x,y);
    }
    
}
