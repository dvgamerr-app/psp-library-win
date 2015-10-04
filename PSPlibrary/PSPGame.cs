using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DiscUtils;
using DiscUtils.Iso9660;

namespace PSPlibrary
{
    public class PSPGame
    {
        public Boolean Readable { get; set; }
        public String FileName { get; set; }
        public Int64 FileSize { get; set; }
        public PSPGame(String iso)
        {
            Readable = true;
            try
            {
                FileStream FileISO = File.Open(iso, FileMode.Open, FileAccess.Read);
                CDReader ReaderISO = new CDReader(FileISO, true);
                FileName = iso;
                FileSize = FileISO.Length;
                if (!ReaderISO.FileExists("PSP_GAME\\PARAM.SFO")) throw new NotImplementedException();
                FileISO.Close();
            }
            catch (Exception ex)
            {
                Readable = false;
                Console.WriteLine("-----------------------------------------------------");
                Console.WriteLine("PSPGame(" + iso + ")");
                Console.WriteLine(ex.Message);
                Console.WriteLine("-----------------------------------------------------");
            }
        }

        public void ExtractFile(String FileSource, String FileTarget)
        {
            FileStream FileISO = File.Open(FileName, FileMode.Open, FileAccess.Read);
            try
            {
                CDReader ReaderISO = new CDReader(FileISO, true);
                SparseStream _SourceStream = ReaderISO.OpenFile(FileSource, FileMode.Open, FileAccess.Read);
                Byte[] _ReadAllByte = new Byte[_SourceStream.Length];
                _SourceStream.Read(_ReadAllByte, 0, _ReadAllByte.Length);
                _SourceStream.Close();

                FileStream _FileCreated = new FileStream(FileTarget, FileMode.CreateNew);
                _FileCreated.Position = 0;
                _FileCreated.Write(_ReadAllByte, 0, _ReadAllByte.Length);
                _FileCreated.Close();
            }
            catch (IOException ex)
            {
                if (ex.Message != "No such file")
                {
                    Console.WriteLine("-----------------------------------------------------");
                    Console.WriteLine("ExtractFile(" + FileSource + ", " + FileTarget + ")");
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("-----------------------------------------------------");
                }
                else
                {
                    Console.WriteLine("ExtractFile(" + FileSource + ") --> " + Path.GetFileName(FileName));
                }
            }
            FileISO.Close();
        }

        public Byte[] ReadAllByte(String FileSource)
        {
            Byte[] result = { 0x0 };
            FileStream FileISO = File.Open(FileName, FileMode.Open, FileAccess.Read);
            try
            {
                CDReader ReaderISO = new CDReader(FileISO, true);
                SparseStream _SourceStream = ReaderISO.OpenFile(FileSource, FileMode.Open, FileAccess.Read);
                Byte[] _ReadAllByte = new Byte[_SourceStream.Length];
                _SourceStream.Read(_ReadAllByte, 0, _ReadAllByte.Length);
                result = _ReadAllByte;
                _SourceStream.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("-----------------------------------------------------");
                Console.WriteLine("ReadAllByte(" + FileSource + ")");
                Console.WriteLine(ex.Message);
                Console.WriteLine("-----------------------------------------------------");
            }
            FileISO.Close();
            return result;
        }

        public static String GetSize(long byteSize)
        {
            String[] Unit = { " Bytes", " KB", " MB", " GB" };
            Int32 i = 0;
            decimal share = byteSize;
            while (share >= 1024)
            {
                share = share / 1024;
                i++;
            }

            return Math.Round(share, 2) + Unit[i];
        } 
    }

}