using System.Collections;
using System.Data.SqlTypes;
using System.Text.RegularExpressions;
using Microsoft.SqlServer.Server;

namespace SqlClrTools
{
    public sealed class SqlRegEx
    {
        [SqlFunction(DataAccess = DataAccessKind.None, IsDeterministic = true, IsPrecise = true, Name = "ufn_RegExIsMatch", SystemDataAccess = SystemDataAccessKind.None)]
        public static SqlBoolean RegExIsMatch(SqlString input, SqlString pattern, SqlBoolean ignoreCase)
        {
            if (input.IsNull || pattern.IsNull)
            {
                return SqlBoolean.Null;
            }
            return new SqlBoolean(Regex.IsMatch(input.Value, pattern.Value, ignoreCase.Value ? RegexOptions.IgnoreCase : RegexOptions.None));
        }

        [SqlFunction(DataAccess = DataAccessKind.None, IsDeterministic = true, IsPrecise = true, Name = "ufn_RegExReplace", SystemDataAccess = SystemDataAccessKind.None)]
        public static SqlString RegExReplace(SqlString input, SqlString pattern, SqlString replacement, SqlBoolean ignoreCase)
        {
            if (input.IsNull || pattern.IsNull)
            {
                return SqlString.Null;
            }
            return new SqlString(Regex.Replace(input.Value, pattern.Value, replacement.Value, ignoreCase.Value ? RegexOptions.IgnoreCase : RegexOptions.None));
        }

        [SqlFunction(DataAccess = DataAccessKind.None, IsDeterministic = true, IsPrecise = true, Name = "ufn_RegExMatches", SystemDataAccess = SystemDataAccessKind.None, FillRowMethodName = "GetRegExMatches", TableDefinition = "Match NVARCHAR(MAX), MatchIndex INT, MatchLength INT")]
        public static IEnumerable RegExMatches(SqlString input, SqlString pattern, SqlBoolean ignoreCase)
        {
            if (input.IsNull || pattern.IsNull)
            {
                return null;
            }
            return Regex.Matches(input.Value, pattern.Value, ignoreCase.Value ? RegexOptions.IgnoreCase : RegexOptions.None);
        }

        private static void GetRegExMatches(object input, out SqlString match, out SqlInt32 matchIndex, out SqlInt32 matchLength)
        {
            Match match2 = (Match)input;
            match = new SqlString(match2.Value);
            matchIndex = new SqlInt32(match2.Index);
            matchLength = new SqlInt32(match2.Length);
        }

        [SqlFunction(DataAccess = DataAccessKind.None, IsDeterministic = true, IsPrecise = true, Name = "ufn_RegExSplit", SystemDataAccess = SystemDataAccessKind.None, FillRowMethodName = "GetRegExSplits", TableDefinition = "SplitPart NVARCHAR(MAX)")]
        public static IEnumerable RegExSplit(SqlString input, SqlString pattern, SqlBoolean ignoreCase)
        {
            if (input.IsNull || pattern.IsNull)
            {
                return null;
            }
            return Regex.Split(input.Value, pattern.Value, ignoreCase.Value ? RegexOptions.IgnoreCase : RegexOptions.None);
        }

        private static void GetRegExSplits(object input, out SqlString match)
        {
            match = new SqlString((string)input);
        }

        private SqlRegEx()
        {
        }
    }
}
