using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetPractice
{
    class Polynomial
    {
        public Polynomial(Fraction[] ceofficients)
        {
            Ceofficients = ceofficients;
        }
        public Fraction[] Ceofficients { get; set; }
        public Fraction ExactFunction(Fraction input)
        {
            Fraction x = (Fraction)1, y = (Fraction)0;
            for (int i = 0; i < Ceofficients.Length; i++)
            {
                y += Ceofficients[i] * x;
                x *= input;
                y.Simplify();
            }
            return y;
        }
        public Fraction ExactFunction(long input)
        {
            long x = 1;
            Fraction y = (Fraction)0;
            for (int i = 0; i < Ceofficients.Length; i++)
            {
                y += Ceofficients[i] * x;
                x *= input;
                y.Simplify();
            }
            return y;
        }
        public double EstimatedFunction(double input)
        {
            double x = 1, y = 0;
            for(int i = 0; i < Ceofficients.Length; i++)
            {
                y += (double)Ceofficients[i] * x;
                x *= input;
            }
            return y;
        }
        public override string ToString()
        {
            string str = "f(x) = ";
            for (int i = Ceofficients.Length - 1; i >= 0; i--)
                if (Ceofficients[i].Numberator != 0)
                {
                    if (str.Length > 7)
                        str += " + ";
                    if (i == 0)
                    {
                        if (0 != Ceofficients[i])
                            str += Ceofficients[i];
                        break;
                    }
                    if(1 != Ceofficients[i])
                    {
                        if (-1 == Ceofficients[i])
                            str += '-';
                        else
                            str += Ceofficients[i];
                    }
                    str += 'X';
                    if (i > 1)
                        str += "^" + i;
                }
            return str;
        }
    }
}
