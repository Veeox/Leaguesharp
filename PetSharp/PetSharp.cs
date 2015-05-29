using System;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace PetSharp
{
    public class PetSharp
    {
        private static string DragonBuff = "s5test_dragonslayerbuff";
        private static string BaroonBuff = "exaltedwithbaronnashor";
        private static int AllyD;
        private static int AllyB;
        private static bool DoOnce = false;

        public static Obj_AI_Hero Player { get { return ObjectManager.Player; } }
        

        //Drawing
        public static System.Drawing.Color NotificationColor = System.Drawing.Color.FromArgb(0, 255, 0);
        public static Notification sick1;
        public static Notification sick2;

        public PetSharp()
        {
            new Pet();
            SharpMenu.Run();      

            Notifications.AddNotification("PetSharp: Loaded!", 5).SetTextColor(NotificationColor);
        }

        internal static void Drawing_OnDraw(EventArgs args)
        {
            if (!SharpMenu.Z.Item("drawstats").GetValue<bool>() || !SharpMenu.Z.Item("track").GetValue<bool>())
            {
                return;
            }

            var xpos = 1660;
            var ypos = 640;

            Drawing.DrawText(xpos, ypos, System.Drawing.Color.LightSkyBlue, "PetSharp");
            Drawing.DrawText(xpos, ypos + 20, System.Drawing.Color.LightSkyBlue, "Pet Name: " + Pet.PetName);
            Drawing.DrawText(xpos, ypos + 40, System.Drawing.Color.LightSkyBlue, "Level: " + (int)Pet.Lvl);
            Drawing.DrawText(xpos, ypos + 60, System.Drawing.Color.LightSkyBlue, "XP: " + (int)Pet.CurXP + "/" + (int)Pet.MaxXP);
            Drawing.DrawText(xpos, ypos + 80, System.Drawing.Color.LightSkyBlue, "PetBux: $" + (int)Pet.CashBalance);
            if (Pet.Sick)
            {
                Drawing.DrawText(xpos, ypos + 100, System.Drawing.Color.LightSkyBlue, "Pet Health: Sick (Will die soon!)");
            }
            else
            {
                Drawing.DrawText(xpos, ypos + 100, System.Drawing.Color.LightSkyBlue, "Pet Health: Fine");
            }
        }

        internal static void OnGameNotify(GameNotifyEventArgs args)
        {
            var killer = args.NetworkId;
            var al = FindPlayerByNetworkId(killer);

            switch (args.EventId) //Check for XP events
            {

                case GameEventId.OnChampionDoubleKill:
                    if (killer == Player.NetworkId)
                    {
                        Pet.CurXP += (Pet.MaxXP / 80) * Pet.XPMulti;
                        Pet.CashBalance += 10;
                    }
                    break;
                case GameEventId.OnChampionPentaKill:
                    if (killer == Player.NetworkId)
                    {
                        Pet.CurXP += (Pet.MaxXP / 15) * Pet.XPMulti;
                        Pet.CashBalance += 75;
                    }
                    break;
                case GameEventId.OnChampionQuadraKill:
                    if (killer == Player.NetworkId)
                    {
                        Pet.CurXP += (Pet.MaxXP / 45) * Pet.XPMulti;
                        Pet.CashBalance += 50;
                    }
                    break;
                case GameEventId.OnChampionTripleKill:
                    if (killer == Player.NetworkId)
                    {
                        Pet.CurXP += (Pet.MaxXP / 75) * Pet.XPMulti;
                        Pet.CashBalance += 35;
                    }
                    break;
                case GameEventId.OnAce:
                    var pl = FindPlayerByNetworkId(killer);

                    if (pl != null && pl.IsAlly)
                    {
                        Pet.CurXP += (Pet.MaxXP / 80) * Pet.XPMulti;
                        Pet.CashBalance += 15;
                    }
                    break;
                case GameEventId.OnChampionDie:

                    if (killer == Player.NetworkId)
                    {
                        Pet.CurXP += (Pet.MaxXP / 75) * Pet.XPMulti;
                        Pet.CashBalance += 5;
                    }
                    break;

                //This is a test case - remove me when finished testing!
                case GameEventId.OnDie:
                    if (al != null && al.IsAlly)
                    {
                        Pet.CurXP += Pet.MaxXP / 75;
                        Console.WriteLine("this is a test");
                        Console.WriteLine(Pet.CurXP + "/" + Pet.MaxXP);
                    }
                    break;

                case GameEventId.OnKillWard:
                    if (ObjectManager.Player.IsMe)
                    {
                        KillWard();
                        Console.WriteLine("Killed a ward!");
                    }
                    break;
                case GameEventId.OnChampionKillPost:
                    if (killer == Player.NetworkId && !Pet.Sick)
                    {
                        Console.WriteLine("Sick");
                        if (Pet.Lvl > 2)
                        {
                            Pet.GetSick();
                        }
                    }
                    break;


                //case GameEventId.OnQuit:
                //    ConvertInt(Lvl, CurXP, MaxXP, CashBalance);
                //    break;
            }
        }

        public static Obj_AI_Hero FindPlayerByNetworkId(int id)
        {
            Obj_AI_Hero player = null;
            foreach (var n in HeroManager.AllHeroes)
            {
                if (n.NetworkId == id)
                    player = n;
            }
            return player;
        }

        public static void DragonCheck()
        {
            var allyDbuff = Player.Buffs.Find(x => x.Name == DragonBuff);

            // ally kill dragon
            if (allyDbuff != null && allyDbuff.Count > AllyD)
            {
                AllyD = allyDbuff.Count;
                KillDrag();
            }
            // dragon 5 expiry
            if (allyDbuff != null && allyDbuff.Count < AllyD)
            {
                AllyD = allyDbuff.Count;
            }
        }

        public static void BaroonCheck()
        {
            var allyBbuff = Player.Buffs.Find(x => x.Name == BaroonBuff);

            // ally kill baroon
            if (allyBbuff != null && allyBbuff.Count > AllyB)
            {
                AllyD = allyBbuff.Count;
                KillBaroon();
            }
            // baroon expiry
            if (allyBbuff != null && allyBbuff.Count < AllyB)
            {
                AllyB = allyBbuff.Count;
            }
        }

        private static void WinGame()
        {
            var nexus = ObjectManager.Get<Obj_HQ>().Find(n => n.Health < 1);

            if (nexus == null)
            {
                return;
            }

            if (!DoOnce)
            {
                if (nexus.IsEnemy)
                {
                    Pet.CurXP += Pet.MaxXP / 10;
                    Pet.CashBalance += 100;
                    Converters.ConvertInt(Pet.Lvl, Pet.CurXP, Pet.MaxXP, Pet.CashBalance);
                    if (Pet.Sick)
                    {
                        Pet.PetDie();
                    }
                    DoOnce = true;
                }
                else
                {
                    Converters.ConvertInt(Pet.Lvl, Pet.CurXP, Pet.MaxXP, Pet.CashBalance);
                    if (Pet.Sick)
                    {
                        Pet.PetDie();
                    }
                    DoOnce = true;
                }
            }
        }

        private static void KillDrag()
        {
            Pet.CurXP += (Pet.MaxXP / 30) * Pet.XPMulti;
            Pet.CashBalance += 20;
            Console.WriteLine("Drag Killed");
        }

        private static void KillBaroon()
        {
            Pet.CurXP += (Pet.MaxXP / 50) * Pet.XPMulti;
            Pet.CashBalance += 35;
            Console.WriteLine("Baroon Killed");
        }

        private static void KillWard()
        {
            Pet.CurXP += (Pet.MaxXP / 100) * Pet.XPMulti;
            Pet.CashBalance += 1;
        }

        //End of game Save
        internal static void OnEnd(EventArgs args)
        {
            Converters.ConvertInt(Pet.Lvl, Pet.CurXP, Pet.MaxXP, Pet.CashBalance);
        }

    }
}
