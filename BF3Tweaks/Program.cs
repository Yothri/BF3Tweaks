using System;
using System.Threading;

namespace BF3Tweaks
{
    class Program
    {
        static void Main(string[] args)
        {
            using (TMem.Instance)
            {
                if(TMem.Instance.TryOpen("bf3"))
                {
                    bool killLoop = false;
                    while(!killLoop)
                    {
                        var pGameContext = TMem.Instance.Read<IntPtr>(new IntPtr(0x2380b58));
                        var pClientLevel = TMem.Instance.Read<IntPtr>(pGameContext + 0x10);
                        var pWorldRenderModule = TMem.Instance.Read<IntPtr>(pClientLevel + 0xB8);
                        var pWorldRenderer = TMem.Instance.Read<IntPtr>(pWorldRenderModule + 24);
                        var pWorldRenderSettings = TMem.Instance.Read<IntPtr>(pWorldRenderer + 0x8988);

                        Console.WriteLine("(1): Toggle Sun");
                        Console.WriteLine("(2): Toggle Sky");
                        Console.WriteLine("(0): Exit");

                        Console.WriteLine();
                        Console.Write("$> ");
                        if (int.TryParse(Console.ReadLine(), out var value))
                        {
                            switch (value)
                            {
                                case 1:
                                    TMem.Instance.Write(pWorldRenderSettings + 0x1BE, !(TMem.Instance.Read<bool>(pWorldRenderSettings + 0x1BE)));
                                    break;
                                case 2:
                                    TMem.Instance.Write(pWorldRenderSettings + 0x1B7, !(TMem.Instance.Read<bool>(pWorldRenderSettings + 0x1B7)));
                                    break;
                                case 3:
                                default:
                                    killLoop = true;
                                    continue;
                            }

                            Console.Clear();
                            Console.WriteLine("Success");
                            Thread.Sleep(1000);
                            Console.Clear();
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Unable to obtain handle, please run the game first or make sure you are running as administrator.");
                    Console.ReadLine();
                }
            }
        }
    }
}