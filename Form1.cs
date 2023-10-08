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
        public double allTime = 0;
        private void RunSolutionThread()
        {
            Solution solution = new Solution();
            double result = solution.Count().TotalMilliseconds;
            allTime += result;
            label1.Text = $"Общее время:\n{allTime} мс.";
        }
        private void StartThreads()
        {
            for (int i = 0; i < 100; i++)
            {
                Thread thread = new Thread(new ThreadStart(RunSolutionThread));
                thread.Start();
            }
        }
        private async Task<List<(double[], double[], double, int[])>> StartTasksAsync()
        {
            List<Task<(double[], double[], double, int[])>> tasks = new List<Task<(double[], double[], double, int[])>>();

            for (int i = 0; i < 100; i++)
            {
                tasks.Add(RunSolutionAsync());
            }

            await Task.WhenAll(tasks);

            return tasks.Select(t => t.Result).ToList();
        }

        private async Task<(double[], double[], double, int[])> RunSolutionAsync()
        {
            Solution solution = new Solution();
            return await Task.Run(() => solution.SortAndReturnResults());
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
            label1.Text= "Общее время: ";
            allTime = 0;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            StartThreads();
        }
        private async void button3_Click(object sender, EventArgs e)
        {
            List<(double[], double[], double, int[])> results = await StartTasksAsync();

            dataGridView1.Rows.Clear();

            foreach (var result in results)
            {
                dataGridView1.Rows.Add(
                    string.Join(", ", result.Item1),
                    string.Join(", ", result.Item2),
                    result.Item3,
                    string.Join(", ", result.Item4)
                );
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
