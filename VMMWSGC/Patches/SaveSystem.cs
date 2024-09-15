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
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace VMMWSGC.Patches
{
    public class SaveSystem
    {
        private static readonly ILog s_log = Logger.GetLogger(nameof(VMMWSGC));

        /* Skip all autosaves -- welcome to the 1990s, kids! */
        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameInstance), "Save", new Type[] { typeof(SaveReason) })]
        public static bool NoAutoSave(ref SaveReason reason)
        {
            if (Main.Settings.NoAutosaves)
            {
                s_log.Log($"<SaveReason:{reason}>... No save for you!");

                return false;
            }

            return true;
        }

        /* Prevents compression. Leads to very large but faster saves
         * ~5 second improvement on a 5800X3D Samsung 980 Pro 2TB
         * Loading does not work.
         */
        [HarmonyPrefix]
        [HarmonyPatch(typeof(SaveBlock<GameInstanceSave>), "CompressBytes", new Type[] { typeof(byte[]) })]
        public static bool CompressBytesNew(out byte[] __result, byte[] bytes)
        {
            if (Main.Settings.OverrideSaveSystem)
            {
                s_log.Log($"VMMWSGC Saving {bytes.Length} bytes with no compression");
                __result = bytes.ToArray();

                return false;
            }

            __result = null;

            return true;
        }

        /* ToDo: Configure method to load uncompressed saves.
         * Implement fallback for compressed saves. */
        [HarmonyPrefix]
        [HarmonyPatch(typeof(SaveBlock<string>), "DecompressBytes", new Type[] { typeof(byte[]) })]
        public static bool DecompressBytesNew(out byte[] __result, byte[] bytes)
        {
            if (Main.Settings.OverrideSaveSystem)
            {
                s_log.Log($"VMMWSGC Loading {bytes.Length} bytes from save");
                using (var ms = new MemoryStream(bytes))
                using (var ds = new MemoryStream())
                using (var gzipStream = new GZipStream(ms, CompressionMode.Decompress, false))
                {
                    gzipStream.CopyTo(ds);
                    __result = ds.ToArray();
                }

                return false;
            }

            __result = null;

            return true;
        }
    }
}
