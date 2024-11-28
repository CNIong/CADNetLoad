using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace AutoNetLoad
{
	// Token: 0x02000006 RID: 6
	public class Utility
	{
		// Token: 0x06000020 RID: 32
		[DllImport("Kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern bool Wow64DisableWow64FsRedirection(ref IntPtr ptr);

		// Token: 0x06000021 RID: 33
		[DllImport("Kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern bool Wow64RevertWow64FsRedirection(IntPtr ptr);

		// Token: 0x06000022 RID: 34
		[DllImport("Advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern uint RegOpenKeyEx(UIntPtr hKey, string lpSubKey, uint ulOptions, int samDesired, out IntPtr phkResult);

		// Token: 0x06000023 RID: 35
		[DllImport("Advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern long RegDisableReflectionKey(IntPtr hKey);

		// Token: 0x06000024 RID: 36
		[DllImport("Advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern long RegEnableReflectionKey(IntPtr hKey);

		// Token: 0x06000025 RID: 37
		[DllImport("Advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		private static extern int RegQueryValueEx(IntPtr hKey, string lpValueName, int lpReserved, out uint lpType, StringBuilder lpData, ref uint lpcbData);

		// Token: 0x06000026 RID: 38 RVA: 0x00004190 File Offset: 0x00002390
		[MethodImpl(MethodImplOptions.NoInlining)]
		private static UIntPtr TransferKeyName(string keyName)
		{
			UIntPtr uintPtr;
			if (!(keyName == "HKEY_CLASSES_ROOT"))
			{
				if (!(keyName == "HKEY_CURRENT_USER"))
				{
					if (!(keyName == "HKEY_LOCAL_MACHINE"))
					{
						if (!(keyName == "HKEY_USERS"))
						{
							if (!(keyName == "HKEY_CURRENT_CONFIG"))
							{
								uintPtr = Utility.HKEY_CLASSES_ROOT;
							}
							else
							{
								uintPtr = Utility.HKEY_CURRENT_CONFIG;
							}
						}
						else
						{
							uintPtr = Utility.HKEY_USERS;
						}
					}
					else
					{
						uintPtr = Utility.HKEY_LOCAL_MACHINE;
					}
				}
				else
				{
					uintPtr = Utility.HKEY_CURRENT_USER;
				}
			}
			else
			{
				uintPtr = Utility.HKEY_CLASSES_ROOT;
			}
			return uintPtr;
		}

		// Token: 0x06000027 RID: 39 RVA: 0x00004214 File Offset: 0x00002414
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static string Get64BitRegistryKey(string parentKeyName, string subKeyName, string keyName)
		{
			int num = 257;
			string text;
			try
			{
				UIntPtr uintPtr = Utility.TransferKeyName(parentKeyName);
				IntPtr zero = IntPtr.Zero;
				StringBuilder stringBuilder = new StringBuilder("".PadLeft(1024));
				uint num2 = 1024U;
				uint num3 = 0U;
				IntPtr intPtr = (IntPtr)0;
				if (Utility.Wow64DisableWow64FsRedirection(ref intPtr))
				{
					Utility.RegOpenKeyEx(uintPtr, subKeyName, 0U, num, out zero);
					Utility.RegDisableReflectionKey(zero);
					Utility.RegQueryValueEx(zero, keyName, 0, out num3, stringBuilder, ref num2);
					Utility.RegEnableReflectionKey(zero);
				}
				Utility.Wow64RevertWow64FsRedirection(intPtr);
				text = stringBuilder.ToString().Trim();
			}
			catch (Exception)
			{
				text = null;
			}
			return text;
		}

		// Token: 0x0400002D RID: 45
		private static UIntPtr HKEY_CLASSES_ROOT = (UIntPtr)2147483648U;

		// Token: 0x0400002E RID: 46
		private static UIntPtr HKEY_CURRENT_USER = (UIntPtr)2147483649U;

		// Token: 0x0400002F RID: 47
		private static UIntPtr HKEY_LOCAL_MACHINE = (UIntPtr)2147483650U;

		// Token: 0x04000030 RID: 48
		private static UIntPtr HKEY_USERS = (UIntPtr)2147483651U;

		// Token: 0x04000031 RID: 49
		private static UIntPtr HKEY_CURRENT_CONFIG = (UIntPtr)2147483653U;
	}
}
