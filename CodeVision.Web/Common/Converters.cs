using System;
using System.Collections.Generic;

namespace CodeVision.Web.Common
{
    public class Converters
    {
        /// <summary>
        /// Converts comma separated list of values to an array of enums.
        /// Only valid enums will be present in the array, invalid enum values are igrnored.
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <param name="commaSeparatedList">comma separated values of enums</param>
        /// <returns>Enum array</returns>
        public static T[] FromCommaSeparatedListToEnumArray<T>(string commaSeparatedList) where T : struct
        {
            List<T> enumList = new List<T>();
            if (!string.IsNullOrEmpty(commaSeparatedList))
            {
                string[] parts = commaSeparatedList.Split(',');
                foreach (var valueString in parts)
                {
                    T item;
                    if (Enum.TryParse(valueString, out item) && Enum.IsDefined(typeof(T), item))
                    {
                        enumList.Add(item);
                    }
                }
            }
            return enumList.ToArray();
        }
    }
}