﻿using LunaClient.Base;
using LunaClient.Systems.Lock;
using LunaClient.Systems.SettingsSys;
using UniLinq;
using UnityEngine;

namespace LunaClient.Systems.VesselImmortalSys
{
    /// <summary>
    /// This class makes the other vessels immortal, this way if we crash against them they are not destroyed but we do.
    /// In the other player screens they will be destroyed and they will send their new vessel definition.
    /// </summary>
    public class VesselImmortalSystem : Base.System
    {
        #region Fields & properties

        private bool VesselImmortalSystemReady => Enabled && HighLogic.LoadedSceneIsFlight && FlightGlobals.ready && Time.timeSinceLevelLoad > 1f;

        #endregion

        #region Base overrides

        protected override void OnEnabled()
        {
            base.OnEnabled();
            SetupRoutine(new RoutineDefinition(2000, RoutineExecution.Update, MakeOtherPlayerVesselsImmortal));
        }

        protected override void OnDisabled()
        {
            base.OnDisabled();
            RemoveRoutines();

            //In case we disable this system, set all the vessels back as mortal...
            foreach (var vessel in FlightGlobals.Vessels)
            {
                SetVesselImmortalState(vessel, false);
            }
        }

        #endregion

        #region Update methods

        /// <summary>
        /// Make the other player vessels inmortal
        /// </summary>
        private void MakeOtherPlayerVesselsImmortal()
        {
            if (Enabled && VesselImmortalSystemReady)
            {
                var ownedVessels = LockSystem.LockQuery.GetAllControlLocks(SettingsSystem.CurrentSettings.PlayerName)
                                        .Select(l => l.VesselId)
                                    .Union(LockSystem.LockQuery.GetAllUpdateLocks(SettingsSystem.CurrentSettings.PlayerName)
                                        .Select(l => l.VesselId))
                                    .Select(FlightGlobals.FindVessel)
                                    .Where(v => v != null)
                                    .ToArray();

                var othersPeopleVessels = LockSystem.LockQuery.GetAllControlLocks()
                    .Union(LockSystem.LockQuery.GetAllUpdateLocks())
                    .Select(l => l.VesselId)
                    .Except(ownedVessels.Select(v => v.id))
                    .Select(FlightGlobals.FindVessel)
                    //Select the vessels and filter out the nulls
                    .Where(v => v != null).ToArray();

                foreach (var vessel in ownedVessels)
                {
                    SetVesselImmortalState(vessel, false);
                }

                foreach (var vessel in othersPeopleVessels)
                {
                    SetVesselImmortalState(vessel, true);
                }
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Set all vessel parts to unbreakable or not (makes the vessel immortal or not)
        /// </summary>
        private static void SetVesselImmortalState(Vessel vessel, bool immortal)
        {
            vessel.Parts.Where(p => p.attachJoint != null).ToList()
                .ForEach(p => p.attachJoint.SetUnbreakable(immortal, immortal));
        }

        #endregion
    }
}
