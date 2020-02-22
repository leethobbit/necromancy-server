using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Arrowgene.Services.Buffers;
using Arrowgene.Services.Logging;

namespace Necromancy.Server.Data
{
    public class UnknownItem
    {
        public int count;
        public int size;
    }
    public class FpmfArchiveIO
    {
        private static byte[] MagicBytes = {0x46, 0x50, 0x4D, 0x46};
        private static byte[] MagicBytes_WOITM = {0x57, 0x4F, 0x49, 0x54, 0x4D};
        private ILogger _logger;

        public FpmfArchiveIO()
        {
            _logger = LogProvider.Logger(this);
        }

        public FpmfArchive Open(string hedFilePath, string outPath = "")
        {
            Dictionary<uint, UnknownItem> unknownList = new Dictionary<uint, UnknownItem>();
            FileInfo hedFile = new FileInfo(hedFilePath);
            if (!hedFile.Exists)
            {
                throw new FileNotFoundException($"File: {hedFilePath} not found.");
            }

            IBuffer hedBuffer = new StreamBuffer(hedFile.FullName);
            StreamBuffer headerBuff = new StreamBuffer();
            headerBuff.SetPositionStart();

            if (hedBuffer.Size < 12)
            {
                throw new Exception("File to small");
            }
            hedBuffer.SetPositionStart();
            byte[] magicBytes = hedBuffer.ReadBytes(4);
            for (int i = 0; i < 4; i++)
            {
                if (magicBytes[i] != MagicBytes[i])
                {
                    throw new Exception("Invalid File");
                }
                headerBuff.WriteByte(magicBytes[i]);
            }

            FpmfArchive archive = new FpmfArchive();
            archive.Size = hedBuffer.ReadUInt32();
            headerBuff.WriteInt32(archive.Size);
            uint unknown0 = hedBuffer.ReadUInt32();

            //BinaryWriter tmpwriter = new BinaryWriter(File.Open("C:\\Users\\kevin\\Desktop\\GameFilesSteamTest\\script_encrypted.hed", FileMode.Create));
            //tmpwriter.Write(hedBuffer.GetAllBytes());
            //tmpwriter.Flush();
            //tmpwriter.Close();

            hedBuffer = DecryptHed(hedBuffer);
            //tmpwriter = new BinaryWriter(File.Open("C:\\Users\\kevin\\Desktop\\GameFilesSteamTest\\script_unencrypted.hed", FileMode.Create));
            //tmpwriter.Write(magicBytes);
            //tmpwriter.Write(archive.Size);
            //tmpwriter.Write(unknown0);
            //tmpwriter.Write(hedBuffer.GetAllBytes());
            //tmpwriter.Flush();
            //tmpwriter.Close();

            hedBuffer.SetPositionStart();
            headerBuff.WriteInt32(unknown0);
            uint unknown1 = hedBuffer.ReadUInt32();
            headerBuff.WriteInt32(unknown1);
            uint unknown2 = hedBuffer.ReadUInt32();
            headerBuff.WriteInt32(unknown2);
            byte unknown3 = hedBuffer.ReadByte();
            headerBuff.WriteByte(unknown3);
            byte unknown4 = hedBuffer.ReadByte();
            headerBuff.WriteByte(unknown4);
            uint unknown5 = hedBuffer.ReadUInt32();
            headerBuff.WriteInt32(unknown5);
            uint unknown6 = hedBuffer.ReadUInt32();
            headerBuff.WriteInt32(unknown6);
            int strLen = hedBuffer.ReadByte();
            headerBuff.WriteByte((byte)strLen);
            archive.DatPath = hedBuffer.ReadString(strLen);
            headerBuff.WriteCString(archive.DatPath, Encoding.UTF8);
            headerBuff.Position = headerBuff.Position - 1;
            uint unknown7 = hedBuffer.ReadUInt32();
            headerBuff.WriteInt32(unknown7);
            uint unknown8 = hedBuffer.ReadUInt32();
            headerBuff.WriteInt32(unknown8);
            uint unknown9 = hedBuffer.ReadUInt32();
            headerBuff.WriteInt32(unknown9);
            uint unknown10 = hedBuffer.ReadUInt32();
            headerBuff.WriteInt32(unknown10);
            uint keyLen = hedBuffer.ReadUInt32();
            headerBuff.WriteInt32(keyLen);
            archive.Key = hedBuffer.ReadBytes((int) keyLen);
            headerBuff.WriteBytes(archive.Key);
            uint unknown11 = hedBuffer.ReadUInt32();
            headerBuff.WriteInt32(unknown11);
            uint unknown12 = hedBuffer.ReadUInt32();
            headerBuff.WriteInt32(unknown12);
            uint numFiles = hedBuffer.ReadUInt32();
            headerBuff.WriteInt32(numFiles);
            archive.Header = headerBuff.GetAllBytes();

            //headerBuff.SetPositionStart();
            string relativeArchiveDir = archive.DatPath
                .Replace("/%08x.dat", "")
                .Replace("./", "")
                .Replace('/', Path.DirectorySeparatorChar);
            string hedPath = hedFile.FullName.Replace(".hed", "");
            string hedName = hedPath.Substring(hedPath.LastIndexOf("\\") + 1);
            string rootPath = hedPath.Replace(relativeArchiveDir, "");
            if (outPath.Length > 0)
            {
                headerBuff.SetPositionStart();
                BinaryWriter writer = new BinaryWriter(File.Open(outPath + "\\" + hedName + "_header.bin", FileMode.Create));
                for (int i = 0; i < headerBuff.Size; i++)
                {
                    writer.Write(headerBuff.ReadByte());
                }
                writer.Flush();
                writer.Close();
            }
            DirectoryInfo rootDirectory = new DirectoryInfo(rootPath);
            if (!rootDirectory.Exists)
            {
                throw new FileNotFoundException(
                    $"Could not determinate root path. (Rel:{relativeArchiveDir} Hed:{hedPath}  Root:{rootPath}");
            }

            _logger.Info($"Using Root:{rootPath}");

            Dictionary<uint, IBuffer> datBufferPool = new Dictionary<uint, IBuffer>();
            for (int i = 0; i < numFiles; i++)
            {
                FpmfArchiveFile archiveFile = new FpmfArchiveFile();
                strLen = hedBuffer.ReadByte();
                archiveFile.DirectoryPath = hedBuffer.ReadString(strLen);
                strLen = hedBuffer.ReadByte();
                archiveFile.FilePath = hedBuffer.ReadString(strLen);
                archiveFile.DatNumber = hedBuffer.ReadUInt32();
                archiveFile.Offset = hedBuffer.ReadUInt32();
                archiveFile.Size = hedBuffer.ReadUInt32();
                uint unknown13 = hedBuffer.ReadUInt32();
                uint unknown14 = hedBuffer.ReadUInt32();

                uint unknown15 = addFileName(archiveFile.FilePath);
                uint unknown16 = addFileName(archiveFile.DirectoryPath);
                _logger.Info($"Processing: {archiveFile.FilePath}");

                IBuffer datBuffer;
                if (datBufferPool.ContainsKey(archiveFile.DatNumber))
                {
                    datBuffer = datBufferPool[archiveFile.DatNumber];
                }
                else
                {
                    string datFileName = archive.DatPath
                        .Replace("%08x", $"{archiveFile.DatNumber:X8}")
                        .Replace("./", "")
                        .Replace('/', Path.DirectorySeparatorChar);
                    string datFilePath = Path.Combine(rootDirectory.FullName, datFileName);
                    FileInfo datFile = new FileInfo(datFilePath);
                    if (!datFile.Exists)
                    {
                        throw new FileNotFoundException($"File: {datFilePath} not found.");
                    }

                    datBuffer = new StreamBuffer(datFile.FullName);
                    datBufferPool.Add(archiveFile.DatNumber, datBuffer);
                }

                IBuffer decrypted  = DecryptDat(datBuffer, archiveFile.Offset, archiveFile.Size, archive.Key);
                if (archiveFile.FilePath.Contains("\\item.csv"))
                {
                    decrypted = OpenWoItm(decrypted);
                }
                archiveFile.Data = decrypted.GetAllBytes();

                archive.AddFile(archiveFile);
            }
            return archive;
        }

