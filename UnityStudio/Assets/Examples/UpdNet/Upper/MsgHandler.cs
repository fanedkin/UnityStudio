/*
 * 网络以及同步管理
 * 
 * TODO 检查
 * ServerMonsterNetSyncController
 * OtherMonsterNetSyncController
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRNetLibrary;

public class MsgHandler : Singleton<MsgHandler>
{
    #region 内部属性
    /// <summary>
    /// 客户端协议处理
    /// </summary>
    private IMessageHandle mClientHandle;

    /// <summary>
    /// 服务器协议处理
    /// </summary>
    private IMessageHandle mHostHandle;

    /// <summary>
    /// 重连间隔
    /// </summary>
    const float RECONNTION_INTERVAL = 1f;
    #endregion

    #region 生命周期
    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {
        try
        {
            //协议工具初始化
            Protocol.Instance.init(new FlatbuffersUtils());

            //实例化客户端协议处理类
            mClientHandle = new ClientHandleMessage();

            //if (SettingManager.Instance.IsServer)
            //{
                //初始化客户端
                ClientManager.Instance.Init(
                    NetType.Local,                          //本地模式客户端
                    NetARQType.None,                        //ARQ模式
                    mClientHandle                           //注册客户端消息处理
                    );

                //实例化服务器协议处理类
                mHostHandle = new HostHandleMessage();

                //if (SettingManager.Instance.IsOffline)  //离线模式
                //{
                    //初始化本地服务器
                    ServersManager.Instance.Init(
                        NetType.Local,                              //本地模式服务器
                        NetARQType.None,                            //ARQ模式
                        mHostHandle                               //注册客户端消息处理
                        //SettingManager.Instance.ClientIP,          //本地客户端监听IP
                        //SettingManager.Instance.ClientPort          //本地客户端监听端口
                        );

                    Debug.Log("<color=#ffa500ff>当前为Host模式(Offline)!!!</color>");
                //}
                //else
                {
                    //初始化远程服务器
                    ServersManager.Instance.Init(
                        NetType.Udp,                                //udp模式服务器
                        NetARQType.None,                            //ARQ模式
                        mHostHandle                              //注册服务器消息处理
                        //SettingManager.Instance.ServerIP,           //服务器监听IP
                        //SettingManager.Instance.ServerPort,         //服务器监听端口
                        //(object)SettingManager.Instance.ClientIPs,  //客户端ip列表，用于发送与校验
                        //(object)SettingManager.Instance.ClientPorts //客户端port列表，用于发送与校验
                        );

                    Debug.Log("<color=#ffa500ff>当前为Host模式(Online)!!!</color>");
                }
            //}
            //else             //初始化纯客户端
            {
                ClientManager.Instance.Init(
                    NetType.Udp,                        //udp模式客户端
                    NetARQType.None,                    //ARQ模式
                    mClientHandle                      //注册客户端消息处理
                    //SettingManager.Instance.ClientIP,   //客户端监听ip
                    //SettingManager.Instance.ClientPort, //客户端监听端口
                    //SettingManager.Instance.ServerIP,   //服务器ip，用于发送与校验
                    //SettingManager.Instance.ServerPort  //服务器端口，用于发送与校验              
                    );

                Debug.Log("<color=#ffa500ff>启动客户端模式!!!</color>");
            }

            //初始化网络状态处理
            //GameWorld.GlobalNetworkStatus = new NetworkStatusHandle();
            //GameWorld.GlobalNetworkStatus.Init(RECONNTION_INTERVAL);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("MsgHandler.init 错误!!!");
            Debug.LogException(ex);
        }
    }

    /// <summary>
    /// 注销
    /// </summary>
    public void UnInit()
    {
        try
        {
            //if (SettingManager.Instance.IsServer)
            //{
            //    HostSendMessage.BroadcastDisconnectToHost();
            //}
            //else
            //{
            //    ClientSendMessage.SendDisConnectToHost();
            //}

            ClientManager.Instance.OnDisable();
            ServersManager.Instance.OnDisable();
        }
        catch (System.Exception ex)
        {
            Debug.LogError("MsgHandler.UnInit 错误!!!");
            Debug.LogException(ex);
        }
    }

    /// <summary>
    /// 更新
    /// </summary>
    public void Update()
    {
        ClientManager.Instance.OnUpdate();
        ServersManager.Instance.OnUpdate();
        //GameWorld.GlobalNetworkStatus.Update(WTime.deltaTime);
    }
    #endregion 生命周期
}
