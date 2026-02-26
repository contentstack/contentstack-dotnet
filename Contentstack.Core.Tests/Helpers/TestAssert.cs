using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Contentstack.Core.Tests.Helpers
{
    /// <summary>
    /// Drop-in replacement for xUnit Assert that automatically logs Expected vs Actual values.
    /// Uses CallerArgumentExpression (C# 10+) to capture the expression being asserted,
    /// and AsyncLocal to route logs to the current test's TestOutputHelper.
    /// 
    /// Usage: Replace 'Assert.' with 'TestAssert.' in test files.
    /// The IntegrationTestBase constructor calls TestAssert.SetHelper(TestOutput) automatically.
    /// </summary>
    public static class TestAssert
    {
        private static readonly AsyncLocal<TestOutputHelper> _helper = new();

        /// <summary>
        /// Set the TestOutputHelper for the current async context (called by IntegrationTestBase ctor)
        /// </summary>
        public static void SetHelper(TestOutputHelper helper)
        {
            _helper.Value = helper;
        }

        private static void Log(string name, object expected, object actual, bool passed)
        {
            _helper.Value?.LogAssertion(name ?? "assertion", expected, actual, passed);
        }

        #region NotNull / Null

        public static void NotNull(
            object obj,
            [CallerArgumentExpression(nameof(obj))] string expr = null)
        {
            Log(expr, "NotNull", obj != null ? Truncate(obj) : "null", obj != null);
            Assert.NotNull(obj);
        }

        public static void Null(
            object obj,
            [CallerArgumentExpression(nameof(obj))] string expr = null)
        {
            Log(expr, "null", obj == null ? "null" : Truncate(obj), obj == null);
            Assert.Null(obj);
        }

        #endregion

        #region Equal / NotEqual

        public static void Equal<T>(
            T expected, T actual,
            [CallerArgumentExpression(nameof(actual))] string expr = null)
        {
            bool passed = EqualityComparer<T>.Default.Equals(expected, actual);
            Log(expr, Truncate(expected), Truncate(actual), passed);
            Assert.Equal(expected, actual);
        }

        public static void NotEqual<T>(
            T expected, T actual,
            [CallerArgumentExpression(nameof(actual))] string expr = null)
        {
            bool passed = !EqualityComparer<T>.Default.Equals(expected, actual);
            Log(expr, $"Not: {Truncate(expected)}", Truncate(actual), passed);
            Assert.NotEqual(expected, actual);
        }

        #endregion

        #region NotEmpty / Empty

        public static void NotEmpty(
            IEnumerable collection,
            [CallerArgumentExpression(nameof(collection))] string expr = null)
        {
            string display;
            bool hasItems;

            if (collection is string s)
            {
                hasItems = !string.IsNullOrEmpty(s);
                display = hasItems ? $"\"{Truncate(s, 80)}\"" : "(empty string)";
            }
            else if (collection != null)
            {
                var enumerator = collection.GetEnumerator();
                hasItems = enumerator.MoveNext();
                if (enumerator is IDisposable d) d.Dispose();
                display = hasItems ? "(has items)" : "(empty collection)";
            }
            else
            {
                hasItems = false;
                display = "null";
            }

            Log(expr, "NotEmpty", display, hasItems);
            Assert.NotEmpty(collection);
        }

        public static void Empty(
            IEnumerable collection,
            [CallerArgumentExpression(nameof(collection))] string expr = null)
        {
            string display;
            bool isEmpty;

            if (collection is string s)
            {
                isEmpty = string.IsNullOrEmpty(s);
                display = isEmpty ? "(empty string)" : $"\"{Truncate(s, 80)}\"";
            }
            else if (collection != null)
            {
                var enumerator = collection.GetEnumerator();
                isEmpty = !enumerator.MoveNext();
                if (enumerator is IDisposable d) d.Dispose();
                display = isEmpty ? "(empty collection)" : "(has items)";
            }
            else
            {
                isEmpty = true;
                display = "null";
            }

            Log(expr, "Empty", display, isEmpty);
            Assert.Empty(collection);
        }

        #endregion

        #region True / False

        public static void True(
            bool condition,
            string userMessage = null,
            [CallerArgumentExpression(nameof(condition))] string expr = null)
        {
            Log(userMessage ?? expr, true, condition, condition);
            if (userMessage != null)
                Assert.True(condition, userMessage);
            else
                Assert.True(condition);
        }

        public static void False(
            bool condition,
            string userMessage = null,
            [CallerArgumentExpression(nameof(condition))] string expr = null)
        {
            Log(userMessage ?? expr, false, condition, !condition);
            if (userMessage != null)
                Assert.False(condition, userMessage);
            else
                Assert.False(condition);
        }

        #endregion

        #region Contains / DoesNotContain

        public static void Contains(
            string expectedSubstring, string actualString,
            [CallerArgumentExpression(nameof(actualString))] string expr = null)
        {
            bool passed = actualString != null && actualString.Contains(expectedSubstring);
            Log($"Contains in {expr}", $"\"{Truncate(expectedSubstring, 50)}\"", 
                passed ? $"Found in \"{Truncate(actualString, 80)}\"" : $"Not found in \"{Truncate(actualString, 80)}\"", passed);
            Assert.Contains(expectedSubstring, actualString);
        }

        public static void Contains<T>(
            T expected, IEnumerable<T> collection,
            [CallerArgumentExpression(nameof(collection))] string expr = null)
        {
            bool passed = collection != null && collection.Contains(expected);
            Log($"Contains in {expr}", Truncate(expected), passed ? "Found" : "Not found", passed);
            Assert.Contains(expected, collection);
        }

        public static void DoesNotContain(
            string expectedSubstring, string actualString,
            [CallerArgumentExpression(nameof(actualString))] string expr = null)
        {
            bool passed = actualString == null || !actualString.Contains(expectedSubstring);
            Log($"DoesNotContain in {expr}", $"Should not contain \"{Truncate(expectedSubstring, 50)}\"",
                passed ? "Not found" : "Found", passed);
            Assert.DoesNotContain(expectedSubstring, actualString);
        }

        public static void DoesNotContain<T>(
            T expected, IEnumerable<T> collection,
            [CallerArgumentExpression(nameof(collection))] string expr = null)
        {
            bool passed = collection == null || !collection.Contains(expected);
            Log($"DoesNotContain in {expr}", $"Should not contain {Truncate(expected)}", 
                passed ? "Not found" : "Found", passed);
            Assert.DoesNotContain(expected, collection);
        }

        public static void Contains<T>(
            IEnumerable<T> collection, Predicate<T> filter,
            [CallerArgumentExpression(nameof(filter))] string expr = null)
        {
            Assert.Contains(collection, filter);
            Log($"Contains (predicate): {expr}", "Match found", "Match found", true);
        }

        public static void DoesNotContain<T>(
            IEnumerable<T> collection, Predicate<T> filter,
            [CallerArgumentExpression(nameof(filter))] string expr = null)
        {
            Assert.DoesNotContain(collection, filter);
            Log($"DoesNotContain (predicate): {expr}", "No match", "No match", true);
        }

        #endregion

        #region Matches / DoesNotMatch / StartsWith

        public static void Matches(
            string regexPattern, string actualString,
            [CallerArgumentExpression(nameof(actualString))] string expr = null)
        {
            bool passed = actualString != null && Regex.IsMatch(actualString, regexPattern);
            Log($"Matches: {expr}", $"Pattern: {regexPattern}", Truncate(actualString, 100), passed);
            Assert.Matches(regexPattern, actualString);
        }

        public static void DoesNotMatch(
            string regexPattern, string actualString,
            [CallerArgumentExpression(nameof(actualString))] string expr = null)
        {
            bool passed = actualString == null || !Regex.IsMatch(actualString, regexPattern);
            Log($"DoesNotMatch: {expr}", $"Not: {regexPattern}", Truncate(actualString, 100), passed);
            Assert.DoesNotMatch(regexPattern, actualString);
        }

        public static void StartsWith(
            string expectedStartString, string actualString,
            [CallerArgumentExpression(nameof(actualString))] string expr = null)
        {
            bool passed = actualString != null && actualString.StartsWith(expectedStartString);
            Log($"StartsWith: {expr}", $"\"{Truncate(expectedStartString, 50)}\"", 
                $"\"{Truncate(actualString, 80)}\"", passed);
            Assert.StartsWith(expectedStartString, actualString);
        }

        #endregion

        #region Type Assertions

        public static T IsType<T>(
            object obj,
            [CallerArgumentExpression(nameof(obj))] string expr = null)
        {
            bool passed = obj != null && obj.GetType() == typeof(T);
            Log($"IsType: {expr}", typeof(T).Name, obj?.GetType()?.Name ?? "null", passed);
            return Assert.IsType<T>(obj);
        }

        public static T IsAssignableFrom<T>(
            object obj,
            [CallerArgumentExpression(nameof(obj))] string expr = null)
        {
            bool passed = obj is T;
            Log($"IsAssignableFrom: {expr}", typeof(T).Name, obj?.GetType()?.Name ?? "null", passed);
            return Assert.IsAssignableFrom<T>(obj);
        }

        #endregion

        #region Collection Assertions

        public static void All<T>(IEnumerable<T> collection, Action<T> action)
        {
            Assert.All(collection, action);
        }

        public static T Single<T>(
            IEnumerable<T> collection,
            [CallerArgumentExpression(nameof(collection))] string expr = null)
        {
            var list = collection?.ToList();
            int count = list?.Count ?? 0;
            Log($"Single: {expr}", "1 item", $"{count} item(s)", count == 1);
            return Assert.Single(list);
        }

        public static void InRange<T>(
            T actual, T low, T high,
            [CallerArgumentExpression(nameof(actual))] string expr = null) where T : IComparable<T>, IComparable
        {
            bool passed = actual.CompareTo(low) >= 0 && actual.CompareTo(high) <= 0;
            Log($"InRange: {expr}", $"[{low} .. {high}]", Truncate(actual), passed);
            Assert.InRange(actual, low, high);
        }

        #endregion

        #region Exception Assertions (pass-through, logging is less useful for lambdas)

        public static T Throws<T>(Action action) where T : Exception
            => Assert.Throws<T>(action);

        public static T Throws<T>(Func<object> action) where T : Exception
            => Assert.Throws<T>(action);

        public static async Task<T> ThrowsAsync<T>(Func<Task> action) where T : Exception
            => await Assert.ThrowsAsync<T>(action);

        public static Exception ThrowsAny<T>(Action action) where T : Exception
            => Assert.ThrowsAny<T>(action);

        public static async Task<T> ThrowsAnyAsync<T>(Func<Task> action) where T : Exception
            => await Assert.ThrowsAnyAsync<T>(action);

        #endregion

        #region Fail

        public static void Fail(string message)
        {
            Log("Fail", "No failure", message, false);
            Assert.Fail(message);
        }

        #endregion

        #region Helpers

        private static string Truncate(object value, int maxLength = 200)
        {
            if (value == null) return "null";
            var str = value.ToString();
            if (string.IsNullOrEmpty(str)) return "(empty)";
            return str.Length > maxLength ? str.Substring(0, maxLength) + "..." : str;
        }

        #endregion
    }
}
