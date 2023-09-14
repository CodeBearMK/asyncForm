using System.Data;
using System.Data.SqlClient;

namespace asyncForm
{
    public partial class Form1 : Form
    {

        private Progress<ProgressReport> progress;

        public Form1()
        {
            InitializeComponent();
            progress = new Progress<ProgressReport>();
            progress.ProgressChanged += Progress_ProgressChanged;
            LoadDataAsync();
        }

        private void Progress_ProgressChanged(object? sender, ProgressReport e)
        {
            progressBar1.Value = e.Percentage;
            statusLabel.Text = e.StatusMessage;
        }

        private async void LoadDataAsync()
        {
            // ��ܸ��Ū�������T��
            UpdateStatusLabel("Loading data...");

            // ��l�� ProgressReport
            var progressReport = new ProgressReport();

            // �b�D�P�B��k��Ū����Ʈw�����
            var data = await Task.Run(() =>
            {
                var result = GetDataFromDatabase(progressReport);
                return result;
            });

            // ���Ū���������sUI
            UpdateUIWithData(data);

            // ���ø��Ū�������T��
            UpdateStatusLabel("");
        }

        private void UpdateStatusLabel(string message)
        {
            // �bUI������W��sLabel�������r
            if (statusLabel.InvokeRequired)
            {
                statusLabel.Invoke(new Action(() => statusLabel.Text = message));
            }
            else
            {
                statusLabel.Text = message;
            }
        }

        private DataTable GetDataFromDatabase(ProgressReport progress)
        {
            // �b�o�Ӥ�k���d�߸�Ʈw�ê�^���

            // �ܨҡG

            int totalRecords = 0;
            int currentRecords = 0;
            DataTable data = new DataTable("Student");

            string connectionString = "Server=localhost;Database=School;User Id=sa;Password=mike6419625";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT * FROM dbo.student", connection))
                {
                    var sqlDataReader = new SqlDataAdapter(command);
                    sqlDataReader.Fill(data);
                    totalRecords = data.Rows.Count;
                    for (int i = 0; i < data.Rows.Count; i++)
                    {
                        DataRow row = data.NewRow();
                        row["sno"] = data.Rows[i]["sno"];
                        row["sname"] = data.Rows[i]["sname"];
                        row["sage"] = data.Rows[i]["sage"];
                        row["ssex"] = data.Rows[i]["ssex"];
                        data.Rows.Add(row);

                        int percentageComplete = (int)((currentRecords++) / (double)totalRecords * 100);
                        progress.Percentage = percentageComplete;
                        progress.StatusMessage = $"Loading data: {percentageComplete}% complete";
                    }
                }
            }
            return data;
        }

        private void UpdateUIWithData(DataTable data)
        {
            // �N���ô���� WinForm �� DataGridView �Ψ�L����W
            dataGridView1.DataSource = data;
        }
    }
}