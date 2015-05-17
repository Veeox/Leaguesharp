#region

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

#endregion

#region ToDo

    //Name generator - #Done! (might be inefficient)
    //Pet Sprite
    //Cash system
    //Shop system
        //Buff Food
        //Needs moar ideas pls
    //Event System fixes and testing
    //Battles?
    //Test XP gains
    //Save pet info to server
    //Top scores/leaderboard
    //anti-cheat
    //GUI Stuff
        //Shop
        //Sprites for pets

#endregion

namespace PetSharp
{
    class Program
    {
        public static int CurXP;
        public static int MaxXP;
        public static int Lvl;
        public static string PetName;
        
        //Drag and Baroon Stuff
        private static string DragonBuff = "s5test_dragonslayerbuff";
        private static string BaroonBuff = "exaltedwithbaronnashor";
        private static int AllyD;
        private static int AllyB;

        public static string FileName;

        private static Obj_AI_Minion Baron { get; set; }
        private static Obj_AI_Minion Dragon { get; set; }

        public static Menu Menu;
        public const string Ver = "0.0.1.5";

        private static System.Drawing.Color NotificationColor = System.Drawing.Color.FromArgb(0, 255, 0);

        private static Obj_AI_Hero Player { get { return ObjectManager.Player; } }


        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += OnLoad;
            
        }

        private static void OnLoad(EventArgs args)
        {
            InitEvents();

            //Grab data from text file else create it
            FileName = "PetSharp.txt";
            if (!Directory.Exists(Config.AppDataDirectory + @"\PetSharp"))
            {
                Directory.CreateDirectory(Config.AppDataDirectory + @"\PetSharp");
                FirstRun();
                
            }
            //else read the save
            else
            {
                ReadSave();
           
            }

            // Menu
            Menu = new Menu("PetSharp v." + Ver, "petsharp", true);

            //shop menu
            Menu.AddSubMenu(new Menu("PetSharp Shop", "shop"));
            Menu.SubMenu("shop").AddItem(new MenuItem("bfood", "Buff Food"));
            Menu.SubMenu("shop").AddItem(new MenuItem("food1", "Buy Food 1").SetValue(false));
            Menu.SubMenu("shop").AddItem(new MenuItem("food2", "Buy Food 2").SetValue(false));
            Menu.SubMenu("shop").AddItem(new MenuItem("food3", "Buy Food 3").SetValue(false));

            //Draw menu
            Menu.AddSubMenu(new Menu("Drawings", "draw"));
            Menu.SubMenu("draw").AddItem(new MenuItem("drawstats", "Draw Stats").SetValue(true));
            Menu.SubMenu("draw").AddItem(new MenuItem("drawsprites", "Draw Sprites").SetValue(true));

            //Misc menu
            Menu.AddSubMenu(new Menu("Misc", "misc"));
            Menu.SubMenu("misc").AddItem(new MenuItem("track", "Track Game").SetValue(true));
            Menu.SubMenu("misc").AddItem(new MenuItem("new", "New Pet (Start Over)").SetValue(false));

            Menu.AddToMainMenu();
            
            
        }

        static void Drawing_OnDraw(EventArgs args)
        {
            if (!Menu.Item("drawstats").GetValue<bool>() || !Menu.Item("track").GetValue<bool>())
            {
                return;
            }
            var xpos = 1660;
            var ypos = 680;

            Drawing.DrawText(xpos, ypos, System.Drawing.Color.LightSkyBlue, "PetSharp v" + Ver + " by Veeox");
            Drawing.DrawText(xpos, ypos + 20, System.Drawing.Color.LightSkyBlue, "Pet Name: " + PetName);
            Drawing.DrawText(xpos, ypos + 40, System.Drawing.Color.LightSkyBlue, "Level: " + (int)Lvl);
            Drawing.DrawText(xpos, ypos + 60, System.Drawing.Color.LightSkyBlue, "XP: " + (int)CurXP + "/" + (int)MaxXP);
        }

        private static void OnUpdate(EventArgs args)
        {
            
            //Check if enabled
            if (!Menu.Item("track").GetValue<bool>())
            {
                return;
            }
            else
            {
                NewPet();
                DragonCheck();
                BaroonCheck();
                GainXP();
                ShopBuy();
            }
            

        }

        //Initialize all the things!
        private static void InitEvents()
        {
            Game.OnUpdate += OnUpdate;
            CustomEvents.Game.OnGameEnd += OnEnd;
            Game.OnNotify += OnGameNotify;
            Drawing.OnDraw += Drawing_OnDraw;
            Notifications.AddNotification("PetSharp by Veeox: Loaded!", 5).SetTextColor(NotificationColor);
        }

