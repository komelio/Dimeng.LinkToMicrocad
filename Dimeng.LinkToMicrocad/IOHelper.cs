using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Dimeng.LinkToMicrocad
{
    public class IOHelper
    {
        public static void CopyDirectory(String sourcePath, String destinationPath)
        {
            DirectoryInfo info = new DirectoryInfo(sourcePath);
            Directory.CreateDirectory(destinationPath);
            foreach (FileSystemInfo fsi in info.GetFileSystemInfos())
            {
                String destName = Path.Combine(destinationPath, fsi.Name);

                if (fsi is System.IO.FileInfo)          //如果是文件，复制文件
                    File.Copy(fsi.FullName, destName);
                else                                    //如果是文件夹，新建文件夹，递归
                {
                    Directory.CreateDirectory(destName);
                    CopyDirectory(fsi.FullName, destName);
                }
            }
        }

        /// <summary>
        /// 将文件名中不合法的字符进行过滤，如各种/\之类的符号，主要不要包含后缀及文件夹路径
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string GetRegularFileName(string filename)
        {
            Regex containsABadCharacter = new Regex("[\\/:*?\"<>|]+");
            return containsABadCharacter.Replace(filename, "_");
        }
    }
}
