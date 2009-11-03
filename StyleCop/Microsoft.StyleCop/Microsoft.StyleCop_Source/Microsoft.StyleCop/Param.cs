namespace Microsoft.StyleCop
{
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;

    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId="Param", Justification="This name represents a Parameter, and should be short as it is used often.")]
    public sealed class Param
    {
        private Param()
        {
        }

        [Conditional("DEBUG")]
        public static void Assert(bool test, string parameterName, string exceptionMessage)
        {
            Require(test, parameterName, exceptionMessage);
        }

        [Conditional("DEBUG")]
        public static void AssertGreaterThan(double number, double minimum, string parameterName)
        {
            RequireGreaterThan(number, minimum, parameterName);
        }

        [Conditional("DEBUG")]
        public static void AssertGreaterThan(short number, short minimum, string parameterName)
        {
            RequireGreaterThan(number, minimum, parameterName);
        }

        [Conditional("DEBUG")]
        public static void AssertGreaterThan(int number, int minimum, string parameterName)
        {
            RequireGreaterThan(number, minimum, parameterName);
        }

        [Conditional("DEBUG")]
        public static void AssertGreaterThan(long number, long minimum, string parameterName)
        {
            RequireGreaterThan(number, minimum, parameterName);
        }

        [Conditional("DEBUG")]
        public static void AssertGreaterThan(float number, float minimum, string parameterName)
        {
            RequireGreaterThan(number, minimum, parameterName);
        }

        [Conditional("DEBUG")]
        public static void AssertGreaterThanOrEqualTo(double number, double minimum, string parameterName)
        {
            RequireGreaterThanOrEqualTo(number, minimum, parameterName);
        }

        [Conditional("DEBUG")]
        public static void AssertGreaterThanOrEqualTo(short number, short minimum, string parameterName)
        {
            RequireGreaterThanOrEqualTo(number, minimum, parameterName);
        }

        [Conditional("DEBUG")]
        public static void AssertGreaterThanOrEqualTo(int number, int minimum, string parameterName)
        {
            RequireGreaterThanOrEqualTo(number, minimum, parameterName);
        }

        [Conditional("DEBUG")]
        public static void AssertGreaterThanOrEqualTo(long number, long minimum, string parameterName)
        {
            RequireGreaterThanOrEqualTo(number, minimum, parameterName);
        }

        [Conditional("DEBUG")]
        public static void AssertGreaterThanOrEqualTo(float number, float minimum, string parameterName)
        {
            RequireGreaterThanOrEqualTo(number, minimum, parameterName);
        }

        [Conditional("DEBUG")]
        public static void AssertGreaterThanOrEqualToZero(double number, string parameterName)
        {
            RequireGreaterThanOrEqualToZero(number, parameterName);
        }

        [Conditional("DEBUG")]
        public static void AssertGreaterThanOrEqualToZero(short number, string parameterName)
        {
            RequireGreaterThanOrEqualToZero(number, parameterName);
        }

        [Conditional("DEBUG")]
        public static void AssertGreaterThanOrEqualToZero(int number, string parameterName)
        {
            RequireGreaterThanOrEqualToZero(number, parameterName);
        }

        [Conditional("DEBUG")]
        public static void AssertGreaterThanOrEqualToZero(long number, string parameterName)
        {
            RequireGreaterThanOrEqualToZero(number, parameterName);
        }

        [Conditional("DEBUG")]
        public static void AssertGreaterThanOrEqualToZero(float number, string parameterName)
        {
            RequireGreaterThanOrEqualToZero(number, parameterName);
        }

        [Conditional("DEBUG")]
        public static void AssertGreaterThanZero(double number, string parameterName)
        {
            RequireGreaterThanZero(number, parameterName);
        }

        [Conditional("DEBUG")]
        public static void AssertGreaterThanZero(short number, string parameterName)
        {
            RequireGreaterThanZero(number, parameterName);
        }

        [Conditional("DEBUG")]
        public static void AssertGreaterThanZero(int number, string parameterName)
        {
            RequireGreaterThanZero(number, parameterName);
        }

        [Conditional("DEBUG")]
        public static void AssertGreaterThanZero(long number, string parameterName)
        {
            RequireGreaterThanZero(number, parameterName);
        }

        [Conditional("DEBUG")]
        public static void AssertGreaterThanZero(float number, string parameterName)
        {
            RequireGreaterThanZero(number, parameterName);
        }

        [Conditional("DEBUG")]
        public static void AssertLessThan(double number, double maximum, string parameterName)
        {
            RequireLessThan(number, maximum, parameterName);
        }

        [Conditional("DEBUG")]
        public static void AssertLessThan(short number, short maximum, string parameterName)
        {
            RequireLessThan(number, maximum, parameterName);
        }

        [Conditional("DEBUG")]
        public static void AssertLessThan(int number, int maximum, string parameterName)
        {
            RequireLessThan(number, maximum, parameterName);
        }

        [Conditional("DEBUG")]
        public static void AssertLessThan(long number, long maximum, string parameterName)
        {
            RequireLessThan(number, maximum, parameterName);
        }

        [Conditional("DEBUG")]
        public static void AssertLessThan(float number, float maximum, string parameterName)
        {
            RequireLessThan(number, maximum, parameterName);
        }

        [Conditional("DEBUG")]
        public static void AssertLessThanOrEqualTo(double number, double maximum, string parameterName)
        {
            RequireLessThanOrEqualTo(number, maximum, parameterName);
        }

        [Conditional("DEBUG")]
        public static void AssertLessThanOrEqualTo(short number, short maximum, string parameterName)
        {
            RequireLessThanOrEqualTo(number, maximum, parameterName);
        }

        [Conditional("DEBUG")]
        public static void AssertLessThanOrEqualTo(int number, int maximum, string parameterName)
        {
            RequireLessThanOrEqualTo(number, maximum, parameterName);
        }

        [Conditional("DEBUG")]
        public static void AssertLessThanOrEqualTo(long number, long maximum, string parameterName)
        {
            RequireLessThanOrEqualTo(number, maximum, parameterName);
        }

        [Conditional("DEBUG")]
        public static void AssertLessThanOrEqualTo(float number, float maximum, string parameterName)
        {
            RequireLessThanOrEqualTo(number, maximum, parameterName);
        }

        [Conditional("DEBUG")]
        public static void AssertNotNull(object parameter, string parameterName)
        {
            RequireNotNull(parameter, parameterName);
        }

        [Conditional("DEBUG")]
        public static void AssertNotNull(object parameter, string parameterName, string exceptionMessage)
        {
            RequireNotNull(parameter, parameterName, exceptionMessage);
        }

        [Conditional("DEBUG")]
        public static void AssertValidCollection(ICollection parameter, string parameterName)
        {
            RequireValidCollection(parameter, parameterName);
        }

        [Conditional("DEBUG")]
        public static void AssertValidIndex(bool test, string parameterName, string exceptionMessage)
        {
            if (!test)
            {
                throw new ArgumentOutOfRangeException(exceptionMessage, parameterName);
            }
        }

        [Conditional("DEBUG")]
        public static void AssertValidString(string parameter, string parameterName)
        {
            RequireValidString(parameter, parameterName);
        }

        [Conditional("DEBUG")]
        public static void AssertValueBetween(double number, double low, double high, string parameterName)
        {
            RequireValueBetween(number, low, high, parameterName);
        }

        [Conditional("DEBUG")]
        public static void AssertValueBetween(short number, short low, short high, string parameterName)
        {
            RequireValueBetween(number, low, high, parameterName);
        }

        [Conditional("DEBUG")]
        public static void AssertValueBetween(int number, int low, int high, string parameterName)
        {
            RequireValueBetween(number, low, high, parameterName);
        }

        [Conditional("DEBUG")]
        public static void AssertValueBetween(long number, long low, long high, string parameterName)
        {
            RequireValueBetween(number, low, high, parameterName);
        }

        [Conditional("DEBUG")]
        public static void AssertValueBetween(float number, float low, float high, string parameterName)
        {
            RequireValueBetween(number, low, high, parameterName);
        }

        [Conditional("DEBUG")]
        public static void Ignore(params object[] values)
        {
        }

        public static void Require(bool test, string parameterName, ParamErrorTextHandler errorTextHandler)
        {
            if (!test)
            {
                string message = string.Empty;
                if (errorTextHandler != null)
                {
                    message = errorTextHandler();
                }
                throw new ArgumentException(message, parameterName);
            }
        }

        public static void Require(bool test, string parameterName, string exceptionMessage)
        {
            if (!test)
            {
                throw new ArgumentException(exceptionMessage, parameterName);
            }
        }

        public static void RequireGreaterThan(double number, double minimum, string parameterName)
        {
            if (number <= minimum)
            {
                string mustBeGreaterThan = Strings.MustBeGreaterThan;
                throw new ArgumentOutOfRangeException(string.Format(CultureInfo.CurrentCulture, mustBeGreaterThan, new object[] { minimum }), parameterName);
            }
        }

        public static void RequireGreaterThan(short number, short minimum, string parameterName)
        {
            if (number <= minimum)
            {
                string mustBeGreaterThan = Strings.MustBeGreaterThan;
                throw new ArgumentOutOfRangeException(string.Format(CultureInfo.CurrentCulture, mustBeGreaterThan, new object[] { minimum }), parameterName);
            }
        }

        public static void RequireGreaterThan(int number, int minimum, string parameterName)
        {
            if (number <= minimum)
            {
                string mustBeGreaterThan = Strings.MustBeGreaterThan;
                throw new ArgumentOutOfRangeException(string.Format(CultureInfo.CurrentCulture, mustBeGreaterThan, new object[] { minimum }), parameterName);
            }
        }

        public static void RequireGreaterThan(long number, long minimum, string parameterName)
        {
            if (number <= minimum)
            {
                string mustBeGreaterThan = Strings.MustBeGreaterThan;
                throw new ArgumentOutOfRangeException(string.Format(CultureInfo.CurrentCulture, mustBeGreaterThan, new object[] { minimum }), parameterName);
            }
        }

        public static void RequireGreaterThan(float number, float minimum, string parameterName)
        {
            if (number <= minimum)
            {
                string mustBeGreaterThan = Strings.MustBeGreaterThan;
                throw new ArgumentOutOfRangeException(string.Format(CultureInfo.CurrentCulture, mustBeGreaterThan, new object[] { minimum }), parameterName);
            }
        }

        public static void RequireGreaterThanOrEqualTo(double number, double minimum, string parameterName)
        {
            if (number < minimum)
            {
                string mustBeGreaterThanOrEqualTo = Strings.MustBeGreaterThanOrEqualTo;
                throw new ArgumentOutOfRangeException(string.Format(CultureInfo.CurrentCulture, mustBeGreaterThanOrEqualTo, new object[] { minimum }), parameterName);
            }
        }

        public static void RequireGreaterThanOrEqualTo(short number, short minimum, string parameterName)
        {
            if (number < minimum)
            {
                string mustBeGreaterThanOrEqualTo = Strings.MustBeGreaterThanOrEqualTo;
                throw new ArgumentOutOfRangeException(string.Format(CultureInfo.CurrentCulture, mustBeGreaterThanOrEqualTo, new object[] { minimum }), parameterName);
            }
        }

        public static void RequireGreaterThanOrEqualTo(int number, int minimum, string parameterName)
        {
            if (number < minimum)
            {
                string mustBeGreaterThanOrEqualTo = Strings.MustBeGreaterThanOrEqualTo;
                throw new ArgumentOutOfRangeException(string.Format(CultureInfo.CurrentCulture, mustBeGreaterThanOrEqualTo, new object[] { minimum }), parameterName);
            }
        }

        public static void RequireGreaterThanOrEqualTo(long number, long minimum, string parameterName)
        {
            if (number < minimum)
            {
                string mustBeGreaterThanOrEqualTo = Strings.MustBeGreaterThanOrEqualTo;
                throw new ArgumentOutOfRangeException(string.Format(CultureInfo.CurrentCulture, mustBeGreaterThanOrEqualTo, new object[] { minimum }), parameterName);
            }
        }

        public static void RequireGreaterThanOrEqualTo(float number, float minimum, string parameterName)
        {
            if (number < minimum)
            {
                string mustBeGreaterThanOrEqualTo = Strings.MustBeGreaterThanOrEqualTo;
                throw new ArgumentOutOfRangeException(string.Format(CultureInfo.CurrentCulture, mustBeGreaterThanOrEqualTo, new object[] { minimum }), parameterName);
            }
        }

        public static void RequireGreaterThanOrEqualToZero(double number, string parameterName)
        {
            RequireValidIndex(number >= 0.0, parameterName, delegate {
                return Strings.MustBeGreaterThanOrEqualToZero;
            });
        }

        public static void RequireGreaterThanOrEqualToZero(short number, string parameterName)
        {
            RequireValidIndex(number >= 0, parameterName, delegate {
                return Strings.MustBeGreaterThanOrEqualToZero;
            });
        }

        public static void RequireGreaterThanOrEqualToZero(int number, string parameterName)
        {
            RequireValidIndex(number >= 0, parameterName, delegate {
                return Strings.MustBeGreaterThanOrEqualToZero;
            });
        }

        public static void RequireGreaterThanOrEqualToZero(long number, string parameterName)
        {
            RequireValidIndex(number >= 0L, parameterName, delegate {
                return Strings.MustBeGreaterThanOrEqualToZero;
            });
        }

        public static void RequireGreaterThanOrEqualToZero(float number, string parameterName)
        {
            RequireValidIndex(number >= 0f, parameterName, delegate {
                return Strings.MustBeGreaterThanOrEqualToZero;
            });
        }

        public static void RequireGreaterThanZero(double number, string parameterName)
        {
            RequireValidIndex(number > 0.0, parameterName, delegate {
                return Strings.MustBeGreaterThanZero;
            });
        }

        public static void RequireGreaterThanZero(short number, string parameterName)
        {
            RequireValidIndex(number > 0, parameterName, delegate {
                return Strings.MustBeGreaterThanZero;
            });
        }

        public static void RequireGreaterThanZero(int number, string parameterName)
        {
            RequireValidIndex(number > 0, parameterName, delegate {
                return Strings.MustBeGreaterThanZero;
            });
        }

        public static void RequireGreaterThanZero(long number, string parameterName)
        {
            RequireValidIndex(number > 0L, parameterName, delegate {
                return Strings.MustBeGreaterThanZero;
            });
        }

        public static void RequireGreaterThanZero(float number, string parameterName)
        {
            RequireValidIndex(number > 0f, parameterName, delegate {
                return Strings.MustBeGreaterThanZero;
            });
        }

        public static void RequireLessThan(double number, double maximum, string parameterName)
        {
            if (number >= maximum)
            {
                string mustBeLessThan = Strings.MustBeLessThan;
                throw new ArgumentOutOfRangeException(string.Format(CultureInfo.CurrentCulture, mustBeLessThan, new object[] { maximum }), parameterName);
            }
        }

        public static void RequireLessThan(short number, short maximum, string parameterName)
        {
            if (number >= maximum)
            {
                string mustBeLessThan = Strings.MustBeLessThan;
                throw new ArgumentOutOfRangeException(string.Format(CultureInfo.CurrentCulture, mustBeLessThan, new object[] { maximum }), parameterName);
            }
        }

        public static void RequireLessThan(int number, int maximum, string parameterName)
        {
            if (number >= maximum)
            {
                string mustBeLessThan = Strings.MustBeLessThan;
                throw new ArgumentOutOfRangeException(string.Format(CultureInfo.CurrentCulture, mustBeLessThan, new object[] { maximum }), parameterName);
            }
        }

        public static void RequireLessThan(long number, long maximum, string parameterName)
        {
            if (number >= maximum)
            {
                string mustBeLessThan = Strings.MustBeLessThan;
                throw new ArgumentOutOfRangeException(string.Format(CultureInfo.CurrentCulture, mustBeLessThan, new object[] { maximum }), parameterName);
            }
        }

        public static void RequireLessThan(float number, float maximum, string parameterName)
        {
            if (number >= maximum)
            {
                string mustBeLessThan = Strings.MustBeLessThan;
                throw new ArgumentOutOfRangeException(string.Format(CultureInfo.CurrentCulture, mustBeLessThan, new object[] { maximum }), parameterName);
            }
        }

        public static void RequireLessThanOrEqualTo(double number, double maximum, string parameterName)
        {
            if (number > maximum)
            {
                string mustBeLessThanOrEqualTo = Strings.MustBeLessThanOrEqualTo;
                throw new ArgumentOutOfRangeException(string.Format(CultureInfo.CurrentCulture, mustBeLessThanOrEqualTo, new object[] { maximum }), parameterName);
            }
        }

        public static void RequireLessThanOrEqualTo(short number, short maximum, string parameterName)
        {
            if (number > maximum)
            {
                string mustBeLessThanOrEqualTo = Strings.MustBeLessThanOrEqualTo;
                throw new ArgumentOutOfRangeException(string.Format(CultureInfo.CurrentCulture, mustBeLessThanOrEqualTo, new object[] { maximum }), parameterName);
            }
        }

        public static void RequireLessThanOrEqualTo(int number, int maximum, string parameterName)
        {
            if (number > maximum)
            {
                string mustBeLessThanOrEqualTo = Strings.MustBeLessThanOrEqualTo;
                throw new ArgumentOutOfRangeException(string.Format(CultureInfo.CurrentCulture, mustBeLessThanOrEqualTo, new object[] { maximum }), parameterName);
            }
        }

        public static void RequireLessThanOrEqualTo(long number, long maximum, string parameterName)
        {
            if (number > maximum)
            {
                string mustBeLessThanOrEqualTo = Strings.MustBeLessThanOrEqualTo;
                throw new ArgumentOutOfRangeException(string.Format(CultureInfo.CurrentCulture, mustBeLessThanOrEqualTo, new object[] { maximum }), parameterName);
            }
        }

        public static void RequireLessThanOrEqualTo(float number, float maximum, string parameterName)
        {
            if (number > maximum)
            {
                string mustBeLessThanOrEqualTo = Strings.MustBeLessThanOrEqualTo;
                throw new ArgumentOutOfRangeException(string.Format(CultureInfo.CurrentCulture, mustBeLessThanOrEqualTo, new object[] { maximum }), parameterName);
            }
        }

        public static void RequireNotNull(object parameter, string parameterName)
        {
            RequireNotNull(parameter, parameterName, delegate {
                return Strings.CannotBeNull;
            });
        }

        public static void RequireNotNull(object parameter, string parameterName, ParamErrorTextHandler errorTextHandler)
        {
            if (parameter == null)
            {
                string paramName = string.Empty;
                if (errorTextHandler != null)
                {
                    paramName = errorTextHandler();
                }
                throw new ArgumentNullException(paramName, parameterName);
            }
        }

        public static void RequireNotNull(object parameter, string parameterName, string exceptionMessage)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException(exceptionMessage, parameterName);
            }
        }

        public static void RequireValidCollection(ICollection parameter, string parameterName)
        {
            Require((parameter != null) && (parameter.Count > 0), parameterName, delegate {
                return Strings.CollectionCannotBeEmptyOrNull;
            });
        }

        public static void RequireValidIndex(bool test, string parameterName, ParamErrorTextHandler errorTextHandler)
        {
            if (!test)
            {
                string paramName = string.Empty;
                if (errorTextHandler != null)
                {
                    paramName = errorTextHandler();
                }
                throw new ArgumentOutOfRangeException(paramName, parameterName);
            }
        }

        public static void RequireValidIndex(bool test, string parameterName, string exceptionMessage)
        {
            if (!test)
            {
                throw new ArgumentOutOfRangeException(exceptionMessage, parameterName);
            }
        }

        public static void RequireValidString(string parameter, string parameterName)
        {
            Require((parameter != null) && (parameter.Length > 0), parameterName, delegate {
                return Strings.StringCannotBeEmptyOrNull;
            });
        }

        public static void RequireValueBetween(double number, double low, double high, string parameterName)
        {
            RequireValidIndex((number >= low) && (number <= high), parameterName, delegate {
                return string.Format(CultureInfo.CurrentCulture, Strings.MustBeBetween, new object[] { low, high });
            });
        }

        public static void RequireValueBetween(short number, short low, short high, string parameterName)
        {
            RequireValidIndex((number >= low) && (number <= high), parameterName, delegate {
                return string.Format(CultureInfo.CurrentCulture, Strings.MustBeBetween, new object[] { low, high });
            });
        }

        public static void RequireValueBetween(int number, int low, int high, string parameterName)
        {
            RequireValidIndex((number >= low) && (number <= high), parameterName, delegate {
                return string.Format(CultureInfo.CurrentCulture, Strings.MustBeBetween, new object[] { low, high });
            });
        }

        public static void RequireValueBetween(long number, long low, long high, string parameterName)
        {
            RequireValidIndex((number >= low) && (number <= high), parameterName, delegate {
                return string.Format(CultureInfo.CurrentCulture, Strings.MustBeBetween, new object[] { low, high });
            });
        }

        public static void RequireValueBetween(float number, float low, float high, string parameterName)
        {
            RequireValidIndex((number >= low) && (number <= high), parameterName, delegate {
                return string.Format(CultureInfo.CurrentCulture, Strings.MustBeBetween, new object[] { low, high });
            });
        }
    }
}

