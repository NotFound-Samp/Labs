using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsLabs1
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }

    public class Solution
    {
        static Random rnd = new Random();

        static double[] CreateNum()
        {
            double[] Array = new double[17000];
            for (int k = 0; k < 17000; k++)
            {
                Array[k] = rnd.NextDouble() * 100;
            }
            return Array;
        }

        static void Swap(double[] array, int i, int j)
        {
            double temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }

        static double[] GnomeSort(double[] inArray)
        {
            int i = 1;
            int j = 2;
            while (i < inArray.Length)
            {
                if (inArray[i - 1] < inArray[i])
                {
                    i = j;
                    j += 1;
                }
                else
                {
                    Swap(inArray, i - 1, i);
                    i -= 1;
                    if (i == 0)
                    {
                        i = j;
                        j += 1;
                    }
                }
            }
            return inArray; // Вернем отсортированный массив
        }

        static int[] CreateSysNum()
        {
            int[] sysNum = new int[17000];
            for (int k = 0; k < 17000; k++)
            {
                sysNum[k] = rnd.Next(100);
            }
            return sysNum;
        }

        public (double, int) SortAndReturnResults()
        {
            double executionTime = Count().TotalMilliseconds;
            int sysNum = Thread.CurrentThread.ManagedThreadId;

            return (executionTime, sysNum);
        }

        public TimeSpan Count()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            double[] Array = CreateNum();
            GnomeSort((double[])Array.Clone()); // Используем копию массива
            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            return ts;
        }
    }
}
