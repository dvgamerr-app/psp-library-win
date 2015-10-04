using System;
using System.Text;
using System.IO;

namespace PSPlibrary
{
    public class SfoFile
    {
        String _Magic;              /* Always PSF */
        Double _Version;            /* Usually 1.1 */
        UInt32 _KeyTableStart_;      /* Start position of key_table */
        UInt32 _DataTableStart_;     /* Start position of data_table */
        UInt32 _IndexTableEntries_;  /* Number of entries in index_table*/

        public struct TableEntrie
        {
            public String Key;             /* SFO Key */
            public String Value;           /* SFO Value */
            public UInt32 OffsetKey;       /* Offset of the param_key from start of key_table */
            public UInt32 OffsetData;      /* Offset of the param_data from start of data_table */
            public UInt32 ParamFormat;     /* Type of data of param_data in the data_table */
            public UInt32 ParamLength;     /* Used Bytes by param_data in the data_table */
            public UInt32 ParamMaxLength;  /* Total bytes reserved for param_data in the data_table */
        };

        TableEntrie[] _Entries = { };

        public String Magic { get { return _Magic; } }
        public Double Version { get { return _Version; } }
        public String Param(String key)
        {
            String _Data = "0";
            foreach (TableEntrie item in _Entries) if (item.Key.ToLower() == key.ToLower().Trim()) _Data = item.Value;
            return _Data;
        }
        public void Param(String key, UInt32 value)
        {
            this.AddEntrie(key, value.ToString(), 0x404);
        }
        public void Param(String key, String value)
        {
            this.AddEntrie(key, value.ToString(), 0x204);
        }

        public SfoFile()
        {
            // Constructor
        }

        public void Load(Byte[] RawData)
        {
            try
            {
                _Magic = this.GetMagic(RawData);
                _Version = this.GetVersion(RawData);
                _KeyTableStart_ = BitConverter.ToUInt32(RawData, 0x08);
                _DataTableStart_ = BitConverter.ToUInt32(RawData, 0x0C);
                _IndexTableEntries_ = BitConverter.ToUInt32(RawData, 0x10);

                _Entries = new TableEntrie[_IndexTableEntries_];
                Int32 iEtries = 0;
                while (iEtries < _IndexTableEntries_)
                {
                    _Entries[iEtries].OffsetKey = BitConverter.ToUInt16(RawData, 0x14 + (0x10 * iEtries));
                    _Entries[iEtries].ParamFormat = BitConverter.ToUInt16(RawData, 0x16 + (0x10 * iEtries));
                    _Entries[iEtries].ParamLength = BitConverter.ToUInt32(RawData, 0x18 + (0x10 * iEtries));
                    _Entries[iEtries].ParamMaxLength = BitConverter.ToUInt32(RawData, 0x1C + (0x10 * iEtries));
                    _Entries[iEtries].OffsetData = BitConverter.ToUInt32(RawData, 0x20 + (0x10 * iEtries));
                    
                    _Entries[iEtries].Key = this.ConvertToKey(RawData, (Int32)(_Entries[iEtries].OffsetKey + _KeyTableStart_));
                    if (_Entries[iEtries].ParamFormat == 0x204)
                        _Entries[iEtries].Value = this.ConvertToString(RawData, (Int32)(_Entries[iEtries].OffsetData + _DataTableStart_), (Int32)_Entries[iEtries].ParamLength);
                    else if (_Entries[iEtries].ParamFormat == 0x404)
                        _Entries[iEtries].Value = this.ConvertToInt32(RawData, (Int32)(_Entries[iEtries].OffsetData + _DataTableStart_)).ToString();

                    // Console.WriteLine(_Entries[iEtries].Key + " ::: " + _Entries[iEtries].Value);
                    iEtries++;
                }
                //Console.WriteLine("Key: " + _KeyTableStart_ + " | Data: " + _DataTableStart_ + " | Length: " + _IndexTableEntries_);
            }
            catch (Exception ex)
            {
                Console.WriteLine("-----------------------------------------------------");
                Console.WriteLine("SfoFIle.Load(" + RawData.Length + " Bytes) Method");
                Console.WriteLine("EX:: " + ex.Message);
                Console.WriteLine("-----------------------------------------------------");
            }
        }


