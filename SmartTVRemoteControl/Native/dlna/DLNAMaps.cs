using System;
using System.Collections.Generic;

namespace SmartTVRemoteControl.Native.DLNA
{
	internal static class DLNAMaps
	{
		public readonly static string DefaultStreaming;

		public readonly static string DefaultInteractive;

		private readonly static Dictionary<string, string> PN;

		static DLNAMaps()
		{
			DLNAMaps.DefaultStreaming = DLNAMaps.FlagsToString((DLNAFlags)((long)560988160));
			DLNAMaps.DefaultInteractive = DLNAMaps.FlagsToString((DLNAFlags)((long)552599552));
			Dictionary<string, string> strs = new Dictionary<string, string>()
			{
				{ "video/x-mkv", "DLNA.ORG_PN=MATROSKA" },
				{ "video/x-msvideo", "DLNA.ORG_PN=AVI" },
				{ "video/mpeg", "DLNA.ORG_PN=MPEG1" },
				{ "video/vnd.dlna.mpeg-tts", "DLNA.ORG_PN=MPEG1" },
				{ "image/jpeg", "DLNA.ORG_PN=JPEG" },
				{ "image/png", "DLNA.ORG_PN=PNG" },
				{ "video/mp4", "DLNA.ORG_PN=AVC_MP4_MP_SD_AAC_MULT5" },
				{ "video/quicktime", "DLNA.ORG_PN=AVC_MP4_MP_SD_AAC_MULT5" },
				{ "video/x-m4v", "DLNA.ORG_PN=AVC_MP4_MP_SD_AAC_MULT5" },
				{ "video/3gpp", "DLNA.ORG_PN=AVC_MP4_MP_SD_AAC_MULT5" },
				{ "video/x-flv", "DLNA.ORG_PN=AVC_MP4_MP_SD_AAC_MULT5" },
				{ "audio/mpeg", "DLNA.ORG_PN=MP3" },
				{ "audio/aac", "DLNA.ORG_PN=AAC" },
				{ "audio/ogg", "DLNA.ORG_PN=OGG" },
				{ "video/x-ms-wmv", "DLNA.ORG_PN=WMVHIGH_FULL" },
				{ "audio/x-ms-wma", "DLNA.ORG_PN=WMVHIGH_FULL" },
				{ "video/x-ms-asf", "DLNA.ORG_PN=WMVHIGH_FULL" }
			};
			DLNAMaps.PN = strs;
		}

		private static string FlagsToString(DLNAFlags flags)
		{
			return string.Format("{0:X8}{1:D24}", (ulong)flags, 0);
		}

		public static string GetPNByMime(string mime)
		{
			if (!DLNAMaps.PN.ContainsKey(mime))
			{
				return "";
			}
			return DLNAMaps.PN[mime];
		}
	}
}