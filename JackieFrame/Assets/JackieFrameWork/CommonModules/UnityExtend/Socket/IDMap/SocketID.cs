using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fs
{
    public class SocketID
    {
        public const int InitByteArraySize = 32767;
        public const int MaxByteArraySize = 65535;

        public const int PacketHeadSize = 2;

        public const int SendRecvMaxSize = 4096;

    }
    /// <summary>
    /// 网络连接状态
    /// </summary>
    public enum eNetConnectedState
    {
        Create,
        Connecting,
        Connected,
        Closing,
        Closed,
    }
}

/**C#Socket的ErrorCode的说明
OutOfMemory   7   内存不足   
InvalidPropertyValue   380   属性值无效。   
GetNotSupported   394   属性不可读。   
SetNotSupported   383   属性是只读的。   
BadState   40006   所请求的事务或请求本身的错误协议或者错误连接状态。   
InvalidArg 40014 传递给函数的参数格式不确定，或者不在指定范围内。 
Success   40017   成功。   
Unsupported   40018   不受支持的变量类型。   
InvalidOp   40020   在当前状态下的无效操作   
OutOfRange   40021   参数越界。   
WrongProtocol   40026   所请求的事务或请求本身的错误协议   
OpCanceled   1004   取消操作。   
InvalidArgument   10014   所请求的地址是广播地址，但未设置标记。   
WouldBlock   10035   套接字不成块，而指定操作将使之成块。   
InProgress 10036 制造块的 Winsock 操作在进行之中。 
AlreadyComplete   10037   完成操作。未进行制造块的操作。   
NotSocket   10038   描述符不是套接字。   
MsgTooBig   10040   数据报太大，不适于缓冲区的要求，因而被截断。   
PortNotSupported   10043   不支持指定的端口。   
AddressInUse   10048   地址在使用中。   
AddressNotAvailable   10049   来自本地机器的不可用地址。   
NetworkSubsystemFailed   10050   网络子系统失败。   
NetworkUnreachable   10051   此时不能从主机到达网络。   
NetReset   10052   在设置   SO_KEEPALIVE   时连接超时。   
ConnectAborted   11053   由于超时或者其它失败而中止连接。   
ConnectionReset   10054   通过远端重新设置连接。   
NoBufferSpace   10055   没有可用的缓冲空间。   
AlreadyConnected   10056   已连接套接字。   
NotConnected   10057   未连接套接字。   
SocketShutdown   10058   已关闭套接字。   
Timedout   10060   已关闭套接字。   
ConnectionRefused   10061   强行拒绝连接。  

WSAESERVERDOWN (10064) 服务器暂时连接不上

WSAESERVERUNREACH (10065)无法连接服务器可能是你的网络有问题 
NotInitialized   10093   应首先调用   WinsockInit。   
HostNotFound   11001   授权应答：未找到主机。   
HostNotFoundTryAgain   11002   非授权应答：未找到主机。   
NonRecoverableError   11003   不可恢复的错误。   
NoData   11004   无效名，对所请求的类型无数据记录。
*/
