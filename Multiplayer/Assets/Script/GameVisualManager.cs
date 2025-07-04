using Unity.Netcode;
using UnityEngine;

public class GameVisualManager : NetworkBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Transform crossPrefab;
    [SerializeField] private Transform circlePrefab;
    private const float GRID_SIZE = 3.5f;
   
    private void Start()//����
    {
        GameManager.Instance.OnClickedOnGridPosition += GameManager_OnClickedOnGridPositon;//�����߸���������ӵķ���
    }
    private void GameManager_OnClickedOnGridPositon(object sender,GameManager.OnClickedOnGridPositionEventArgs e)//
    {
        Debug.Log("GameManager_OnClickedOnGridPositon");
        SpawnObjectRpc(e.x, e.y);

    }
    [Rpc(SendTo.Server)]//�ͻ��� �� ��������ָ������
    private void SpawnObjectRpc(int x, int y)
    {
        Debug.Log("SpawnObjectRpc");
        Transform spawnedCrossTransform = Instantiate(crossPrefab, GetGridWorldPosition(x,y),Quaternion.identity);//Instantiate������ ������ճ���� ������������������ʱ����Ԥ���壨Prefab�������ж���Ŀ�¡��
        spawnedCrossTransform.GetComponent<NetworkObject>().Spawn(true);//���߷��������Դ�����ͻ��˿�
      

    }


    private Vector2 GetGridWorldPosition(int x,int y)
    {
        return new Vector2(-GRID_SIZE + x * GRID_SIZE, -GRID_SIZE + y * GRID_SIZE);
    }
}
