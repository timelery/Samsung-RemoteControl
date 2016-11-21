using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SmartTVRemoteControl.Native.Wlan
{
	public static class WlanApiWrapper
	{
		private const int WLAN_API_VERSION_2_0 = 2;

		private const int ERROR_SUCCESS = 0;

		public static IEnumerable<WlanInterfaceInfo> EnumInterfaces()
		{
			uint num = 0;
			IntPtr zero = IntPtr.Zero;
			if (WlanApiWrapper.WlanOpenHandle(2, IntPtr.Zero, out num, out zero) == 0)
			{
				IntPtr intPtr = IntPtr.Zero;
				if (WlanApiWrapper.WlanEnumInterfaces(zero, IntPtr.Zero, out intPtr) == 0)
				{
					int num1 = Marshal.ReadInt32(intPtr, 0);
					Marshal.ReadInt32(intPtr, 4);
					for (int i = 0; i < num1; i++)
					{
						IntPtr intPtr1 = new IntPtr(intPtr.ToInt32() + i * 532 + 8);
						WlanApiWrapper.WLAN_INTERFACE_INFO structure = (WlanApiWrapper.WLAN_INTERFACE_INFO)Marshal.PtrToStructure(intPtr1, typeof(WlanApiWrapper.WLAN_INTERFACE_INFO));
						WlanInterfaceInfo wlanInterfaceInfo = new WlanInterfaceInfo()
						{
							Guid = structure.InterfaceGuid,
							Description = structure.strInterfaceDescription,
							State = structure.isState
						};
						yield return wlanInterfaceInfo;
					}
					if (intPtr != IntPtr.Zero)
					{
						WlanApiWrapper.WlanFreeMemory(intPtr);
					}
				}
				WlanApiWrapper.WlanCloseHandle(zero, IntPtr.Zero);
			}
		}

		public static WlanConnectionInfo GetCurrentConnection(Guid guid)
		{
			uint num;
			uint num1 = 0;
			IntPtr zero = IntPtr.Zero;
			WlanConnectionInfo wlanConnectionInfo = null;
			if (WlanApiWrapper.WlanOpenHandle(2, IntPtr.Zero, out num1, out zero) == 0)
			{
				IntPtr intPtr = IntPtr.Zero;
				if (WlanApiWrapper.WlanQueryInterface(zero, ref guid, WlanApiWrapper.WLAN_INTF_OPCODE.wlan_intf_opcode_current_connection, IntPtr.Zero, out num, ref intPtr, IntPtr.Zero) == 0)
				{
					WlanApiWrapper.WLAN_CONNECTION_ATTRIBUTES structure = (WlanApiWrapper.WLAN_CONNECTION_ATTRIBUTES)Marshal.PtrToStructure(intPtr, typeof(WlanApiWrapper.WLAN_CONNECTION_ATTRIBUTES));
					WlanConnectionInfo wlanConnectionInfo1 = new WlanConnectionInfo()
					{
						State = structure.isState,
						ConnectionMode = structure.wlanConnectionMode,
						ProfileName = structure.strProfileName
					};
					wlanConnectionInfo = wlanConnectionInfo1;
					if (intPtr != IntPtr.Zero)
					{
						WlanApiWrapper.WlanFreeMemory(intPtr);
					}
				}
				WlanApiWrapper.WlanCloseHandle(zero, IntPtr.Zero);
			}
			return wlanConnectionInfo;
		}

		[DllImport("wlanapi.dll", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		private static extern uint WlanCloseHandle(IntPtr hClientHandle, IntPtr pReserved);

		[DllImport("wlanapi.dll", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		private static extern uint WlanEnumInterfaces(IntPtr hClientHandle, IntPtr pReserved, out IntPtr ppInterfaceList);

		[DllImport("wlanapi.dll", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		private static extern void WlanFreeMemory(IntPtr pmemory);

		[DllImport("wlanapi.dll", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		private static extern uint WlanOpenHandle(uint dwClientVersion, IntPtr pReserved, out uint pdwNegotiatedVersion, out IntPtr phClientHandle);

		[DllImport("wlanapi.dll", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		private static extern uint WlanQueryInterface(IntPtr hClientHandle, ref Guid pInterfaceGuid, WlanApiWrapper.WLAN_INTF_OPCODE OpCode, IntPtr pReserved, out uint pdwDataSize, ref IntPtr ppData, IntPtr pWlanOpcodeValueType);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
        private struct WLAN_CONNECTION_ATTRIBUTES
		{
			public WlanInterfaceState isState;

			public WlanConnectionMode wlanConnectionMode;

            [MarshalAs(UnmanagedType.LPWStr, SizeConst = 256)]
            public string strProfileName;
		}

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct WLAN_INTERFACE_INFO
		{
			public Guid InterfaceGuid;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string strInterfaceDescription;

			public WlanInterfaceState isState;
		}

		private enum WLAN_INTF_OPCODE
		{
			wlan_intf_opcode_autoconf_start = 0,
			wlan_intf_opcode_autoconf_enabled = 1,
			wlan_intf_opcode_background_scan_enabled = 2,
			wlan_intf_opcode_media_streaming_mode = 3,
			wlan_intf_opcode_radio_state = 4,
			wlan_intf_opcode_bss_type = 5,
			wlan_intf_opcode_interface_state = 6,
			wlan_intf_opcode_current_connection = 7,
			wlan_intf_opcode_channel_number = 8,
			wlan_intf_opcode_supported_infrastructure_auth_cipher_pairs = 9,
			wlan_intf_opcode_supported_adhoc_auth_cipher_pairs = 10,
			wlan_intf_opcode_supported_country_or_region_string_list = 11,
			wlan_intf_opcode_current_operation_mode = 12,
			wlan_intf_opcode_supported_safe_mode = 13,
			wlan_intf_opcode_certified_safe_mode = 14,
			wlan_intf_opcode_autoconf_end = 268435455,
			wlan_intf_opcode_msm_start = 268435712,
			wlan_intf_opcode_statistics = 268435713,
			wlan_intf_opcode_rssi = 268435714,
			wlan_intf_opcode_msm_end = 536870911,
			wlan_intf_opcode_security_start = 536936448,
			wlan_intf_opcode_security_end = 805306367,
			wlan_intf_opcode_ihv_start = 805306368,
			wlan_intf_opcode_ihv_end = 1073741823
		}
	}
}