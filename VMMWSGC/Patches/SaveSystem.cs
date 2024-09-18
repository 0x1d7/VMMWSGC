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
using BattleTech.Save;
using BattleTech.Save.Core;
using BattleTech.Save.SaveGameStructure;
using HarmonyLib;
using HBS.Logging;
using HBS.Pooling;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace VMMWSGC.Patches
{
    public class SaveSystem
    {
        private static readonly ILog s_log = Logger.GetLogger(nameof(VMMWSGC));

        /* Autosave control */
        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameInstance), "Save", new Type[] { typeof(SaveReason) })]
        public static bool NoAutoSave(ref SaveReason reason)
        {
            //Default game autosave settings
            if (Main.Settings.Autosaves.EnableAll)
            {
                s_log.Log($"<SaveReason: {reason}> all autosaves enabled");

                return true;
            }

            if (Main.Settings.Autosaves.DisableAll)
            {
                s_log.Log($"<SaveReason:{reason}> all autosaves disabled");

                return false;
            }

            //Selected useful autosave events
            switch (reason)
            {
                case SaveReason.SIM_GAME_ARRIVED_AT_PLANET:
                    if (Main.Settings.Autosaves.ArrivedAtPlanet)
                    {
                        s_log.Log($"<SaveReason: {reason}> taking autosave");

                        return true;
                    }
                    break;
                case SaveReason.SIM_GAME_EVENT_RESOLVED:
                    if (Main.Settings.Autosaves.EventResolved)
                    {
                        s_log.Log($"<SaveReason: {reason}> taking autosave");

                        return true;
                    }
                    break;
                case SaveReason.SIM_GAME_CONTRACT_ACCEPTED:
                    if (Main.Settings.Autosaves.ContractAccepted)
                    {
                        s_log.Log($"<SaveReason: {reason}> taking autosave");

                        return true;
                    }
                    break;
                case SaveReason.SIM_GAME_COMPLETED_CONTRACT:
                    if (Main.Settings.Autosaves.ContractCompleted)
                    {
                        s_log.Log($"<SaveReason: {reason}> taking autosave");

                        return true;
                    }
                    break;
                case SaveReason.SIM_GAME_QUARTERLY_REPORT:
                    if (Main.Settings.Autosaves.FinancialReport)
                    {
                        s_log.Log($"<SaveReason: {reason}> taking autosave");

                        return true;
                    }
                    break;
                default:
                    break;
            }

            //Some other event occurred that isn't as useful to autosave on
            return false;
        }

        /* Prevents compression. Leads to very large but faster saves
         * ~5 second improvement on a 5800X3D Samsung 980 Pro 2TB
         */
        [HarmonyPrefix]
        [HarmonyPatch(typeof(SaveBlock<GameInstanceSave>), "CompressBytes", new Type[] { typeof(byte[]) })]
        public static bool CompressBytesNew(out byte[] __result, byte[] bytes)
        {
            if (Main.Settings.OverrideSaveSystem)
            {
                s_log.Log($"Saving {bytes.Length} bytes with no compression");
                __result = bytes.ToArray();

                return false;
            }

            __result = null;

            return true;
        }

        /* Determine if save is compressed or uncompressed, then load save.
         * If either route fails, fallback to the normal Battletech file load process 
         */
        [HarmonyPrefix]
        [HarmonyPatch(typeof(SaveBlock<string>), "DecompressBytes", new Type[] { typeof(byte[]) })]
        public static bool DecompressBytesNew(out byte[] __result, byte[] bytes)
        {

            if (Main.Settings.OverrideSaveSystem)
            {
                byte[] compressedHeader = new byte[] { 0x1f, 0x8b, 0x08, 0x00 };
                byte[] uncompressedHeader = new byte[] { 0x08, 0x01, 0x12, 0x3a };

                if (bytes.Take(4).SequenceEqual(compressedHeader))
                {
                    try
                    {
                        s_log.Log($"Loading compressed {bytes.Length} bytes from save");
                        using (var ms = new MemoryStream(bytes))
                        using (var ds = new MemoryStream())
                        using (var gzipStream = new GZipStream(ms, CompressionMode.Decompress, false))
                        {
                            gzipStream.CopyTo(ds);
                            __result = ds.ToArray();

                            return false;
                        }
                    }
                    catch (IOException ex)
                    {
                        s_log.Log("Unable to read save file. Letting Battletech perform " +
                            $"the normal load process \n {ex}");

                        __result = null;
                        return true;
                    }
                }
                else if (bytes.Take(4).SequenceEqual(uncompressedHeader))
                {
                    s_log.Log($"Loading uncompressed {bytes.Length} bytes from save");
                    __result = bytes.ToArray();

                    return false;
                }
            }

            __result = null;

            return true;
        }
    }
}
