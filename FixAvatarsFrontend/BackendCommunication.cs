using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaiWuRPC;

namespace LockAvatarsFrontend
{
    class BackendCommunication
    {

        public static string checkReplaceType(int charId)
        {
            if (client == null)
            {
                client = TWRPCClient.CreateStream("LockAvatarsFrontendTWRPCClient");
            }
            // maybe put a LRU cache here someday...
            return (string) client.CallMethod(typeName, "checkReplaceTypeBE", charId);
        }


        private static TWRPCClient client = null;
        private static string typeName = 
            "LockAvatarsBackend.FrontendCommunication, " +
            "LockAvatarsBackend, " +
            "Version = 1.0.0.0, " +
            "Culture = neutral, " +
            "PublicKeyToken = null";

    }
}
