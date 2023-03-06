using Config;
using GameData.Domains.TaiwuEvent.EventHelper;
using GameData.Utilities;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TaiwuModdingLib.Core.Plugin;

namespace LockAvatarsBackend
{
    [PluginConfig("LockAvatarsBackendPlugin", "Wilhelmw", "0.1")]

    public class LockAvatarsBackendPlugin : TaiwuRemakePlugin
    {
        Harmony harmony;

        public override void Dispose()
        {
        }

        public override void Initialize()
        {
            AdaptableLog.Info("load LockAvatarsBackendPlugin");
            this.harmony = Harmony.CreateAndPatchAll(typeof(LockAvatarsBackendPlugin), null);

        }
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(EventHelper), "CreateNonIntelligentCharacter")]
        public static void CreateNonIntelligentCharacter_Post(ref int __result)
        {
            AdaptableLog.Info("In Post CNIC");
            // object[] __args seems to not work and breaks game.
            short templateId = EventHelper.GetCharacterById(__result).GetTemplateId();
            CharacterItem characterCfg = Config.Character.Instance[templateId];
            byte creatingType = characterCfg.CreatingType;
            if (creatingType != 2 || templateId == 477)
            {
                return;
            }

            var created = EventHelper.GetCharacterById(__result);
            var gender = created.GetGender();
            string name;
            if (gender == 0)
            {
                name = "F";
            }
            else
            {
                name = "M";
            }

            SetLockedAvatar(__result, name, false);
        }

        public static void SetLockedAvatar(int charID, string name, bool persistent)
        {
            /*
             * Use this function to lock the portrait of a character.
             * (TODO) implement persistent (write to save file)
             * 
             * name:    "" for no replacement
             *          "M" "F" for any male/female replacement ("hashed" from charID through the RNG generator)
             *          (TODO) F/M + category e.g. "FNpcFace_jinhuanger", "MNpcFace_dadao" for selection from a specific category
             *          (TODO) X + folder: e.g. "XYour_Name_Here", Force Display of a specific portrait (look up that folder)
             * */
            if (name == null || name == "")
            {
                AdaptableLog.Info(String.Format("移除Avatar {0}", charID));
                _lockAvatars.Remove(charID);
            }
            else
            {
                AdaptableLog.Info(String.Format("新增Avatar {0}:{1}", charID, name));
                _lockAvatars.Add(charID, name);
            }
        }

        public static string GetLockedAvatar(int charID)
        {
            string val = _lockAvatars.TryGetValue(charID, out var result) ? result : string.Empty;
            /*if (!String.IsNullOrEmpty(val))
            {
                AdaptableLog.Info(String.Format("返回Avatar {0}:{1}", charID, val));
            }*/
            return val;
        }

        private static Dictionary<int, string> _lockAvatars = new Dictionary<int, string>();

    }
}
