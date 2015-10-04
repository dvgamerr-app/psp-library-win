using System;
using System.Text;

namespace PSPlibrary
{
    class BinaryReader
    {
        protected String GetMagic(Byte[] data) { return Encoding.UTF8.GetString(data, 1, 3); }
        protected Double GetVersion(Byte[] data) { return data[5] + (data[4] / 100.0); }

        protected String ConvertToString(Byte[] data, Int32 index, Int32 length)
        {
            Int32 iLoop = 0;
            String result = "";
            while (iLoop < length)
            {
                Int32 i = index + iLoop;
                if (data[i] > 0x00 && data[i] < 0xC0)
                {
                    result += Encoding.UTF8.GetChars(data, (i), 1)[0].ToString();
                }
                else if (data[i] >= 0xC0 && data[i] <= 0xCF)
                {
                    result += Encoding.UTF8.GetString(data, (i), 2);
                    iLoop += 1;
                }
                else if (data[i] >= 0xE0 && data[i] <= 0xEF)
                {
                    result += Encoding.UTF8.GetString(data, (i), 3);
                    iLoop += 2;
                }
                iLoop += 1;
            }
            return result;
        }

        protected Int32 ConvertToInt32(Byte[] data, Int32 index)
        {
            return BitConverter.ToInt32(data, index);
        }

        protected String ConvertToKey(Byte[] data, Int32 index)
        {
            Int32 iLoop = 0;
            String result = "";
            while (data[index + iLoop] > 0x00)
            {
                result += Encoding.UTF8.GetChars(data, (index + iLoop), 1)[0].ToString();
                iLoop += 1;
            }
            return result;
        }
        

    }
}
