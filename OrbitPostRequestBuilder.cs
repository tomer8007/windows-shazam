using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;

namespace shazam
{
    public class OrbitPostRequestBuilder : IRequestBuilder
    {
        private MemoryStream requestDataStream = new MemoryStream();

        private string Boundary
        {
            get;
            set;
        }

        public IceKey Encryptor
        {
            get;
            private set;
        }

        public char[] Key
        {
            get;
            private set;
        }

        public OrbitPostRequestBuilder(IceKey encryptor, char[] key)
        {
            this.Encryptor = encryptor;
            this.Key = key;
            this.Boundary = "AJ8xP50454bf20Gp";
        }

        public void AddEncryptedFile(string name, string fileName, byte[] fileData, int fileSize)
        {
            this.Encryptor.set(this.Key);
            byte[] numArray = this.Encryptor.encBinary(fileData, fileSize);
            this.AddFile(name, fileName, numArray, (int)numArray.Length);
        }

        public void AddEncryptedParameter(string name, string value)
        {
            this.Encryptor.set(this.Key);
            char[] chrArray = this.Encryptor.encString(value);
            this.AddParameter(name, new string(chrArray));
        }

        public void AddFile(string name, string fileName, byte[] fileData, int fileSize)
        {
            string[] newLine = new string[] { "--{0}", Environment.NewLine, "Content-Disposition: form-data; name=\"{1}\"; filename=\"{2}\"", Environment.NewLine, "Content-Type: {3}", Environment.NewLine, Environment.NewLine };
            string str = string.Concat(newLine);
            object[] boundary = new object[] { this.Boundary, name, fileName, "application/octet-stream" };
            string str1 = string.Format(str, boundary);
            byte[] bytes = Encoding.UTF8.GetBytes(str1);
            this.requestDataStream.Write(bytes, 0, (int)bytes.Length);
            this.requestDataStream.Write(fileData, 0, fileSize);
            string newLine1 = Environment.NewLine;
            byte[] numArray = Encoding.UTF8.GetBytes(newLine1);
            this.requestDataStream.Write(numArray, 0, (int)numArray.Length);
        }

        public void AddParameter(string name, string value)
        {
            string[] newLine = new string[] { "--{0}", Environment.NewLine, "Content-Disposition: form-data; name=\"{1}\"", Environment.NewLine, Environment.NewLine, "{2}", Environment.NewLine };
            string str = string.Concat(newLine);
            object[] boundary = new object[] { this.Boundary, name, value };
            string str1 = string.Format(str, boundary);
            byte[] bytes = Encoding.UTF8.GetBytes(str1);
            this.requestDataStream.Write(bytes, 0, (int)bytes.Length);
        }

        public string MakeRequestUri(string scheme, string hostName, string path)
        {
            return string.Concat(scheme, "://", hostName, path);
        }

        public void PopulateWebRequestHeaders(WebRequest webRequest)
        {
            webRequest.Method = "POST";
            webRequest.ContentType = string.Concat("multipart/form-data; boundary=", this.Boundary);
        }

        public void WriteToRequestStream(Stream requestStream)
        {
            string str = string.Concat("--", this.Boundary, "--");
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            this.requestDataStream.Write(bytes, 0, (int)bytes.Length);
            byte[] array = this.requestDataStream.ToArray();
            requestStream.Write(array, 0, (int)array.Length);
        }
    }

    public interface IRequestBuilder
    {
        void AddEncryptedFile(string name, string fileName, byte[] fileData, int fileSize);

        void AddEncryptedParameter(string name, string value);

        void AddFile(string name, string fileName, byte[] fileData, int fileSize);

        void AddParameter(string name, string value);

        string MakeRequestUri(string scheme, string hostName, string path);

        void PopulateWebRequestHeaders(WebRequest webRequest);

        void WriteToRequestStream(Stream requestStream);
    }
}
