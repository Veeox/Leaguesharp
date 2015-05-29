using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace PetSharp
{
    public class Program
    {
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        static void Game_OnGameLoad(EventArgs args)
        {
            var Bot = PetSharp.Bots();
            if (Bot)
            {
                Notifications.AddNotification("PetSharp: Not Loaded With Bots!").SetTextColor(System.Drawing.Color.FromArgb(255, 0, 0));
                return;
            }
            else
                new PetSharp();
        }
    }
}
