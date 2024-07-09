﻿using Sandbox.Game.Entities;
using Sandbox.ModAPI;
using SC.SUGMA.GameState;
using SC.SUGMA.HeartNetworking;
using SC.SUGMA.HeartNetworking.Custom;
using VRage.Game;
using VRage.Game.ModAPI;
using VRageMath;

namespace SC.SUGMA.Textures
{
    public static class SUtils
    {
        private const int DamageToggleInt = 0x1;
        private const int MatchPermsInt = 0x29B;

        private const int FullPermsInt = 0x3FF;

        // Good lord this is horrendous, thanks Digi
        public static T CastProhibit<T>(T ptr, object val)
        {
            return (T)val;
        }

        public static void SetDamageEnabled(bool value)
        {
            MyAPIGateway.Utilities.ShowMessage("SUGMA", $"Global damage {(value ? "enabled" : "disabled")}.");
            var existing = (int)MySessionComponentSafeZones.AllowedActions;
            MySessionComponentSafeZones.AllowedActions = CastProhibit(MySessionComponentSafeZones.AllowedActions,
                value ? existing | DamageToggleInt : existing & ~DamageToggleInt);
        }

        public static void SetWorldPermissionsForMatch(bool matchActive)
        {
            MyAPIGateway.Utilities.ShowMessage("SUGMA",
                $"Match global permissions {(matchActive ? "enabled" : "disabled")}.");

            MySessionComponentSafeZones.AllowedActions = CastProhibit(MySessionComponentSafeZones.AllowedActions,
                matchActive ? MatchPermsInt : FullPermsInt);
        }

        public static IMyFaction GetFaction(this IMyCubeGrid grid)
        {
            return PlayerTracker.I.GetGridFaction(grid);
        }

        public static Color ColorMaskToRgb(this Vector3 colorMask)
        {
            return MyColorPickerConstants.HSVOffsetToHSV(colorMask).HSVtoColor();
        }


        public static void ReportProblem(string issueMessage = "")
        {
            if (MyAPIGateway.Session.IsServer)
                new ProblemReportPacket(true, issueMessage).Received(0);
            else
                HeartNetwork.I.SendToServer(new ProblemReportPacket(true, issueMessage));
        }

        public static void ResolvedProblem()
        {
            if (MyAPIGateway.Session.IsServer)
                new ProblemReportPacket(false).Received(0);
            else
                HeartNetwork.I.SendToServer(new ProblemReportPacket(false));
        }
    }
}