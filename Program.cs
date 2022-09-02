using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Memory32;
using System.Threading;
namespace ejakulatExternal
{
    internal class Program
    {
        #region DLL
        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(Keys vKeys);
        #endregion

        #region OFFSETS
        const int localplayer = 0xDC14CC;
        const int forcejump = 0x52878FC;
        const int mflags = 0x104;

        const int dwEntityList = 0x4DDD91C;
        const int teamNum = 0xF4;
        const int mSpotted = 0x93D;

        const int m_flFlashDuration = 0x10470;
        #endregion
        static void Main(string[] args)
        {
            Memory mem = new Memory();
            mem.GetProcess("csgo");
            var client = mem.GetModuleBase("client.dll");

            #region CONSOLE
            Console.Title = $"{mem.proc.Id} ${mem.proc.SessionId}";

            Console.WriteLine(mem.proc.ProcessName.ToUpper(), Console.ForegroundColor=ConsoleColor.DarkRed);
            Console.WriteLine(Convert.ToString(localplayer), Console.ForegroundColor = ConsoleColor.DarkRed);
            Console.WriteLine(Convert.ToString(forcejump), Console.ForegroundColor = ConsoleColor.DarkRed); Console.WriteLine();
            Thread.Sleep(30);
            Console.WriteLine("BHOP ON", Console.ForegroundColor = ConsoleColor.DarkGreen);
            Thread.Sleep(30);
            Console.WriteLine("RADAR HACK ON", Console.ForegroundColor = ConsoleColor.DarkGreen);
            Thread.Sleep(30);
            Console.WriteLine("NO FLASH ON", Console.ForegroundColor = ConsoleColor.DarkGreen);
            #endregion

            Thread ciach = new Thread(bajera) { IsBackground = true};
            ciach.Start();
            
            while (true)
            {
                for (int i = 1; i <= 64; i++)
                {
                    var EntityBuffer = mem.ReadPointer(client, dwEntityList + i * 0x10);
                    var shuldIE = mem.ReadPointer(client, teamNum);
                    var nyga = mem.ReadPointer(EntityBuffer, teamNum);
                    if(nyga == shuldIE)
                        continue;
                    mem.WriteBytes(EntityBuffer, mSpotted, BitConverter.GetBytes(1));
                }
                var buffer = mem.ReadPointer(client, localplayer);
                mem.WriteBytes(buffer,m_flFlashDuration, BitConverter.GetBytes(0));
                

                Thread.Sleep(1);
            }
            
            Console.ReadLine();

            void bajera()
            {
                while (true)
                {
                    if(GetAsyncKeyState(Keys.Space) < 0)
                    {
                        var buffer = mem.ReadPointer(client, localplayer);
                        var flag = BitConverter.ToInt32(mem.ReadBytes(buffer, mflags, 4), 0);

                        if(flag == 257 || flag == 263)
                        {
                            mem.WriteBytes(client, forcejump, BitConverter.GetBytes(5));
                            Thread.Sleep(1);
                            mem.WriteBytes(client, forcejump, BitConverter.GetBytes(4));
                        }
                    }
                    System.Threading.Thread.Sleep(1);
                }
            }
        }
    }
}
