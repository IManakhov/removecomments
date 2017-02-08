using System;
using System.Linq;
using System.Text;

namespace RemoveComments
{
    using System.IO;

    class Program
    {
        private static StringBuilder deletedLines = new StringBuilder(); // log deleted comments
		
        static void Main(string[] args)
        {
            const string dirPath = ""; // Path to the folder
            FileSearchFunction(dirPath);
            File.WriteAllText(string.Format("{0}\\deletedLines.txt"), deletedLines.ToString()); //
            var i = Console.ReadLine();
        }

        /// <summary>
        /// Iterate all files in project folder, and call RemoveComments function for a files
        /// </summary>
        /// <param name="dir">folder of solution</param>
        private static void FileSearchFunction(string dir)
        {
            var di = new DirectoryInfo(dir);
            var subDir = di.GetDirectories();
            foreach (DirectoryInfo t in subDir)
            {
                FileSearchFunction(t.FullName);
            }
            var files = di.GetFiles();
            foreach (var file in files)
            {
                if (file.Extension == ".js" || file.Extension == ".cs")
                {
                    RemoveComments(file);
                }
            }
        }

		/// <summary>
        /// Remove comments from a file
        /// </summary>
        private static void RemoveComments(FileInfo file)
        {
            var filePath = string.Format("{0}//{1}", file.DirectoryName, file.Name);
            var result = new StringBuilder();
            using (var fs = new StreamReader(filePath))
            {
                var commentNeedDelete = false;
                while (true)
                {
                    var commentCont = false;
                    var temp = fs.ReadLine();
                    if (temp == null) break;
                    if (temp.Contains("//"))
                    {
                        var tmpArray = temp.Split(new[] { "//" }, StringSplitOptions.None);
                        var char1 = tmpArray[0].Count(x => x == '\"');
                        var cha2 = tmpArray[0].Count(x => x == '\'');
                        if (char1 % 2 == 0 && cha2 % 2 == 0)
                        {
                            deletedLines.Append(string.Format("{0}\n", temp));
                            temp = tmpArray[0];
                            if (string.IsNullOrEmpty(temp) || string.IsNullOrEmpty(temp.Replace("\t","").Trim())) continue;
                        }
                    }
                    if (temp.Contains("/*") && !temp.Contains("\"./*"))
                    {
                        var tmpArray = temp.Replace("/*", "Ё").Replace("*/", "Ё").Split('Ё');
                        var indx = 0;
                        var resultStr = string.Empty;
                        foreach (var item in tmpArray)
                        {
                            indx++;
                            if (indx % 2 == 0) continue;
                            resultStr += item;
                        }
                        deletedLines.Append(string.Format("{0}\n", temp));
                        var tmpCount = temp.Replace("/*", "Ё").Count(x => x == 'Ё') > temp.Replace("*/", "Ё").Count(x => x == 'Ё');
                        if (tmpCount)
                        {
                            if (!string.IsNullOrEmpty(temp.Replace("\t", "").Trim()))
                            {
                                commentCont = true;
                            }
                            commentNeedDelete = true;
                        }
                        temp = resultStr;
                        if (string.IsNullOrEmpty(temp)) continue;
                    }
                    if (commentNeedDelete && !commentCont)
                    {
                        if (temp.Contains("*/")) commentNeedDelete = false;
                        deletedLines.Append(string.Format("{0}\n", temp));
                    }
                    else result.Append(string.Format("{0}\n",temp));
                }
            }
            var str = result.ToString();
            File.WriteAllText(filePath, str);
        }
    }
}
