namespace ClaymoreWatchDog
{
    class Program
    {
        static void Main(string[] args)
        {
            ActivityChacker ac = new ActivityChacker(60, @"C:\Users\Miner\Desktop\Claymore's Dual Ethereum+Decred_Siacoin_Lbry_Pascal AMD+NVIDIA GPU Miner v8.0");
            
            ac.StartMonitoring();
        }
    }
}
