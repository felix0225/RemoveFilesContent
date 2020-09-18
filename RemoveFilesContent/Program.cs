using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RemoveFilesContent
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            //移除TXT檔內特定文字的行
            //RemoveTxtLine();

            //移除特定的註解代碼行
            //RemoveMarkCode();

            //移除IISExpress Default WebSite，在Visual Studio專案目錄內 applicationhost.config 的 WebSite1 內容，並增加 index.aspx 的預設文件
            //RemoveIISExpressDefaultWebSite();

            //Console.WriteLine("press any key!!");
            //Console.ReadKey();
        }

        public static void RemoveTxtLine()
        {
            const string filePath = @"E:\EasyFilePrint.txt";
            var filebakPath = filePath.Replace(".txt", DateTime.Today.ToString("yyyyMMdd") + ".txt");

            //先將檔案備份
            var f = new FileInfo(filebakPath);
            if (f.Exists)
                f.Delete();
            File.Copy(filePath, filebakPath);

            var lines = new List<string>(File.ReadAllLines(filePath));
            //使用新的變數存要寫入的資料
            var newLine = new List<string>();

            foreach (var line in lines)
            {
                //不加入特定文字的行
                if (!line.Contains("Properties"))
                {
                    newLine.Add(line);
                }
            }

            //將修改後的資料回寫
            File.WriteAllLines(filePath, newLine.ToArray());
        }

        public static void RemoveMarkCode()
        {
            const string removeCode = @"//if (selObj == null) return retObj;";

            const string SourcePath = @"D:\Dropbox\MyProject\";

            var fileList = Directory.EnumerateFiles(SourcePath, "*BC.cs", SearchOption.AllDirectories);

            foreach (var file in fileList)
            {
                //跳過特定目錄下的檔案
                if (file.Contains("obj") || file.Contains("Properties")) continue;

                var lines = new List<string>(File.ReadAllLines(file));

                var index = lines.FindIndex(x => x.Contains(removeCode));
                if (index <= 0) continue;

                Console.WriteLine("file= " + file);
                Console.WriteLine("index=" + index);

                lines.RemoveAt(index);
                lines.RemoveRange(index, 4);

                //將修改後的資料回寫
                File.WriteAllLines(file, lines.ToArray());
            }
        }

        public static void RemoveIISExpressDefaultWebSite()
        {
            const string SourcePath = @"D:\Dropbox\MyProject\";

            var fileList = Directory.EnumerateFiles(SourcePath, "applicationhost.config", SearchOption.AllDirectories);
            foreach (var file in fileList)
            {
                Console.WriteLine("file= " + file);

                //將檔案的隱藏屬性移除，不然回寫會有問題
                FileAttributes attributes = File.GetAttributes(file);
                if ((attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                {
                    attributes &= ~FileAttributes.Hidden;
                    File.SetAttributes(file, attributes);
                }

                var lines = new List<string>(File.ReadAllLines(file));

                //移除 WebSite1
                var siteIndex = lines.FindIndex(x => x.Contains("name=\"WebSite1\""));
                if (siteIndex > 0)
                {
                    Console.WriteLine("siteIndex=" + siteIndex);
                    lines.RemoveRange(siteIndex, 8);
                }

                //重設新 site 的 id
                var siteIdIndex = lines.FindIndex(x => x.Contains("id=\"2\""));
                if (siteIdIndex > 0)
                {
                    Console.WriteLine("siteIdIndex=" + siteIdIndex);
                    lines[siteIdIndex] = lines[siteIdIndex].Replace("\"2\"", "\"1\"");
                }

                //移除 Default.asp 的預設文件
                var removeIndex = lines.FindIndex(x => x.Contains("Default.asp"));
                if (removeIndex > 0)
                {
                    Console.WriteLine("removeIndex=" + removeIndex);
                    lines.RemoveAt(removeIndex);
                }

                //增加 index.aspx 的預設文件
                var index = lines.FindIndex(x => x.Contains("iisstart.htm"));
                if (index > 0)
                {
                    Console.WriteLine("index=" + index);
                    lines[index] = lines[index].Replace("iisstart.htm", "index.aspx");
                }

                File.WriteAllLines(file, lines.ToArray());
            }
        }
    }
}