        public void Save(FpmfArchive archive, string directoryPath)
        {
            string archiveType = archive.DatPath
                .Replace("/%08x.dat", "")
                .Replace("./", "")
                .Replace('/', Path.DirectorySeparatorChar);
            DirectoryInfo directory = new DirectoryInfo(directoryPath);
            if (!directory.Exists)
            {
                throw new FileNotFoundException($"Directory: {directoryPath} not found.");
            }

            string relativeArchiveDir = archive.DatPath
                .Replace("/%08x.dat", "")
                .Replace("./", "")
                .Replace('/', Path.DirectorySeparatorChar);

            string rootPath = Path.Combine(directory.FullName, relativeArchiveDir);

            List<FpmfArchiveFile> files = archive.GetFiles();
            foreach (FpmfArchiveFile file in files)
            {
                string relativeFilePath = file.FilePath
                    .Replace(".\\", "")
                    .Replace('\\', Path.DirectorySeparatorChar);
                string filePath = Path.Combine(rootPath, relativeFilePath);

                FileInfo fileInfo = new FileInfo(filePath);
                if (!Directory.Exists(fileInfo.DirectoryName))
                {
                    Directory.CreateDirectory(fileInfo.DirectoryName);
                }

                File.WriteAllBytes(filePath, file.Data);
            }
            string keyPath = archive.DatPath;
            BinaryWriter writer = new BinaryWriter(File.Open(directoryPath + "\\" + archiveType + ".key", FileMode.Create));
            for (int i = 0; i < archive.Key.Length; i++)
            {
                writer.Write(archive.Key[i]);
            }
            writer.Flush();
            writer.Close();
            writer = new BinaryWriter(File.Open(directoryPath + "\\" + archiveType + "_header.bin", FileMode.Create));
            writer.Write(archive.Header);
            writer.Flush();
            writer.Close();
        }

