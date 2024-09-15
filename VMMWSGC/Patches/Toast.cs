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

using BattleTech.UI;
using HarmonyLib;
using HBS.Logging;

namespace VMMWSGC.Patches
{
    public class Toast
    {
        private static readonly ILog s_log = Logger.GetLogger(nameof(VMMWSGC));

        [HarmonyPostfix]
        [HarmonyPatch(typeof(SGSaveToast), "Invisible")]
        public static void PostInvisibleToast()
        {
            s_log.Log("Cleaning up on invisible toast");
            Gc.RunGc();
        }
    }
}
