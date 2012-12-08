using System;
using System.Collections.Generic;

namespace Renderer {
	public static class Maths {
		
		public static readonly Random RandomGenerator = new Random();
		public const double PI = Math.PI;
		public const double PI2 = 2.0d*PI;
		public const double PI_2 = 0.5d*PI;
		public const double PI4 = 4.0d*PI;
		public const double PI_4 = 0.25d*PI;
		public const double PI2Inv = 1.0d/PI2;
		public const double OneThird = 1.0d/3.0d;
		public const double GlobalEpsilon = 1e-6;
		public const int NotBinaryIntMask = ~0x01;
		public const double UShortToPercentage = 1.0d/ushort.MaxValue;
		public const double ToPercentage = 0.01d;
			
		///<summary>
		///Borders the given value between the given borders. If the value isn't outside the borders, the method returns
		///the exceeded border.
		///</summary>
		///<param name="min">The given minimum-border.</param>
		///<param name="value">The given value to border.</param>
		///<param name="max">The given maximum-border.</param>
		public static byte Border (byte min, byte value, byte max) {
			return System.Math.Max(min, System.Math.Min(max, value));
		}
		///<summary>
		///Borders the given value between the given borders. If the value isn't outside the borders, the method returns
		///the exceeded border.
		///</summary>
		///<param name="min">The given minimum-border.</param>
		///<param name="value">The given value to border.</param>
		///<param name="max">The given maximum-border.</param>
		public static decimal Border (decimal min, decimal value, decimal max) {
			return System.Math.Max(min, System.Math.Min(max, value));
		}
		///<summary>
		///Borders the given value between the given borders. If the value isn't outside the borders, the method returns
		///the exceeded border.
		///</summary>
		///<param name="min">The given minimum-border.</param>
		///<param name="value">The given value to border.</param>
		///<param name="max">The given maximum-border.</param>
		public static double Border (double min, double value, double max) {
			return System.Math.Max(min, System.Math.Min(max, value));
		}
		///<summary>
		///Borders the given value between the given borders. If the value isn't outside the borders, the method returns
		///the exceeded border.
		///</summary>
		///<param name="min">The given minimum-border.</param>
		///<param name="value">The given value to border.</param>
		///<param name="max">The given maximum-border.</param>
		public static short Border (short min, short value, short max) {
			return System.Math.Max(min, System.Math.Min(max, value));
		}
		///<summary>
		///Borders the given value between the given borders. If the value isn't outside the borders, the method returns
		///the exceeded border.
		///</summary>
		///<param name="min">The given minimum-border.</param>
		///<param name="value">The given value to border.</param>
		///<param name="max">The given maximum-border.</param>
		public static ushort Border (ushort min, ushort value, ushort max) {
			return System.Math.Max(min, System.Math.Min(max, value));
		}
		///<summary>
		///Borders the given value between the given borders. If the value isn't outside the borders, the method returns
		///the exceeded border.
		///</summary>
		///<param name="min">The given minimum-border.</param>
		///<param name="value">The given value to border.</param>
		///<param name="max">The given maximum-border.</param>
		public static int Border (int min, int value, int max) {
			return System.Math.Max(min, System.Math.Min(max, value));
		}
		///<summary>
		///Borders the given value between the given borders. If the value isn't outside the borders, the method returns
		///the exceeded border.
		///</summary>
		///<param name="min">The given minimum-border.</param>
		///<param name="value">The given value to border.</param>
		///<param name="max">The given maximum-border.</param>
		public static uint Border (uint min, uint value, uint max) {
			return System.Math.Max(min, System.Math.Min(max, value));
		}
		///<summary>
		///Borders the given value between the given borders. If the value isn't outside the borders, the method returns
		///the exceeded border.
		///</summary>
		///<param name="min">The given minimum-border.</param>
		///<param name="value">The given value to border.</param>
		///<param name="max">The given maximum-border.</param>
		public static long Border (long min, long value, long max) {
			return System.Math.Max(min, System.Math.Min(max, value));
		}
		///<summary>
		///Borders the given value between the given borders. If the value isn't outside the borders, the method returns
		///the exceeded border.
		///</summary>
		///<param name="min">The given minimum-border.</param>
		///<param name="value">The given value to border.</param>
		///<param name="max">The given maximum-border.</param>
		public static ulong Border (ulong min, ulong value, ulong max) {
			return System.Math.Max(min, System.Math.Min(max, value));
		}
		///<summary>
		///Borders the given value between the given borders. If the value isn't outside the borders, the method returns
		///the exceeded border.
		///</summary>
		///<param name="min">The given minimum-border.</param>
		///<param name="value">The given value to border.</param>
		///<param name="max">The given maximum-border.</param>
		public static sbyte Border (sbyte min, sbyte value, sbyte max) {
			return System.Math.Max(min, System.Math.Min(max, value));
		}
		///<summary>
		///Borders the given value between the given borders. If the value isn't outside the borders, the method returns
		///the exceeded border.
		///</summary>
		///<param name="min">The given minimum-border.</param>
		///<param name="value">The given value to border.</param>
		///<param name="max">The given maximum-border.</param>
		public static float Border (float min, float value, float max) {
			return System.Math.Max(min, System.Math.Min(max, value));
		}
		///<summary>
		///A generic method to border the given value between the given minimum and maximum. The constraint of the type is
		///of course that their exists an order-relation between the different elements of the class T.
		///</summary>
		///<typeparam name="T">The type of the objects to border the value from.</typeparam>
		///<param name="min">The given minimum-border.</param>
		///<param name="value">The given value to border.</param>
		///<param name="max">The given maximum-border.</param>
		public static T Border<T> (T min, T value, T max) where T : System.IComparable<T> {
			if(value.CompareTo(min) < 0x00)
				return min;
			else if(value.CompareTo(max) > 0x00)
				return max;
			else
				return value;
		}
		public static double SoftInv (double val) {
			if(Math.Abs(val) < GlobalEpsilon) {
				return double.NaN;
			}
			else {
				return 1.0d/val;
			}
		}
		public static double ZeroInv (double val) {
			if(Math.Abs(val) < GlobalEpsilon) {
				return 0.0d;
			}
			else {
				return 1.0d/val;
			}
		}
		public static int SoftSign (double val) {
			if(Math.Abs(val) < GlobalEpsilon) {
				return 0x00;
			}
			else {
				return Math.Sign(val);
			}
		}
		public static byte BinarySign (double val) {
			int sgn = Math.Sign(val);
			return (byte)((sgn-sgn*sgn)/0x02+0x01);
		}
		public static int Random (int n) {
			return (int)(RandomGenerator.NextDouble()*n);
		}
		public static double MinPos (double a, double b) {
			if(a < 0.0d) {
				return b;
			}
			else if(b < 0.0d) {
				return a;
			}
			else {
				return Math.Min(a, b);
			}
		}
		///<summary>
		///Interpolates between the two given values with an interpolation-factor.
		///</summary>
		///<returns>The result of the interpolation of the two values with the interpolation-factor.</returns>
		public static float Interpolate (float value1, float value2, float factor) {
			return (1.0f-(float)System.Math.Cos(factor*System.Math.PI))*(value2-value1)*0.5f+value1;
		}
		///<summary>
		///Interpolates between the two given values with an interpolation-factor.
		///</summary>
		///<returns>The result of the interpolation of the two values with the interpolation-factor.</returns>
		public static float Interpolate (float value1, float value2, double factor) {
			return (1.0f-(float)System.Math.Cos(factor*System.Math.PI))*(value2-value1)*0.5f+value1;
		}
		///<summary>
		///Returns the largest value specified by the <see cref="System.IComparable{T}"/>-element of the two objects of T.
		///</summary>
		///<returns>val2 if val1 &lt; val2 or val1 if val2 &#8804; val1.</returns>
		///<typeparam name="T">The type of the two elements to compare with. For this case T needs to implement the
		///<see cref="System.IComparable{T}"/>-interface.</typeparam>
		///<param name="val1">The first value to compare with.</param>
		///<param name="val2">The second value to compare with.</param>
		public static T Max<T> (T val1, T val2) where T : System.IComparable<T> {
			if(val1.CompareTo(val2) < 0x00)
				return val2;
			else
				return val1;
		}
		///<summary>
		///Returns the largest value of the three given values.
		///</summary>
		///<returns>The largest value of the three given values.</returns>
		///<param name="val1">The first value.</param>
		///<param name="val2">The second value.</param>
		///<param name="val3">The third value.</param>
		public static byte Max (byte val1, byte val2, byte val3) {
			return System.Math.Max(System.Math.Max(val1, val2), val3);
		}
		///<summary>
		///Returns the largest value of the three given values.
		///</summary>
		///<returns>The largest value of the three given values.</returns>
		///<param name="val1">The first value.</param>
		///<param name="val2">The second value.</param>
		///<param name="val3">The third value.</param>
		public static decimal Max (decimal val1, decimal val2, decimal val3) {
			return System.Math.Max(System.Math.Max(val1, val2), val3);
		}
		///<summary>
		///Returns the largest value of the three given values.
		///</summary>
		///<returns>The largest value of the three given values.</returns>
		///<param name="val1">The first value.</param>
		///<param name="val2">The second value.</param>
		///<param name="val3">The third value.</param>
		public static double Max (double val1, double val2, double val3) {
			return System.Math.Max(System.Math.Max(val1, val2), val3);
		}
		///<summary>
		///Returns the largest value of the three given values.
		///</summary>
		///<returns>The largest value of the three given values.</returns>
		///<param name="val1">The first value.</param>
		///<param name="val2">The second value.</param>
		///<param name="val3">The third value.</param>
		public static short Max (short val1, short val2, short val3) {
			return System.Math.Max(System.Math.Max(val1, val2), val3);
		}
		///<summary>
		///Returns the largest value of the three given values.
		///</summary>
		///<returns>The largest value of the three given values.</returns>
		///<param name="val1">The first value.</param>
		///<param name="val2">The second value.</param>
		///<param name="val3">The third value.</param>
		public static int Max (int val1, int val2, int val3) {
			return System.Math.Max(System.Math.Max(val1, val2), val3);
		}
		///<summary>
		///Returns the largest value of the three given values.
		///</summary>
		///<returns>The largest value of the three given values.</returns>
		///<param name="val1">The first value.</param>
		///<param name="val2">The second value.</param>
		///<param name="val3">The third value.</param>
		public static long Max (long val1, long val2, long val3) {
			return System.Math.Max(System.Math.Max(val1, val2), val3);
		}
		///<summary>
		///Returns the largest value of the three given values.
		///</summary>
		///<returns>The largest value of the three given values.</returns>
		///<param name="val1">The first value.</param>
		///<param name="val2">The second value.</param>
		///<param name="val3">The third value.</param>
		public static sbyte Max (sbyte val1, sbyte val2, sbyte val3) {
			return System.Math.Max(System.Math.Max(val1, val2), val3);
		}
		///<summary>
		///Returns the largest value of the three given values.
		///</summary>
		///<returns>The largest value of the three given values.</returns>
		///<param name="val1">The first value.</param>
		///<param name="val2">The second value.</param>
		///<param name="val3">The third value.</param>
		public static float Max (float val1, float val2, float val3) {
			return System.Math.Max(System.Math.Max(val1, val2), val3);
		}
		///<summary>
		///Returns the largest value of the three given values.
		///</summary>
		///<returns>The largest value of the three given values.</returns>
		///<param name="val1">The first value.</param>
		///<param name="val2">The second value.</param>
		///<param name="val3">The third value.</param>
		public static ushort Max (ushort val1, ushort val2, ushort val3) {
			return System.Math.Max(System.Math.Max(val1, val2), val3);
		}
		///<summary>
		///Returns the largest value of the three given values.
		///</summary>
		///<returns>The largest value of the three given values.</returns>
		///<param name="val1">The first value.</param>
		///<param name="val2">The second value.</param>
		///<param name="val3">The third value.</param>
		public static uint Max (uint val1, uint val2, uint val3) {
			return System.Math.Max(System.Math.Max(val1, val2), val3);
		}
		///<summary>
		///Returns the largest value of the three given values.
		///</summary>
		///<returns>The largest value of the three given values.</returns>
		///<param name="val1">The first value.</param>
		///<param name="val2">The second value.</param>
		///<param name="val3">The third value.</param>
		public static ulong Max (ulong val1, ulong val2, ulong val3) {
			return System.Math.Max(System.Math.Max(val1, val2), val3);
		}
		///<summary>
		///Returns the largest value of the three given values  specified by the
		///<see cref="System.IComparable{T}"/>-element of the two objects of T.
		///</summary>
		///<returns>The largest value of the three given values.</returns>
		///<typeparam name="T">The type of the three elements to compare with. For this case T needs to implement the
		///<see cref="System.IComparable{T}"/>-interface.</typeparam>
		///<param name="val1">The first value.</param>
		///<param name="val2">The second value.</param>
		///<param name="val3">The third value.</param>
		public static T Max<T> (T val1, T val2, T val3) where T : System.IComparable<T> {
			return Max(Max(val1, val2), val3);
		}
		///<summary>
		///Returns the smallest value specified by the <see cref="System.IComparable{T}"/>-element of the two objects of T.
		///</summary>
		///<returns>val1 if val1 &lt; val2 or val2 if val2 &#8804; val1.</returns>
		///<typeparam name="T">The type of the two elements to compare with. For this case T needs to implement the
		///<see cref="System.IComparable{T}"/>-interface.</typeparam>
		///<param name="val1">The first value to compare with.</param>
		///<param name="val2">The second value to compare with.</param>
		public static T Min<T> (T val1, T val2) where T : System.IComparable<T> {
			if(val1.CompareTo(val2) < 0x00)
				return val1;
			else
				return val2;
		}
		///<summary>
		///Returns the smallest value of the three given values.
		///</summary>
		///<returns>The smallest value of the three given values.</returns>
		///<param name="val1">The first value.</param>
		///<param name="val2">The second value.</param>
		///<param name="val3">The third value.</param>
		public static byte Min (byte val1, byte val2, byte val3) {
			return System.Math.Min(System.Math.Min(val1, val2), val3);
		}
		///<summary>
		///Returns the smallest value of the three given values.
		///</summary>
		///<returns>The smallest value of the three given values.</returns>
		///<param name="val1">The first value.</param>
		///<param name="val2">The second value.</param>
		///<param name="val3">The third value.</param>
		public static decimal Min (decimal val1, decimal val2, decimal val3) {
			return System.Math.Min(System.Math.Min(val1, val2), val3);
		}
		///<summary>
		///Returns the smallest value of the three given values.
		///</summary>
		///<returns>The smallest value of the three given values.</returns>
		///<param name="val1">The first value.</param>
		///<param name="val2">The second value.</param>
		///<param name="val3">The third value.</param>
		public static double Min (double val1, double val2, double val3) {
			return System.Math.Min(System.Math.Min(val1, val2), val3);
		}
		///<summary>
		///Returns the smallest value of the three given values.
		///</summary>
		///<returns>The smallest value of the three given values.</returns>
		///<param name="val1">The first value.</param>
		///<param name="val2">The second value.</param>
		///<param name="val3">The third value.</param>
		public static short Min (short val1, short val2, short val3) {
			return System.Math.Min(System.Math.Min(val1, val2), val3);
		}
		///<summary>
		///Returns the smallest value of the three given values.
		///</summary>
		///<returns>The smallest value of the three given values.</returns>
		///<param name="val1">The first value.</param>
		///<param name="val2">The second value.</param>
		///<param name="val3">The third value.</param>
		public static int Min (int val1, int val2, int val3) {
			return System.Math.Min(System.Math.Min(val1, val2), val3);
		}
		///<summary>
		///Returns the smallest value of the three given values.
		///</summary>
		///<returns>The smallest value of the three given values.</returns>
		///<param name="val1">The first value.</param>
		///<param name="val2">The second value.</param>
		///<param name="val3">The third value.</param>
		public static long Min (long val1, long val2, long val3) {
			return System.Math.Min(System.Math.Min(val1, val2), val3);
		}
		///<summary>
		///Returns the smallest value of the three given values.
		///</summary>
		///<returns>The smallest value of the three given values.</returns>
		///<param name="val1">The first value.</param>
		///<param name="val2">The second value.</param>
		///<param name="val3">The third value.</param>
		public static sbyte Min (sbyte val1, sbyte val2, sbyte val3) {
			return System.Math.Min(System.Math.Min(val1, val2), val3);
		}
		///<summary>
		///Returns the smallest value of the three given values.
		///</summary>
		///<returns>The smallest value of the three given values.</returns>
		///<param name="val1">The first value.</param>
		///<param name="val2">The second value.</param>
		///<param name="val3">The third value.</param>
		public static float Min (float val1, float val2, float val3) {
			return System.Math.Min(System.Math.Min(val1, val2), val3);
		}
		///<summary>
		///Returns the smallest value of the three given values.
		///</summary>
		///<returns>The smallest value of the three given values.</returns>
		///<param name="val1">The first value.</param>
		///<param name="val2">The second value.</param>
		///<param name="val3">The third value.</param>
		public static ushort Min (ushort val1, ushort val2, ushort val3) {
			return System.Math.Min(System.Math.Min(val1, val2), val3);
		}
		///<summary>
		///Returns the smallest value of the three given values.
		///</summary>
		///<returns>The smallest value of the three given values.</returns>
		///<param name="val1">The first value.</param>
		///<param name="val2">The second value.</param>
		///<param name="val3">The third value.</param>
		public static uint Min (uint val1, uint val2, uint val3) {
			return System.Math.Min(System.Math.Min(val1, val2), val3);
		}
		///<summary>
		///Returns the smallest value of the three given values.
		///</summary>
		///<returns>The smallest value of the three given values.</returns>
		///<param name="val1">The first value.</param>
		///<param name="val2">The second value.</param>
		///<param name="val3">The third value.</param>
		public static ulong Min (ulong val1, ulong val2, ulong val3) {
			return System.Math.Min(System.Math.Min(val1, val2), val3);
		}
		///<summary>
		///Returns the smallest value of the three given values  specified by the
		///<see cref="System.IComparable{T}"/>-element of the two objects of T.
		///</summary>
		///<returns>The smallest value of the three given values.</returns>
		///<typeparam name="T">The type of the three elements to compare with. For this case T needs to implement the
		///<see cref="System.IComparable{T}"/>-interface.</typeparam>
		///<param name="val1">The first value.</param>
		///<param name="val2">The second value.</param>
		///<param name="val3">The third value.</param>
		public static T Min<T> (T val1, T val2, T val3) where T : System.IComparable<T> {
			return Min(Min(val1, val2), val3);
		}
	
