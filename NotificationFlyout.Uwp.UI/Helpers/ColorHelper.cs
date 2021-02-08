// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Reflection;
using Windows.UI;
using Color = Windows.UI.Color;

namespace NotificationFlyout.Uwp.UI.Helpers
{

    public static class ColorHelper
    {
        public static Color ToColor(this string colorString)
        {
            if (string.IsNullOrEmpty(colorString))
            {
                ThrowArgumentException();
            }

            if (colorString[0] == '#')
            {
                switch (colorString.Length)
                {
                    case 9:
                        {
                            var cuint = Convert.ToUInt32(colorString.Substring(1), 16);
                            var a = (byte)(cuint >> 24);
                            var r = (byte)((cuint >> 16) & 0xff);
                            var g = (byte)((cuint >> 8) & 0xff);
                            var b = (byte)(cuint & 0xff);

                            return Color.FromArgb(a, r, g, b);
                        }

                    case 7:
                        {
                            var cuint = Convert.ToUInt32(colorString.Substring(1), 16);
                            var r = (byte)((cuint >> 16) & 0xff);
                            var g = (byte)((cuint >> 8) & 0xff);
                            var b = (byte)(cuint & 0xff);

                            return Color.FromArgb(255, r, g, b);
                        }

                    case 5:
                        {
                            var cuint = Convert.ToUInt16(colorString.Substring(1), 16);
                            var a = (byte)(cuint >> 12);
                            var r = (byte)((cuint >> 8) & 0xf);
                            var g = (byte)((cuint >> 4) & 0xf);
                            var b = (byte)(cuint & 0xf);
                            a = (byte)(a << 4 | a);
                            r = (byte)(r << 4 | r);
                            g = (byte)(g << 4 | g);
                            b = (byte)(b << 4 | b);

                            return Color.FromArgb(a, r, g, b);
                        }

                    case 4:
                        {
                            var cuint = Convert.ToUInt16(colorString.Substring(1), 16);
                            var r = (byte)((cuint >> 8) & 0xf);
                            var g = (byte)((cuint >> 4) & 0xf);
                            var b = (byte)(cuint & 0xf);
                            r = (byte)(r << 4 | r);
                            g = (byte)(g << 4 | g);
                            b = (byte)(b << 4 | b);

                            return Color.FromArgb(255, r, g, b);
                        }

                    default: return ThrowFormatException();
                }
            }

            if (colorString.Length > 3 && colorString[0] == 's' && colorString[1] == 'c' && colorString[2] == '#')
            {
                var values = colorString.Split(',');

                if (values.Length == 4)
                {
                    var scA = double.Parse(values[0].Substring(3), CultureInfo.InvariantCulture);
                    var scR = double.Parse(values[1], CultureInfo.InvariantCulture);
                    var scG = double.Parse(values[2], CultureInfo.InvariantCulture);
                    var scB = double.Parse(values[3], CultureInfo.InvariantCulture);

                    return Color.FromArgb((byte)(scA * 255), (byte)(scR * 255), (byte)(scG * 255), (byte)(scB * 255));
                }

                if (values.Length == 3)
                {
                    var scR = double.Parse(values[0].Substring(3), CultureInfo.InvariantCulture);
                    var scG = double.Parse(values[1], CultureInfo.InvariantCulture);
                    var scB = double.Parse(values[2], CultureInfo.InvariantCulture);

                    return Color.FromArgb(255, (byte)(scR * 255), (byte)(scG * 255), (byte)(scB * 255));
                }

                return ThrowFormatException();
            }

            var prop = typeof(Colors).GetTypeInfo().GetDeclaredProperty(colorString);

            if (prop != null)
            {
                return (Color)prop.GetValue(null);
            }

            return ThrowFormatException();

            static void ThrowArgumentException() => throw new ArgumentException("The parameter \"colorString\" must not be null or empty.");
            static Color ThrowFormatException() => throw new FormatException("The parameter \"colorString\" is not a recognized Color format.");
        }
    }
}