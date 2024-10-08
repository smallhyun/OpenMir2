﻿using PluginSystem;
using System.Threading.Tasks;

namespace SocketPlugin
{
    /// <summary>
    /// 具有完成连接动作的插件接口
    /// </summary>
    public interface IConnectedPlugin : IPlugin
    {
        /// <summary>
        /// 客户端连接成功后触发
        /// </summary>
        /// <param name="client">客户端</param>
        /// <param name="e">参数</param>
        [AsyncRaiser]
        void OnConnected(object client, PluginEventArgs e);

        /// <summary>
        /// 客户端连接成功后触发
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        Task OnConnectedAsync(object client, PluginEventArgs e);
    }
}