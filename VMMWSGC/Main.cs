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

using HarmonyLib;
using HBS.Logging;
using Newtonsoft.Json;
using System;
using System.Reflection;

namespace VMMWSGC
{
    public static class Main
    {
        private static readonly ILog s_log = Logger.GetLogger(nameof(VMMWSGC));
        public static ModSettings Settings = new ModSettings();
        public static void Init(string directory, string settingsJSON)
        {
            Harmony.CreateAndPatchAll(typeof(GcPostSaveSerialization), "github.com.0x1d7.vmmwsgc");
            Harmony.CreateAndPatchAll(typeof(SaveSystem), "github.com.0x1d7.vmmwsgc");
            s_log.Log($"VMMWSGC assembly version {Assembly.GetExecutingAssembly().GetName().Version} loaded");

            try
            {
                Settings = JsonConvert.DeserializeObject<ModSettings>(settingsJSON);
                s_log.Log($"Settings:\n" +
                    $"RunPostSaveSerialization: {Settings.RunPostSaveSerialization}\n" +
                    $"RunPostSaveRefresh: {Settings.RunPostSaveRefresh}\n" +
                    $"RunPostLoadComplete: {Settings.RunPostLoadComplete}\n" +
                    $"RunPostHeadlessAttachedState: {Settings.RunPostHeadAttachedState}\n" +
                    $"RunFirstRound: {Settings.RunFirstRound}\n" +
                    $"RunOnNewRound: {Settings.RunOnNewRound}\n" +
                    $"ResolveCompleteContract: {Settings.ResolveCompleteContract}\n" +
                    $"NoAutosaves: {Settings.NoAutosaves}");
            }
            catch (Exception)
            {
                Settings = new ModSettings();
                s_log.Log("Failed to parse settings");
            }
        }

        public class ModSettings
        {
            public bool RunPostSaveSerialization { get; set; } = true;
            public bool RunPostSaveRefresh { get; set; } = true;
            public bool RunPostLoadComplete { get; set; } = true;
            public bool RunPostHeadAttachedState { get; set; } = true;
            public bool RunFirstRound { get; set; } = true;
            public bool RunOnNewRound { get; set; } = false;
            public bool ResolveCompleteContract { get; set; } = false;
            public bool NoAutosaves { get; set; } = false;
        }
    }
}