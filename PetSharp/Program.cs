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
    //Happy Hour
    //Cash system - #Done
    //Shop system - #Done mostly
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
        public const string Ver = "0.0.3.0";

        //Main Pet Vars
        public static int CurXP;
        public static int MaxXP;
        public static int Lvl;
        public static string PetName;
        public static int CashBalance;

        //Buff Food
            //Costs
        private static int Food1Cost = 500;
        private static int Food2Cost = 500;
        private static int Food3Cost = 500;
            //Names
        private static string Food1 = "2x XP";
        private static string Food2 = "Need Ideas";
        private static string Food3 = "Need Ideas";
            //Bools
        private static bool FoodXP = false;
        
        //Drag and Baroon Stuff
        private static string DragonBuff = "s5test_dragonslayerbuff";
        private static string BaroonBuff = "exaltedwithbaronnashor";
        private static int AllyD;
        private static int AllyB;

        private static Obj_AI_Minion Baron { get; set; }
        private static Obj_AI_Minion Dragon { get; set; }

        //File name setup for saving
        public static string FileName;

        //Drawing Colour
        private static System.Drawing.Color NotificationColor = System.Drawing.Color.FromArgb(0, 255, 0);

        private static Obj_AI_Hero Player { get { return ObjectManager.Player; } }

        public static Menu Menu;


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
            Menu.SubMenu("shop").AddItem(new MenuItem("food1", "Buy " + Food1 + " ($" + Food1Cost + ")").SetValue(false));
            Menu.SubMenu("shop").AddItem(new MenuItem("food2", "Buy " + Food2 + " ($" + Food2Cost + ")").SetValue(false));
            Menu.SubMenu("shop").AddItem(new MenuItem("food3", "Buy " + Food3 + " ($" + Food3Cost + ")").SetValue(false));

            //Draw menu
            Menu.AddSubMenu(new Menu("Drawings", "draw"));
            Menu.SubMenu("draw").AddItem(new MenuItem("drawstats", "Draw Stats").SetValue(true));
            Menu.SubMenu("draw").AddItem(new MenuItem("drawsprites", "Draw Sprites").SetValue(true));

            //Misc menu
            Menu.AddSubMenu(new Menu("Misc", "misc"));
            Menu.SubMenu("misc").AddItem(new MenuItem("track", "Track Game").SetValue(true));
            Menu.SubMenu("misc").AddItem(new MenuItem("save", "Manual Save").SetValue(false));
            Menu.SubMenu("misc").AddItem(new MenuItem("new", "New Pet (Start Over)").SetValue(false));

            //Credits menu
            Menu.AddSubMenu(new Menu("Credits", "credits"));
            Menu.SubMenu("credits").AddItem(new MenuItem("Veeox", "Veeox"));
            Menu.SubMenu("credits").AddItem(new MenuItem("TehBlaxxor", "TehBlaxxor"));
            Menu.SubMenu("credits").AddItem(new MenuItem("zvodd", "zvodd"));

            Menu.AddToMainMenu();
            
            
        }

        static void Drawing_OnDraw(EventArgs args)
        {
            if (!Menu.Item("drawstats").GetValue<bool>() || !Menu.Item("track").GetValue<bool>())
            {
                return;
            }

            var xpos = 1660;
            var ypos = 660;

            Drawing.DrawText(xpos, ypos, System.Drawing.Color.LightSkyBlue, "PetSharp v" + Ver + " by Veeox");
            Drawing.DrawText(xpos, ypos + 20, System.Drawing.Color.LightSkyBlue, "Pet Name: " + PetName);
            Drawing.DrawText(xpos, ypos + 40, System.Drawing.Color.LightSkyBlue, "Level: " + (int)Lvl);
            Drawing.DrawText(xpos, ypos + 60, System.Drawing.Color.LightSkyBlue, "XP: " + (int)CurXP + "/" + (int)MaxXP);
            Drawing.DrawText(xpos, ypos + 80, System.Drawing.Color.LightSkyBlue, "PetBux: $" + (int)CashBalance);
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
                ManualSave();
            }
        }

        //Initialize all the things!
        private static void InitEvents()
        {
            Game.OnUpdate += OnUpdate;
            CustomEvents.Game.OnGameEnd += OnEnd;
            Game.OnNotify += OnGameNotify;
            Drawing.OnDraw += Drawing_OnDraw;
            Notifications.AddNotification("PetSharp v" + Ver + ": Loaded!", 5).SetTextColor(NotificationColor);
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
                        CashBalance += 10;
                    }
                    break;
                case GameEventId.OnChampionPentaKill:
                    if (killer == Player.NetworkId)
                    {
                        CurXP += MaxXP / 15;
                        CashBalance += 75;
                    }
                    break;
                case GameEventId.OnChampionQuadraKill:
                    if (killer == Player.NetworkId)
                    {
                        CurXP += MaxXP / 45;
                        CashBalance += 50;
                    }
                    break;
                case GameEventId.OnChampionTripleKill:
                    if (killer == Player.NetworkId)
                    {
                        CurXP += MaxXP / 75;
                        CashBalance += 35;
                    }
                    break;
                case GameEventId.OnAce:
                    var pl = FindPlayerByNetworkId(killer);

                    if (pl != null && pl.IsAlly)
                        {
                            CurXP += MaxXP / 80;
                            CashBalance += 15;
                        }
                    break;
                case GameEventId.OnChampionDie:
                    
                    if (killer == Player.NetworkId)
                    {
                        CurXP += MaxXP / 75;
                        CashBalance += 5;
                    }
                    break;

                //This is a test case - remove me when finished testing!
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
                //    ConvertInt(Lvl, CurXP, MaxXP, CashBalance);
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
            ConvertInt(Lvl, CurXP, MaxXP, CashBalance);
        }

        //Run first time only
        private static void FirstRun()
        {
            RandomName();
            Lvl = 1;
            CurXP = 0;
            MaxXP = 100;
            CashBalance = 0;
            ConvertInt(Lvl, CurXP, MaxXP, CashBalance);
        }

        private static void ManualSave()
        {
            if (Menu.Item("save").GetValue<bool>())
            {
                Notifications.AddNotification("PetSharp: Saving...", 2).SetTextColor(NotificationColor);
                ConvertInt(Lvl, CurXP, MaxXP, CashBalance);
                Menu.Item("save").SetValue(false);
                Notifications.AddNotification("PetSharp: Progress Saved!", 2).SetTextColor(NotificationColor);
            }

        }

        //Convert Int
        public static void ConvertInt(int lvl, int currxp, int maxxp, int cash) 
        {
            string level = Lvl.ToString();
            string currentXP = CurXP.ToString();
            string MaximumXP = MaxXP.ToString();
            string CashBal = CashBalance.ToString();
            SaveData(level, currentXP, MaximumXP, CashBal);
        }
        
        //Convert String
        public static void ConvertString(string lvl, string currxp, string maxxp, string cash)
        {
            
            int level = int.Parse(lvl);
            int currentXP = int.Parse(currxp);
            int maximumXP = int.Parse(maxxp);
            int CashBal = int.Parse(cash);

            Lvl = level;
            CurXP = currentXP;
            MaxXP = maximumXP;
            CashBalance = CashBal;
        }

        //Used to read data
        private static void ReadSave()
        {
            string LvlStr = null;
            string CurXPStr = null;
            string MaxXPStr = null;
            string CashStr = null;

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
                        case 5:
                            CashStr = line;
                            break;
                    }
                }
                ConvertString(LvlStr, CurXPStr, MaxXPStr, CashStr);
            }
                            
        }

        //Used to save data
        public static void SaveData(string lvl, string currxp, string maxxp, string cash) 
        {
            File.WriteAllText(Config.AppDataDirectory + @"\PetSharp\" + FileName, PetName + "\n");
            using (var file = new StreamWriter(Config.AppDataDirectory + @"\PetSharp\" + FileName, true))
            {
                file.WriteLine(lvl);
                file.WriteLine(currxp);
                file.WriteLine(maxxp);
                file.WriteLine(cash);
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
                

                if (CashBalance >= Food1Cost)
                {
                    if (FoodXP)
                    {
                        Notifications.AddNotification("PetSharp: Cannot Buy Twice", 2).SetTextColor(NotificationColor);
                        Menu.Item("food1").SetValue(false);
                        return;
                    }
                    else
                    {
                        Notifications.AddNotification("PetSharp: " + Food1 + " Bought!", 2).SetTextColor(NotificationColor);
                        FoodXP = true;
                    }

                    //Deduct Cost
                    CashBalance -= Food1Cost;
                }
                else
                {
                    Notifications.AddNotification("PetSharp: Not Enough Cash!", 2).SetTextColor(NotificationColor);
                }
                
                Menu.Item("food1").SetValue(false);
            }

            if (Menu.Item("food2").GetValue<bool>())
            {
                if (CashBalance >= Food2Cost)
                {
                    Notifications.AddNotification("PetSharp: " + Food2 + " Bought!", 2).SetTextColor(NotificationColor);

                    //Deduct Cost
                    CashBalance -= Food2Cost;
                }
                else
                    Notifications.AddNotification("PetSharp: Not Enough Cash!", 2).SetTextColor(NotificationColor);

                Menu.Item("food2").SetValue(false);
            }

            if (Menu.Item("food3").GetValue<bool>())
            {
                if (CashBalance >= Food3Cost)
                {
                    Notifications.AddNotification("PetSharp: " + Food3 + " Bought!", 2).SetTextColor(NotificationColor);

                    //Deduct Cost
                    CashBalance -= Food3Cost;
                }
                else
                    Notifications.AddNotification("PetSharp: Not Enough Cash!", 2).SetTextColor(NotificationColor);

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
                Notifications.AddNotification("PetSharp: Leveled up!", 2).SetTextColor(NotificationColor);
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
                    CashBalance += 100;
                    ConvertInt(Lvl, CurXP, MaxXP, CashBalance);
                    Console.WriteLine("WON!");
                    DoOnce = true;
                }
                else
                {
                    ConvertInt(Lvl, CurXP, MaxXP, CashBalance);
                    DoOnce = true;
                }
            }
        }

        private static void KillDrag()
        {
            CurXP += MaxXP / 30;
            CashBalance += 20;
            Console.WriteLine("Drag Killed");
        }

        private static void KillBaroon()
        {
            CurXP += MaxXP / 50;
            CashBalance += 35;
            Console.WriteLine("Baroon Killed");
        }

        private static void KillWard()
        {
            CurXP += MaxXP / 100;
            CashBalance += 1;
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

