using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
   public static GameManager Instance { get; private set; }//单例模式

    public event EventHandler<OnClickedOnGridPositionEventArgs> OnClickedOnGridPosition;//事件分为发布者和订阅者，
                                                                                        //相当于订阅者不需要看发布者有没有这项服务，可以自己提供，
                                                                                        //发布者只是触发订阅者自己的服务
                                                                                        //游戏中的应用为：就是说如果我想创造一个不会死亡的角色，我自己创建一个角色然后在这个角色中添加一个不会死订阅事件，就可以了，也不用改总player的脚本，移除这个角色这个不会死的事件也消失了
    public class OnClickedOnGridPositionEventArgs : EventArgs {//发布者服务自带的一些参数
        public int x;
        public int y;

    }

    private void Awake()
   {    
        if(Instance!=null)
        {
            Debug.LogError("错误");
        }
        Instance = this;
   }
   public void ClickedOnGridPosition(int x,int y)//触发事件
   {
        Debug.Log("ClickedOnGridPosition"+x+","+y);
        OnClickedOnGridPosition?.Invoke(this,new OnClickedOnGridPositionEventArgs{
            x = x,
            y = y,
        });
   }
}
