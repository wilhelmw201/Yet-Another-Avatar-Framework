using System;

namespace LockAvatarsBackend
{
    public class FrontendCommunication
    {
        public static string checkReplaceTypeBE(int charId)
        {
            /**
             * returned value see SetLockedAvatar
             * 
             */
            return LockAvatarsBackendPlugin.GetLockedAvatar(charId);
        }
    }
}