        public void Pack(string inPath, string outPath, string archiveName)
        {
            uint fileTime = 0x506fa78e;

            FpmfArchive archive = new FpmfArchive();
            if (inPath.EndsWith("\\"))
            {
                inPath = inPath.Substring(0, inPath.Length - 1);
            }
            uint currentOffset = 0;
            //HedFile hedFile = new HedFile();
            IBuffer keyReader = new StreamBuffer(inPath + "\\" + archiveName + ".key");
            string baseArchivePath = inPath + "\\" + archiveName;

            archive.KeyLen = keyReader.Size;
            archive.Key = keyReader.GetAllBytes();
            string[] inFiles = Directory.GetFiles(baseArchivePath, "*", SearchOption.AllDirectories);
            archive.NumFiles = (uint)inFiles.Length;
            uint archiveSize = 0;

            foreach (string inFile in inFiles)
            {
                IBuffer inReader = new StreamBuffer(inFile);
                FpmfArchiveFile datFile = new FpmfArchiveFile();
                datFile.Size = (uint)inReader.Size;
                datFile.DatNumber = 0;
                datFile.Offset = currentOffset;
                IBuffer encryptedBuff = EncryptDat(inReader, archive.Key);
                datFile.Data = encryptedBuff.GetAllBytes();
                archiveSize += (uint)datFile.Data.Length;
                datFile.FilePath = inFile.Replace(inPath + "\\" + archiveName, ".");
                //datFile.FilePath =  ".\\" + inFile.Substring(inFile.LastIndexOf(archiveName)+archiveName.Length+1);
                datFile.FilePathSize = (uint)datFile.FilePath.Length;
                datFile.DirectoryPath = ".\\" + archiveName + "\\";
                datFile.DirectoryPathSize = (uint)datFile.DirectoryPath.Length;
                datFile.Unknown0 = fileTime;
                datFile.Unknown1 = 0;
                archive.AddFile(datFile);
                currentOffset += datFile.Size;
            }
            IBuffer headerReader = new StreamBuffer(inPath + "\\" + archiveName + "_header.bin");
            archive.Header = headerReader.GetAllBytes();

            SavePack(archive, inPath, outPath, archiveName);
        }
        private void SavePack(FpmfArchive archive, string inPath, string archivePath, string archiveName)
        {
            Directory.CreateDirectory(archivePath + "\\" + archiveName);
            IBuffer headerBuff = new StreamBuffer();
            headerBuff.WriteBytes(archive.Header);
            List<FpmfArchiveFile> archiveFiles = archive.GetFiles();
            foreach (FpmfArchiveFile archiveFile in archiveFiles)
            {
                headerBuff.WriteByte((byte)archiveFile.DirectoryPathSize);
                headerBuff.WriteCString(archiveFile.DirectoryPath);
                headerBuff.Position = headerBuff.Position - 1;
                headerBuff.WriteByte((byte)archiveFile.FilePathSize);
                headerBuff.WriteCString(archiveFile.FilePath);
                headerBuff.Position = headerBuff.Position - 1;
                headerBuff.WriteInt32(archiveFile.DatNumber);
                headerBuff.WriteInt32(archiveFile.Offset);
                headerBuff.WriteInt32(archiveFile.Size);
                headerBuff.WriteInt32(archiveFile.Unknown0);
                headerBuff.WriteInt32(archiveFile.Unknown1);
            }
            //BinaryWriter hedWriter = new BinaryWriter(File.Open(archivePath + "\\" + archiveName + "_unencrypted.hed", FileMode.Create));
            //hedWriter.Write(headerBuff.GetAllBytes());
            //hedWriter.Flush();
            //hedWriter.Close();
            headerBuff = EncryptHed(headerBuff);
            BinaryWriter headerWriter = new BinaryWriter(File.Open(archivePath + "\\" + archiveName + ".hed", FileMode.Create));
            headerBuff.Position = 4;
            headerBuff.WriteInt32(headerBuff.Size - 12);
            headerWriter.Write(headerBuff.GetAllBytes(), 0, headerBuff.Size);
            headerWriter.Flush();
            headerWriter.Close();

            BinaryWriter datWriter = new BinaryWriter(File.Open(archivePath + "\\" + archiveName + "\\" + "00000000.dat", FileMode.Create));
            IBuffer outBuff = new StreamBuffer();
            foreach (FpmfArchiveFile archiveFile in archiveFiles)
            {
                string inputFile = inPath + "\\" + archiveName + archiveFile.FilePath.Substring(1);
                IBuffer datFileReader = new StreamBuffer(inputFile);
                datFileReader = EncryptDat(datFileReader, archive.Key);
                outBuff.WriteBytes(datFileReader.GetAllBytes());
            }
            datWriter.Write(outBuff.GetAllBytes(), 0, outBuff.Size);
            datWriter.Flush();
            datWriter.Close();
        }

