using System;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
   public static GameManager Instance { get; private set; }//����ģʽ

    public event EventHandler<OnClickedOnGridPositionEventArgs> OnClickedOnGridPosition;//�¼���Ϊ�����ߺͶ����ߣ�
                                                                                        //�൱�ڶ����߲���Ҫ����������û��������񣬿����Լ��ṩ��
                                                                                        //������ֻ�Ǵ����������Լ��ķ���
                                                                                        //��Ϸ�е�Ӧ��Ϊ������˵������봴��һ�����������Ľ�ɫ�����Լ�����һ����ɫȻ���������ɫ�����һ�������������¼����Ϳ����ˣ�Ҳ���ø���player�Ľű����Ƴ������ɫ������������¼�Ҳ��ʧ��
    public class OnClickedOnGridPositionEventArgs : EventArgs {//�����߷����Դ���һЩ����
        public int x;
        public int y;
        public PlayerType playerType;
    }

    public enum PlayerType
    {
        None,
        Cross,
        Circle,
    }
    private PlayerType localPlayerType;
    private PlayerType currentPlayablePlayerType;

    private void Awake()
   {    
        if(Instance!=null)
        {
            Debug.LogError("����");
        }
        Instance = this;
   }

    public override void OnNetworkSpawn()
    {
        Debug.Log("OnNetworkSpawn"+NetworkManager.Singleton.LocalClientId);//���������Ϊ0���ͻ���Ϊ1,�Ӷ�ʹ�ÿ�˭ʹ��O��˭ʹ��X
        if(NetworkManager.Singleton.LocalClientId ==0)
        {
            localPlayerType = PlayerType.Cross;
        }
        else
        {
            localPlayerType = PlayerType.Circle; 
        }
        if (IsServer)//�ж��ǲ��Ƿ�����
        {
            currentPlayablePlayerType = PlayerType.Cross;
        }
    }

    [Rpc(SendTo.Server)]
    public void ClickedOnGridPositionRpc(int x,int y,PlayerType playerType)//1. �����¼�
                                                                           //2.ͬʱҲ���жϿͻ��˺ͷ�����һ��ֻ����һ���壬��Ϊ�ͻ��˺ͷ������е�localPlayerType��currentPlayablePlayerType��ͬ
    {
        Debug.Log("ClickedOnGridPosition"+x+","+y);
        if(playerType != currentPlayablePlayerType)//����ж�����÷������Ϳͻ���ÿ����ֻ����һ��
        {
            return;
        }
       
        OnClickedOnGridPosition?.Invoke(this, new OnClickedOnGridPositionEventArgs
        {
            x = x,
            y = y,
            playerType = playerType,
        });
       
        switch (currentPlayablePlayerType) 
        {
            default:
            case PlayerType.Cross:
            
                currentPlayablePlayerType = PlayerType.Circle;
                break;
            case PlayerType.Circle:
                
                currentPlayablePlayerType = PlayerType.Cross;
                break;
        }

    }
    public PlayerType GetLocalPlayerType()
   {
        return localPlayerType;
   }
}