        private static void OnGameNotify(GameNotifyEventArgs args)
        {
            var killer = args.NetworkId;

            switch (args.EventId) //Check for XP events
            {
                    
                case GameEventId.OnChampionDoubleKill:
                    if (killer == Player.NetworkId)
                    {
                        CurXP += MaxXP / 80;
                    }
                    break;
                case GameEventId.OnChampionPentaKill:
                    if (killer == Player.NetworkId)
                    {
                        CurXP += MaxXP / 15;
                    }
                    break;
                case GameEventId.OnChampionQuadraKill:
                    if (killer == Player.NetworkId)
                    {
                        CurXP += MaxXP / 45;
                    }
                    break;
                case GameEventId.OnChampionTripleKill:
                    if (killer == Player.NetworkId)
                    {
                        CurXP += MaxXP / 75;
                    }
                    break;
                case GameEventId.OnAce:
                    var pl = FindPlayerByNetworkId(killer);

                    if (pl != null && pl.IsAlly)
                        {
                            CurXP += MaxXP / 80;
                        }
                    break;
                case GameEventId.OnChampionDie:
                    
                    if (killer == Player.NetworkId)
                    {
                        CurXP += MaxXP / 75;
                        Console.WriteLine("Yay a Kill!");
                        Console.WriteLine(CurXP);
                    }
                    break;
                case GameEventId.OnDie:
                    var al = FindPlayerByNetworkId(killer);

                    if (al != null && al.IsAlly)
                    {
                        CurXP += MaxXP / 75;
                        Console.WriteLine("this is a test");
                        Console.WriteLine(CurXP + "/" + MaxXP);
                    }
                    break;

                case GameEventId.OnKillWard:
                    if (ObjectManager.Player.IsMe)
                    {
                        KillWard();
                        Console.WriteLine("Killed a ward!");
                    }
                    break;
                //case GameEventId.OnQuit:
                //    ConvertInt(Lvl, CurXP, MaxXP);
                //    break;
            }
        }

        private static void DragonCheck()
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

        private static void BaroonCheck()
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

        //End of game Save
        static void OnEnd(EventArgs args)
        {
            ConvertInt(Lvl, CurXP, MaxXP);
        }

        //Run first time only
        private static void FirstRun()
        {
            RandomName();
            Lvl = 1;
            CurXP = 0;
            MaxXP = 100;
            ConvertInt(Lvl, CurXP, MaxXP);
        }

        //Convert Int
        public static void ConvertInt(int lvl, int currxp, int maxxp) 
        {
            string level = Lvl.ToString();
            string currentXP = CurXP.ToString();
            string MaximumXP = MaxXP.ToString();
            SaveData(level, currentXP, MaximumXP);
        }
        
        //Convert String
        public static void ConvertString(string lvl, string currxp, string maxxp)
        {
            
            int level = int.Parse(lvl);
            int currentXP = int.Parse(currxp);
            int maximumXP = int.Parse(maxxp);

            Lvl = level;
            CurXP = currentXP;
            MaxXP = maximumXP;
        }

