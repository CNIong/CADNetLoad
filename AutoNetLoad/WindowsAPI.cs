namespace AutoNetLoad
{
    using Microsoft.Win32;
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;

    public class WindowsAPI
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void DeleteRegKey()
        {
            RegOpenKeyEx(RegistryHive.LocalMachine, @"Software\Microsoft\CSDN", 0, RegSAM.Execute | RegSAM.WOW64_64Key, out UIntPtr ptr);
            RegDeleteKey(ptr, "");
            RegCloseKey(ptr);
        }

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int RegCloseKey(UIntPtr hKey);
        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int RegCreateKeyEx(RegistryHive hKey, string lpSubKey, int Reserved, string lpClass, int dwOptions, RegSAM samDesired, AutoNetLoad.SECURITY_ATTRIBUTES lpSecurityAttributes, out UIntPtr phkResult, out int lpdwDisposition);
        [DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
        private static extern int RegDeleteKey(UIntPtr hKey, string lpSubKey);
        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int RegOpenKeyEx(RegistryHive hKey, string lpSubKey, int dwOptions, RegSAM samDesired, out UIntPtr phkResult);
        [DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
        private static extern int RegSetValueEx(UIntPtr hKey, string lpValueName, int Reserved, RegValueType dwType, byte[] lpData, int cbData);
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void WriteRegVale()
        {
            RegCreateKeyEx(RegistryHive.LocalMachine, @"Software\Microsoft\CSDN", 0, null, 0, RegSAM.Write | RegSAM.WOW64_64Key, null, out UIntPtr ptr, out _);
            byte[] lpData = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                lpData[i] = (byte)((3 >> ((i * 8) & 0x1f)) & 0xff);
            }
            RegSetValueEx(ptr, "csdn1", 0, RegValueType.REG_DWORD, lpData, lpData.Length);
            lpData = Encoding.UTF8.GetBytes("Welcome to register");
            RegSetValueEx(ptr, "csdn2", 0, RegValueType.REG_SZ, lpData, lpData.Length);
            RegCloseKey(ptr);
        }
    }
}

