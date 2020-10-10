using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EchoBot.Bots
{
    public class Equation
    {
        //Описание:
        //Функция возращаетс строку: полином n-ной степени
        //Для генерации этого полинома передаются пары (x, f(x)) в виде двух массивов
        //Количество таких пар должно быть (n+1), при этом f(x) должно быть равно 0 в n случаях из n+1 возможных
        //Пар должно быть не меньше 3-х (квадратное уравнение)
        //
        //Формирование строки надо будет доработать
        //
        //

        public static string Polinom(double[] X, double[] Y)
        {
            double max;
            int index;
            int n = X.Length;
            const double eps = 0.000001;  // точность
            double[] w = new double[n];
            int k = 0;
            double[][] AX = new double[n][];

            for (int i = 0; i < X.Length; i++)
            {
                AX[i] = new double[X.Length];
                for (int j = 0; j < X.Length; j++)
                {
                    AX[i][j] = Math.Pow(X[i], X.Length - 1 - j);
                }
            }

            while (k < n)
            {
                max = Math.Abs(AX[k][k]);
                index = k;
                for (int i = k; i < n; i++)
                {
                    if (Math.Abs(AX[i][k]) > max)
                    {
                        max = Math.Abs(AX[i][k]);
                        index = i;
                    }
                }

                double temp;
                for (int j = 0; j < n; j++)
                {
                    temp = AX[k][j];
                    AX[k][j] = AX[index][j];
                    AX[index][j] = temp;
                }
                temp = Y[k];
                Y[k] = Y[index];
                Y[index] = temp;

                for (int i = k; i < n; i++)
                {
                    temp = AX[i][k];
                    if (Math.Abs(temp) < eps) continue;
                    for (int j = 0; j < n; j++)
                        AX[i][j] = AX[i][j] / temp;
                    Y[i] = Y[i] / temp;
                    if (i == k) continue;
                    for (int j = 0; j < n; j++)
                        AX[i][j] = AX[i][j] - AX[k][j];
                    Y[i] = Y[i] - Y[k];
                }
                k++;
            }

            for (k = n - 1; k >= 0; k--)
            {
                w[k] = Y[k];
                for (int i = 0; i < k; i++)
                    Y[i] = Y[i] - AX[i][k] * w[k];
            }

            string str = "";
            str += Math.Round(w[0], 1).ToString() + "(x^" + Math.Round(Convert.ToDouble(w.Length) - 1, 1).ToString() + ")" + (w[1] >= 0 ? "+" : "-");
            for (int i = 1; i < w.Length - 1; i++)
            {
                if (Math.Round(w[i], 1) != 0.0)
                    str += Math.Abs(Math.Round(w[i], 1)).ToString() + "(x^" + Math.Round(Convert.ToDouble(w.Length) - 1 - i, 1).ToString() + ")" + (w[i + 1] >= 0 ? "+" : "-");
            }
            str += Math.Round(w[^1], 1).ToString() + "=";
            return str;
        }

        /// <summary>
        /// генерирует строку правильного ответа
        /// </summary>
        /// <param name="X">массив ответов (+ один(последний) элемент в массиве не ответ, а просто число)</param>
        /// <returns></returns>
        private static string GenerateTrueAns(double[] X)
        {
            string str = "";
            for (int i = 0; i < X.Length - 1; i++)
            {
                str += X[i].ToString() + ", ";
            }
            return str;
        }

        /// <summary>
        /// генерирует строку правильного ответа
        /// </summary>
        /// <param name="X">массив ответов (+ один(последний) элемент в массиве не ответ, а просто число)</param>
        /// <returns></returns>
        private static string GenerateFalseAns(double[] X)
        {
            string str = "";
            for (int i = 0; i < X.Length - 1; i++)
            {
                str += (((new Random()).Next() % 2 == 1) ? X[i] : ((new Random()).Next() % 2 == 1) ? X[i] + ((new Random()).Next() % 10 - 5) : X[i] * ((new Random()).Next() % 8 - 4)).ToString() + ", ";
            }
            return str;
        }

        /// <summary>
        /// возвращает задание
        /// </summary>
        /// <param name="dim">степень полинома</param>
        /// <param name="ansCount">количество вариантов ответов</param>
        /// <returns>массив строк:: 0-я: уравнение, 1-я: индекс верного из последующих ответов (т.е. нужно будет прибавить +2) осталное: ответы</returns>
        private static string[] SolveEquation(int dim, int ansCount)
        {
            string[] retStr = new string[ansCount + 2];
            double[] X = new double[dim + 1];
            double[] Y = new double[dim + 1];
            for (int i = 0; i < dim; i++)
            {
                X[i] = (new Random()).Next() % 20 - 10;
                Y[i] = 0;
            }
            X[dim] = (new Random()).Next() % 20 - 10;
            Y[dim] = (new Random()).Next() % 50 - 10;
            retStr[0] = GeneratePolinom(X, Y);
            retStr[1] = ((new Random()).Next() % (ansCount - 2) + 2).ToString();
            for (int i = 2; i < 2 + ansCount; i++)
            {
                if (i == Convert.ToInt32(retStr[1]))
                {
                    retStr[i] = GenerateTrueAns(X);
                }
                else
                {
                    retStr[i] = GenerateFalseAns(X);
                }
            }
            return retStr;
        }

        /// <summary>
        /// генерирует уравнение
        /// </summary>
        /// <param name="X">ответы + 1 доп. элемент</param>
        /// <param name="Y">нули + 1. доп элемент для доп. элемента Х</param>
        /// <returns></returns>
        private static string GeneratePolinom(double[] X, double[] Y)
        {
            double max;
            int index;
            int n = X.Length;
            const double eps = 0.000001;  // точность
            double[] w = new double[n];
            int k = 0;
            double[][] AX = new double[n][];

            for (int i = 0; i < X.Length; i++)
            {
                AX[i] = new double[X.Length];
                for (int j = 0; j < X.Length; j++)
                {
                    AX[i][j] = Math.Pow(X[i], X.Length - 1 - j);
                }
            }

            while (k < n)
            {
                max = Math.Abs(AX[k][k]);
                index = k;
                for (int i = k; i < n; i++)
                {
                    if (Math.Abs(AX[i][k]) > max)
                    {
                        max = Math.Abs(AX[i][k]);
                        index = i;
                    }
                }

                double temp;
                for (int j = 0; j < n; j++)
                {
                    temp = AX[k][j];
                    AX[k][j] = AX[index][j];
                    AX[index][j] = temp;
                }
                temp = Y[k];
                Y[k] = Y[index];
                Y[index] = temp;

                for (int i = k; i < n; i++)
                {
                    temp = AX[i][k];
                    if (Math.Abs(temp) < eps) continue;
                    for (int j = 0; j < n; j++)
                        AX[i][j] = AX[i][j] / temp;
                    Y[i] = Y[i] / temp;
                    if (i == k) continue;
                    for (int j = 0; j < n; j++)
                        AX[i][j] = AX[i][j] - AX[k][j];
                    Y[i] = Y[i] - Y[k];
                }
                k++;
            }

            for (k = n - 1; k >= 0; k--)
            {
                w[k] = Y[k];
                for (int i = 0; i < k; i++)
                    Y[i] = Y[i] - AX[i][k] * w[k];
            }

            string str = "";
            str += Math.Round(w[0], 1).ToString() + "(x^" + Math.Round(Convert.ToDouble(w.Length) - 1, 1).ToString() + ")" + (w[1] >= 0 ? "+" : "-");
            for (int i = 1; i < w.Length - 1; i++)
            {
                if (Math.Round(w[i], 1) != 0.0)
                    str += Math.Abs(Math.Round(w[i], 1)).ToString() + "(x^" + Math.Round(Convert.ToDouble(w.Length) - 1 - i, 1).ToString() + ")" + (w[i + 1] >= 0 ? "+" : "-");
            }
            str += Math.Abs(Math.Round(w[^1], 1)).ToString() + "=0";
            return str;
        }

        static void Main(string[] args)
        {
            string[] q = SolveEquation(3, 4);
            foreach (string h in q)
                Console.WriteLine(h);
        }
    }
}
}
