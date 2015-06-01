using System;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using Color = System.Drawing.Color;


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
        private static bool HasBaron = false;
        private static bool DoOnce = false;
        public static float QuadraDelay;
        public static float DoubleDelay;
        public static float TrippleDelay;
        public static float PentaDelay;
        public static float AceDelay;
        public static float WardDelay;
        public static float bDelay;

        public static Obj_AI_Hero Player { get { return ObjectManager.Player; } }
        

        //Drawing
        public static System.Drawing.Color NotificationColor = System.Drawing.Color.FromArgb(0, 255, 0);
        public static Notification sick1;
        public static Notification sick2;

        public static int xpos = 0;
        public static int ypos = 0;
        public static int drawX1 = (int)(Drawing.Width * 0.68);
        public static int drawY1 = (int)(Drawing.Height * 0.97);
        public static int drawX2 = (int)(Drawing.Width * 0.68);
        public static int drawY2 = (int)(Drawing.Height * 0.97) - 40;
        public static int myTeamDmgX = (int)(Drawing.Width * 0.68);
        public static int myTeamDmgY = (int)(Drawing.Height * 0.97) - 20;
        public static int enemyTeamDmgX = (int)(Drawing.Width * 0.68);
        public static int enemyTeamDmgY = (int)(Drawing.Height * 0.97) - 60;

            //Sprite
        public static Render.Sprite sprite = null;
        public static string petSprite;

        public PetSharp()
        {
            new Pet();
            SharpMenu.Run();

            Drawing.OnDraw += Drawing_OnDraw;

            if (!SharpMenu.Z.Item("disDraw").GetValue<bool>())
            {
            
                if (SharpMenu.Z.Item("drawsprites").GetValue<bool>() && sprite != null)
                {
                    Save.ReadSave();
                    Console.WriteLine(Pet.mySprite);

                    //sprite = new Render.Sprite(Resources.Resource1.g4205, new Vector2(xpos + 20, ypos - 75));
                    Console.WriteLine(sprite);
                    DrawSprite();
                }
                else
                {
                    Save.ReadSave();
                    DrawSprite();
                    sprite.Hide();
                }
            }

            Notifications.AddNotification("PetSharp: Loaded!", 5).SetTextColor(NotificationColor);
        }

        internal static void Drawing_OnDraw(EventArgs args)
        {
            if (!SharpMenu.Z.Item("track").GetValue<bool>())
            {
                return;
            }

            if (sprite == null)
            {
                DrawSprite();
                sprite.Hide();
            }

            xpos = SharpMenu.Z.Item("xpos").GetValue<Slider>().Value;
            ypos = SharpMenu.Z.Item("ypos").GetValue<Slider>().Value;

            if (SharpMenu.Z.Item("drawstats").GetValue<bool>() && !SharpMenu.Z.Item("disDraw").GetValue<bool>())
                {
                //Drawing Box

                var borderColor = Color.Black;
                var bgColor = Color.LightGray;
                var textColor = Color.Black;

            
                    //Left
                    Drawing.DrawLine(xpos - 7,
                                     ypos + 120,
                                     xpos - 7,
                                     ypos - 90, 3,
                                     borderColor);
                    //Top
                    Drawing.DrawLine(xpos - 6,
                                     ypos - 92,
                                     xpos + 170,
                                     ypos - 92, 3,
                                     borderColor);
                    //Right
                    Drawing.DrawLine(xpos + 168,
                                     ypos + 120,
                                     xpos + 168,
                                     ypos - 91, 3,
                                     borderColor);
                    //Bottom
                    Drawing.DrawLine(xpos - 6,
                                     ypos + 120,
                                     xpos + 171,
                                     ypos + 120, 3,
                                     borderColor);
                    //Drawing Background

                    Drawing.DrawLine(xpos - 4,
                                     ypos + 120,
                                     xpos - 4,
                                     ypos - 88, 172,
                                     bgColor);

                //Drawing Stats
                
                Drawing.DrawText(xpos + 25, ypos - 85, System.Drawing.Color.DarkRed, "PetSharp BETA");
                Drawing.DrawText(xpos, ypos + 20, textColor, "Pet Name: " + Pet.PetName);
                Drawing.DrawText(xpos, ypos + 40, textColor, "Level: " + (int)Pet.Lvl);
                Drawing.DrawText(xpos, ypos + 60, textColor, "XP: " + (int)Pet.CurXP + "/" + (int)Pet.MaxXP);
                Drawing.DrawText(xpos, ypos + 80, textColor, "PetBux: $" + (int)Pet.CashBalance);
                if (Pet.Sick)
                {
                    Drawing.DrawText(xpos, ypos + 100, System.Drawing.Color.Red, "Pet Health: Sick (Will die soon!)");
                }
                else
                {
                    Drawing.DrawText(xpos, ypos + 100, textColor, "Pet Health: Fine");
                }
            }

            if (SharpMenu.Z.Item("drawsprites").GetValue<bool>() && sprite != null)
            {
                
                sprite.Show();
                sprite.X = xpos + 35;
                sprite.Y = ypos - 60;

            }
            else
            {
                sprite.Hide();
            }

            if (SharpMenu.Z.Item("disDraw").GetValue<bool>())
            {
                sprite.Hide();
            }
        }
        
        public static void DrawSprite()
        {
            xpos = SharpMenu.Z.Item("xpos").GetValue<Slider>().Value;
            ypos = SharpMenu.Z.Item("ypos").GetValue<Slider>().Value;
                        
            //Draw Sprites
                //Get Pet Sprite
            GetPetSprite();
            sprite.Scale = new Vector2(0.12f, 0.12f);
            sprite.Add();

        }

        public static void GetPetSprite()
        {
            switch (Pet.mySprite)
            {
                case "g4148":
                    sprite = new Render.Sprite(Resources.Resource1.g4148, new Vector2(xpos + 20, ypos - 75));
                    break;
                case "g4174":
                    sprite = new Render.Sprite(Resources.Resource1.g4174, new Vector2(xpos + 20, ypos - 75));
                    break;
                case "g4205":
                    sprite = new Render.Sprite(Resources.Resource1.g4205, new Vector2(xpos + 20, ypos - 75));
                    break;
                case "g4238":
                    sprite = new Render.Sprite(Resources.Resource1.g4238, new Vector2(xpos + 20, ypos - 75));
                    break;
                case "path4249":
                    sprite = new Render.Sprite(Resources.Resource1.path4249, new Vector2(xpos + 20, ypos - 75));
                    break;
            }
        }

        public static bool Bots()
        {
            var CountBots = 0;
            var bot = false;
            var isAram = Utility.Map.GetMap().Name;
            Console.WriteLine(isAram);

            if (HeroManager.AllHeroes.Count <= 0 || isAram.ToString() == "Howling Abyss")
            {
                bot = true;
            }
            else
            {
                foreach (var n in HeroManager.AllHeroes)
                {
                    if (n.Name.Contains(" ssBot"))
                        CountBots++;
                }
                if (CountBots > 1)
                {
                    bot = true;
                }
            }
            return bot;
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
                        if (Game.Time > DoubleDelay)
                        {
                            Pet.CurXP += (Pet.MaxXP / 80) * Pet.XPMulti;
                            Pet.CashBalance += 10;
                            DoubleDelay = Game.Time + 3000;
                        }
                        
                    }
                    break;
                case GameEventId.OnChampionPentaKill:

                    if (killer == Player.NetworkId)
                    {
                        if (Game.Time > PentaDelay)
                        {
                            Pet.CurXP += (Pet.MaxXP / 15) * Pet.XPMulti;
                            Pet.CashBalance += 75;
                            PentaDelay = Game.Time + 3000;
                        }
                        
                    }
                    break;
                case GameEventId.OnChampionQuadraKill:

                    if (killer == Player.NetworkId)
                    {
                        if (Game.Time > QuadraDelay)
                        {
                            Pet.CurXP += (Pet.MaxXP / 45) * Pet.XPMulti;
                            Pet.CashBalance += 50;
                            QuadraDelay = Game.Time + 3000;
                        }
                        
                    }
                    break;
                case GameEventId.OnChampionTripleKill:

                    if (killer == Player.NetworkId)
                    {
                        if (Game.Time > TrippleDelay)
                        {
                            Pet.CurXP += (Pet.MaxXP / 75) * Pet.XPMulti;
                            Pet.CashBalance += 35;
                            TrippleDelay = Game.Time + 3000;
                        }
                        
                    }
                    break;
                case GameEventId.OnAce:
                    
                    var pl = FindPlayerByNetworkId(killer);
                    if (Game.Time > AceDelay)
                    {
                        if (pl != null && pl.IsAlly)
                        {
                            Pet.CurXP += (Pet.MaxXP / 80) * Pet.XPMulti;
                            Pet.CashBalance += 15;
                            AceDelay = Game.Time + 3000;
                        }
                    }
                    break;
                case GameEventId.OnChampionDie:

                    if (killer == Player.NetworkId)
                    {
                        Pet.CurXP += (Pet.MaxXP / 75) * Pet.XPMulti;
                        Pet.CashBalance += 5;
                    }
                    break;
                case GameEventId.OnKillWard:

                    if (ObjectManager.Player.IsMe)
                    {
                        if (Game.Time > WardDelay)
                        {
                            KillWard();
                            WardDelay = Game.Time + 3000;
                        }
                        
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
            if (Game.Time > bDelay)
            {
                // ally kill baroon
                if (allyBbuff != null && allyBbuff.Count > AllyB && !HasBaron)
                {
                    AllyB = allyBbuff.Count;
                    HasBaron = true;
                    KillBaroon();
                    bDelay = Game.Time + 3000;
                }
            }
            // baroon expiry
            if (allyBbuff != null && allyBbuff.Count < AllyB)
            {
                AllyB = allyBbuff.Count;
                HasBaron = false;
            }
        }
        
        public static void WinGame()
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
            sprite.Hide();
            Converters.ConvertInt(Pet.Lvl, Pet.CurXP, Pet.MaxXP, Pet.CashBalance);
        }

    }
}
