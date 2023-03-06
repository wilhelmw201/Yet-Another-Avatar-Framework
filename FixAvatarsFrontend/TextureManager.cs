using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using XLua.Cast;
using Random = System.Random;

namespace LockAvatarsFrontend
{

    class TextureManager
    {

        public static int CvtAvatarSizeToIdx(UICommon.Character.Avatar.AvatarSize size)
        {
            if (size == UICommon.Character.Avatar.AvatarSize.Big)
            {
                return 0;
            }
            else if (size == UICommon.Character.Avatar.AvatarSize.Normal)
            {
                return 1;
            }
            else return 2;
        }

        public static void AddTexture(string name, string storagePath, 
            List<String> allowedToons, bool isMale, int offsetX, int offsetY)
        {
            TextureEntry en = new TextureEntry(name, storagePath);
            en.offsetX = offsetX;
            en.offsetY = offsetY;
            if (isMale)
            {
                freeAvatarsMale.Add(en);
            }
            else
            {
                freeAvatarsFemale.Add(en);
            }
        }

        public static Texture2D RetrieveFreeTexture(int seed, bool isMale, UICommon.Character.Avatar.AvatarSize size)
        {
            List<TextureEntry> source;
            if (isMale)
            {
                source = freeAvatarsMale;
            }
            else
            {
                source = freeAvatarsFemale;
            }
            if (source.Count == 0)
            {
                return null;
            }

            Random random = new Random(seed);
            int idx = random.Next() % source.Count;
            TextureEntry entry = source[idx];
            if (entry.texture == null)
            {
                throw new InvalidOperationException(String.Format("加载失败:{0}@{1}.", entry.name, entry.storagePath));
            }
            return entry.texture[CvtAvatarSizeToIdx(size)];
            

        }
        public static Texture2D RetrieveCharSpecificTexture(int seed, bool isMale, UICommon.Character.Avatar.AvatarSize size, string spec)
        {
            // not implemented 
            return RetrieveFreeTexture(seed, isMale, size);
        }

        public static int GetUniqueId()
        {
            return curr_id++;
        }

        public static int GetAvatarDictSize(string specifier, bool isMale)
        {
            if (isMale)
            {
                return freeAvatarsMale.Count;
            }
            return freeAvatarsFemale.Count;
        }

        static List<TextureEntry> freeAvatarsMale = new List<TextureEntry> { };
        static List<TextureEntry> freeAvatarsFemale = new List<TextureEntry> { };
        static Dictionary<string, List<TextureEntry>> specificAvatarsMale;
        static Dictionary<string, List<TextureEntry>> specificAvatarsFemale;
        static int curr_id = 0;

    }

    class TextureEntry
    {
        public int id { get; set; }
        public string name { get; set; }
        public string storagePath { get; set; }

        public int offsetX { get; set; }
        public int offsetY { get; set; }
        public List<Texture2D> texture 
        { 
            get 
            {
                if (_texture == null)
                {
                    _texture = AssetLoader.ReallyLoadTexture(this);
                }
                return _texture; 
            }
            set
            {
                _texture = value;
            }
        }
        private List<Texture2D> _texture;
        public TextureEntry(string name, string storagePath, Texture2D textureBig, Texture2D textureMid, Texture2D textureSmall)
        {
            this.id =  TextureManager. GetUniqueId();
            this.name = name;
            this.storagePath = storagePath;
            this.texture = new List<Texture2D>(3);

            this.texture[0] = textureBig;
            this.texture[1] = textureMid;
            this.texture[2] = textureSmall;

            this.offsetX = 0;
            this.offsetY = 0;
        }
        public TextureEntry(string name, string storagePath)
        {
            this.id = TextureManager.GetUniqueId();
            this.name = name;
            this.storagePath = storagePath;
            this.texture = null;
            this.offsetX = 0;
            this.offsetY = 0;
        }


    }

    class TypeEntry
    {
        public int id { get; set; }
        public string name { get; set; }

        public List<int> allowedTextures; 
    }




}
