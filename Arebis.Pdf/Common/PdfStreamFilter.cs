using System;
using System.Text;

namespace Arebis.Pdf.Common
{
    [Serializable]
    public abstract class PdfStreamFilter
    {
        protected PdfStreamFilter(string name)
        {
            this.Name = name;
        }

        public string Name { get; private set; }

        public abstract byte[] Encode(byte[] bytes);

        public byte[] EncodeString(string str)
        {
            return this.Encode(this.ConvertStringToBytes(str));
        }

        protected virtual byte[] ConvertStringToBytes(string str)
        {
            return Encoding.Default.GetBytes(str);
        }

        public abstract byte[] Decode(byte[] bytes);

        public string DecodeToString(byte[] bytes)
        {
            return ConvertBytesToString(this.Decode(bytes));
        }

        protected virtual string ConvertBytesToString(byte[] bytes)
        {
            return Encoding.Default.GetString(bytes);
        }
    }
}