        /// <summary>
        /// 0x403700
        /// </summary>
        public void OpenWoItm(string itemPath)
        {
            FileInfo itemFile = new FileInfo(itemPath);
            if (!itemFile.Exists)
            {
                throw new FileNotFoundException($"File: {itemPath} not found.");
            }

            IBuffer buffer = new StreamBuffer(itemFile.FullName);
            buffer.SetPositionStart();
            byte[] magicBytes = buffer.ReadBytes(5);
            for (int i = 0; i < 5; i++)
            {
                if (magicBytes[i] != MagicBytes_WOITM[i])
                {
                    //throw new Exception("Invalid WOITM File");
                }
            }

            short version = buffer.ReadInt16(); // cmp to 1
            List<WoItm> woItems = new List<WoItm>();
            while (buffer.Position < buffer.Size)
            {
                int itemId = buffer.ReadInt32();
                int chunkSize = buffer.ReadInt32();
                int chunkLen = buffer.ReadInt32();
                byte[] data = buffer.ReadBytes(chunkSize - 4);

                WoItm woItm = new WoItm();
                woItm.Id = itemId;
                woItm.Size = chunkSize;
                woItm.Size2 = chunkLen;
                woItm.Data = data;
                woItems.Add(woItm);
            }
            List<string> str = new List<string>();
            foreach (WoItm woItem in woItems)
            {
                IBuffer itemBuffer = new StreamBuffer(woItem.Data);
                itemBuffer.SetPositionStart();

                IBuffer outBuffer = new StreamBuffer();

                uint[] xor =
                {
                    0xA522C3ED,
                    0x482E64B9,
                    0x0E52712B,
                    0x3ABC1D26
                };
                
                for (int i = 0; i < 4; i++)
                {
                    uint a = itemBuffer.ReadUInt32();
                    uint b = RotateRight(a, 8); // 00403035 | C1CE 08 | ror esi,8
                    uint c = b & 0xFF00FF00;
                    uint d = RotateLeft(a, 8); // 0040303E | C1C0 08 | rol eax,8
                    uint e = d & 0xFF00FF;
                    uint f = c | e;

                    uint g =f ^ xor[i];
                    
                    outBuffer.WriteInt32(g);
                }

/*              These 4 words are from the previous function after xor of xor[] above
                uint word1 = 0x6B9306F7;    
                uint word2 = 0xFE7D4F35;
                uint word3 = 0x406D7743;
                uint word4 = 0x9C07F4C0;

                uint seed = 0;
                uint seed1 = 0xFFFFFFFE;
                word1 = word1 ^ seed;
                uint a = ((word1 >> 16) & 0xFF) * 4;
                uint b = ((word1 >> 8) & 0xFF) * 4;
                uint c = (word1 >> 24) * 4;
                uint e = table1[c];
                uint f = table3[a];
                uint g = e ^ f;
                uint i = table3[b];
                uint j = g ^ i;

                uint k = (word2 & 0xFF) * 4;
                uint l = j ^ table2[k];
                uint m = (((word2 ^ seed) >> 16) & 0xFF) * 4;
                uint n = ((word2 >> 24) * 4;
                uint o = table3[n];
                uint p = table4[m];
                uint q = o ^ p;
                uint r = ((word2 >> 8) & 0xFF) * 4;
                uint s = (q ^ table2[r]);
                uint t = (word2 & 0xFF) * 4;
                uint u = table1[t] ^ s;
                uint v = u ^ l;

                word3 = word3 ^ v;
                uint w = ((l & 0xFF) << 18) | ((l >> 8) & 0x00FFFFFF);
                uint x = w ^ word4;
                uint y = u ^ x;
                word4 = l ^ y;
                uint z = (seed1 * 4) + ???? (1628EEC8);   // Missed a push earlier
                uint aa = word3 ^ z;
                uint ab = ((aa >> 16) & 0xFF) * 4;
                uint ac = (aa >> 24) * 4;
                uint ad = table1[ac];
                uint ae = (ad ^ table3[ab]);
                */




            }

            _logger.Info("done");
        }
        
