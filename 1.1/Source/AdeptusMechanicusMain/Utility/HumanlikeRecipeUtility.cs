using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AdeptusMechanicus
{
    public class HumanlikeRecipeUtility
    {
        public static void AddHumanlikeRecipes()
        {

            foreach (RecipeDef item in ArmouryMain.humanRecipes)
            {
                if (item.recipeUsers.NullOrEmpty())
                {
                    item.recipeUsers = new List<ThingDef>();
                }
                //    Log.Message("Checking Human recipes " + item);
                if (ArmouryMain.mechanicus != null)
                {
                    //    Log.Message("ArmouryMain 1 mechanicus");
                    if (!item.AllRecipeUsers.Contains(ArmouryMain.mechanicus))
                    {
                        item.recipeUsers.Add(ArmouryMain.mechanicus);
                    }
                }
                if (ArmouryMain.astartes != null)
                {
                    if (!item.AllRecipeUsers.EnumerableNullOrEmpty())
                    {
                        if (!item.AllRecipeUsers.Contains(ArmouryMain.astartes))
                        {
                            //    Log.Message("ArmouryMain 1 astartes");
                            //    Log.Message("Adding " + item + " to astartes");
                            item.recipeUsers.Add(ArmouryMain.astartes);
                            //    Log.Message("Added " + item + " to astartes");
                        }
                    }
                }
                if (AdeptusIntergrationUtility.enabled_GeneSeed)
                {
                    if (ArmouryMain.geneseedAstartes != null)
                    {
                        if (!item.AllRecipeUsers.EnumerableNullOrEmpty())
                        {
                            if (!item.AllRecipeUsers.Contains(ArmouryMain.geneseedAstartes))
                            {
                                //    Log.Message("ArmouryMain 1 astartes");
                                //    Log.Message("Adding " + item + " to astartes");
                                item.recipeUsers.Add(ArmouryMain.geneseedAstartes);
                                //    Log.Message("Added " + item + " to astartes");
                            }
                        }
                    }
                    if (ArmouryMain.geneseedCustodes != null)
                    {
                        if (!item.AllRecipeUsers.EnumerableNullOrEmpty())
                        {
                            if (!item.AllRecipeUsers.Contains(ArmouryMain.geneseedCustodes))
                            {
                                //    Log.Message("ArmouryMain 1 astartes");
                                //    Log.Message("Adding " + item + " to astartes");
                                item.recipeUsers.Add(ArmouryMain.geneseedCustodes);
                                //    Log.Message("Added " + item + " to astartes");
                            }
                        }
                    }
                }
                if (ArmouryMain.ogryn != null)
                {
                    //        Log.Message("ArmouryMain 1 ogryn");
                    if (!item.AllRecipeUsers.Contains(ArmouryMain.ogryn))
                    {
                        item.recipeUsers.Add(ArmouryMain.ogryn);
                    }
                }
                if (ArmouryMain.ratlin != null)
                {
                    //       Log.Message("ArmouryMain 1 ratlin");
                    if (!item.AllRecipeUsers.Contains(ArmouryMain.ratlin))
                    {
                        item.recipeUsers.Add(ArmouryMain.ratlin);
                    }
                }
                if (ArmouryMain.beastman != null)
                {
                    //        Log.Message("ArmouryMain 1 beastman");
                    if (!item.AllRecipeUsers.Contains(ArmouryMain.beastman))
                    {
                        item.recipeUsers.Add(ArmouryMain.beastman);
                    }
                }
            }
        }
    }
}
