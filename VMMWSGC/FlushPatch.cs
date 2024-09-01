/*  Copyright (C) 2024  0x1d7 https://github.com/0x1d7/VMMWSGC

    This library is free software; you can redistribute it and/or
    modify it under the terms of the GNU Lesser General Public
    License as published by the Free Software Foundation; either
    version 2.1 of the License, or (at your option) any later version.

    This library is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
    Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public
    License along with this library; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301
    USA
*/

using BattleTech;
using BattleTech.Save.SaveGameStructure;
using HarmonyLib;
using HBS.Logging;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace VMMWSGC
{
    /* Runs after saving a game in certain scenarios */
    [HarmonyPatch(typeof(SimGameState), "SaveSerializationComplete")]
    static internal class GcPostSaveSerialization
    {
        private static readonly ILog s_log = Logger.GetLogger(nameof(VMMWSGC));

        [HarmonyPostfix]
        private static void PostLoad()
        {
            if (Main.Settings.RunPostSaveSerialization != true) return;

            s_log.Log("<SimGameState.SaveSerializationComplete> Running save serialization complete");
            Gc.RunGc();
        }
    }

    /* Runs after saving a game in certain scenarios */
    [HarmonyPatch(typeof(SaveGameStructure), "NotifyRefresh")]
    static internal class GcPostSaveRefresh
    {
        private static readonly ILog s_log = Logger.GetLogger(nameof(VMMWSGC));

        [HarmonyPostfix]
        private static void PostLoad()
        {
            if (Main.Settings.RunPostSaveRefresh != true) return;

            s_log.Log("<SaveGameStructure.NotifyRefresh> Running post save refresh");
            Gc.RunGc();
        }
    }

    /* Runs after loading a game in certain scenarios */
    [HarmonyPatch(typeof(SaveGameStructure), "NotifyLoadCcomplete")]
    static internal class GcPostLoadComplete
    {
        private static readonly ILog s_log = Logger.GetLogger(nameof(VMMWSGC));

        [HarmonyPostfix]
        private static void PostLoad()
        {
            if (Main.Settings.RunPostLoadComplete != true) return;

            s_log.Log("<SaveGameStructure.NotifyLoadCcomplete> Running post notify load complete");
            Gc.RunGc();
        }
    }

    /* Runs during and after a game is loaded in certain scenarios */
    [HarmonyPatch(typeof(SimGameState), "OnHeadAttachedStateCompleteListener")]
    static internal class GcPostHeadAttachedState
    {
        private static readonly ILog s_log = Logger.GetLogger(nameof(VMMWSGC));

        [HarmonyPostfix]
        private static void PostLoad()
        {
            if (Main.Settings.RunPostHeadAttachedState != true) return;

            s_log.Log("<SimGameState.OnHeadAttachedStateCompleteListener> Running post attached state complete");
            Gc.RunGc();
        }
    }

    /* Runs in-mission when the first round begins */
    [HarmonyPatch(typeof(TurnDirector), "StartFirstRound")]
    static internal class GcFirstRoundMission
    {
        private static readonly ILog s_log = Logger.GetLogger(nameof(VMMWSGC));

        [HarmonyPostfix]
        private static void FirstRoundMission()
        {
            if (Main.Settings.RunFirstRound != true) return;

            s_log.Log("<TurnDirector.StartFirstRound> Running start first round");
            Gc.RunGc();
        }
    }

    /* Runs post-mission after clicking Continue on the salvage screen
     * Likely not required anymore, but will leave it up to the player */
    [HarmonyPatch(typeof(SimGameState), "ResolveCompleteContract")]
    static internal class GcPostMission
    {
        private static readonly ILog s_log = Logger.GetLogger(nameof(VMMWSGC));

        [HarmonyPostfix]
        private static void PostMission()
        {
            if (Main.Settings.ResolveCompleteContract != true) return;

            s_log.Log("<SimGateState.ResolveCompleteContract> Running post contract");
            Gc.RunGc();
        }
    }

    internal static class Gc
    {
        private static readonly ILog s_log = Logger.GetLogger(nameof(VMMWSGC));

        //Ref: https://learn.microsoft.com/windows/win32/api/psapi/nf-psapi-emptyworkingset
        [DllImport("psapi.dll")]
        public static extern bool EmptyWorkingSet(IntPtr hProcess);
        internal static void RunGc()
        {
            try
            {
                /*Looks like there is an issue with Mono/Unity that causes Process.WorkingSet64 to return 0.
                This prevents us from providing the end user with a nice value of 'saved' working set. :-(
                */
                EmptyWorkingSet(Process.GetCurrentProcess().Handle);

                s_log.Log("Private WorkingSet emptied");
            }
            catch (Exception ex)
            {
                s_log.Log("Error emptying Private WorkingSet " + ex.ToString());
                s_log.Log("Please open an issue with the stack trace at https://github.com/0x1d7/VMMWSGC/issues");
            }
        }
    }
}