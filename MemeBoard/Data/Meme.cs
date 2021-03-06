﻿using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemeBoard
{
    public enum MemeType
    {
        Image,
        TTS
    }
    
    public class Meme
    {
        public string Path { get; private set; }
        public string Prefix { get; private set; }
        public bool IsAnimated { get; private set; }
        public string Name { get; private set; }
        public MemeType Type { get; private set; }
        
        public bool IsActive { get; private set; }

        public void Activate()
        {
            this.IsActive = true;
        }

        public void Deactivate()
        {
            this.IsActive = false;
        }

        public Meme(string file)
        {
            var fileInfo = new FileInfo(file);
            
            this.Path = fileInfo.FullName;
            this.IsAnimated = fileInfo.Extension.ToLower() == ".gif";
            this.Type = fileInfo.Extension.ToLower() == ".txt" ? MemeType.TTS : MemeType.Image;
            this.Prefix = fileInfo.Name.Split('_').First();
            this.Name = fileInfo.Name;
        }
    }
}
