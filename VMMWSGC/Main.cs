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

        //Ref: https://learn.microsoft.com/windows/win32/api/psapi/nf-psapi-emptyworkingset
        [DllImport("psapi.dll")]
        public static extern bool EmptyWorkingSet(IntPtr hProcess);

        [HarmonyPostfix]
        private static void Postfix()
        {
            try
            {
                /*Looks like there is an issue with Mono/Unity that causes Process.WorkingSet64 to return 0.
                This prevents us from providing the end user with a nice value of 'saved' working set. :-(
                */

                EmptyWorkingSet(Process.GetCurrentProcess().Handle);
                s_log.Log("[VMMWSGC] Private WorkingSet emptied");
            }
            catch (Exception ex)
            {
                s_log.Log("[VMMWSGC] Error emptying Private WorkingSet " + ex.ToString());
                s_log.Log("[VMMWSGC] Please open an issue with the stack trace at https://github.com/0x1d7/VMMWSGC/issues");
            }
        }
    }
}