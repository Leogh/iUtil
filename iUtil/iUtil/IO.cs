using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iUtil
{
    public class IO
    {
        /// <summary>
        /// 清空指定的文件夹，并删除文件夹
        /// </summary>
        /// <param name="dir"></param>
        public static void DeleteFolder(string dir)
        {
            foreach (string d in Directory.GetFileSystemEntries(dir))
            {
                if (File.Exists(d))
                {
                    FileInfo fi = new FileInfo(d);
                    if (fi.Attributes.ToString().IndexOf("ReadOnly") != -1)
                        fi.Attributes = FileAttributes.Normal;
                    File.Delete(d);//直接删除其中的文件  
                }
                else
                {
                    DirectoryInfo d1 = new DirectoryInfo(d);
                    DeleteFolder(d1.FullName);////递归删除子文件夹
                }
            }
            Directory.Delete(dir);
        }
        public static void unzipTarGZ(string zipfilename)
        {
            string unzipfilename = "static.tar";
            string dir = "";
            //解压文件夹为空时默认与压缩文件同一级目录下，跟压缩文件同名的文件夹   
            if (dir == "")
                dir = zipfilename.Replace(Path.GetFileName(zipfilename), Path.GetFileNameWithoutExtension(zipfilename));
            if (!dir.EndsWith("//"))
                dir += "//";
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            //创建压缩文件的输入流实例
            using (GZipInputStream zipFile = new GZipInputStream(File.OpenRead(zipfilename)))
            {
                //创建目标文件的流
                using (FileStream destFile = File.Open(dir + unzipfilename, FileMode.Create))
                {
                    int buffersize = 2048;//缓冲区的尺寸，一般是2048的倍数
                    byte[] FileData = new byte[buffersize];//创建缓冲数据
                    while (buffersize > 0)//一直读取到文件末尾
                    {
                        buffersize = zipFile.Read(FileData, 0, buffersize);//读取压缩文件数据
                        destFile.Write(FileData, 0, buffersize);//写入目标文件
                    }
                }
            }
            UnpackTarFiles(dir + unzipfilename, dir);
        }
        /// <summary>
        /// tar包解压
        /// </summary>
        /// <param name="strFilePath">tar包路径</param>
        /// <param name="strUnpackDir">解压到的目录</param>
        /// <returns></returns>
        public static bool UnpackTarFiles(string strFilePath, string strUnpackDir)
        {
            try
            {
                if (!File.Exists(strFilePath))
                {
                    return false;
                }

                strUnpackDir = strUnpackDir.Replace("/", "\\");
                if (!strUnpackDir.EndsWith("\\"))
                {
                    strUnpackDir += "\\";
                }

                if (!Directory.Exists(strUnpackDir))
                {
                    Directory.CreateDirectory(strUnpackDir);
                }

                FileStream fr = new FileStream(strFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                TarInputStream s = new TarInputStream(fr);
                TarEntry theEntry;
                while ((theEntry = s.GetNextEntry()) != null)
                {
                    string directoryName = Path.GetDirectoryName(theEntry.Name);
                    string fileName = Path.GetFileName(theEntry.Name);

                    if (directoryName != String.Empty)
                        Directory.CreateDirectory(strUnpackDir + directoryName);

                    if (fileName != String.Empty)
                    {
                        FileStream streamWriter = File.Create(strUnpackDir + theEntry.Name);

                        int size = 2048;
                        byte[] data = new byte[2048];
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

                        streamWriter.Close();
                    }
                }
                s.Close();
                fr.Close();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        } 
    }
}
