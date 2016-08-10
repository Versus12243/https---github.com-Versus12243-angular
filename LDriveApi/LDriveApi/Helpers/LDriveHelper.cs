using LDriveWebApi.Hubs;
using LDriveWebApi.Models;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.SessionState;

namespace LDriveWebApi.Helpers
{
    public static class LDriveHelper
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static ListOfLinksWithFilesInfo GetDirectoriInfo(string root, string sessionId)
        {
            ListOfLinksWithFilesInfo result = new ListOfLinksWithFilesInfo();
            object m = new object();
            bool finish = false;
            result.Links = new List<LinkModel>();
            if(root == null)
            {
                root = System.IO.Directory.GetCurrentDirectory();                 
            }

            Task.Factory.StartNew(() =>
            {
                while (!finish)
                {
                    Thread.Sleep(3000);
                    lock (m)
                    {
                        IHubContext context = GlobalHost.ConnectionManager.GetHubContext<SimpleHub>();
                        context.Clients.All.getResults(result, sessionId);
                    }                    
                    }
                });
           
            if (root != "..")
            {
                lock (m)
                {
                    result.CurrentPath = root;
                }
                Stack<string> dirs = new Stack<string>();

                if (!System.IO.Directory.Exists(root))
                {
                    throw new ArgumentException();
                }

                dirs.Push(root);

                var rootDir = new System.IO.DirectoryInfo(root);
                lock (m)
                {
                    if (rootDir.Parent != null)
                    {
                        result.Links.Add(new LinkModel { Text = "..", Link = rootDir.Parent.FullName });
                    }
                    else
                    {
                        result.Links.Add(new LinkModel { Text = "..", Link = ".." });
                    }
                }
                while (dirs.Count > 0)
                {
                    string currentDir = dirs.Pop();
                    string[] subDirs = null;

                    try
                    {
                        subDirs = System.IO.Directory.GetDirectories(currentDir);
                    }
                    catch (UnauthorizedAccessException e)
                    {
                        logger.Info(e.Message, e);                       
                    }
                    catch (System.IO.DirectoryNotFoundException e)
                    {
                        logger.Info(e.Message, e);
                    }
                    catch (System.IO.PathTooLongException e)
                    {
                        logger.Info(e.Message, e);
                        continue;
                    }
                    string[] files = null;

                    try
                    {
                        files = System.IO.Directory.GetFiles(currentDir);
                    }
                    catch (UnauthorizedAccessException e)
                    {
                        logger.Info(e.Message, e);
                        continue;
                    }
                    catch (System.IO.DirectoryNotFoundException e)
                    {
                        logger.Info(e.Message, e);
                        continue;
                    }
                    catch (System.IO.PathTooLongException e)
                    {
                        logger.Info(e.Message, e);
                        continue;
                    }
                        foreach (string file in files)
                        {
                            try
                            {
                                System.IO.FileInfo fi = new System.IO.FileInfo(file);

                                var fileSize = ConvertBytesToMegabytes(fi.Length);
                                lock (m)
                                {
                                    if (fileSize <= 10)
                                        result.Less10Mb++;
                                    else if (fileSize <= 50)
                                        result.From10To50Mb++;
                                    else if (fileSize >= 100)
                                        result.More100Mb++;
                                }
                            }
                            catch(System.IO.FileNotFoundException e)
                            {
                                logger.Info(e.Message, e);
                                continue;
                            }
                            catch(System.IO.PathTooLongException e)
                            {
                                logger.Info(e.Message, e);
                                continue;
                            }
                        }

                    if(currentDir == root)
                    {
                        lock (m)
                        {
                            foreach (string str in subDirs)
                            {
                                dirs.Push(str);
                                var dir = new System.IO.DirectoryInfo(str);
                                result.Links.Add(new LinkModel { Text = dir.Name, Link = dir.FullName });
                            }
                            foreach (string file in files)
                            {
                                System.IO.FileInfo fi = new System.IO.FileInfo(file);
                                result.Links.Add(new LinkModel { Text = fi.Name });
                            }
                        }
                    }
                    else
                    {
                        if (subDirs != null)
                        {
                            foreach (string str in subDirs)
                                dirs.Push(str);
                        }
                    }
                }
            } 
            else
            {
               var drives = System.IO.DriveInfo.GetDrives();
               lock (m)
               {
                   foreach (var drive in drives)
                   {
                       result.Links.Add(new LinkModel { Link = drive.Name, Text = drive.Name });
                   }
               }
            }

            finish = true;

            return result;
        }

        static double ConvertBytesToMegabytes(long bytes)
        {
            return (bytes / 1024f) / 1024f;
        }


    }
}