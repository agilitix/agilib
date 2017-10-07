﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace AxUtils
{
    public static class EnumUtils<T> where T : struct
    {
        private static readonly IDictionary<T, EnumInfos<T>> _valueToInfos;
        private static readonly IDictionary<string, EnumInfos<T>> _descriptionToInfos;

        static EnumUtils()
        {
            var type = typeof(T);
            if (!type.IsEnum)
            {
                throw new NotSupportedException();
            }

            IList<EnumInfos<T>> enumInfos = Initialize().ToList();

            _valueToInfos = enumInfos.ToDictionary(x => x.Value);
            _descriptionToInfos = enumInfos.Where(x => x.Description != null).ToDictionary(x => x.Description);
        }

        public static IEnumerable<T> GetValues()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        public static string GetName(T enumValue)
        {
            return Enum.GetName(typeof(T), enumValue);
        }

        public static IEnumerable<string> GetNames()
        {
            return Enum.GetNames(typeof(T));
        }

        public static string GetDescription(T enumValue)
        {
            EnumInfos<T> info;
            return _valueToInfos.TryGetValue(enumValue, out info)
                       ? info.Description
                       : null;
        }

        public static IEnumerable<string> GetDescriptions()
        {
            return _descriptionToInfos.Keys;
        }

        public static int GetNumber(T enumValue)
        {
            return ToInt(enumValue);
        }

        public static IEnumerable<int> GetNumbers()
        {
            return _valueToInfos.Values.Select(x => x.Number);
        }

        public static T FromDescription(string enumDescription)
        {
            return _descriptionToInfos[enumDescription].Value;
        }

        public static T FromDescriptionOrDefault(string enumDescription, T defaultValue)
        {
            T enumOutput;
            TryFromDescriptionOrDefault(enumDescription, out enumOutput, defaultValue);
            return enumOutput;
        }

        public static bool TryFromDescription(string enumDescription, out T enumOutput)
        {
            EnumInfos<T> info = null;
            if (enumDescription != null)
                _descriptionToInfos.TryGetValue(enumDescription, out info);
            enumOutput = info?.Value ?? default(T);
            return info != null;
        }

        public static bool TryFromDescriptionOrDefault(string enumDescription, out T enumOutput, T defaultValue)
        {
            if (TryFromDescription(enumDescription, out enumOutput))
                return true;

            enumOutput = defaultValue;
            return false;
        }

        public static T FromName(string enumName)
        {
            return (T)Enum.Parse(typeof(T), enumName);
        }

        public static T FromNameOrDefault(string enumName, T defaultValue)
        {
            T enumOutput;
            TryFromNameOrDefault(enumName, out enumOutput, defaultValue);
            return enumOutput;
        }

        public static bool TryFromName(string enumName, out T enumOutput)
        {
            return Enum.TryParse(enumName, false, out enumOutput);
        }

        public static bool TryFromNameOrDefault(string enumName, out T enumOutput, T defaultValue)
        {
            if (TryFromName(enumName, out enumOutput))
                return true;

            enumOutput = defaultValue;
            return false;
        }

        public static T Cast(int enumNumber)
        {
            return (T)Enum.ToObject(typeof(T), enumNumber);
        }

        public static T CastOrDefault(int enumNumber, T defaultValue)
        {
            T enumOutput;
            TryCastOrDefault(enumNumber, out enumOutput, defaultValue);
            return enumOutput;
        }

        public static bool TryCast(int enumNumber, out T enumOutput)
        {
            try
            {
                enumOutput = (T)Enum.ToObject(typeof(T), enumNumber);
                return true;
            }
            catch
            {
                // ignored
            }

            enumOutput = default(T);
            return false;
        }

        public static bool TryCastOrDefault(int enumNumber, out T enumOutput, T defaultValue)
        {
            if (TryCast(enumNumber, out enumOutput))
                return true;

            enumOutput = defaultValue;
            return false;
        }

        public static TTarget ChangeTo<TTarget>(T enumInput) where TTarget : struct
        {
            return (TTarget)Enum.Parse(typeof(TTarget), Enum.GetName(typeof(T), enumInput) ?? "");
        }

        public static TTarget ChangeToOrDefault<TTarget>(T enumInput, TTarget defaultValue) where TTarget : struct
        {
            TTarget enumOutput;
            TryChangeToOrDefault(enumInput, out enumOutput, defaultValue);
            return enumOutput;
        }

        public static bool TryChangeTo<TTarget>(T enumInput, out TTarget enumOutput) where TTarget : struct
        {
            return Enum.TryParse(Enum.GetName(typeof(T), enumInput) ?? "", false, out enumOutput);
        }

        public static bool TryChangeToOrDefault<TTarget>(T enumInput, out TTarget enumOutput, TTarget defaultValue) where TTarget : struct
        {
            if (TryChangeTo(enumInput, out enumOutput))
                return true;

            enumOutput = defaultValue;
            return false;
        }

        public static bool IsNumberDefined(int enumNumber)
        {
            try
            {
                return Enum.IsDefined(typeof(T), enumNumber);
            }
            catch
            {
                // ignored
            }

            return false;
        }

        public static bool IsNameDefined(string enumName)
        {
            try
            {
                return Enum.IsDefined(typeof(T), enumName);
            }
            catch
            {
                // ignored
            }

            return false;
        }

        public static bool IsDescriptionDefined(string enumDescription)
        {
            return enumDescription != null && _descriptionToInfos.ContainsKey(enumDescription);
        }

        public static IEnumerable<EnumInfos<T>> GetEnumInfos()
        {
            return _valueToInfos.Values;
        }

        public static int ToInt(T enumValue)
        {
            return (int)(object)enumValue;
        }

        private static IEnumerable<EnumInfos<T>> Initialize()
        {
            Type type = typeof(T);
            return Enum.GetValues(type)
                       .Cast<T>()
                       .Select(enumValue =>
                               {
                                   string enumName = Enum.GetName(type, enumValue);
                                   return new EnumInfos<T>
                                          {
                                              Value = enumValue,
                                              Number = ToInt(enumValue),
                                              Description = type.GetField(enumName)
                                                                .GetCustomAttributes(typeof(DescriptionAttribute), false)
                                                                .Cast<DescriptionAttribute>()
                                                                .Select(attribute => attribute.Description)
                                                                .FirstOrDefault(),
                                              Name = enumName
                                          };
                               });
        }
    }
}