        public void SaveAs(String path)
        {
            _IndexTableEntries_ = (UInt32)_Entries.Length;
            _KeyTableStart_ = ((UInt32)_Entries.Length * 0x10) + 0x14;
            UInt32 LengthKeyEntrie = 0;
            UInt32 LengthDataEntrie = 0;
            Int32 iEtries = 0;
            while (iEtries < _IndexTableEntries_)
            {
                // New Offset
                _Entries[iEtries].OffsetKey = LengthKeyEntrie;
                _Entries[iEtries].OffsetData = LengthDataEntrie;

                // Calculator Langth
                UInt32 CountBytes = 0;
                Byte[] ReadBytes = new Byte[_Entries[iEtries].Value.Length * 3];
                Encoding.UTF8.GetBytes(_Entries[iEtries].Value, 0, _Entries[iEtries].Value.Length, ReadBytes, 0);
                foreach (Byte wChar in ReadBytes) if (wChar > 0x0) CountBytes++;

                // New Langth
                _Entries[iEtries].ParamLength = CountBytes;
                if (_Entries[iEtries].ParamMaxLength < CountBytes) _Entries[iEtries].ParamMaxLength = CountBytes + 1;

                // Calculator Offset
                LengthKeyEntrie += ((UInt32)_Entries[iEtries].Key.Length + 1);
                LengthDataEntrie += _Entries[iEtries].ParamMaxLength;
                iEtries++;
            }
            _DataTableStart_ = LengthKeyEntrie + _KeyTableStart_;

            if(File.Exists(path)) File.Delete(path);
            FileStream ParamWrite = new FileStream(path, FileMode.Create, FileAccess.ReadWrite);
            ParamWrite.Position = 0x1;

            // Stream Write Header SFO
            ParamWrite.Write(new Byte[] { 0x50, 0x53, 0x46, 0xFF, 0xFF }, 0, 5);
            this.WriteToStream(ParamWrite, _KeyTableStart_, 0x8);
            this.WriteToStream(ParamWrite, _DataTableStart_, 0xC);
            this.WriteToStream(ParamWrite, _IndexTableEntries_, 0x10);

            iEtries = 0;
            while (iEtries < _IndexTableEntries_)
            {
                this.WriteToStream(ParamWrite, _Entries[iEtries].OffsetKey, 0x14 + (0x10 * iEtries));
                this.WriteToStream(ParamWrite, _Entries[iEtries].ParamFormat, 0x16 + (0x10 * iEtries));
                this.WriteToStream(ParamWrite, _Entries[iEtries].ParamLength, 0x18 + (0x10 * iEtries));
                this.WriteToStream(ParamWrite, _Entries[iEtries].ParamMaxLength, 0x1C + (0x10 * iEtries));
                this.WriteToStream(ParamWrite, _Entries[iEtries].OffsetData, 0x20 + (0x10 * iEtries));

                this.WriteToStream(ParamWrite, _Entries[iEtries].Key, (_KeyTableStart_ + _Entries[iEtries].OffsetKey));
                if (_Entries[iEtries].ParamFormat == 0x404)
                    this.WriteToStream(ParamWrite, UInt32.Parse(_Entries[iEtries].Value), (_DataTableStart_ + _Entries[iEtries].OffsetData));
                else if (_Entries[iEtries].ParamFormat == 0x204)
                    this.WriteToStream(ParamWrite, _Entries[iEtries].Value, (_DataTableStart_ + _Entries[iEtries].OffsetData));
                this.WriteToStream(ParamWrite, 0x00, (_DataTableStart_ + _Entries[iEtries].OffsetData + _Entries[iEtries].ParamMaxLength));
                
                iEtries++;
            }
            ParamWrite.Close();
        }

        private void AddEntrie(String key, String value, UInt32 format)
        {
            try
            {
                if (value.Length > 0)
                {
                    Boolean FoundEntrie = false;
                    for (Int32 iEntrie = 0; iEntrie < _Entries.Length; iEntrie++)
                    {
                        if (_Entries[iEntrie].Key.ToLower() == key.ToLower().Trim())
                        {
                            _Entries[iEntrie].Key = key.Trim();
                            _Entries[iEntrie].Value = value.Trim();
                            _Entries[iEntrie].ParamFormat = format;
                            FoundEntrie = true;
                            break;
                        }
                    }
                    if (!FoundEntrie)
                    {
                        _IndexTableEntries_++;
                        TableEntrie[] tmpData = _Entries;
                        _Entries = new TableEntrie[_IndexTableEntries_];
                        Int32 iEtries = 0;
                        while (iEtries < tmpData.Length)
                        {
                            _Entries[iEtries] = tmpData[iEtries];
                            iEtries++;
                        }
                        _Entries[iEtries] = new TableEntrie()
                        {
                            Key = key,
                            Value = value,
                            OffsetKey = 0x0,
                            OffsetData = 0x0,
                            ParamFormat = format,
                        };

                    }
                }
                else
                {
                    Console.WriteLine("SfoFile.AddEntrie(" + key + ", " + value + ") Method");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("-----------------------------------------------------");
                Console.WriteLine("SfoFile.AddEntrie(" + key + ", " + value + ") Method");
                Console.WriteLine(ex.Message);
                Console.WriteLine("-----------------------------------------------------");
            }
        }

        private void WriteToStream(FileStream stream, String value, long position)
        {
            Byte[] ValBytes = new Byte[value.Length * 3];
            Encoding.UTF8.GetBytes(value, 0, value.Length, ValBytes, 0);

            stream.Position = position;
            foreach (Byte wChar in ValBytes) if (wChar > 0x0) stream.WriteByte(wChar);
        }

        private void WriteToStream(FileStream stream, UInt32 value, long position)
        {
            Byte[] ValBytes = BitConverter.GetBytes(value);
            stream.Position = position;
            stream.Write(ValBytes, 0, ValBytes.Length);
        }

        #region Convert Binary Method

        private String GetMagic(Byte[] data) { return Encoding.UTF8.GetString(data, 1, 3); }
        private Double GetVersion(Byte[] data) { return data[5] + (data[4] / 100.0); }

        private String ConvertToString(Byte[] data, Int32 index, Int32 length)
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

        private Int32 ConvertToInt32(Byte[] data, Int32 index)
        {
            return BitConverter.ToInt32(data, index);
        }

        private String ConvertToKey(Byte[] data, Int32 index)
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
        
        #endregion        
    }
}
