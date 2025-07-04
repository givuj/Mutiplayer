using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
   public static GameManager Instance { get; private set; }//����ģʽ

    public event EventHandler<OnClickedOnGridPositionEventArgs> OnClickedOnGridPosition;//�¼���Ϊ�����ߺͶ����ߣ�
                                                                                        //�൱�ڶ����߲���Ҫ����������û��������񣬿����Լ��ṩ��
                                                                                        //������ֻ�Ǵ����������Լ��ķ���
                                                                                        //��Ϸ�е�Ӧ��Ϊ������˵������봴��һ�����������Ľ�ɫ�����Լ�����һ����ɫȻ���������ɫ�����һ�������������¼����Ϳ����ˣ�Ҳ���ø���player�Ľű����Ƴ������ɫ������������¼�Ҳ��ʧ��
    public class OnClickedOnGridPositionEventArgs : EventArgs {//�����߷����Դ���һЩ����
        public int x;
        public int y;

    }

    private void Awake()
   {    
        if(Instance!=null)
        {
            Debug.LogError("����");
        }
        Instance = this;
   }
   public void ClickedOnGridPosition(int x,int y)//�����¼�
   {
        Debug.Log("ClickedOnGridPosition"+x+","+y);
        OnClickedOnGridPosition?.Invoke(this,new OnClickedOnGridPositionEventArgs{
            x = x,
            y = y,
        });
   }
}
