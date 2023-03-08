using Config;
using GameData.Domains.Mod;
using GameData.Utilities;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using TaiwuModdingLib;
using TaiwuModdingLib.Core.Plugin;
using UnityEngine;
using System.Linq;
using GameData.Domains.Character.Display;
using UICommon.Character.Avatar;
using UICommon.Character;
using CharacterDataMonitor;
using TaiWuRPC;
using XLua.Cast;
using System.Diagnostics;

namespace LockAvatarsFrontend
{
    [PluginConfig("LockAvatarsFrontendPlugin", "Wilhelmw", "0.1")]
    public class LockAvatarsFrontendPlugin : TaiwuRemakePlugin
    {
        Harmony harmony;
        public override void Dispose()
        {
            if (this.harmony != null)
            {
                this.harmony.UnpatchSelf();
            }
        }

        public override void Initialize()
        {
            AdaptableLog.Info("load LockAvatarsFrontendPlugin");
            var sw = Stopwatch.StartNew();
            this.harmony = Harmony.CreateAndPatchAll(typeof(LockAvatarsFrontendPlugin), null);

            AssetLoader.LoadImageInfo();
            AdaptableLog.Info(String.Format("Done, time elapsed: {0}ms", sw.ElapsedMilliseconds));
            sw.Stop();

        }


        [HarmonyPrefix]
        [HarmonyPatch(typeof(UICommon.Character.Avatar.Avatar), "Refresh", new Type[] { typeof(CharacterDisplayData) })]
        public static bool Avatar_Refresh_Prefix1(UICommon.Character.Avatar.Avatar __instance, CharacterDisplayData displayData)
        {
            int charId = displayData.CharacterId;
            var texture = CommunicateAndGetTexture(charId, __instance.Size);
            if (texture == null)
            {
                return true;
            }
            __instance.Refresh(texture);
            return false;
        }


        [HarmonyPrefix]
        [HarmonyPatch(typeof(CharacterAvatar), "FillElement")]
        public static bool CharacterAvatar_FillElement_Prefix(CharacterAvatar __instance, Avatar ____avatar, MonitorDataItemBase ___MonitorDataItem)
        {
            int charId = __instance.CharacterId;

            AvatarInfoMonitor avatarInfoMonitor = ___MonitorDataItem as AvatarInfoMonitor;
            if (avatarInfoMonitor.Character.IsDead && __instance.CanShowGrave)
            {
                return true;
            }
            var texture = CommunicateAndGetTexture(charId, ____avatar.Size);
            if (!texture)
            {
                return true;
            }

            ____avatar.Refresh(texture);
            
            // copied from disassembly...
            bool flag7 = __instance.OnFillAvatar != null;
            if (flag7)
            {
                SingletonObject.getInstance<YieldHelper>().DelayFrameDo(2U, __instance.OnFillAvatar);
            }

            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(NpcAvatarOffset), "GetOffset")]
        public static bool NpcAvatarOffset_GetOffset_Prefix(Vector2 __result, string npcAvatarTextureName)
        {
            if (npcAvatarTextureName.StartsWith("YAAF"))
            {
                string[] subs = npcAvatarTextureName.Split('_');
                __result = new Vector2(int.Parse(subs[1]), int.Parse(subs[2]));
                AdaptableLog.Info(String.Format("Offset:{0},{1}", __result[0], __result[1]));

                return false;
            }
            return true;
            
        }


        private static Texture2D CommunicateAndGetTexture(int charId, AvatarSize size)
        {
            string replaceType = BackendCommunication.checkReplaceType(charId);
            if (replaceType == String.Empty) return null;
            return TextureManager.RetrieveTexture(charId, replaceType, size);
           

        }


    }



}
