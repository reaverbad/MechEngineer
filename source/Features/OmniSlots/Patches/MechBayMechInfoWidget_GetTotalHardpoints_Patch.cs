﻿using System;
using System.Linq;
using BattleTech;
using BattleTech.UI;
using Harmony;
using MechEngineer.Features.DynamicSlots;

namespace MechEngineer.Features.OmniSlots.Patches
{
    [HarmonyPatch(typeof(MechLabMechInfoWidget), "GetTotalHardpoints")]
    internal static class MechBayMechInfoWidget_GetTotalHardpoints_Patch
    {
        internal static void Postfix(
            MechLabMechInfoWidget __instance,
            ref MechLabPanel ___mechLab,
            ref MechLabHardpointElement[] ___hardpoints)
        {
            try
            {
                var mechDef = ___mechLab.activeMechDef;
                if (mechDef == null)
                {
                    return;
                }
                
                var inventory = mechDef.Inventory.Select(x => x.Def);
                var hardpoints = MechDefBuilder.Locations.SelectMany(x => mechDef.Chassis.GetLocationDef(x).Hardpoints).ToArray();

                var calc = new HardpointOmniUsageCalculator(inventory, hardpoints);

                //Control.mod.Logger.Log(calc);

                __instance.totalBallisticHardpoints = calc.Ballistic.TheoreticalMax;
                __instance.totalEnergyHardpoints = calc.Energy.TheoreticalMax;
                __instance.totalMissileHardpoints = calc.Missile.TheoreticalMax;
                __instance.totalSmallHardpoints = calc.Small.TheoreticalMax;

                ___hardpoints[0].SetData(WeaponCategory.Ballistic, calc.Ballistic.HardpointTotalString);
                ___hardpoints[1].SetData(WeaponCategory.Energy, calc.Energy.HardpointTotalString);
                ___hardpoints[2].SetData(WeaponCategory.Missile, calc.Missile.HardpointTotalString);
                ___hardpoints[3].SetData(WeaponCategory.AntiPersonnel, calc.Small.HardpointTotalString);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}