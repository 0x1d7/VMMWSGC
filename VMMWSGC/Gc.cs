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

using HBS.Logging;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;


namespace VMMWSGC
{
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