using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Configuration;
namespace LGSTrackingSystem
{
    public partial class ExamListPanel : Form
    {
        string connectionString = ConfigurationManager.ConnectionStrings["LGSDB"].ConnectionString;
        public ExamListPanel()
        {
            InitializeComponent();
            listExams();
            textBoxExamName.TextChanged += textBoxExamName_TextChanged;
        }
        private void listExams()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT EXAM_NAME AS [Exam Name], EXAM_TYPE AS [Exam Status] FROM EXAMS";
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                dataGridViewExams.DataSource = dataTable;
            }
        }
        public void FilterExams(string filter)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT EXAM_NAME AS [Exam Name], EXAM_TYPE AS [Exam Status] FROM EXAMS WHERE EXAM_NAME LIKE @filter";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@filter", "%" + filter + "%");
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                dataGridViewExams.DataSource = dataTable;
            }
        }

        private void textBoxExamName_TextChanged(object sender, EventArgs e)
        {
            FilterExams(textBoxExamName.Text.Trim());
        }
    }
}