        public uint addFileName(string path)
        {
            uint fileValue = 0;
            for (int i = 0; i < path.Length; i++)
            {
                char value = path[i];
                fileValue += (uint)Convert.ToByte(value);
            }
            return fileValue;
        }

        private uint RotateLeft(uint x, int n)
        {
            return (x << n) | (x >> (32 - n));
        }

        public IBuffer OpenWoItm(IBuffer buffer)
        {
            List<string> itemData = new List<string>();
            buffer.SetPositionStart();
            byte[] magicBytes = buffer.ReadBytes(5);
            for (int i = 0; i < 5; i++)
            {
                if (magicBytes[i] != MagicBytes_WOITM[i])
                {
                    //throw new Exception("Invalid WOITM File");
                }
            }
            itemData.Add(System.Text.Encoding.UTF8.GetString(magicBytes)  + "\r\n");
            //System.IO.StreamWriter outFile = new System.IO.StreamWriter(filePath);
            short version = buffer.ReadInt16(); // cmp to 1
            itemData.Add(version.ToString() + "\r\n");
            List<WoItm> woItems = new List<WoItm>();
            while (buffer.Position < buffer.Size)
            {
                int itemId = buffer.ReadInt32();
                int chunkSize = buffer.ReadInt32();
                int chunkLen = buffer.ReadInt32();
                byte[] data = buffer.ReadBytes(chunkSize - 4);

                itemData.Add(itemId.ToString() + "," + chunkSize.ToString() + "," + chunkLen.ToString() + ","+ BitConverter.ToString(data).Replace("-", string.Empty) + "\r\n");
                //outFile.WriteLine(outLine);
                //outFile.Flush();
                WoItm woItm = new WoItm();
                woItm.Id = itemId;
                woItm.Size = chunkSize;
                woItm.Size2 = chunkLen;
                woItm.Data = data;
                woItems.Add(woItm);
            }
            //outFile.Close();
            IBuffer itemRet = new StreamBuffer(itemData.SelectMany(s => Encoding.ASCII.GetBytes(s)).ToArray());

            List<string> str = new List<string>();
            foreach (WoItm woItem in woItems)
            {
                byte[] outp = Xor(woItem.Data, new byte[] { 0x00 });
                string test = Encoding.UTF8.GetString(outp);
                if (str.Contains(","))
                {
                    str.Add(test);
                    _logger.Info(test);
                }
            }

            _logger.Info("done");
            return itemRet;
        }
        private uint RotateRight(uint x, int n)
        {
            return (x >> n) | (x << (32 - n));
        }
        
