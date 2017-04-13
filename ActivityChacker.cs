using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace ClaymoreWatchDog
{
    class ActivityChacker
    {
        private readonly int _delaySec;
        private bool _processing;
        private readonly DirectoryInfo _minerDir;

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
                    Console.WriteLine(GetLastLogFile().LastAccessTime + " " + DateTime.Now);
                    //TODO: Send email notification
                    //TODO: Reboot OS

                    try
                    {
                        var process = System.Diagnostics.Process.GetProcessesByName("EthDcrMiner64").FirstOrDefault();
                        if (process != null)
                        {
                            try
                            {
                                process.Kill();
                                process.WaitForExit();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Exception while trying to stop miner :{0},{1}", ex.Message, ex.StackTrace);    
                            }
                        }
                        string targetDir = string.Format(_minerDir.FullName);
                        process = new Process
                        {
                            StartInfo =
                            {
                                WorkingDirectory = targetDir,
                                FileName = "startDwarfpoolETH.bat",
                                CreateNoWindow = false
                            }
                        };
                        
                        process.Start();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Exception Occurred :{0},{1}", ex.Message, ex.StackTrace);
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
