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
    class Shop
    {
        public static void ShopBuy()
        {

            if (!SharpMenu.Z.Item("food1").GetValue<bool>() && (!SharpMenu.Z.Item("food2").GetValue<bool>()))
            {
                return;
            }

            if (SharpMenu.Z.Item("food1").GetValue<bool>())
            {
                var CanBuy = GameAssets.PurchaseAvailable(GameAssets.med);

                if (CanBuy)
                {
                    if (!Pet.Sick)
                    {
                        Notifications.AddNotification("PetSharp: Pet is not Sick!", 2).SetTextColor(PetSharp.NotificationColor);
                        SharpMenu.Z.Item("food1").SetValue(false);
                        return;
                    }
                    else
                    {
                        Notifications.RemoveNotification(PetSharp.sick1);
                        Notifications.RemoveNotification(PetSharp.sick2);
                        PetSharp.sick1 = null;
                        PetSharp.sick2 = null;
                        Notifications.AddNotification("PetSharp: " + GameAssets.med.Name + " Bought!", 20).SetTextColor(PetSharp.NotificationColor);
                        Notifications.AddNotification("PetSharp: Pet has been cured!", 20).SetTextColor(PetSharp.NotificationColor);
                        Pet.Sick = false;

                    }

                    //Deduct Cost
                    Pet.CashBalance -= GameAssets.med.Cost;
                }
                else
                {
                    Notifications.AddNotification("PetSharp: Not Enough Cash!", 2).SetTextColor(PetSharp.NotificationColor);
                }

                SharpMenu.Z.Item("food1").SetValue(false);
            }

            if (SharpMenu.Z.Item("food2").GetValue<bool>())
            {
                var CanBuy = GameAssets.PurchaseAvailable(GameAssets.expdouble);

                if (CanBuy)
                {
                    if (Pet.FoodXP)
                    {
                        Notifications.AddNotification("PetSharp: Cannot Buy Twice", 2).SetTextColor(PetSharp.NotificationColor);
                        SharpMenu.Z.Item("food2").SetValue(false);
                        return;
                    }
                    else
                    {
                        Notifications.AddNotification("PetSharp: " + GameAssets.expdouble.Name + " Bought!", 2).SetTextColor(PetSharp.NotificationColor);
                        Pet.FoodXP = true;
                        Pet.XPMulti = 2;
                    }

                    //Deduct Cost
                    Pet.CashBalance -= GameAssets.expdouble.Cost;
                }
                else
                    Notifications.AddNotification("PetSharp: Not Enough Cash!", 2).SetTextColor(PetSharp.NotificationColor);
                
                SharpMenu.Z.Item("food2").SetValue(false);
            }
                
        }


    }



}
