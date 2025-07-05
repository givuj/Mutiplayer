using System;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
   public static GameManager Instance { get; private set; }//单例模式

    public event EventHandler<OnClickedOnGridPositionEventArgs> OnClickedOnGridPosition;//事件分为发布者和订阅者，
                                                                                        //相当于订阅者不需要看发布者有没有这项服务，可以自己提供，
                                                                                        //发布者只是触发订阅者自己的服务
                                                                                        //游戏中的应用为：就是说如果我想创造一个不会死亡的角色，我自己创建一个角色然后在这个角色中添加一个不会死订阅事件，就可以了，也不用改总player的脚本，移除这个角色这个不会死的事件也消失了
    public class OnClickedOnGridPositionEventArgs : EventArgs {//发布者服务自带的一些参数
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
            Debug.LogError("错误");
        }
        Instance = this;
   }

    public override void OnNetworkSpawn()
    {
        Debug.Log("OnNetworkSpawn"+NetworkManager.Singleton.LocalClientId);//服务器点击为0，客户端为1,从而使得看谁使用O，谁使用X
        if(NetworkManager.Singleton.LocalClientId ==0)
        {
            localPlayerType = PlayerType.Cross;
        }
        else
        {
            localPlayerType = PlayerType.Circle; 
        }
        if (IsServer)//判断是不是服务器
        {
            currentPlayablePlayerType = PlayerType.Cross;
        }
    }

    [Rpc(SendTo.Server)]
    public void ClickedOnGridPositionRpc(int x,int y,PlayerType playerType)//1. 触发事件
                                                                           //2.同时也是判断客户端和服务器一人只能下一步棋，因为客户端和服务器中的localPlayerType和currentPlayablePlayerType不同
    {
        Debug.Log("ClickedOnGridPosition"+x+","+y);
        if(playerType != currentPlayablePlayerType)//这个判断语句让服务器和客户端每个人只能下一步
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
