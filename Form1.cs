using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsLabs1
{
    public partial class Form1 : Form
    {
        private static object lockObject = new object();
        private static int threadCounter = 0;
        private static int taskCounter = 0;
        private static int totalThreads = 100;
        private void StartThreads()
        {
            for (int i = 0; i < totalThreads; i++)
            {
                Thread thread = new Thread(RunSolution);
                thread.Start(i + 1);
            }
        }

        private void RunSolution(object threadNumber)
        {
            Solution solution = new Solution();
            int startNum = (int)threadNumber;

            var result = solution.SortAndReturnResults();

            lock (lockObject)
            {
                int endNum = GetNextThreadNumber();
                if (startNum == 1)
                    endNum = 1;
                UpdateDataGridView(result.Item1, result.Item2, startNum, endNum);
            }
        }

        private int GetNextThreadNumber()
        {
            return Interlocked.Increment(ref threadCounter);
        }

        private void UpdateDataGridView(double executionTime, int sysNum, int startNum, int endNum)
        {
            if (dataGridView1.InvokeRequired)
            {
                dataGridView1.Invoke(new MethodInvoker(delegate {
                    dataGridView1.Rows.Add(
                        startNum,
                        endNum,
                        executionTime,
                        sysNum
                    );
                }));
            }
            else
            {
                dataGridView1.Rows.Add(
                    startNum,
                    endNum,
                    executionTime,
                    sysNum
                );
            }
        }
        private async Task StartTasksAsync()
        {
            List<Task<(double, int, int)>> tasks = new List<Task<(double, int, int)>>();

            for (int i = 0; i < totalThreads; i++)
            {
                tasks.Add(RunSolutionAsync());
            }

            var results = await Task.WhenAll(tasks);

            lock (lockObject)
            {
                foreach (var result in results)
                {
                    int endNum = GetNextThreadNumber() + 1;
                    if (result.Item3 == 1)
                        endNum = 1;
                    UpdateDataGridView(result.Item1, result.Item3, endNum, result.Item3);
                }
            }
        }

        private async Task<(double, int, int)> RunSolutionAsync()
        {
            return await Task.Run(() => RunSolution());
        }

        private (double, int, int) RunSolution()
        {
            Solution solution = new Solution();

            var result = solution.SortAndReturnResults();

            int startNum = Interlocked.Increment(ref taskCounter);

            return (result.Item1, result.Item2, startNum);
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            label1.Text = "Общее время: ";
            dataGridView1.Rows.Clear();
            dataGridView2.Rows.Clear();
            taskCounter = 0;
            threadCounter = 0;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            StartThreads();
        }
        private async void button3_Click(object sender, EventArgs e)
        {
            await StartTasksAsync();
        }
        private void button4_Click(object sender, EventArgs e)
        {
            dataGridView2.Rows.Clear();

            // Получить минимальное и максимальное время выполнения
            double minExecutionTime = double.MaxValue;
            double maxExecutionTime = double.MinValue;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                double executionTime = Convert.ToDouble(row.Cells[2].Value);
                minExecutionTime = Math.Min(minExecutionTime, executionTime);
                maxExecutionTime = Math.Max(maxExecutionTime, executionTime);
            }

            // Вычислить интервалы и их количество
            double intervalSize = (maxExecutionTime - minExecutionTime) / 10;
            double currentIntervalStart = minExecutionTime;
            double currentIntervalEnd = minExecutionTime + intervalSize;

            for (int i = 0; i < 10; i++)
            {
                int amount = 0;

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    double executionTime = Convert.ToDouble(row.Cells[2].Value);

                    if (executionTime >= currentIntervalStart && executionTime <= currentIntervalEnd)
                    {
                        amount++;
                    }
                }

                string piece = $"{currentIntervalStart:F0}-{currentIntervalEnd:F0}";
                dataGridView2.Rows.Add(piece, amount);

                currentIntervalStart = currentIntervalEnd + 1;
                currentIntervalEnd += intervalSize;
            }

            double totalExecutionTime = 0;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                double executionTime = Convert.ToDouble(row.Cells[2].Value);
                totalExecutionTime += executionTime;
            }
            label1.Text = $"Общее время:\n{totalExecutionTime} мс";

            double averageExecutionTime = totalExecutionTime / dataGridView1.Rows.Count;
            dataGridView2.Rows.Add("Среднее", 0);
            dataGridView2.Rows[10].Cells[1].Value = averageExecutionTime;
        }
    }
}