        public byte[] Xor(byte[] text, byte[] key)
        {
            byte[] xor = new byte[text.Length];
            for (int i = 0; i < text.Length; i++)
            {
                xor[i] = (byte) (text[i] ^ key[i % key.Length]);
            }

            return xor;
        }

        /// <summary>
        /// 0xA7E480
        /// </summary>
        private IBuffer DecryptHed(IBuffer buffer)
        {
            byte bl = 0;
            byte al = 0;

            //Uncomment for US Steam Client
            byte dl = 0xA6;
            byte sub = 0x21;

            //Uncomment for US Sunset Client.
            //byte dl = 0xEA;
            //byte sub = 0x0A;

            //Uncomment for Beta Client
            //byte dl = 0x7D;
            //byte sub = 0xC4;

            // Uncomment for JP client
            // dl = 0x67;
            // sub = 0xC7;

            //Uncomment for beta client
            //dl = 0x7D;
            //sub = 0xC4;
            buffer.Position = 12;
            IBuffer outBuffer = new StreamBuffer();
            while (buffer.Position < buffer.Size)
            {
                byte cl = buffer.ReadByte();
                bl = al;
                bl = (byte) (bl + dl);
                bl = (byte) (bl ^ cl);
                bl = (byte) (bl - sub);
                al = (byte) (al + 1);
                dl = cl;
                outBuffer.WriteByte(bl);
            }
            return outBuffer;
        }

        /// <summary>
        /// 0xA7E3F0
        /// </summary>
        private IBuffer DecryptDat(IBuffer buffer, uint fileOffset, uint fileLength, byte[] key)
        {
            uint endPosition = fileOffset + fileLength;
            if (endPosition > buffer.Size)
            {
                throw new Exception("Buffer to small");
            }

            int rotKeyIndex = 0;
            buffer.Position = (int) fileOffset;
            IBuffer outBuffer = new StreamBuffer();
            while (buffer.Position < endPosition)
            {
                byte al = buffer.ReadByte();
                al = (byte) (al - key[rotKeyIndex]);
                outBuffer.WriteByte(al);
                rotKeyIndex++;
                if (rotKeyIndex >= key.Length)
                {
                    rotKeyIndex = 0;
                }
            }

            return outBuffer;
        }
        private IBuffer EncryptHed(IBuffer inBuff)
        {
            byte bl = 0;
            byte al = 0;

            //Uncomment for US Steam Client
            byte dl = 0xA6;
            byte add = 0x21;

            //Uncomment for US Sunset Client.
            //byte dl = 0xEA;
            //byte sub = 0x0A;

            //Uncomment for Beta Client
            //byte dl = 0x7D;
            //byte sub = 0xC4;

            // Uncomment for JP client
            // dl = 0x67;
            // sub = 0xC7;

            //Uncomment for beta client
            //dl = 0x7D;
            //sub = 0xC4;

            //inBuff.Position = 12;
            inBuff.SetPositionStart();
            IBuffer outBuffer = new StreamBuffer();
            outBuffer.WriteBytes(inBuff.ReadBytes(4));
            outBuffer.WriteInt32(inBuff.Size - 12);
            inBuff.Position = 8;
            outBuffer.WriteInt32(inBuff.ReadInt32());

            int hedPos = inBuff.Position;
            //inBuff.SetPositionStart();
            // WORKING!!!!
            while (inBuff.Position < inBuff.Size)
            {
                byte cl = inBuff.ReadByte();
                cl = (byte)(cl + add);
                bl = (byte)(dl - al);
                bl = (byte)(bl ^ cl);
                al = (byte)(al - 1);
                dl = bl;
                outBuffer.WriteByte(bl);
            }
            return outBuffer;
        }
        private IBuffer EncryptDat(IBuffer buffer, byte[] Key)
        {

            int rotKeyIndex = 0;
            buffer.SetPositionStart();
            IBuffer outBuffer = new StreamBuffer();
            while (buffer.Position < buffer.Size)
            {
                byte al = buffer.ReadByte();
                al = (byte)(al + Key[rotKeyIndex]);
                outBuffer.WriteByte(al);
                rotKeyIndex++;
                if (rotKeyIndex >= Key.Length)
                {
                    rotKeyIndex = 0;
                }
            }

            return outBuffer;
        }

    }
}
