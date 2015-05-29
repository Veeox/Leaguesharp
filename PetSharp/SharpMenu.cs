using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace PetSharp
{
    public class SharpMenu
    {
        public static Menu Z;

        // menu

        public static void Run()
        {
            Z = new Menu("PetSharp", "petsharp", true);

            //shop menu
            Z.AddSubMenu(new Menu("PetSharp Shop", "shop"));
            Z.SubMenu("shop").AddItem(new MenuItem("food1", "Buy " + GameAssets.med.Name + " ($" + GameAssets.med.Cost + ")").SetValue(false));
            Z.SubMenu("shop").AddItem(new MenuItem("food2", "Buy " + GameAssets.expdouble.Name + " ($" + GameAssets.expdouble.Cost + ")").SetValue(false));

            //Draw menu
            Z.AddSubMenu(new Menu("Drawings", "draw"));
            Z.SubMenu("draw").AddItem(new MenuItem("drawstats", "Draw Stats").SetValue(true));
            Z.SubMenu("draw").AddItem(new MenuItem("drawsprites", "Draw Sprites").SetValue(true));

            //Misc menu
            Z.AddSubMenu(new Menu("Misc", "misc"));
            Z.SubMenu("misc").AddItem(new MenuItem("track", "Track Game").SetValue(true));
            Z.SubMenu("misc").AddItem(new MenuItem("save", "Manual Save").SetValue(false));
            Z.SubMenu("misc").AddItem(new MenuItem("new", "New Pet (Start Over)").SetValue(false));

            //Credits menu
            Z.AddSubMenu(new Menu("Credits", "credits"));
            Z.SubMenu("credits").AddItem(new MenuItem("Veeox", "Veeox"));
            Z.SubMenu("credits").AddItem(new MenuItem("TehBlaxxor", "TehBlaxxor"));
            Z.SubMenu("credits").AddItem(new MenuItem("zvodd", "zvodd"));

            Z.AddToMainMenu();
        }
    }
}
