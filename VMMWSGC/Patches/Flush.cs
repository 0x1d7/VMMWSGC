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
using System.Reflection;

namespace VMMWSGC.Patches
{
    public class Flush
    {
        private static readonly ILog s_log = Logger.GetLogger(nameof(VMMWSGC));

        [HarmonyPostfix]
        [HarmonyPatch(typeof(SimGameState), "SaveSerializationComplete")]
        [HarmonyPatch(typeof(SaveGameStructure), "NotifyRefresh")]
        [HarmonyPatch(typeof(SaveGameStructure), "NotifyLoadCcomplete")]
        [HarmonyPatch(typeof(SimGameState), "OnHeadAttachedStateCompleteListener")]
        [HarmonyPatch(typeof(TurnDirector), "StartFirstRound")]
        [HarmonyPatch(typeof(TurnDirector), "BeginNewRound")]
        [HarmonyPatch(typeof(SimGameState), "ResolveCompleteContract")]
        public static void PostLoad(MethodBase __originalMethod)
        {
            switch (__originalMethod.Name)
            {
                /* Runs after saving a game in certain scenarios */
                case "SaveSerializationComplete":
                    if (Main.Settings.RunPostSaveSerialization)
                    {
                        s_log.Log("<SimGameState.SaveSerializationComplete> Running save serialization complete");
                        Gc.RunGc();
                    }
                    break;
                /* Runs after saving a game in certain scenarios */
                case "NotifyRefresh":
                    if (Main.Settings.RunPostSaveRefresh)
                    {
                        s_log.Log("<SaveGameStructure.NotifyRefresh> Running post save refresh");
                        Gc.RunGc();
                    }
                    break;
                /* Runs after loading a game in certain scenarios */
                case "NotifyLoadCcomplete":
                    if (Main.Settings.RunPostLoadComplete)
                    {
                        s_log.Log("<SaveGameStructure.NotifyLoadCcomplete> Running post notify load complete");
                        Gc.RunGc();
                    }
                    break;
                /* Runs during and after a game is loaded in certain scenarios */
                case "OnHeadAttachedStateCompleteListener":
                    if (Main.Settings.RunPostHeadAttachedState)
                    {
                        s_log.Log("<SimGameState.OnHeadAttachedStateCompleteListener> Running post attached state complete");
                        Gc.RunGc();
                    }
                    break;
                /* Runs in-mission when the first round begins */
                case "StartFirstRound":
                    if (Main.Settings.RunFirstRound)
                    {
                        s_log.Log("<TurnDirector.StartFirstRound> Running start first round");
                        Gc.RunGc();
                    }
                    break;
                /* Runs at the start of each round of a mission
                * Not sure if this has any perf impact during gameplay
                * Will leave this setting up to the player */
                case "BeginNewRound":
                    if (Main.Settings.RunOnNewRound)
                    {
                        s_log.Log("<TurnDirector.BeginNewRound> Running on new round");
                        Gc.RunGc();
                    }
                    break;
                /* Runs post-mission after clicking Continue on the salvage screen
                * Likely not required anymore, but will leave it up to the player */
                case "ResolveCompleteContract":
                    if (Main.Settings.ResolveCompleteContract)
                    {
                        s_log.Log("<SimGateState.ResolveCompleteContract> Running post contract");
                        Gc.RunGc();
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
