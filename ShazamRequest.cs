using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shazam
{
    public struct ShazamRequest
    {
        public string hostname;

        public string deviceid;

        public string service;

        public string language;

        public string model;

        public string appid;

        public string token;

        public char[] key;

        public byte[] audioBuffer;

        public string filename;

        public int art_width;

        public ulong requestId;
    }
}
