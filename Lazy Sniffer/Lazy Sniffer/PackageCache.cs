using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpPcap;
using PacketDotNet;
using SharpPcap.LibPcap;
using SharpPcap.WinPcap;

namespace Lazy_Sniffer
{
    public class PackageCache
    {
        Queue<RawCapture> qPackage = new Queue<RawCapture>();
        /// <summary>
        /// 入队
        /// </summary>
        /// <param name="package"></param>
        public void AddItem(RawCapture package)
        {
            qPackage.Enqueue(package);
        }
        /// <summary>
        /// 移出队列首元素
        /// </summary>
        /// <returns></returns>
        public RawCapture GetAndRemoveFirst()
        {
            if (qPackage.Count > 0)
            {
                return qPackage.Dequeue();
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 获取队列首元素
        /// </summary>
        /// <returns></returns>
        public RawCapture GetFirst()
        {
            if (qPackage.Count > 0)
            {
                return qPackage.Peek();
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 获取元素个数
        /// </summary>
        /// <returns></returns>
        public int GetCount()
        {
            return qPackage.Count;
        }
        /// <summary>
        /// 清空队列
        /// </summary>
        public void Clear()
        {
            qPackage.Clear();
        }
    }
}
