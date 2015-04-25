#region

using System;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

#endregion

namespace PetSharp
{
    class Program
    {
        private static int CurXP;
        private static int MaxXP;
        private static int Lvl;
        private static string PetName;

        public static string FileName;

        private static Obj_AI_Minion Baron { get; set; }
        private static Obj_AI_Minion Dragon { get; set; }

        public static Menu Menu;
        public const string Ver = "0.0.1";

        public static Obj_AI_Hero Player
        {
            get { return ObjectManager.Player; }
        }


        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += OnLoad;
            CustomEvents.Game.OnGameEnd += OnEnd;
            Game.OnNotify += OnGameNotify;
        }

        private static void OnLoad(EventArgs args)
        {
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
            Menu = new Menu("PetSharp", "petsharp", true);
            Menu.AddSubMenu(new Menu("Pet Info", "petinfo"));
            Menu.SubMenu("petinfo").AddItem(new MenuItem("petname", "Pet Name: " + PetName));
            Menu.SubMenu("petinfo").AddItem(new MenuItem("petlvl", "Level: " + Lvl));
            Menu.SubMenu("petinfo").AddItem(new MenuItem("petxp", "XP: " + CurXP + "/" + MaxXP));
            Menu.AddItem(new MenuItem("track", "Track Game").SetValue(true));

            Menu.AddToMainMenu();

            //Prints
            Game.PrintChat("PetSharp v" + Ver + " by Veeox Loaded!");
            
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            
            //Check if enabled
            if (!Menu.Item("track").IsActive())
            {
                return;
            }
            else
            {
                GainXP();
            }
        }

        private static void OnGameNotify(GameNotifyEventArgs args)
        {
            if (args.NetworkId != ObjectManager.Player.NetworkId)
            {
                // not my hero
                return;
            }

            switch (args.EventId) //Check for XP events
            {
                case GameEventId.OnChampionDoubleKill:
                    Game.PrintChat("Yay! Double kill!");
                    break;
                case GameEventId.OnChampionPentaKill:
                    Game.PrintChat("YAY pent kill!");
                    break;
                case GameEventId.OnChampionQuadraKill:
                    Game.PrintChat("YAY quad kill");
                    break;
                case GameEventId.OnChampionTripleKill:
                    Game.PrintChat("YAY trip kill!!");
                    break;
                case GameEventId.OnAce:
                    Game.PrintChat("YAY ACE!");
                    break;
                case GameEventId.OnFirstBlood:
                    Game.PrintChat("Yay FB!");
                    break;
                
                case GameEventId.OnKillDragon:
                    foreach (var i in ObjectManager.Get<Obj_AI_Hero>())
                    {
                        if (i.Team == Player.Team)
                            KillDrag();
                            Game.PrintChat("YAY DRAGON DEAD!");
                        if (i.Team != Player.Team)
                            return;
                    }
                    break;
                case GameEventId.OnKillWorm:
                    foreach (var i in ObjectManager.Get<Obj_AI_Hero>())
                    {
                        if (i.Team == Player.Team)
                            KillBaroon();
                        Game.PrintChat("YAY BAROON DEAD!");
                        if (i.Team != Player.Team)
                            return;
                    }
                    break;
                case GameEventId.OnKillWard:
                    foreach (var i in ObjectManager.Get<Obj_AI_Hero>())
                    {
                        if (i.Team == Player.Team)
                            KillWard();
                        Game.PrintChat("Killed a ward!");
                        if (i.Team != Player.Team)
                            return;
                    }
                    break;
                case GameEventId.OnQuit:
                    ConvertInt(Lvl, CurXP, MaxXP);
                    break;
            }
        }

        static void OnEnd(EventArgs args)
        {
            ConvertInt(Lvl, CurXP, MaxXP);
        }

        private static void FirstRun()
        {
            Lvl = 1;
            CurXP = 0;
            MaxXP = 100;
            PetName = "TestName";
            ConvertInt(Lvl, CurXP, MaxXP);
        }

        //Convert Int
        public static void ConvertInt(int lvl, int curxp, int maxxp) 
        {
            string level = Lvl.ToString();
            string currentXP = CurXP.ToString();
            string MaximumXP = MaxXP.ToString();
            SaveData(level, currentXP, MaximumXP);
        }
        
        //Convert String
        public static void ConvertString(string lvl, string curxp, string maxxp)
        {
            
            int level = int.Parse(lvl);
            int currentXP = int.Parse(curxp);
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

           // if (LvlStr != null && CurXPStr != null && MaxXPStr != null)
                
        }

        //Used to save data
        public static void SaveData(string lvl, string curxp, string maxxp) 
        {
            File.WriteAllText(Config.AppDataDirectory + @"\PetSharp\" + FileName, PetName + "\n");
            using (var file = new StreamWriter(Config.AppDataDirectory + @"\PetSharp\" + FileName, true))
            {
                //file.WriteLine(System.Environment.NewLine);
                file.WriteLine(lvl);
                file.WriteLine(curxp);
                file.WriteLine(maxxp);
                file.Close();
            }
        }

        ////Used to delete old save
        //public static void DeleteSave(string path)
        //{
        //    DeleteSave(path, false);
        //}

        //public static void DeleteSave(string path, bool recursive)
        //{
        //    // Delete all files and sub-folders?
        //    if (recursive)
        //    {
        //        // Yep... Let's do this
        //        var subfolders = Directory.GetDirectories(path);
        //        foreach (var s in subfolders)
        //        {
        //            DeleteSave(s, recursive);
        //        }
        //    }

        //    // Get all files of the folder
        //    var files = Directory.GetFiles(path);
        //    foreach (var f in files)
        //    {
        //        // Get the attributes of the file
        //        var attr = File.GetAttributes(f);

        //        // Is this file marked as 'read-only'?
        //        if ((attr & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
        //        {
        //            // Yes... Remove the 'read-only' attribute, then
        //            File.SetAttributes(f, attr ^ FileAttributes.ReadOnly);
        //        }

        //        // Delete the file
        //        File.Delete(f);
        //    }

        //    // When we get here, all the files of the folder were
        //    // already deleted, so we just delete the empty folder
        //    Directory.Delete(path);
        //}

        private static void LevelUp()
        {
            if (CurXP >= MaxXP)
            {
                CurXP = (CurXP - MaxXP);
                MaxXP = (MaxXP * 2);
            }
        }

        private static void GainXP()
        {
            LevelUp();
            WinGame();
            EndScore();

        }

        private static void WinGame()
        {
            var nexus = ObjectManager.Get<Obj_HQ>().Find(n => n.Health < 1);

            if (nexus.IsEnemy)
            {
                CurXP += (CurXP + (MaxXP / 10));
                ConvertInt(Lvl, CurXP, MaxXP);
            }
            else
            {
                ConvertInt(Lvl, CurXP, MaxXP);
            }
        }

        private static void KillDrag()
        {
            CurXP += (CurXP + (MaxXP / 30));
        }

        private static void KillBaroon()
        {
            CurXP += (CurXP + (MaxXP / 50));
        }

        private static void KillWard()
        {
            CurXP += (CurXP + (MaxXP / 100));
        }

        private static void EndScore()
        {

        }
        
    }
}
