using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.IO;

using SharpDX;
using LeagueSharp.Common;
using LeagueSharp;

namespace PetSharp
{
    public class Pet
    {
        //Main Pet Vars
        public static int CurXP;
        public static int MaxXP;
        public static int Lvl;
        public static string PetName;
        public static int CashBalance = 0;
        public static bool Sick = false;
        public static bool FoodXP = false;
        public static int XPMulti = 1;

        public Pet()
        {
            Save.SaveData();
            Game.OnUpdate += OnUpdate;
            CustomEvents.Game.OnGameEnd += PetSharp.OnEnd;
            Drawing.OnDraw += PetSharp.Drawing_OnDraw;
            Game.OnNotify += PetSharp.OnGameNotify;
        }

        public static void OnUpdate(EventArgs args)
        {
            //Check if enabled
            if (!SharpMenu.Z.Item("track").GetValue<bool>())
            {
                return;
            }
            else
            {
                Save.NewPet();
                PetSharp.DragonCheck();
                PetSharp.BaroonCheck();
                LevelUp();
                Shop.ShopBuy();
                Save.ManualSave();
                PetSharp.WinGame();
            }
        }

        public static void LevelUp()
        {
            if (CurXP >= MaxXP)
            {
                CurXP = (CurXP - MaxXP);
                MaxXP = (MaxXP * 2);
                Lvl++;
                Notifications.AddNotification("PetSharp: Leveled up!", 2).SetTextColor(PetSharp.NotificationColor);
            }
        }

        public static void GetSick()
        {
            Random rnd = new Random();

            int r = rnd.Next(10) + 1;

            if (r >= 7)
            {
                Sick = true;
                PetSharp.sick1 = Notifications.AddNotification("PetSharp: Your pet is sick!").SetTextColor(PetSharp.NotificationColor);
                PetSharp.sick2 = Notifications.AddNotification("PetSharp: Buy Medicine from the Shop!").SetTextColor(PetSharp.NotificationColor);
            }
        }

        public static void PetDie()
        {
            Notifications.RemoveNotification(PetSharp.sick1);
            Notifications.RemoveNotification(PetSharp.sick2);
            PetSharp.sick1 = null;
            PetSharp.sick2 = null;
            Notifications.AddNotification("PetSharp: Your pet has died!", 30).SetTextColor(PetSharp.NotificationColor);
            Save.FirstRun();
            Notifications.AddNotification("PetSharp: New Pet Created!", 30).SetTextColor(PetSharp.NotificationColor);
        }
    }
        
}
