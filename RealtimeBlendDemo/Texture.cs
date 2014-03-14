using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeBlendDemo
{
    public class Texture
    {
        public Uri File { get; private set; }
        public Uri Thumbnail { get; private set; }
        public bool IsPositional { get; private set; }

        public Texture(Uri file)
            : this(file, file)
        {
        }

        public Texture(Uri file, Uri thumbnail)
            : this(file, thumbnail, false)
        {
        }

        public Texture(Uri file, Uri thumbnail, bool isPositional)
        {
            File = file;
            Thumbnail = thumbnail;
            IsPositional = isPositional;
        }
    }

}
