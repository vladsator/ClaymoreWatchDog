using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace ClaymoreWatchDog
{
    class ActivityChacker
    {
        private int _delaySec;
        private bool _processing;
        private DirectoryInfo _minerDir;

        public ActivityChacker(int delaySec, string dir)
        {
            _delaySec = delaySec;
            _minerDir = new DirectoryInfo(dir);
        }

        public void StartMonitoring()
        {
            _processing = true;
            Process();
        }

        public void StopMonitoring()
        {
            _processing = false;
        }

        private void Process()
        {
            while (_processing)
            {
                if (GetLastLogFile().LastWriteTime.ToBinary() < DateTime.Now.AddMinutes(-1).ToBinary())
                {
                    Console.WriteLine(GetLastLogFile().LastAccessTime + " " + DateTime.Now.AddMinutes(-1));
                    //TODO: Send email notification
                    //TODO: Reboot OS
                    string command = Path.Combine(_minerDir.FullName, "start — DwarfpoolETH.bat");

                    try
                    {
                        string targetDir = string.Format(_minerDir.FullName);
                        var proc = new Process
                        {
                            StartInfo =
                            {
                                WorkingDirectory = targetDir,
                                FileName = "startDwarfpoolETH.bat",
                                CreateNoWindow = false
                            }
                        };
                        proc.Start();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Exception Occurred :{0},{1}", ex.Message, ex.StackTrace.ToString());
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Last activity time = {0}", GetLastLogFile().LastWriteTime);
                    Console.ResetColor();
                }
                Sleep();
            }
        }

        private FileInfo GetLastLogFile()
        {
            FileInfo[] files = _minerDir.GetFiles("*_log.txt", SearchOption.TopDirectoryOnly);
            FileInfo resFile = files[0];
            foreach (var file in files)
            {
                if (resFile.LastWriteTime.ToBinary() < file.LastWriteTime.ToBinary())
                {
                    resFile = file;
                }   
            }
            return resFile;
        }

        private void Sleep()
        {
            for (int i = 0; i < _delaySec; i++)
            {
                if (!_processing)
                {
                    break;
                }
                Thread.Sleep(1000);
            }
        }
    }
}
