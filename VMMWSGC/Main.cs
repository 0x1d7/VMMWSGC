using HarmonyLib;
using HBS.Logging;
using BattleTech;
using System;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace VMMWSGC
{ 
    [HarmonyPatch(typeof(SimGameState), "ResolveCompleteContract")]
    public static class Main
    {
        private static readonly ILog s_log = Logger.GetLogger(nameof(VMMWSGC));
        public static void Start()
        {
            Harmony.CreateAndPatchAll(typeof(Main).Assembly, "github.com.0x1d7.vmmwsgc");
            s_log.Log("VMMWSGC loaded");
        }

        [DllImport("psapi.dll")]
        public static extern bool EmptyWorkingSet(IntPtr hProcess);

        [HarmonyPostfix]
        private static void Postfix()
        {
            try
            {
                EmptyWorkingSet(Process.GetCurrentProcess().Handle);
                s_log.Log("[VMMWSGC] Private WorkingSet emptied");
            }
            catch (Exception ex)
            {
                s_log.Log("[VMMWSGC] Error emptying Private WorkingSet " + ex.ToString());
            }
        }
    }
}