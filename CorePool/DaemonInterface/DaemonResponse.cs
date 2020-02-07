using CorePool.Configuration;
using CorePool.JsonRpc;

namespace CorePool.DaemonInterface
{
    public class DaemonResponse<T>
    {
        public JsonRpcException Error { get; set; }
        public T Response { get; set; }
        public AuthenticatedNetworkEndpointConfig Instance { get; set; }
    }
}
