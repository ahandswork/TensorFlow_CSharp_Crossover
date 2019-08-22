using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetPractice
{
    class Fraction
    {
        public static Fraction Zero => new Fraction(0, 1);
        public Fraction(long numerator, long denominator)
        {
            Numberator = numerator;
            Denominator = denominator;
        }
        public long Numberator { get; set; }
        public long Denominator { get; set; }
        //
        //
        public static Fraction operator +(Fraction a,Fraction b) =>
            new Fraction(a.Numberator*b.Denominator+b.Numberator*a.Denominator,a.Denominator*b.Denominator);

        public static Fraction operator -(Fraction a, Fraction b) =>
            new Fraction(a.Numberator * b.Denominator - b.Numberator * a.Denominator, a.Denominator * b.Denominator);

        public static Fraction operator -(Fraction a) =>
            new Fraction(-a.Numberator, a.Denominator);

        public static Fraction operator *(Fraction a, Fraction b) =>
            new Fraction(a.Numberator * b.Numberator, a.Denominator * b.Denominator);
        public static Fraction operator *(long a, Fraction b) =>
            new Fraction(a * b.Numberator, b.Denominator);
        public static Fraction operator *(Fraction a, long b) => b * a;

        public static Fraction operator /(Fraction a, Fraction b) =>
            new Fraction(a.Numberator * b.Denominator, a.Denominator * b.Numberator);

        public static bool operator <(long a, Fraction b) =>
            (long)a * b.Denominator < b.Numberator;

        public static bool operator >(long a, Fraction b) =>
            (long)a * b.Denominator > b.Numberator;

        public static bool operator ==(long a, Fraction b) =>
            (long)a * b.Denominator == b.Numberator;

        public static bool operator !=(long a, Fraction b) =>
            (long)a * b.Denominator != b.Numberator;
        //
        public static explicit operator Fraction(long n) =>
            new Fraction(n,1);
        public static explicit operator double(Fraction n) =>
            n.Numberator * 1.0 / n.Denominator;
        //
        public void Simplify()
        {
            if(Numberator == 0)
            {
                Denominator = 1;
                return;
            }
            sbyte sign = 1;
            if (Numberator < 0)
            {
                Numberator = -Numberator;
                sign = -1;
            }
            if(Denominator < 0)
            {
                Denominator = -Denominator;
                sign *= -1;
            }
            for (int i = 2; i * i <= Numberator && i * i <= Denominator; i++)
                while(Numberator % i == 0 && Denominator % i == 0)
                {
                    Numberator /= i;
                    Denominator /= i;
                }
            if (Denominator % Numberator == 0)
            {
                Denominator /= Numberator;
                Numberator = 1;
            }
            else if(Numberator % Denominator == 0)
            {
                Numberator /= Denominator;
                Denominator = 1;
            }
            Numberator *= sign;
        }
        public override string ToString()
        {
            if (Denominator != 1)
                return Numberator + "/" + Denominator;
            else return Numberator.ToString();
        }
    }
}
