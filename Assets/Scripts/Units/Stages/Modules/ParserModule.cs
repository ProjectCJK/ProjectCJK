using System;
using UnityEngine;

namespace Units.Stages.Modules
{
    public static class ParserModule
    {
        public static TEnum1? ParseStringToEnum<TEnum1>(string input)
            where TEnum1 : struct, Enum
        {
            if (string.IsNullOrEmpty(input)) return null;

            TEnum1? parsedA = null;

            // 첫 번째 부분을 TEnum1으로 파싱 시도
            if (Enum.TryParse(input, out TEnum1 resultA))
                parsedA = resultA;
            
            return parsedA;
        }
        
        public static (TEnum1?, TEnum2?) ParseStringToEnum<TEnum1, TEnum2>(string input)
            where TEnum1 : struct, Enum
            where TEnum2 : struct, Enum
        {
            if (string.IsNullOrEmpty(input)) return (null, null);

            var parts = input.Split('_');

            TEnum1? parsedA = null;
            TEnum2? parsedB = null;

            // 첫 번째 부분을 TEnum1으로 파싱 시도
            if (Enum.TryParse(parts[0], out TEnum1 resultA))
                parsedA = resultA;
            else
                // 첫 번째 인자가 TEnum1으로 파싱되지 않으면 실패
                return (null, null);

            // 두 번째 부분이 존재하고, 이를 TEnum2로 파싱 시도
            if (parts.Length > 1 && Enum.TryParse(parts[1], out TEnum2 resultB)) parsedB = resultB;

            return (parsedA, parsedB);
        }
        
        public static (TEnum1?, TEnum2?, TEnum3?) ParseStringToEnum<TEnum1, TEnum2, TEnum3>(string input)
            where TEnum1 : struct, Enum
            where TEnum2 : struct, Enum
            where TEnum3 : struct, Enum
        {
            if (string.IsNullOrEmpty(input)) return (null, null, null);

            var parts = input.Split('_');

            TEnum1? parsedA = null;
            TEnum2? parsedB = null;
            TEnum3? parsedC = null;

            // 첫 번째 부분을 TEnum1으로 파싱 시도
            if (Enum.TryParse(parts[0], out TEnum1 resultA))
                parsedA = resultA;
            else
                // 첫 번째 인자가 TEnum1으로 파싱되지 않으면 실패
                return (null, null, null);

            // 두 번째 부분이 존재하고, 이를 TEnum2로 파싱 시도
            if (parts.Length > 1 && Enum.TryParse(parts[1], out TEnum2 resultB))
                parsedB = resultB;

            // 세 번째 부분이 존재하고, 이를 TEnum3로 파싱 시도
            if (parts.Length > 2 && Enum.TryParse(parts[2], out TEnum3 resultC))
                parsedC = resultC;

            return (parsedA, parsedB, parsedC);
        }
        
        public static string ParseEnumToString<TEnum1>(TEnum1 enum1)
            where TEnum1 : Enum
        {
            return $"{enum1}";
        }

        public static string ParseEnumToString<TEnum1, TEnum2>(TEnum1 enum1, TEnum2 enum2)
            where TEnum1 : Enum where TEnum2 : Enum
        {
            return $"{enum1}_{enum2}";
        }
        
        public static string ParseEnumToString<TEnum1, TEnum2, TEnum3>(TEnum1 enum1, TEnum2 enum2, TEnum3 enum3)
            where TEnum1 : Enum where TEnum2 : Enum where TEnum3 : Enum
        {
            return $"{enum1}_{enum2}_{enum3}";
        }

        public static T ParseOrDefault<T>(string input, T defaultValue)
        {
            try
            {
                if (typeof(T) == typeof(int) && int.TryParse(input, out var intValue)) return (T)(object)intValue;
                if (typeof(T) == typeof(float) && float.TryParse(input, out var floatValue))
                    return (T)(object)floatValue;
                if (typeof(T) == typeof(bool) && bool.TryParse(input, out var boolValue)) return (T)(object)boolValue;
            }
            catch
            {
                Debug.LogError($"Failed to parse {typeof(T).Name}");
            }

            return defaultValue;
        }
    }
}