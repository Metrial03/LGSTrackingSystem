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
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using OxyPlot.WindowsForms;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Configuration;

namespace LGSTrackingSystem
{
    public partial class AdminReportPanel : Form
    {
        string connectionString = ConfigurationManager.ConnectionStrings["LGSDB"].ConnectionString;
        private List<string> allStudentNames = new List<string>();
        private List<string> allExamNames = new List<string>();
        public AdminReportPanel()
        {
            InitializeComponent();
            CoboxListExam();
            CoboxListStudent();
            textBoxTestStudentName.TextChanged += textBoxTestStudentName_TextChanged;
            textBoxExamName.TextChanged += textBoxExamName_TextChanged;
        }
        private void btnExportPDF_Click(object sender, EventArgs e)
        {
            string selectedStudent = cbStudentList.Text;
            string selectedExam = cbExamList.Text;
            if (string.IsNullOrEmpty(selectedStudent) || string.IsNullOrEmpty(selectedExam))
            {
                MessageBox.Show("Please select both a student and an exam.");
                return;
            }
            
            if (checkExistResult(selectedStudent, selectedExam))
            {
                MessageBox.Show("No results found for the selected student and exam.");
                return;
            }

            int examId = getExamId();

            var studentResults = new List<(string StudentName, int True, int Blank, int False)>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"
            SELECT 
                S.NAME + ' ' + S.SURNAME AS StudentName,
                SUM(R.TRUE_COUNT) AS TrueCount,
                SUM(R.BLANK_COUNT) AS BlankCount,
                SUM(R.FALSE_COUNT) AS FalseCount
            FROM RESULTS R
            INNER JOIN STUDENTS S ON R.STUDENT_ID = S.STUDENT_ID
            WHERE R.EXAM_ID = @examId
            GROUP BY S.NAME, S.SURNAME";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@examId", examId);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        studentResults.Add((
                            reader.GetString(0),
                            reader.IsDBNull(1) ? 0 : reader.GetInt32(1),
                            reader.IsDBNull(2) ? 0 : reader.GetInt32(2),
                            reader.IsDBNull(3) ? 0 : reader.GetInt32(3)
                        ));
                    }
                }
            }

            var rankedStudents = studentResults
                .OrderByDescending(s => s.True)
                .ThenBy(s => s.Blank)
                .ThenBy(s => s.False)
                .Select((s, idx) => new { Rank = idx + 1, s.StudentName, s.True, s.Blank, s.False })
                .ToList();

            var selectedRank = rankedStudents.FirstOrDefault(s => s.StudentName == selectedStudent);

            int studentId = getStudentIdFromName(selectedStudent);
            var results = new List<(string Subject, int True, int Blank, int False)>();
            string[] subjects = { "Mathematics", "Science", "Turkish", "History", "Religion", "English" };
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                foreach (var subject in subjects)
                {
                    string query = @"SELECT TRUE_COUNT, BLANK_COUNT, FALSE_COUNT 
                             FROM RESULTS 
                             WHERE STUDENT_ID = @studentId AND EXAM_ID = @examId AND SUBJECT_NAME = @subject";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@studentId", studentId);
                    command.Parameters.AddWithValue("@examId", examId);
                    command.Parameters.AddWithValue("@subject", subject);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            results.Add((subject, reader.GetInt32(0), reader.GetInt32(1), reader.GetInt32(2)));
                        }
                        else
                        {
                            results.Add((subject, 0, 0, 0));
                        }
                    }
                }
            }

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "PDF files (*.pdf)|*.pdf";
                saveFileDialog.Title = "Save Results as PDF";
                saveFileDialog.FileName = $"{selectedStudent}_{selectedExam}_Results.pdf";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (FileStream stream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                        {
                            var pdfDoc = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4, 36, 36, 36, 36);
                            PdfWriter.GetInstance(pdfDoc, stream);
                            pdfDoc.Open();

                            var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 22, BaseColor.WHITE);
                            var titleCell = new PdfPCell(new Phrase("Exam Results Report", titleFont))
                            {
                                BackgroundColor = new BaseColor(41, 128, 185),
                                HorizontalAlignment = 1,
                                Padding = 16,
                                Border = 0
                            };
                            PdfPTable titleTable = new PdfPTable(1) { WidthPercentage = 100 };
                            titleTable.AddCell(titleCell);
                            pdfDoc.Add(titleTable);
                            pdfDoc.Add(new Paragraph("\n"));

                            var infoFont = FontFactory.GetFont(FontFactory.HELVETICA, 12);
                            var summaryTable = new PdfPTable(2) { WidthPercentage = 60, HorizontalAlignment = iTextSharp.text.Element.ALIGN_LEFT };
                            summaryTable.SpacingAfter = 10;
                            summaryTable.DefaultCell.Border = 0;
                            summaryTable.AddCell(new Phrase("Student:", infoFont));
                            summaryTable.AddCell(new Phrase(selectedStudent, infoFont));
                            summaryTable.AddCell(new Phrase("Exam:", infoFont));
                            summaryTable.AddCell(new Phrase(selectedExam, infoFont));
                            summaryTable.AddCell(new Phrase("Report Date:", infoFont));
                            summaryTable.AddCell(new Phrase($"{DateTime.Now:yyyy-MM-dd HH:mm}", infoFont));
                            pdfDoc.Add(summaryTable);


                            if (selectedRank != null)
                            {
                                var rankFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, BaseColor.BLACK);
                                var rankPhrase = new Phrase($"Placement in this exam: {selectedRank.Rank} / {rankedStudents.Count}", rankFont);
                                var rankCell = new PdfPCell(rankPhrase)
                                {
                                    BackgroundColor = new BaseColor(255, 255, 204),
                                    Padding = 10,
                                    Border = 0,
                                    HorizontalAlignment = 1
                                };
                                PdfPTable rankTable = new PdfPTable(1) { WidthPercentage = 60, HorizontalAlignment = iTextSharp.text.Element.ALIGN_LEFT };
                                rankTable.SpacingAfter = 10;
                                rankTable.AddCell(rankCell);
                                pdfDoc.Add(rankTable);
                            }


                            var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.WHITE);
                            var cellFont = FontFactory.GetFont(FontFactory.HELVETICA, 12);

                            PdfPTable table = new PdfPTable(4);
                            table.WidthPercentage = 100;
                            table.SpacingBefore = 10;
                            table.SetWidths(new float[] { 2, 1, 1, 1 });


                            string[] subjectHeaders = { "Subject", "True", "Blank", "False" };
                            foreach (var h in subjectHeaders)
                            {
                                var cell = new PdfPCell(new Phrase(h, headerFont))
                                {
                                    BackgroundColor = new BaseColor(39, 174, 96),
                                    HorizontalAlignment = 1,
                                    Padding = 7,
                                    BorderWidth = 1.5f
                                };
                                table.AddCell(cell);
                            }

                            for (int i = 0; i < results.Count; i++)
                            {
                                var r = results[i];
                                BaseColor rowColor = i % 2 == 0 ? new BaseColor(245, 245, 245) : BaseColor.WHITE;
                                table.AddCell(new PdfPCell(new Phrase(r.Subject, cellFont)) { BackgroundColor = rowColor, Padding = 6, BorderWidth = 1 });
                                table.AddCell(new PdfPCell(new Phrase(r.True.ToString(), cellFont)) { BackgroundColor = rowColor, HorizontalAlignment = 1, Padding = 6, BorderWidth = 1 });
                                table.AddCell(new PdfPCell(new Phrase(r.Blank.ToString(), cellFont)) { BackgroundColor = rowColor, HorizontalAlignment = 1, Padding = 6, BorderWidth = 1 });
                                table.AddCell(new PdfPCell(new Phrase(r.False.ToString(), cellFont)) { BackgroundColor = rowColor, HorizontalAlignment = 1, Padding = 6, BorderWidth = 1 });
                            }

                            pdfDoc.Add(new Paragraph("Detailed Results:", infoFont));
                            pdfDoc.Add(table);

                            pdfDoc.Close();
                        }

                        MessageBox.Show("PDF exported successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error exporting PDF: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        private void btnShowGraph_Click(object sender, EventArgs e)
        {
            string selectedStudent = cbStudentList.Text;
            string selectedExam = cbExamList.Text;
            if (string.IsNullOrEmpty(selectedStudent) || string.IsNullOrEmpty(selectedExam))
            {
                MessageBox.Show("Please select both a student and an exam.");
                return;
            }
            if (checkExistResult(selectedStudent, selectedExam))
            {
                MessageBox.Show("No results found for the selected student and exam.");
                return;
            }

            int studentId = getStudentIdFromName(selectedStudent);
            int examId = getExamId();

            var results = new List<(string Subject, int True, int Blank, int False)>();
            string[] subjects = { "Mathematics", "Science", "Turkish", "History", "Religion", "English" };

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                foreach (var subject in subjects)
                {
                    string query = @"SELECT TRUE_COUNT, BLANK_COUNT, FALSE_COUNT 
                             FROM RESULTS 
                             WHERE STUDENT_ID = @studentId AND EXAM_ID = @examId AND SUBJECT_NAME = @subject";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@studentId", studentId);
                    command.Parameters.AddWithValue("@examId", examId);
                    command.Parameters.AddWithValue("@subject", subject);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            results.Add((subject, reader.GetInt32(0), reader.GetInt32(1), reader.GetInt32(2)));
                        }
                        else
                        {
                            results.Add((subject, 0, 0, 0));
                        }
                    }
                }
            }

            ShowBarGraph(results);
        }
        private void ShowBarGraph(List<(string Subject, int True, int Blank, int False)> results)
        {
            string[] subjects = { "Mathematics", "Science", "Turkish", "History", "Religion", "English" };
            var orderedResults = subjects
                .Select(subj => results.FirstOrDefault(r => r.Subject == subj))
                .ToList();
            var trueCounts = orderedResults.Select(r => (double)r.True).ToList();
            var blankCounts = orderedResults.Select(r => (double)r.Blank).ToList();
            var falseCounts = orderedResults.Select(r => (double)r.False).ToList();

            Form graphForm = new Form
            {
                Text = "Exam Results Graph",
                Size = new Size(800, 500),
                BackColor = Color.Lavender,
                StartPosition = FormStartPosition.CenterScreen,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            PlotView plotView = new PlotView { Dock = DockStyle.Fill };
            graphForm.Controls.Add(plotView);

            var plotModel = new PlotModel { Title = "Exam Results (True/Blank/False per Subject)" };

            var categoryAxis = new CategoryAxis { Position = AxisPosition.Left };
            categoryAxis.Labels.AddRange(subjects);
            plotModel.Axes.Add(categoryAxis);

            var valueAxis = new LinearAxis { Position = AxisPosition.Bottom, Minimum = 0 };
            plotModel.Axes.Add(valueAxis);

            var trueSeries = new BarSeries { Title = "True", StrokeColor = OxyColors.Black, StrokeThickness = 1 };
            var blankSeries = new BarSeries { Title = "Blank", StrokeColor = OxyColors.Black, StrokeThickness = 1 };
            var falseSeries = new BarSeries { Title = "False", StrokeColor = OxyColors.Black, StrokeThickness = 1 };

            trueSeries.Items.AddRange(trueCounts.Select(v => new BarItem { Value = v }));
            blankSeries.Items.AddRange(blankCounts.Select(v => new BarItem { Value = v }));
            falseSeries.Items.AddRange(falseCounts.Select(v => new BarItem { Value = v }));

            plotModel.Series.Add(trueSeries);
            plotModel.Series.Add(blankSeries);
            plotModel.Series.Add(falseSeries);

            plotView.Model = plotModel;
            graphForm.ShowDialog();
        }
        private bool checkExistResult(string studentName, string examName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string getStudenIdQuery = "SELECT STUDENT_ID FROM Students WHERE NAME + ' ' + SURNAME = @studentName";
                    SqlCommand getStudentIdCommand = new SqlCommand(getStudenIdQuery, connection);
                    getStudentIdCommand.Parameters.AddWithValue("@studentName", studentName);
                    object studentIdObj = getStudentIdCommand.ExecuteScalar();
                    int studentId = Convert.ToInt32(studentIdObj);


                    string checkResultQuery = "SELECT COUNT(*) FROM RESULTS WHERE STUDENT_ID = @studentId AND EXAM_ID = (SELECT EXAM_ID FROM EXAMS WHERE EXAM_NAME = @examName)";
                    SqlCommand checkResultCommand = new SqlCommand(checkResultQuery, connection);
                    checkResultCommand.Parameters.AddWithValue("@studentId", studentId);
                    checkResultCommand.Parameters.AddWithValue("@examName", examName);
                    int resultCount = (int)checkResultCommand.ExecuteScalar();
                    if (resultCount > 0)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Error checking result: " + ex.Message);
                    return false;
                }
            }
        }
        private int getStudentIdFromName(string getFrom)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT STUDENT_ID FROM Students WHERE NAME + ' ' + SURNAME = @studentName";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@studentName", getFrom);
                object result = command.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : -1;
            }
        }
        private int getExamId()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT EXAM_ID FROM EXAMS WHERE EXAM_NAME = @examName";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@examName", cbExamList.Text);
                object result = command.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : -1;
            }
        }
        private void CoboxListExam()
        {
            cbExamList.Items.Clear();
            allExamNames.Clear();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT EXAM_NAME FROM EXAMS";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        string examName = reader["EXAM_NAME"].ToString();
                        allExamNames.Add(examName);
                    }
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Error retrieving exam names: " + ex.Message);
                }
            }
            cbExamList.Items.AddRange(allExamNames.ToArray());
        }
        private void textBoxExamName_TextChanged(object sender, EventArgs e)
        {
            string filter = textBoxExamName.Text.Trim().ToLower();
            cbExamList.Items.Clear();
            var filtered = allExamNames
                .Where(name => name.ToLower().Contains(filter))
                .ToArray();
            cbExamList.Items.AddRange(filtered);
        }
        private void CoboxListStudent()
        {
            cbStudentList.Items.Clear();
            allStudentNames.Clear();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT NAME + ' ' + SURNAME AS [Student Name] FROM Students";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        string studentName = reader["Student Name"].ToString();
                        allStudentNames.Add(studentName);
                    }
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Error retrieving student names: " + ex.Message);
                }
            }
            cbStudentList.Items.AddRange(allStudentNames.ToArray());
        }
        private void textBoxTestStudentName_TextChanged(object sender, EventArgs e)
        {
            string filter = textBoxTestStudentName.Text.Trim().ToLower();
            cbStudentList.Items.Clear();
            var filtered = allStudentNames
                .Where(name => name.ToLower().Contains(filter))
                .ToArray();
            cbStudentList.Items.AddRange(filtered);
        }
    }
}
