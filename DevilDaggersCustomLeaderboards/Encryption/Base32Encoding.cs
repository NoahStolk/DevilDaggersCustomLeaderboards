using System;

namespace DevilDaggersCustomLeaderboards.Encryption;

/// <summary>
/// Taken from https://stackoverflow.com/a/7135008.
/// </summary>
public static class Base32Encoding
{
	public static byte[] ToBytes(string input)
	{
		if (string.IsNullOrEmpty(input))
			throw new ArgumentNullException(nameof(input));

		input = input.TrimEnd('='); // Remove padding characters.
		int byteCount = input.Length * 5 / 8; // This must be truncated.
		byte[] returnArray = new byte[byteCount];

		byte curByte = 0, bitsRemaining = 8;
		int arrayIndex = 0;

		foreach (char c in input)
		{
			int cValue = CharToValue(c);
			int mask;
			if (bitsRemaining > 5)
			{
				mask = cValue << bitsRemaining - 5;
				curByte = (byte)(curByte | mask);
				bitsRemaining -= 5;
			}
			else
			{
				mask = cValue >> 5 - bitsRemaining;
				curByte = (byte)(curByte | mask);
				returnArray[arrayIndex++] = curByte;
				curByte = (byte)(cValue << 3 + bitsRemaining);
				bitsRemaining += 3;
			}
		}

		// If we didn't end with a full byte.
		if (arrayIndex != byteCount)
			returnArray[arrayIndex] = curByte;

		return returnArray;
	}

	public static string ToString(byte[] input)
	{
		if (input == null || input.Length == 0)
			throw new ArgumentNullException(nameof(input));

		int charCount = (int)Math.Ceiling(input.Length / 5d) * 8;
		char[] returnArray = new char[charCount];

		byte nextChar = 0, bitsRemaining = 5;
		int arrayIndex = 0;

		foreach (byte b in input)
		{
			nextChar = (byte)(nextChar | b >> 8 - bitsRemaining);
			returnArray[arrayIndex++] = ValueToChar(nextChar);

			if (bitsRemaining < 4)
			{
				nextChar = (byte)(b >> 3 - bitsRemaining & 31);
				returnArray[arrayIndex++] = ValueToChar(nextChar);
				bitsRemaining += 5;
			}

			bitsRemaining -= 3;
			nextChar = (byte)(b << bitsRemaining & 31);
		}

		// If we didn't end with a full char.
		if (arrayIndex != charCount)
		{
			returnArray[arrayIndex++] = ValueToChar(nextChar);
			while (arrayIndex != charCount)
				returnArray[arrayIndex++] = '='; // Padding
		}

		return new string(returnArray);
	}

	private static int CharToValue(char c)
	{
		int value = c;

		// 65-90 == upper-case letters
		if (value < 91 && value > 64)
			return value - 65;

		// 50-55 == numbers 2-7
		if (value < 56 && value > 49)
			return value - 24;

		// 97-122 == lower-case letters
		if (value < 123 && value > 96)
			return value - 97;

		throw new ArgumentException("Character is not a Base32 character.", nameof(c));
	}

	private static char ValueToChar(byte b)
	{
		if (b < 26)
			return (char)(b + 65);

		if (b < 32)
			return (char)(b + 24);

		throw new ArgumentException("Byte is not a Base32 value.", nameof(b));
	}
}
