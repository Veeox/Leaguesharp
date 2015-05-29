using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;
using LeagueSharp.Common;
using LeagueSharp;

namespace PetSharp
{
    class Converters
    {
        //Convert Int
        public static void ConvertInt(int lvl, int currxp, int maxxp, int cash)
        {
            string level = Pet.Lvl.ToString();
            string currentXP = Pet.CurXP.ToString();
            string MaximumXP = Pet.MaxXP.ToString();
            string CashBal = Pet.CashBalance.ToString();
            Save.SaveData(level, currentXP, MaximumXP, CashBal);
        }

        //Convert String
        public static void ConvertString(string lvl, string currxp, string maxxp, string cash)
        {

            int level = int.Parse(lvl);
            int currentXP = int.Parse(currxp);
            int maximumXP = int.Parse(maxxp);
            int CashBal = int.Parse(cash);

            Pet.Lvl = level;
            Pet.CurXP = currentXP;
            Pet.MaxXP = maximumXP;
            Pet.CashBalance = CashBal;
        }
    }
}
