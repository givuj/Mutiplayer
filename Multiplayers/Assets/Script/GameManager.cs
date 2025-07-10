using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }//����ģʽ

    public event EventHandler<OnClickedOnGridPositionEventArgs> OnClickedOnGridPosition;//�¼���Ϊ�����ߺͶ����ߣ�
                                                                                        //�൱�ڶ����߲���Ҫ����������û��������񣬿����Լ��ṩ��
                                                                                        //������ֻ�Ǵ����������Լ��ķ���
                                                                                        //��Ϸ�е�Ӧ��Ϊ������˵������봴��һ�����������Ľ�ɫ�����Լ�����һ����ɫȻ���������ɫ�����һ�������������¼����Ϳ����ˣ�Ҳ���ø���player�Ľű����Ƴ������ɫ������������¼�Ҳ��ʧ��
    public class OnClickedOnGridPositionEventArgs : EventArgs {//�����߷����Դ���һЩ����,���Դ���������
        public int x;
        public int y;
        public PlayerType playerType;
    }
    public event EventHandler OnGameStarted;//������
    public event EventHandler<OnGameWinEventArgs> OnGameWin;//������
    public class OnGameWinEventArgs:EventArgs//������
    {
        public Line line;
    }
    public event EventHandler OnCurrentPlayablePlayerTypeChanged;//������
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



    private PlayerType localPlayerType;//����Ƿ������Ϳͻ���Ӧ�÷ŵ�ȦȦ���ǲ��
    private NetworkVariable<PlayerType> currentPlayablePlayerType = new NetworkVariable<PlayerType>();//ֻ�з�������д�뵫�ǣ��ͻ��˿��Զ�ȡ
    private PlayerType[,] PlayerTypeArray;
    private List<Line> lineList;
    private void Awake()
   {    
        if(Instance!=null)
        {
            Debug.LogError("����");
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

    public override void OnNetworkSpawn()//�Զ���ʼʱ�ᴥ��
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
           
            NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;//�ͻ�������ʱ��һЩ����
        }
        currentPlayablePlayerType.OnValueChanged += (PlayerType oldPlayerType, PlayerType newPlayerType) =>//������û�����ı�, ��Ҫ�������ı��ͷ��
        {
              OnCurrentPlayablePlayerTypeChanged?.Invoke(this, EventArgs.Empty);
        };
    }
    private void NetworkManager_OnClientConnectedCallback(ulong obj)
    {
       
        if (NetworkManager.Singleton.ConnectedClientsList.Count==2)//�����м�����������
        {


            currentPlayablePlayerType.Value = PlayerType.Cross;
            TriggerOnGameStartedRpc();
        }
    }
    [Rpc(SendTo.ClientsAndHost)]//���������ܺ��ڴ����ͻ���
    private void TriggerOnGameStartedRpc() {
        OnGameStarted?.Invoke(this, EventArgs.Empty);
    }
    [Rpc(SendTo.Server)]//���ֻ�����ڷ������ϣ��ͻ��˵�playerTypee��û�иı䣬ֻ�Ǻ����ú����ѽ�����˿ͻ���
    public void ClickedOnGridPositionRpc(int x,int y,PlayerType playerType)//1. �����¼�
                                                                           //2.ͬʱҲ���жϿͻ��˺ͷ�����һ��ֻ����һ���壬��Ϊ�ͻ��˺ͷ������е�localPlayerType��currentPlayablePlayerType��ͬ
    {
        Debug.Log("ClickedOnGridPosition"+x+","+y);
        if(playerType != currentPlayablePlayerType.Value)//����ж�����÷������Ϳͻ���ÿ����ֻ����һ��
        {
            return;
        }

        if(PlayerTypeArray[x,y]!=PlayerType.None)//ÿһ������ֻ����һ��
        {
            return;
        }
        PlayerTypeArray[x, y] = playerType;
        // �����¼�
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
