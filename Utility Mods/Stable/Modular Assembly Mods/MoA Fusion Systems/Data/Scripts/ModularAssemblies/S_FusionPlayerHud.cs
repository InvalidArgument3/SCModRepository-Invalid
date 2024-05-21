﻿using System;
using System.Linq;
using MoA_Fusion_Systems.Data.Scripts.ModularAssemblies.Communication;
using MoA_Fusion_Systems.Data.Scripts.ModularAssemblies.HudHelpers;
using RichHudFramework.Client;
using RichHudFramework.UI.Client;
using Sandbox.Game;
using VRage.Game.Components;
using VRage.Utils;

namespace MoA_Fusion_Systems.Data.Scripts.ModularAssemblies
{
    /// <summary>
    ///     Semi-independent script for managing the player HUD.
    /// </summary>
    [MySessionComponentDescriptor(MyUpdateOrder.AfterSimulation)]
    public class S_FusionPlayerHud : MySessionComponentBase
    {
        public static S_FusionPlayerHud I;
        private int _ticks;

        private ConsumptionBar ConsumptionBar;
        private static ModularDefinitionApi ModularApi => ModularDefinition.ModularApi;
        private static S_FusionManager FusionManager => S_FusionManager.I;

        #region Base Methods

        public override void LoadData()
        {
            I = this;

            FusionManager.Load();
            RichHudClient.Init("FusionSystems", () => { }, () => { });
        }

        protected override void UnloadData()
        {
            FusionManager.Unload();
            I = null;

            //RichHudClient.Reset();
        }

        private bool _questlogDisposed = false;
        public override void UpdateAfterSimulation()
        {
            _ticks++;
            try
            {
                if (ConsumptionBar == null && RichHudClient.Registered)
                    ConsumptionBar = new ConsumptionBar(HudMain.HighDpiRoot)
                    {
                        Visible = true
                    };

                FusionManager.UpdateTick();
                ConsumptionBar?.Update();

                if (ModularApi.IsDebug())
                {
                    MyVisualScriptLogicProvider.SetQuestlogLocal(true,
                        $"Fusion Systems ({FusionManager.FusionSystems.Count})");

                    // Limits the number of displayed systems to 6
                    var displayedCount = 0;
                    foreach (var system in FusionManager.FusionSystems.Values.ToList())
                    {
                        if (displayedCount > 6 || system.Arms.Count == 0)
                            continue;

                        MyVisualScriptLogicProvider.AddQuestlogDetailLocal(
                            $"[{system.PhysicalAssemblyId}] Power: {Math.Round(system.PowerStored / system.MaxPowerStored * 100f)}% ({Math.Round(system.MaxPowerStored)} @ {Math.Round(system.PowerGeneration * 60, 1)}/s) | Loops: {system.Arms.Count} | Blocks: {system.BlockCount}",
                            false, false);
                        displayedCount++;
                    }
                    _questlogDisposed = false;
                }
                else if (!_questlogDisposed)
                {
                    MyVisualScriptLogicProvider.SetQuestlogLocal(false, "Fusion Systems");
                    _questlogDisposed = true;
                }
            }
            catch (Exception ex)
            {
                MyLog.Default.WriteLineAndConsole(ex.ToString());
            }
        }

        #endregion
    }
}