        //Used to read data
        private static void ReadSave()
        {
            string LvlStr = null;
            string CurXPStr = null;
            string MaxXPStr = null;

            using (var sr = new System.IO.StreamReader(Config.AppDataDirectory + @"\PetSharp\" + FileName, true))
            {
                string line;
                int currentLineNumber = 0;
                while ((line = sr.ReadLine()) != null)
                {
                    switch (++currentLineNumber)
                    {
                        case 1:
                            PetName = line;
                            break;
                        case 2:
                            LvlStr = line;
                            break;
                        case 3:
                            CurXPStr = line;
                            break;
                        case 4:
                            MaxXPStr = line;
                            break;
                    }
                }
                ConvertString(LvlStr, CurXPStr, MaxXPStr);
            }
                            
        }

        //Used to save data
        public static void SaveData(string lvl, string currxp, string maxxp) 
        {
            File.WriteAllText(Config.AppDataDirectory + @"\PetSharp\" + FileName, PetName + "\n");
            using (var file = new StreamWriter(Config.AppDataDirectory + @"\PetSharp\" + FileName, true))
            {
                file.WriteLine(lvl);
                file.WriteLine(currxp);
                file.WriteLine(maxxp);
                file.Close();
            }
        }
        
        private static void GainXP()
        {
            LevelUp();
            WinGame();
            EndScore();
        }

        private static void ShopBuy()
        {
            if (!Menu.Item("food1").GetValue<bool>() && (!Menu.Item("food2").GetValue<bool>() && (!Menu.Item("food3").GetValue<bool>())))
            {
                return;
            }

            if (Menu.Item("food1").GetValue<bool>())
            {
                Notifications.AddNotification("PetSharp: Food 1 Bought!", 2).SetTextColor(NotificationColor);
                Menu.Item("food1").SetValue(false);
            }

            if (Menu.Item("food2").GetValue<bool>())
            {
                Notifications.AddNotification("PetSharp: Food 2 Bought!", 2).SetTextColor(NotificationColor);
                Menu.Item("food2").SetValue(false);
            }

            if (Menu.Item("food3").GetValue<bool>())
            {
                Notifications.AddNotification("PetSharp: Food 3 Bought!", 2).SetTextColor(NotificationColor);
                Menu.Item("food3").SetValue(false);
            }
        
        }
        
        private static void LevelUp()
        {
            if (CurXP >= MaxXP)
            {
                CurXP = (CurXP - MaxXP);
                MaxXP = (MaxXP * 2);
                Lvl++;
            }
        }

        private static void WinGame()
        {
            var DoOnce = false;
            var nexus = ObjectManager.Get<Obj_HQ>().Find(n => n.Health < 1);

            if (nexus == null)
            {
                return;
            }

            if (!DoOnce)
            {
                if (nexus.IsEnemy)
                {
                    CurXP += MaxXP / 10;
                    ConvertInt(Lvl, CurXP, MaxXP);
                    Console.WriteLine("WON!");
                    DoOnce = true;
                }
                else
                {
                    ConvertInt(Lvl, CurXP, MaxXP);
                    DoOnce = true;
                }
            }
        }

        private static void KillDrag()
        {
            CurXP += MaxXP / 30;
            Console.WriteLine("Drag Killed");
        }

        private static void KillBaroon()
        {
            CurXP += MaxXP / 50;
            Console.WriteLine("Baroon Killed");
        }

        private static void KillWard()
        {
            CurXP += MaxXP / 100;
        }

        private static void EndScore()
        {
            return;
        }

        //Name Gen Stuff
        private static void RandomName()
        {
            //Random Name Gen
            string[] NameDatabase1 = { "Ba", "Bax", "Dan", "Fi", "Fix", "Fiz", "Gi", "Gix", "Giz", "Gri", "Gree", "Greex", "Grex", "Ja", "Jax", "Jaz", "Jex", "Ji", "Jix", "Ka", "Kax", "Kay", "Kaz", "Ki", "Kix", "Kiz", "Klee", "Kleex", "Kwee", "Kweex", "Kwi", "Kwix", "Kwy", "Ma", "Max", "Ni", "Nix", "No", "Nox", "Qi", "Rez", "Ri", "Ril", "Rix", "Riz", "Ro", "Rox", "So", "Sox", "Vish", "Wi", "Wix", "Wiz", "Za", "Zax", "Ze", "Zee", "Zeex", "Zex", "Zi", "Zix", "Zot" };
            string[] NameDatabase2 = { "b", "ba", "be", "bi", "d", "da", "de", "di", "e", "eb", "ed", "eg", "ek", "em", "en", "eq", "ev", "ez", "g", "ga", "ge", "gi", "ib", "id", "ig", "ik", "im", "in", "iq", "iv", "iz", "k", "ka", "ke", "ki", "m", "ma", "me", "mi", "n", "na", "ni", "q", "qa", "qe", "qi", "v", "va", "ve", "vi", "z", "za", "ze", "zi", "", "", "", "", "", "", "", "", "", "", "", "", "" };
            string[] NameDatabase3 = { "ald", "ard", "art", "az", "azy", "bit", "bles", "eek", "eka", "et", "ex", "ez", "gaz", "geez", "get", "giez", "iek", "igle", "ik", "il", "in", "ink", "inkle", "it", "ix", "ixle", "lax", "le", "lee", "les", "lex", "lyx", "max", "maz", "mex", "mez", "mix", "miz", "mo", "old", "rax", "raz", "reez", "rex", "riez", "tee", "teex", "teez", "to", "uek", "x", "xaz", "xeez", "xik", "xink", "xiz", "xonk", "yx", "zeel", "zil" };

            Random RandName = new Random();
            string Temp = NameDatabase1[RandName.Next(0, NameDatabase1.Length)] + NameDatabase2[RandName.Next(0, NameDatabase2.Length)];
            PetName = Temp;
        }

        private static void NewPet()
        {
            if (Menu.Item("new").GetValue<bool>())
            {
                FirstRun();
                Notifications.AddNotification("PetSharp: New Pet Created!", 2).SetTextColor(NotificationColor);
                Menu.Item("new").SetValue(false);
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

   }
}

