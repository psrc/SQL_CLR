using System;

public static class StringDistance
{
	public static int GetDamerauLevenshteinDistance(string s, string t)
	{
		var anon = new
		{
			Height = s.Length + 1,
			Width = t.Length + 1
		};
		int[,] array = new int[anon.Height, anon.Width];
		for (int i = 0; i < anon.Height; i++)
		{
			array[i, 0] = i;
		}
		for (int j = 0; j < anon.Width; j++)
		{
			array[0, j] = j;
		}
		for (int k = 1; k < anon.Height; k++)
		{
			for (int l = 1; l < anon.Width; l++)
			{
				int num = ((s[k - 1] != t[l - 1]) ? 1 : 0);
				int val = array[k, l - 1] + 1;
				int val2 = array[k - 1, l] + 1;
				int val3 = array[k - 1, l - 1] + num;
				int num2 = Math.Min(val, Math.Min(val2, val3));
				if (k > 1 && l > 1 && s[k - 1] == t[l - 2] && s[k - 2] == t[l - 1])
				{
					num2 = Math.Min(num2, array[k - 2, l - 2] + num);
				}
				array[k, l] = num2;
			}
		}
		return array[anon.Height - 1, anon.Width - 1];
	}
}
