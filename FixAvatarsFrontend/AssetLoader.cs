using GameData.Domains.Mod;
using GameData.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;


namespace LockAvatarsFrontend
{

    // handles boring IO stuff...

    class AssetLoader
    {

        public static void LoadImageInfo()
        {
            foreach (ModId modId in ModManager.EnabledMods)
            {
                ModInfo modInfo = ModManager.GetModInfo(modId);
                string directoryName = modInfo.DirectoryName;
                DirectoryInfo directoryInfo = new DirectoryInfo(directoryName);
                foreach (DirectoryInfo subdirectory in directoryInfo.GetDirectories())
                {
                    if (subdirectory.Name == "YAAFAsset")
                    {
                        AdaptableLog.Info("发现YAAFAsset文件夹：" + directoryInfo.FullName);

                        foreach (DirectoryInfo subsubdirectory in subdirectory.GetDirectories())
                        {
                            try
                            {
                                TryLoadFolderInfo(subsubdirectory);
                                AdaptableLog.Info(String.Format("完成读取图片文件夹，目前有图片：{0}张",
 TextureManager.GetAvatarDictSize()));
                            }
                            catch (Exception e)
                            {
                                AdaptableLog.Info("读取" + directoryInfo.FullName + "时出现了一些错误：" + e.ToString() + "，跳过这个文件夹。");
                            }
                        }
                    }
                }
            }
        }

        public static List<Texture2D> ReallyLoadTexture(TextureEntry entry)
        {

            string FBig = Path.Combine(entry.storagePath, "big.png");
            string FMid = Path.Combine(entry.storagePath, "mid.png");
            string FSmall = Path.Combine(entry.storagePath, "small.png");

            FileInfo infBig = new FileInfo(FBig);
            FileInfo infMid = new FileInfo(FMid);
            FileInfo infSmall = new FileInfo(FSmall);

            Texture2D textureBig = LoadFileAsSize(infBig, 1024, 1024);
            Texture2D textureMid = LoadFileAsSize(infMid, 512, 512);
            Texture2D textureSmall = LoadFileAsSize(infSmall, 256, 256);

            int offx = entry.offsetX;
            int offy = entry.offsetY;

            textureBig.name = String.Format("YAAFB_{1}_{2}_{0}", entry.name, offx, offy);
            textureMid.name = String.Format("YAAFM_{1}_{2}_{0}", entry.name, offx, offy);
            textureSmall.name = String.Format("YAAFS_{1}_{2}_{0}", entry.name, offx, offy);

            return new List<Texture2D> { textureBig, textureMid, textureSmall };
        }
        private static void TryLoadFolderInfo(DirectoryInfo directory)
        {
            // read the YAALAsset folder
            // first determine metadata (male? female?)
            string metaFile = Path.Combine(directory.FullName, "asset.json");
            if (!File.Exists(metaFile))
            {
                Debug.Log(String.Format("找不到asset.json: {0}", directory.FullName));
                return;
            }

            string json = File.ReadAllText(metaFile);
            Dictionary<string, string> configDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

            bool isMale = false;
            if (configDict.ContainsKey("ismale") && configDict["ismale"] != "false")
            {
                isMale = true;
            }

            string name = directory.Name;
            configDict.TryGetValue("name", out name);

            int offsetX = 0;
            int offsetY = 0;
            string[] allowedFor = null;
            if (configDict.TryGetValue("offsetx", out var _offsetX))
            {
                offsetX = int.Parse(_offsetX);
            }
            if (configDict.TryGetValue("offsety", out var _offsetY))
            {
                offsetX = int.Parse(_offsetY);
            }
            if (configDict.TryGetValue("allowed", out var allowed))
            {
                
                allowedFor = allowed.Trim(new char[] { ' ', ','}).Split(',');
                
            }


            string FBig = Path.Combine(directory.FullName, "big.png");
            string FMid = Path.Combine(directory.FullName, "mid.png");
            string FSmall = Path.Combine(directory.FullName, "small.png");
            if (File.Exists(FBig) && File.Exists(FMid) && File.Exists(FSmall))
            {

                if (allowedFor == null)
                {
                    TextureManager.AddTexture(name, directory.FullName, 
                        new string[] { isMale? "M":"F" }, 
                        offsetX, offsetY);
                }
                else
                {
                    TextureManager.AddTexture(name, directory.FullName,
                        allowedFor, offsetX, offsetY);
                }



            }
            else
            {
                Debug.Log(String.Format("找不到图片: {0}: {1},{2},{3}", directory.FullName,
                    File.Exists(FBig), File.Exists(FMid), File.Exists(FSmall)));
            }
        }



        private static Texture2D LoadFileAsSize(FileInfo pngFile, int w, int h)
        {
            // load the PNG file as a byte array
            byte[] bytes = File.ReadAllBytes(pngFile.FullName);
            // create a new Texture2D object with the desired size
            Texture2D texture = new Texture2D(1024, 1024, TextureFormat.ARGB32, false);
            ImageConversion.LoadImage(texture, bytes);
            return texture;
        }



    }
}
