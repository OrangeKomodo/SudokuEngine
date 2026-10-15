using System;
using System.Text;
using DataTypes;
using UnityEngine;

namespace Definitions
{
    public static class Util
    {
        public static Color StarterCharColor = new Color(0f, 0f, 0f, 1f);
        public static Color SolvedCharColor = new Color(0.4f, 0.4f, 0.4f, 1f);
        public static Color UncertainCharColor = new Color(0.8f, 0.8f, 0.8f, 1f);

        private enum BoolOperation
        {
            And,
            Or,
            Xor,
            Not
        }

        public static bool[] BoolArrayAnd(bool[] arrayA, bool[] arrayB) => PerformBoolArrayOperation(BoolOperation.And, arrayA, arrayB);
        public static bool[] BoolArrayOr(bool[] arrayA, bool[] arrayB) => PerformBoolArrayOperation(BoolOperation.Or, arrayA, arrayB);
        public static bool[] BoolArrayXor(bool[] arrayA, bool[] arrayB) => PerformBoolArrayOperation(BoolOperation.And, arrayA, arrayB);
        public static bool[] BoolArrayNot(bool[] arrayA) => PerformBoolArrayOperation(BoolOperation.Not, arrayA);

        private static bool[] PerformBoolArrayOperation(BoolOperation operation, bool[] arrayA, bool[] arrayB = null)
        {
            int outputLength = arrayB == null ? arrayA.Length : Mathf.Min(arrayA.Length, arrayB.Length);
            bool[] output = new bool[outputLength];

            for (int arrayIndex = 0; arrayIndex < outputLength; ++arrayIndex)
            {
                output[arrayIndex] = operation switch
                {
                    BoolOperation.And => arrayA[arrayIndex] & arrayB[arrayIndex],
                    BoolOperation.Or => arrayA[arrayIndex] | arrayB[arrayIndex],
                    BoolOperation.Xor => arrayA[arrayIndex] ^ arrayB[arrayIndex],
                    BoolOperation.Not => !arrayA[arrayIndex],
                    _ => throw new ArgumentOutOfRangeException(nameof(operation), operation, null)
                };
            }

            return output;
        }

        public static int BoolArrayCount(bool[] array)
        {
            int count = 0;

            for (int arrayIndex = 0; arrayIndex < array.Length; ++arrayIndex)
            {
                if (array[arrayIndex])
                {
                    ++count;
                }
            }

            return count;
        }

        public static string BoolArrayToString(bool[] array, string separator = ", ")
        {
            StringBuilder stringBuilder = new StringBuilder();

            for (int arrayIndex = 0; arrayIndex < array.Length; ++arrayIndex)
            {
                if (array[arrayIndex])
                {
                    stringBuilder.Append(arrayIndex + 1);
                    stringBuilder.Append(separator);
                }
            }
            
            stringBuilder.Remove(stringBuilder.Length - separator.Length, separator.Length);
            return stringBuilder.ToString();
        }
    }
}
