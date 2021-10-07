using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Core;
using System.IO;
using System;

namespace fs
{
    /// <summary>
    /// zip工具类，依赖SharpZipLib库
    /// </summary>
    public sealed class ZipUtils
    {
        /// <summary>
        /// 压缩多个文件/文件夹
        /// </summary>
        /// <param name="sourceList">源文件/文件夹路径列表</param>
        /// <param name="zipFilePath">压缩文件路径和名称</param>
        /// <param name="comment">注释信息</param>
        /// <param name="password">压缩密码</param>
        /// <param name="compressionLevel">压缩等级，范围从0到9，可选，默认为6</param>
        /// <returns></returns>
        public static bool ZipFile(IEnumerable<string> sourceList, string zipFilePath, string comment = "", string password = "", int compressionLevel = 6)
        {
            int BufferSize = 2048;
            bool result = false;
            try
            {
                //检测目标文件所属的文件夹是否存在，如果不存在则建立
                string zipFileDirectory = Path.GetDirectoryName(zipFilePath);
                if (!string.IsNullOrEmpty(zipFileDirectory) && !Directory.Exists(zipFileDirectory))
                {
                    Directory.CreateDirectory(zipFileDirectory);
                }

                Dictionary<string, string> dictionaryList = PrepareFileSystementities(sourceList);

                using (ZipOutputStream zipStream = new ZipOutputStream(File.Create(zipFilePath)))
                {
                    zipStream.Password = password;//设置密码
                    zipStream.SetComment(comment);//添加注释
                    zipStream.SetLevel(CheckCompressionLevel(compressionLevel));//设置压缩等级

                    foreach (string key in dictionaryList.Keys)//从字典取文件添加到压缩文件
                    {
                        if (File.Exists(key))//判断是文件还是文件夹
                        {
                            FileInfo fileItem = new FileInfo(key);

                            using (FileStream readStream = fileItem.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
                            {
                                ZipEntry zipEntry = new ZipEntry(dictionaryList[key]);
                                zipEntry.DateTime = fileItem.LastWriteTime;
                                zipEntry.Size = readStream.Length;
                                zipStream.PutNextEntry(zipEntry);
                                int readLength = 0;
                                byte[] buffer = new byte[BufferSize];

                                do
                                {
                                    readLength = readStream.Read(buffer, 0, BufferSize);
                                    zipStream.Write(buffer, 0, readLength);
                                } while (readLength == BufferSize);

                                readStream.Close();
                            }
                        }
                        else//对文件夹的处理
                        {
                            ZipEntry zipEntry = new ZipEntry(dictionaryList[key] + "/");
                            zipStream.PutNextEntry(zipEntry);
                        }
                    }

                    zipStream.Flush();
                    zipStream.Finish();
                    zipStream.Close();
                }

                result = true;
            }
            catch (System.Exception e)
            {
                Debuger.LogError("压缩文件失败" + e.ToString());
            }

            return result;
        }
        public static bool ZipFile(string sourcePath, string zipFilePath, string comment = "", string password = "", int compressionLevel = 6)
        {
            List<string> list = new List<string>();
            list.Add(sourcePath);
            return ZipFile(list, zipFilePath, comment, password, compressionLevel);
        }
        /// <summary>
        /// 解压文件
        /// </summary>
        /// <param name="zipFilePath">源文件</param>
        /// <param name="unzipFilePath">解压目录</param>
        /// <param name="file_name">针对只有一个压缩文件的情况，重新命名解压文件</param>
        public static void UnzipFile(string zipFilePath, string unzipFilePath, string unzipFileName = "")
        {
            if (!File.Exists(zipFilePath))
            {
                Debuger.LogError("Cannot find file:" + zipFilePath);
                return;
            }

            int size = 2048;
            byte[] data = new byte[2048];
            try
            {
                using (ZipInputStream s = new ZipInputStream(File.OpenRead(zipFilePath)))
                {
                    ZipEntry theEntry;
                    while ((theEntry = s.GetNextEntry()) != null)
                    {
                        Debuger.Log("unzip file:" + theEntry.Name);

                        string directoryName = Path.Combine(unzipFilePath, Path.GetDirectoryName(theEntry.Name));
                        string fileName = Path.GetFileName(theEntry.Name);
                        if (!string.IsNullOrEmpty(unzipFileName)) fileName = unzipFileName;

                        if (!string.IsNullOrEmpty(directoryName) || !Directory.Exists(directoryName))
                        {
                            Directory.CreateDirectory(directoryName);
                        }

                        if (!string.IsNullOrEmpty(fileName))
                        {
                            string file_path = Path.Combine(directoryName, fileName);
                            using (FileStream streamWriter = File.Create(file_path))
                            {
                                while (true)
                                {
                                    size = s.Read(data, 0, data.Length);
                                    if (size > 0)
                                    {
                                        streamWriter.Write(data, 0, size);
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                Debuger.LogError("解压文件失败:" + e.ToString());
            }
        }

        /// <summary>
        /// 压缩字节数组
        /// </summary>
        /// <param name="sourceBytes">源字节数组</param>
        /// <param name="compressionLevel">压缩等级</param>
        /// <param name="password">密码</param>
        /// <returns>压缩后的字节数组</returns>
        public static byte[] ZipBytes(byte[] sourceBytes, string password = "", int compressionLevel = 6)
        {
            int BufferSize = 2048;
            byte[] result = new byte[] { };

            if (sourceBytes.Length > 0)
            {
                try
                {
                    using (MemoryStream tempStream = new MemoryStream())
                    {
                        using (MemoryStream readStream = new MemoryStream(sourceBytes))
                        {
                            using (ZipOutputStream zipStream = new ZipOutputStream(tempStream))
                            {
                                zipStream.Password = password;//设置密码
                                zipStream.SetLevel(CheckCompressionLevel(compressionLevel));//设置压缩等级

                                ZipEntry zipEntry = new ZipEntry("ZipBytes");
                                zipEntry.DateTime = DateTime.Now;
                                zipEntry.Size = sourceBytes.Length;
                                zipStream.PutNextEntry(zipEntry);
                                int readLength = 0;
                                byte[] buffer = new byte[BufferSize];

                                do
                                {
                                    readLength = readStream.Read(buffer, 0, BufferSize);
                                    zipStream.Write(buffer, 0, readLength);
                                } while (readLength == BufferSize);

                                readStream.Close();
                                zipStream.Flush();
                                zipStream.Finish();
                                result = tempStream.ToArray();
                                zipStream.Close();
                            }
                        }
                    }
                }
                catch (System.Exception e)
                {
                    Debuger.LogError("压缩字节数组发生错误:" + e.ToString());
                }
            }

            return result;
        }

        /// <summary>
        /// 解压字节数组
        /// </summary>
        /// <param name="sourceBytes">源字节数组</param>
        /// <param name="password">密码</param>
        /// <returns>解压后的字节数组</returns>
        public static byte[] UnzipBytes(byte[] sourceBytes, string password = "")
        {
            int BufferSize = 2048;
            byte[] result = new byte[] { };

            if (sourceBytes.Length > 0)
            {
                try
                {
                    using (MemoryStream tempStream = new MemoryStream(sourceBytes))
                    {
                        using (MemoryStream writeStream = new MemoryStream())
                        {
                            using (ZipInputStream zipStream = new ZipInputStream(tempStream))
                            {
                                zipStream.Password = password;
                                ZipEntry zipEntry = zipStream.GetNextEntry();

                                if (zipEntry != null)
                                {
                                    byte[] buffer = new byte[BufferSize];
                                    int readLength = 0;

                                    do
                                    {
                                        readLength = zipStream.Read(buffer, 0, BufferSize);
                                        writeStream.Write(buffer, 0, readLength);
                                    } while (readLength == BufferSize);

                                    writeStream.Flush();
                                    result = writeStream.ToArray();
                                    writeStream.Close();
                                }
                                zipStream.Close();
                            }
                        }
                    }
                }
                catch (System.Exception e)
                {
                    Debuger.LogError("解压字节数组发生错误:" + e.ToString());
                }
            }
            return result;
        }

        /// <summary>
        /// 为压缩准备文件系统对象
        /// </summary>
        /// <param name="sourceFileEntityPathList"></param>
        /// <returns></returns>
        private static Dictionary<string, string> PrepareFileSystementities(IEnumerable<string> sourceFileEntityPathList)
        {
            Dictionary<string, string> fileEntityDictionary = new Dictionary<string, string>();//文件字典
            string parentDirectoryPath = "";
            foreach (string fileEntityPath in sourceFileEntityPathList)
            {
                string path = fileEntityPath;
                //保证传入的文件夹也被压缩进文件
                if (path.EndsWith(@"\"))
                {
                    path = path.Remove(path.LastIndexOf(@"\"));
                }

                parentDirectoryPath = Path.GetDirectoryName(path) + @"\";

                if (parentDirectoryPath.EndsWith(@":\\"))//防止根目录下把盘符压入的错误
                {
                    parentDirectoryPath = parentDirectoryPath.Replace(@"\\", @"\");
                }

                //获取目录中所有的文件系统对象
                Dictionary<string, string> subDictionary = GetAllFileSystemEntities(path, parentDirectoryPath);

                //将文件系统对象添加到总的文件字典中
                foreach (string key in subDictionary.Keys)
                {
                    if (!fileEntityDictionary.ContainsKey(key))//检测重复项
                    {
                        fileEntityDictionary.Add(key, subDictionary[key]);
                    }
                }
            }
            return fileEntityDictionary;
        }

        /// <summary>
        /// 获取所有文件系统对象
        /// </summary>
        /// <param name="source">源路径</param>
        /// <param name="topDirectory">顶级文件夹</param>
        /// <returns>字典中Key为完整路径，Value为文件(夹)名称</returns>
        private static Dictionary<string, string> GetAllFileSystemEntities(string source, string topDirectory)
        {
            Dictionary<string, string> entitiesDictionary = new Dictionary<string, string>();
            entitiesDictionary.Add(source, source.Replace(topDirectory, ""));

            if (Directory.Exists(source))
            {
                //一次性获取下级所有目录，避免递归
                string[] directories = Directory.GetDirectories(source, "*.*", SearchOption.AllDirectories);
                foreach (string directory in directories)
                {
                    entitiesDictionary.Add(directory, directory.Replace(topDirectory, ""));
                }

                string[] files = Directory.GetFiles(source, "*.*", SearchOption.AllDirectories);
                foreach (string file in files)
                {
                    entitiesDictionary.Add(file, file.Replace(topDirectory, ""));
                }
            }

            return entitiesDictionary;
        }

        private static int CheckCompressionLevel(int lv)
        {
            return Mathf.Clamp(lv, 0, 9);
        }
    }
}