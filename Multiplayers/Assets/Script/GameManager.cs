using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }//单例模式

    public event EventHandler<OnClickedOnGridPositionEventArgs> OnClickedOnGridPosition;//事件分为发布者和订阅者，
                                                                                        //相当于订阅者不需要看发布者有没有这项服务，可以自己提供，
                                                                                        //发布者只是触发订阅者自己的服务
                                                                                        //游戏中的应用为：就是说如果我想创造一个不会死亡的角色，我自己创建一个角色然后在这个角色中添加一个不会死订阅事件，就可以了，也不用改总player的脚本，移除这个角色这个不会死的事件也消失了
    public class OnClickedOnGridPositionEventArgs : EventArgs {//发布者服务自带的一些参数,可以传给订阅者
        public int x;
        public int y;
        public PlayerType playerType;
    }
    public event EventHandler OnGameStarted;//发布者
    public event EventHandler<OnGameWinEventArgs> OnGameWin;//发布者
    public class OnGameWinEventArgs:EventArgs//发布者
    {
        public Line line;
    }
    public event EventHandler OnCurrentPlayablePlayerTypeChanged;//发布者
    public enum PlayerType
    {
        None,
        Cross,
        Circle,
    }
    public enum Orientation
    {
        Horizontal,
        Vertical,
        DiagonalA,
        DiagonalB,
    }
    public struct Line {
        public List<Vector2Int> gridVector2IntList;
        public Vector2Int centerGridPosition;
        public Orientation orientation;
    }



    private PlayerType localPlayerType;//这个是服务器和客户端应该放的圈圈还是叉叉
    private NetworkVariable<PlayerType> currentPlayablePlayerType = new NetworkVariable<PlayerType>();//只有服务器能写入但是，客户端可以读取
    private PlayerType[,] PlayerTypeArray;
    private List<Line> lineList;
    private void Awake()
   {    
        if(Instance!=null)
        {
            Debug.LogError("错误");
        }
        Instance = this;
        PlayerTypeArray = new PlayerType[3,3];
        lineList = new List<Line>
        {
            new Line
            {
                gridVector2IntList = new List<Vector2Int>{new Vector2Int(0,0),new Vector2Int(1,0),new Vector2Int(2,0),},
                centerGridPosition = new Vector2Int(1,0)
                
            },
            new Line
            {
                gridVector2IntList = new List<Vector2Int>{new Vector2Int(0,1),new Vector2Int(1,1),new Vector2Int(2,1),},
                centerGridPosition = new Vector2Int(1,1)
            },
            new Line
            {
                gridVector2IntList = new List<Vector2Int>{new Vector2Int(0,2),new Vector2Int(1,2),new Vector2Int(2,2),},
                centerGridPosition = new Vector2Int(1,2)
            },

            //Vertical
            new Line
            {
                gridVector2IntList = new List<Vector2Int>{new Vector2Int(0,0),new Vector2Int(0,1),new Vector2Int(0,2),},
                centerGridPosition = new Vector2Int(0,1)
            },
            new Line
            {
                gridVector2IntList = new List<Vector2Int>{new Vector2Int(1,0),new Vector2Int(1,1),new Vector2Int(1,2),},
                centerGridPosition = new Vector2Int(1,1)
            },
            new Line
            {
                gridVector2IntList = new List<Vector2Int>{new Vector2Int(2,0),new Vector2Int(2,1),new Vector2Int(2,2),},
                centerGridPosition = new Vector2Int(2,1)
            },

            //Diagonals
            new Line
            {
                gridVector2IntList = new List<Vector2Int>{new Vector2Int(0,0),new Vector2Int(1,1),new Vector2Int(2,2),},
                centerGridPosition = new Vector2Int(1,1)
            },
            new Line
            {
                gridVector2IntList = new List<Vector2Int>{new Vector2Int(0,2),new Vector2Int(2,1),new Vector2Int(2,2),},
                centerGridPosition = new Vector2Int(1,1)
            },
          
        };
   }

    public override void OnNetworkSpawn()//自动开始时会触发
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
           
            NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;//客户端链接时做一些操作
        }
        currentPlayablePlayerType.OnValueChanged += (PlayerType oldPlayerType, PlayerType newPlayerType) =>//监听发没发生改变, 主要是用来改变箭头的
        {
              OnCurrentPlayablePlayerTypeChanged?.Invoke(this, EventArgs.Empty);
        };
    }
    private void NetworkManager_OnClientConnectedCallback(ulong obj)
    {
       
        if (NetworkManager.Singleton.ConnectedClientsList.Count==2)//看看有几个人在链接
        {


            currentPlayablePlayerType.Value = PlayerType.Cross;
            TriggerOnGameStartedRpc();
        }
    }
    [Rpc(SendTo.ClientsAndHost)]//服务器接受后在传给客户端
    private void TriggerOnGameStartedRpc() {
        OnGameStarted?.Invoke(this, EventArgs.Empty);
    }
    [Rpc(SendTo.Server)]//这个只运行在服务器上，客户端的playerTypee并没有改变，只是后面用函数把结果给了客户端
    public void ClickedOnGridPositionRpc(int x,int y,PlayerType playerType)//1. 触发事件
                                                                           //2.同时也是判断客户端和服务器一人只能下一步棋，因为客户端和服务器中的localPlayerType和currentPlayablePlayerType不同
    {
        Debug.Log("ClickedOnGridPosition"+x+","+y);
        if(playerType != currentPlayablePlayerType.Value)//这个判断语句让服务器和客户端每个人只能下一步
        {
            return;
        }

        if(PlayerTypeArray[x,y]!=PlayerType.None)//每一个格子只能下一步
        {
            return;
        }
        PlayerTypeArray[x, y] = playerType;
        // 触发事件
        OnClickedOnGridPosition?.Invoke(this, new OnClickedOnGridPositionEventArgs
        {
            x = x,
            y = y,
            playerType = playerType,
        });
       
        switch (currentPlayablePlayerType.Value) 
        {
            default:
            case PlayerType.Cross:

                currentPlayablePlayerType.Value = PlayerType.Circle;
                break;
            case PlayerType.Circle:
                
                currentPlayablePlayerType.Value = PlayerType.Cross;
                break;
        }
        TestWinner();
    }
    private bool TestWinnerLine(Line line)
    {
        return TestWinnerLine(
            PlayerTypeArray[line.gridVector2IntList[0].x, line.gridVector2IntList[0].y],
            PlayerTypeArray[line.gridVector2IntList[1].x, line.gridVector2IntList[1].y],
            PlayerTypeArray[line.gridVector2IntList[2].x, line.gridVector2IntList[2].y]
            );
    }
    private bool TestWinnerLine(PlayerType aPlayType, PlayerType bPlayType, PlayerType cPlayType)
    {
        return aPlayType != PlayerType.None &&
               aPlayType == bPlayType &&
               bPlayType == cPlayType;
    }
    private void TestWinner()
    {
        foreach(Line line in lineList)
        {
            if (TestWinnerLine(line))
            {
                Debug.Log("win");
                currentPlayablePlayerType.Value = PlayerType.None;
                OnGameWin?.Invoke(this, new OnGameWinEventArgs{
                    line = line
                });
                break;
            }
        }
      
    }
    public PlayerType GetLocalPlayerType()
    {
        return localPlayerType;
    }
    public PlayerType GetCurrentPlayablePlayerType()
    {
        return currentPlayablePlayerType.Value;
    }
}
