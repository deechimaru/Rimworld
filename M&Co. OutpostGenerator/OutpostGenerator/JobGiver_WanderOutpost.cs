﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;
using RimWorld;
//using RimWorld.Planet;


namespace OutpostGenerator
{
    public class JobGiver_WanderOutpost : JobGiver_Wander
    {
        public JobGiver_WanderOutpost()
        {
            this.wanderRadius = 10f;
            this.ticksBetweenWandersRange = new IntRange(125, 200);
            this.locomotionUrgency = LocomotionUrgency.Amble;
            this.wanderDestValidator = delegate (Pawn pawn, IntVec3 loc)
            {
                if ((OG_Util.FindOutpostArea() != null)
                && (OG_Util.FindOutpostArea().ActiveCells.Contains(loc)))
                {
                    return true;
                }
                return false;
            };
        }

        protected override Job TryGiveTerminalJob(Pawn pawn)
        {
            if (OG_Util.FindOutpostArea() == null)
            {
                return null;
            }
            return base.TryGiveTerminalJob(pawn);
        }

        protected override IntVec3 GetWanderRoot(Pawn pawn)
        {
            return pawn.Position;
        }
    }
}
