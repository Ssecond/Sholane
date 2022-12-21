using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Sholane
{
    internal class Program
    {
        static void Main(string[] args)
        {
            
            GameWindowSettings gSettings = GameWindowSettings.Default;
            NativeWindowSettings nSettings = new NativeWindowSettings()
            {
                Title = "Sholane",
                Size = (700, 700),
                Flags = ContextFlags.Default,
                Profile = ContextProfile.Compatability,
            };
            Game game = new Game(gSettings, nSettings);
            game.Run();
        }
    }
}