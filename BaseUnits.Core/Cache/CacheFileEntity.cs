using System;

namespace BaseUnits.Core.Cache
{
    [Serializable]
    public class CacheFileEntity : CacheByTagBase
    {
        public uint FileLength { get; set; }

        public Guid Did { get; set; }

        public string MimeType { get; set; }

        public string Extension = ".jpg";

        public string FileName = "";
    }
}
