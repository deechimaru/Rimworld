﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;   // Always needed
using RimWorld;      // RimWorld specific functions are found here
using Verse;         // RimWorld universal objects are here
using Verse.AI;      // Needed when you do something with the AI
//using Verse.Sound; // Needed when you do something with the Sound

namespace FishIndustry
{
    /// <summary>
    /// WorkGiver_FillAquacultureHopper class.
    /// </summary>
    /// <author>Rikiki</author>
    /// <permission>Use this code as you want, just remember to add a link to the corresponding Ludeon forum mod release thread.
    /// Remember learning is always better than just copy/paste...</permission>
    public class WorkGiver_FillAquacultureHopper : WorkGiver_Scanner
    {
        // TODO: too bad the WorkGiver_CookFillHopper class is internal... :( I would only have to override the PotentialWorkThingRequest function!

        public override ThingRequest PotentialWorkThingRequest
        {
            get
            {
                return ThingRequest.ForDef(Util_FishIndustry.AquacultureHopperDef);
            }
        }
        public override PathEndMode PathEndMode
        {
            get
            {
                return PathEndMode.ClosestTouch;
            }
        }
        public override Job JobOnThing(Pawn pawn, Thing thing)
        {
            ISlotGroupParent slotGroupParent = thing as ISlotGroupParent;
            if (slotGroupParent == null)
            {
                return null;
            }
            if (!pawn.CanReserve(thing.Position, 1))
            {
                return null;
            }
            int num = 0;
            List<Thing> list = Find.ThingGrid.ThingsListAt(thing.Position);
            for (int i = 0; i < list.Count; i++)
            {
                Thing thing2 = list[i];
                if (thing2.def.IsNutritionSource)
                {
                    num += thing2.stackCount;
                }
            }
            if (num > 25)
            {
                JobFailReason.Is("AlreadyFilledLower".Translate());
                return null;
            }
            return WorkGiver_FillAquacultureHopper.HopperFillFoodJob(pawn, slotGroupParent);
        }
        public static Job HopperFillFoodJob(Pawn pawn, ISlotGroupParent hopperSgp)
        {
            Building building = hopperSgp as Building;
            if (!pawn.CanReserveAndReach(building.Position, PathEndMode.Touch, pawn.NormalMaxDanger(), 1))
            {
                return null;
            }
            ThingDef thingDef = null;
            Thing firstItem = building.Position.GetFirstItem();
            if (firstItem != null)
            {
                if (firstItem.def.IsNutritionSource)
                {
                    thingDef = firstItem.def;
                }
                else
                {
                    if (firstItem.IsForbidden(pawn.Faction))
                    {
                        return null;
                    }
                    return HaulAIUtility.HaulAsideJobFor(pawn, firstItem);
                }
            }
            List<Thing> list;
            if (thingDef == null)
            {
                list = Find.Map.listerThings.ThingsInGroup(ThingRequestGroup.FoodNotPlantOrTree);
            }
            else
            {
                list = Find.Map.listerThings.ThingsOfDef(thingDef);
            }
            for (int i = 0; i < list.Count; i++)
            {
                Thing thing = list[i];
                if (thing.def.ingestible.preferability == FoodPreferability.Raw)
                {
                    if (HaulAIUtility.PawnCanAutomaticallyHaul(pawn, thing))
                    {
                        if (Find.SlotGroupManager.SlotGroupAt(building.Position).Settings.AllowedToAccept(thing))
                        {
                            StoragePriority storagePriority = HaulAIUtility.StoragePriorityAtFor(thing.Position, thing);
                            if (storagePriority < hopperSgp.GetSlotGroup().Settings.Priority)
                            {
                                Job job = HaulAIUtility.HaulMaxNumToCellJob(pawn, thing, building.Position, true);
                                if (job != null)
                                {
                                    return job;
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }
    }
}
