using System;
using System.Globalization;

namespace DevilDaggersCustomLeaderboards
{
	public static class Address
	{
		public static string MakeAddress(byte[] buffer)
		{
			string sTemp = string.Empty;

			for (int i = 0; i < buffer.Length; i++)
			{
				if (Convert.ToInt16(buffer[i]) < 10)
					sTemp = $"0{ToHex(buffer[i])}{sTemp}";
				else
					sTemp = $"{ToHex(buffer[i])}{sTemp}";
			}

			return sTemp;
		}

		public static string ToHex(int dec)
		{
			return dec.ToString("X");
		}

		public static int ToDec(string hex)
		{
			return int.Parse(hex, NumberStyles.HexNumber);
		}
	}
}