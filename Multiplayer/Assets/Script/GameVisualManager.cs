using Unity.Netcode;
using UnityEngine;

public class GameVisualManager : NetworkBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Transform crossPrefab;
    [SerializeField] private Transform circlePrefab;
    private const float GRID_SIZE = 3.5f;
   
    private void Start()//订阅
    {
        GameManager.Instance.OnClickedOnGridPosition += GameManager_OnClickedOnGridPositon;//订阅者给发布者添加的服务
    }
    private void GameManager_OnClickedOnGridPositon(object sender,GameManager.OnClickedOnGridPositionEventArgs e)//
    {
        Debug.Log("GameManager_OnClickedOnGridPositon");
        SpawnObjectRpc(e.x, e.y);

    }
    [Rpc(SendTo.Server)]//客户端 → 服务器（指令请求）
    private void SpawnObjectRpc(int x, int y)
    {
        Debug.Log("SpawnObjectRpc");
        Transform spawnedCrossTransform = Instantiate(crossPrefab, GetGridWorldPosition(x,y),Quaternion.identity);//Instantiate类似于 “复制粘贴” 操作，允许你在运行时生成预制体（Prefab）或现有对象的克隆。
        spawnedCrossTransform.GetComponent<NetworkObject>().Spawn(true);//告诉服务器可以传输给客户端看
      

    }


    private Vector2 GetGridWorldPosition(int x,int y)
    {
        return new Vector2(-GRID_SIZE + x * GRID_SIZE, -GRID_SIZE + y * GRID_SIZE);
    }
}
