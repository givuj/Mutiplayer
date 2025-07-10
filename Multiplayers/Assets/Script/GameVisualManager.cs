using Unity.Netcode;
using UnityEngine;

public class GameVisualManager : NetworkBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Transform crossPrefab;
    [SerializeField] private Transform circlePrefab;
    [SerializeField] private Transform lineCompletePrefab;
    private const float GRID_SIZE = 3.5f;
    private const float GRID_SIZE2 = 3.5f;

    
    private void Start()//����
    {
        GameManager.Instance.OnClickedOnGridPosition += GameManager_OnClickedOnGridPositon;//�����߸���������ӵķ���
        GameManager.Instance.OnGameWin += GameManager_OnGameWin;

      
    }

    private void GameManager_OnGameWin(object sender, GameManager.OnGameWinEventArgs e)
    {
        Transform lineCompleteTransform =
            Instantiate(lineCompletePrefab,GetGridWorldPosition2(e.line.centerGridPosition.x, e.line.centerGridPosition.y), Quaternion.identity);
        lineCompleteTransform.GetComponent<NetworkObject>().Spawn(true);

    }
    private void GameManager_OnClickedOnGridPositon(object sender,GameManager.OnClickedOnGridPositionEventArgs e)//
    {
        
        SpawnObjectRpc(e.x, e.y,e.playerType);

    }
    [Rpc(SendTo.Server)]//�ͻ��� �� ��������ָ������
    private void SpawnObjectRpc(int x, int y,GameManager.PlayerType playerType)
    {
       
        Transform prefab;
       
        switch (playerType)
        {
            default:
            case GameManager.PlayerType.Cross:
                prefab = crossPrefab;
                break;
            case GameManager.PlayerType.Circle:
                prefab = circlePrefab;
                break;
        }
        Transform spawnedCrossTransform = Instantiate(prefab, GetGridWorldPosition(x,y),Quaternion.identity);//Instantiate������ ������ճ���� ������������������ʱ����Ԥ���壨Prefab�������ж���Ŀ�¡��
        spawnedCrossTransform.GetComponent<NetworkObject>().Spawn(true);//���߷��������Դ�����ͻ��˿�,ͬ�����ͻ���
      

    }


    private Vector2 GetGridWorldPosition(int x,int y)
    {
        return new Vector2(-GRID_SIZE + x * GRID_SIZE, -GRID_SIZE + y * GRID_SIZE);
    }

    private Vector2 GetGridWorldPosition2(int x, int y)
    {
        return new Vector2( x * GRID_SIZE2, -4 + y * 4);
    }
}
