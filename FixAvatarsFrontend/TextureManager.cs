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
            string[] allowedToons, int offsetX, int offsetY)
        {
            TextureEntry en = new TextureEntry(name, storagePath);
            en.offsetX = offsetX;
            en.offsetY = offsetY;

            if (allowedToons != null && allowedToons.Length > 0)
            {

                foreach (string s in allowedToons)
                {
                    List<TextureEntry> entries = DefaultDictGet(textureStorage, s);
                    entries.Add(en);

                }

            }
            else
            {
                throw new ArgumentException("cannot have null allowedToons");
            }


        }
        public static Texture2D _RetrieveTexture(int seed, string replaceType, UICommon.Character.Avatar.AvatarSize size, List<TextureEntry> source)
        {
            if (source.Count == 0 || source == null) return null;
            Random random = new Random(seed);
            int idx = random.Next() % source.Count;
            TextureEntry entry = source[idx];
            if (entry.texture == null)
            {
                throw new InvalidOperationException(String.Format("加载失败:{0}@{1}.", entry.name, entry.storagePath));
            }
            return entry.texture[CvtAvatarSizeToIdx(size)];
        }

        public static Texture2D RetrieveTexture(int seed, string replaceType, UICommon.Character.Avatar.AvatarSize size)
        {
            List<TextureEntry> source = DefaultDictGet(textureStorage, replaceType);
            if (String.IsNullOrEmpty(replaceType)) return null;
            if (source.Count == 0 || source == null)
            {
                if (replaceType[0] == 'M')
                {
                    source = DefaultDictGet(textureStorage, "M");
                }
                else if (replaceType[0] == 'F')
                {
                    source = DefaultDictGet(textureStorage, "F");
                }
                else
                {
                    return null;
                }
            };

            return _RetrieveTexture(seed, replaceType, size, source);
            


            

        }

        public static int GetUniqueId()
        {
            return curr_id++;
        }

        public static int GetAvatarDictSize()
        {

            return textureStorage.Count;
        }

        static Dictionary<string, List<TextureEntry>> textureStorage = new Dictionary<string, List<TextureEntry>> { };

        /*static List<TextureEntry> freeAvatarsMale = new List<TextureEntry> { };
        static List<TextureEntry> freeAvatarsFemale = new List<TextureEntry> { };
        static Dictionary<string, List<TextureEntry>> specificAvatarsMale;
        static Dictionary<string, List<TextureEntry>> specificAvatarsFemale;*/
        static int curr_id = 0;
        private static T2 DefaultDictGet<T1, T2>(Dictionary<T1, T2> dict, T1 key)
             where T2 : new()
        {
            if (dict.ContainsKey(key))
            {
                return dict[key];
            }
            else
            {
                T2 val = new T2();
                dict[key] = val;
                return val;
            }
        }
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






}
