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

                        var pClientPlayerManager = TMem.Instance.Read<IntPtr>(pGameContext + 0x30);
                        var pLocalPlayer = TMem.Instance.Read<IntPtr>(pClientPlayerManager + 0xBC);
                        var pControlledControllable = TMem.Instance.Read<IntPtr>(pLocalPlayer + 0x3D8);
                        var pSoldierWeaponsComponent = TMem.Instance.Read<IntPtr>(pControlledControllable + 0x35C);
                        var pCurrentAnimatedWeaponHandler = TMem.Instance.Read<IntPtr>(pSoldierWeaponsComponent + 0xCC);
                        var pCurrentAnimatedWeapon = TMem.Instance.Read<IntPtr>(pCurrentAnimatedWeaponHandler + 0x14);
                        var pWeapon = TMem.Instance.Read<IntPtr>(pCurrentAnimatedWeapon + 0x154);
                        var pFiringData = TMem.Instance.Read<IntPtr>(pWeapon + 0xC);
                        var pPrimaryFire = TMem.Instance.Read<IntPtr>(pFiringData + 0x8);
                        var numberOfBulletsPerShot = TMem.Instance.Read<IntPtr>(pPrimaryFire + 0x60 + 0x50);

                        var bSunEnabled = TMem.Instance.Read<bool>(pWorldRenderSettings + 0x1BE);
                        var bSkyEnabled = TMem.Instance.Read<bool>(pWorldRenderSettings + 0x1B7);
                        var bLightEnabled = TMem.Instance.Read<bool>(pWorldRenderSettings + 0x1D2);
                        var bNumberOfBulletsPerShot = TMem.Instance.Read<int>(pPrimaryFire + 0x60 + 0x50);

                        Console.WriteLine($"(1): Toggle Sun (Currently: {bSunEnabled})");
                        Console.WriteLine($"(2): Toggle Sky (Currently: {bSkyEnabled})");
                        Console.WriteLine($"(3): Toggle Light (Currently: {bLightEnabled})");
                        Console.WriteLine($"(4): Set NumberOfBulletsPerShot (Currently: {bNumberOfBulletsPerShot})");
                        Console.WriteLine($"(5): Reload");
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
                                    TMem.Instance.Write(pWorldRenderSettings + 0x1D2, !(TMem.Instance.Read<bool>(pWorldRenderSettings + 0x1D2)));
                                    break;
                                case 4:
                                    Console.Write("Enter Value: ");
                                    if(int.TryParse(Console.ReadLine(), out value))
                                    {
                                        if(pWeapon == IntPtr.Zero)
                                        {
                                            Console.Clear();
                                            Console.WriteLine("You have to be ingame.");
                                            Thread.Sleep(1000);
                                            Console.Clear();
                                            continue;
                                        }

                                        TMem.Instance.Write(pPrimaryFire + 0x60 + 0x50, value);
                                    }
                                    break;
                                case 5:
                                    Console.Clear();
                                    Console.WriteLine("Updating...");
                                    Thread.Sleep(1000);
                                    Console.Clear();
                                    continue;
                                case 0:
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