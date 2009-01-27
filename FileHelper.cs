using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LanFinder
{
    public static class FileHelper
    {
        public static List<string> GetFilesRecursive(string path, string searchPattern, int depth)
        {
            List<string> result = new List<string>();
            Stack<string> stack = new Stack<string>();

            stack.Push(path);

            while (stack.Count > 0)
            {
                string dir = stack.Pop();

                try
                {
                    result.AddRange(Directory.GetFiles(dir, searchPattern));

                    if (stack.Count <= depth)
                    {
                        foreach (string dn in Directory.GetDirectories(dir))
                        {
                            stack.Push(dn);
                            if (stack.Count == depth)
                            {
                                break;
                            }
                        }
                    }
                }
                catch
                {
                    // D
                    // Could not open the directory
                }
            }
            return result;
        }
    }
}
