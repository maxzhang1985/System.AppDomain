﻿namespace System
{
    internal static class ErrorStrings
    {
        private const string NULL = "null (Nothing in Visual Basic)";

        public static string CannotBeNull(string paramName) => $"'{paramName}' cannot be {NULL}";

        public static string CannotBeNullOrEmpty(string paramName) => $"'{paramName}' cannot be empty or {NULL}";

        public static string CannotBeNullOrWhitespace(string paramName) => $"'{paramName}' cannot be empty, only whitespace, or {NULL}";

        public static string MustBeReal(string paramName, Type checkType) => $"'{paramName}' must be a real {checkType.FullName}";
    }
}
