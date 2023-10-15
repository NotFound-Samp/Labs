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
        Solution solution = new Solution();
        private static int endTaskCounter = 0;
        private static int endThreadCounter = 0;
        readonly static int totalThreads = 100;

        private void StartThreads()
        {
            for (int i = 1; i <= totalThreads; i++)
            {
                Thread thread = new Thread(RunSolution);
                thread.Start(i);
            }
        }
        private void RunSolution(object threadNumber)
        {
            var result = solution.SortAndReturnResults();
            UpdateDataGridView(result.Item1, result.Item2, (int)threadNumber, Interlocked.Increment(ref endThreadCounter));
        }
        private async Task<(double, int, int, int)> RunSolutionAsync(int startNum)
        {
            return await Task.Run(() =>
            {
                var result = solution.SortAndReturnResults();
                return (result.Item1, result.Item2, startNum, Interlocked.Increment(ref endTaskCounter));
            });
        }
        private async Task StartTasksAsync()
        {
            List<Task<(double, int, int, int)>> tasks = new List<Task<(double, int, int, int)>>();

            for (int i = 1; i <= totalThreads; i++)
                tasks.Add(RunSolutionAsync(i));

            var results = await Task.WhenAll(tasks);

            foreach (var result in results)
                UpdateDataGridView(result.Item1, result.Item2, result.Item3, result.Item4);
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
        public Form1()
        {
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            label1.Text = "Общее время: ";
            endThreadCounter = 0;
            endTaskCounter = 0;
            dataGridView1.Rows.Clear();
            dataGridView2.Rows.Clear();
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

            double minExecutionTime = double.MaxValue;
            double maxExecutionTime = double.MinValue;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                double executionTime = Convert.ToDouble(row.Cells[2].Value);
                minExecutionTime = Math.Min(minExecutionTime, executionTime);
                maxExecutionTime = Math.Max(maxExecutionTime, executionTime);
            }

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