		public static bool Order (ref double a, ref double b) {
			if(a > b) {
				double t = a;
				a = b;
				b = t;
				return true;
			}
			return false;
		}
		public static bool Order<T> (ref T a, ref T b) where T : IComparable<T> {
			if(a.CompareTo(b) > 0x00) {
				T t = a;
				a = b;
				b = t;
				return true;
			}
			return false;
		}
		public static bool Order<T> (IComparer<T> comp, ref T a, ref T b) {
			if(comp.Compare(a, b) > 0x00) {
				T t = a;
				a = b;
				b = t;
				return true;
			}
			return false;
		}
		public static double MinGeqZero (double a, double b) {
			if(a < 0.0d) {
				return b;
			}
			else if(b < 0.0d) {
				return a;
			}
			else
				return Math.Min(a, b);
		}
		public static double LinEq (double rico, double offset, double y) {
			return (y-offset)/rico;
		}
		public static void LinEqOrder (double rico, double offset, double y1, double y2, out double x1, out double x2) {
			rico = 1.0d/rico;
			x1 = (y1-offset)*rico;
			x2 = (y2-offset)*rico;
			Order(ref x1, ref x2);
		}
		public static float Pythagoras (float dx, float dy) {
			return (float)Math.Sqrt(dx*dx+dy*dy);
		}
		public static float Random () {
			return (float)(2.0d*RandomGenerator.NextDouble()-1.0d);
		}
		public static void GenerateRandomCirclePoint (double R, out double x, out double y) {
			double r = R*Math.Sqrt(RandomGenerator.NextDouble());
			double theta = PI2*RandomGenerator.NextDouble();
			x = Math.Cos(theta)*r;
			y = Math.Sin(theta)*r;
		}
		public static float Random (float m, float M) {
			return (float)((M-m)*RandomGenerator.NextDouble()+m);
		}
		public static float RandomWithDelta (float averidge, float delta) {
			return averidge+Random()*delta;
		}
			
	}
	
}
