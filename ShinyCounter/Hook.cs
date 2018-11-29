using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ShinyCounter {
    public static class Hook {
        //Found here
        //https://social.msdn.microsoft.com/Forums/en-US/4f731541-1819-4391-bd66-d026b629b786/detect-keypress-in-the-background?forum=csharpgeneral

        private static class API {
            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern IntPtr SetWindowsHookEx(int idHook, HookDel lpfn, IntPtr hMod, uint dwThreadId);

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool UnhookWindowsHookEx(IntPtr hhk);

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern IntPtr GetModuleHandle(string lpModuleName);
        }

        public delegate IntPtr HookDel(int nCode, IntPtr wParam, IntPtr lParam);
        public delegate void KeyHandler(IntPtr wParam, IntPtr lParam);

        private static IntPtr hhk = IntPtr.Zero;
        private static HookDel hd;
        private static KeyHandler kh;

        public static void CreateHook(KeyHandler _kh) {
            Process _this = Process.GetCurrentProcess();
            ProcessModule mod = _this.MainModule;

            hd = HookFunc;
            kh = _kh;

            hhk = API.SetWindowsHookEx(13, hd, API.GetModuleHandle(mod.ModuleName), 0);
        }

        public static bool DestroyHook() {
            return API.UnhookWindowsHookEx(hhk);
        }

        private static IntPtr HookFunc(
            int nCode,
            IntPtr wParam,
            IntPtr lParam) {
            int iwParam = wParam.ToInt32();
            if (nCode >= 0 && (iwParam == 0x100 || iwParam == 0x104))
                kh(wParam, lParam);

            return API.CallNextHookEx(hhk, nCode, wParam, lParam);
        }
    }
}
