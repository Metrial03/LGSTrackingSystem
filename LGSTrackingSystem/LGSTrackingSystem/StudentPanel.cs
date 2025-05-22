using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
namespace LGSTrackingSystem
{
    public partial class StudentPanel : Form
    {
        string connectionString = ConfigurationManager.ConnectionStrings["LGSDB"].ConnectionString;
        private int studentId;
        private StudentReportPanel studentReportPanel;
        public StudentPanel(int studentId)
        {
            InitializeComponent();
            this.studentId = studentId;
            UpdateWelcomeText();
        }

        private void btnAddResult_Click(object sender, EventArgs e)
        {
            bool validation = ResultValidation();
            bool examNameCheck = ExistingExamName();
            bool existResult = checkExistResult();

            if (examNameCheck == false)
            {
                MessageBox.Show("Test name already exists. Try different name.");
            }

            if (existResult == false)
            {
                MessageBox.Show("Result already exists.");
            }

            if (validation == false || examNameCheck == false || existResult == false)
            {
                return;
            }
            try
            {
                AddTest();
                AddResult(textBoxExamName.Text, "Mathematics", Convert.ToInt32(textBoxTrueMath.Text),
                    Convert.ToInt32(textBoxFalseMath.Text), Convert.ToInt32(textBoxBlankMath.Text));
                AddResult(textBoxExamName.Text, "Science", Convert.ToInt32(textBoxTrueScience.Text),
                    Convert.ToInt32(textBoxFalseScience.Text), Convert.ToInt32(textBoxBlankScience.Text));
                AddResult(textBoxExamName.Text, "Turkish", Convert.ToInt32(textBoxTrueTurkish.Text),
                    Convert.ToInt32(textBoxFalseTurkish.Text), Convert.ToInt32(textBoxBlankTurkish.Text));
                AddResult(textBoxExamName.Text, "History", Convert.ToInt32(textBoxTrueHistory.Text),
                    Convert.ToInt32(textBoxFalseHistory.Text), Convert.ToInt32(textBoxBlankHistory.Text));
                AddResult(textBoxExamName.Text, "Religion", Convert.ToInt32(textBoxTrueReligion.Text),
                    Convert.ToInt32(textBoxFalseReligion.Text), Convert.ToInt32(textBoxBlankReligion.Text));
                AddResult(textBoxExamName.Text, "English", Convert.ToInt32(textBoxTrueEnglish.Text),
                    Convert.ToInt32(textBoxFalseEnglish.Text), Convert.ToInt32(textBoxBlankEnglish.Text));
                MessageBox.Show("Result added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch
            {
                MessageBox.Show("Something went wrong.");
            }
        }
        private void btnShowResults_Click(object sender, EventArgs e)
        {
            ShowResult();
        }
        private void btnRemoveResult_Click(object sender, EventArgs e)
        {
            bool checkExamName = ExistingExamName();
            bool checkResult = checkExistResult();

            if (checkExamName)
            {
                MessageBox.Show("Exam does not exist.");
            }
            if (checkResult)
            {
                MessageBox.Show("Result does not exist.");
            }

            string examType = GetExamType(textBoxExamName.Text);
            if (examType == "Official")
            {
                MessageBox.Show("Results for official exams cannot be deleted.", "Deletion Restricted", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (checkExamName || checkResult)
            {
                return;
            }
            DeleteResult();
        }
        private void btnTestHistory_Click(object sender, EventArgs e)
        {
            ShowTestHistory();
        }
        private void ShowTestHistory()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = @"
                SELECT 
                    (SELECT EXAM_NAME FROM EXAMS WHERE EXAMS.EXAM_ID = RESULTS.EXAM_ID) AS [Exam Name], 
                    (SELECT EXAM_TYPE FROM EXAMS WHERE EXAMS.EXAM_ID = RESULTS.EXAM_ID) AS [Exam Type],
                    SUM(TRUE_COUNT) AS [True], 
                    SUM(FALSE_COUNT) AS [False], 
                    SUM(BLANK_COUNT) AS [Blank]
                FROM RESULTS
                WHERE STUDENT_ID = @studentId
                GROUP BY EXAM_ID";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@studentId", studentId);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    dataGridViewResults.DataSource = dataTable;
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Error retrieving test history: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void AddTest()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "INSERT INTO EXAMS (EXAM_NAME, EXAM_TYPE) VALUES (@examName, @examType)";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@examName", textBoxExamName.Text);
                    command.Parameters.AddWithValue("@examType", "Unoffical");
                    command.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Error adding test: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void AddResult(string examName, string subjectName, int trueCount, int falseCount, int blankCount)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string getExamIdQuery = "SELECT EXAM_ID FROM EXAMS WHERE EXAM_NAME = @examName";
                    SqlCommand getExamIdCommand = new SqlCommand(getExamIdQuery, connection);
                    getExamIdCommand.Parameters.AddWithValue("@examName", examName);
                    object examIdObj = getExamIdCommand.ExecuteScalar();

                    if (examIdObj == null)
                    {
                        MessageBox.Show("Exam not found. Please ensure the exam name is correct.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    int examId = Convert.ToInt32(examIdObj);

                    string addResultQuery = "INSERT INTO RESULTS (STUDENT_ID, EXAM_ID, SUBJECT_NAME, TRUE_COUNT, FALSE_COUNT, BLANK_COUNT) " +
                                            "VALUES (@studentId, @examId, @subjectName, @trueCount, @falseCount, @blankCount)";
                    SqlCommand addResultCommand = new SqlCommand(addResultQuery, connection);
                    addResultCommand.Parameters.AddWithValue("@studentId", studentId);
                    addResultCommand.Parameters.AddWithValue("@examId", examId);
                    addResultCommand.Parameters.AddWithValue("@subjectName", subjectName);
                    addResultCommand.Parameters.AddWithValue("@trueCount", trueCount);
                    addResultCommand.Parameters.AddWithValue("@falseCount", falseCount);
                    addResultCommand.Parameters.AddWithValue("@blankCount", blankCount);

                    addResultCommand.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Error adding result: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void ShowResult()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string showResultQuery = @"
                SELECT                     
                    (SELECT EXAM_NAME FROM EXAMS WHERE EXAMS.EXAM_ID = RESULTS.EXAM_ID) AS [Exam Name], 
                    SUBJECT_NAME AS [Subject Name], 
                    TRUE_COUNT AS [True Count], 
                    FALSE_COUNT AS [False Count], 
                    BLANK_COUNT AS [Blank Count]
                FROM RESULTS
                WHERE STUDENT_ID = @studentID 
                AND EXAM_ID = (SELECT EXAM_ID FROM EXAMS WHERE EXAM_NAME = @examName)";

                    SqlCommand showResultCommand = new SqlCommand(showResultQuery, connection);
                    showResultCommand.Parameters.AddWithValue("@studentID", studentId);
                    showResultCommand.Parameters.AddWithValue("@examName", textBoxExamName.Text);

                    SqlDataAdapter adapter = new SqlDataAdapter(showResultCommand);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    dataGridViewResults.DataSource = dataTable;
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Error showing results: " + ex.Message);
                }
            }
        }
        private bool ResultValidation()
        {
            var subjectLimits = new Dictionary<string, int>
            {
                { "Mathematics", 20 },
                { "Science", 20 },
                { "Turkish", 20 },
                { "History", 10 },
                { "Religion", 10 },
                { "English", 10 }
            };

            var subjectTextBoxes = new Dictionary<string, (TextBox True, TextBox False, TextBox Blank)>
            {
                { "Mathematics", (textBoxTrueMath, textBoxFalseMath, textBoxBlankMath) },
                { "Science", (textBoxTrueScience, textBoxFalseScience, textBoxBlankScience) },
                { "Turkish", (textBoxTrueTurkish, textBoxFalseTurkish, textBoxBlankTurkish) },
                { "History", (textBoxTrueHistory, textBoxFalseHistory, textBoxBlankHistory) },
                { "Religion", (textBoxTrueReligion, textBoxFalseReligion, textBoxBlankReligion) },
                { "English", (textBoxTrueEnglish, textBoxFalseEnglish, textBoxBlankEnglish) }
            };

            foreach (var subject in subjectTextBoxes)
            {
                var (trueBox, falseBox, blankBox) = subject.Value;

                if (!int.TryParse(trueBox.Text, out int trueCount) ||
                    !int.TryParse(falseBox.Text, out int falseCount) ||
                    !int.TryParse(blankBox.Text, out int blankCount))
                {
                    MessageBox.Show($"All fields for {subject.Key} must be filled with valid whole numbers.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    trueBox.Focus();
                    return false;
                }

                int total = trueCount + falseCount + blankCount;
                if (total > subjectLimits[subject.Key])
                {
                    MessageBox.Show($"The total number of answers for {subject.Key} cannot exceed {subjectLimits[subject.Key]}.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    trueBox.Focus();
                    return false;
                }

                if (total != subjectLimits[subject.Key] && total != 0)
                {
                    MessageBox.Show($"The total number of answers for {subject.Key} must exactly match {subjectLimits[subject.Key]} if all questions are answered.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    trueBox.Focus();
                    return false;
                }
            }
            return true;
        }
        private bool checkExistResult()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string checkResultQuery = "SELECT COUNT(*) FROM RESULTS WHERE STUDENT_ID = @studentId AND EXAM_ID = (SELECT EXAM_ID FROM EXAMS WHERE EXAM_NAME = @examName)";
                    SqlCommand checkResultCommand = new SqlCommand(checkResultQuery, connection);
                    checkResultCommand.Parameters.AddWithValue("@studentId", studentId);
                    checkResultCommand.Parameters.AddWithValue("@examName", textBoxExamName.Text);
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
        private void DeleteResult()
        {
            var result = MessageBox.Show("Are you sure you want to delete this result?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.No)
            {
                return;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string deleteResultQuery = "DELETE FROM RESULTS WHERE STUDENT_ID = @studentId AND EXAM_ID = (SELECT EXAM_ID FROM EXAMS WHERE EXAM_NAME = @examName)";
                    SqlCommand deleteResultCommand = new SqlCommand(deleteResultQuery, connection);
                    deleteResultCommand.Parameters.AddWithValue("@studentId", studentId);
                    deleteResultCommand.Parameters.AddWithValue("@examName", textBoxExamName.Text);
                    deleteResultCommand.ExecuteNonQuery();

                    MessageBox.Show("Result deleted successfully.");
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Error deleting result: " + ex.Message);
                }
            }
        }
        private bool ExistingExamName()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string checkExamNameQuery = "SELECT COUNT(*) FROM EXAMS WHERE EXAM_NAME = @examName";
                    SqlCommand checkExamNameCommand = new SqlCommand(checkExamNameQuery, connection);
                    checkExamNameCommand.Parameters.AddWithValue("@examName", textBoxExamName.Text);
                    int examCount = (int)checkExamNameCommand.ExecuteScalar();
                    if (examCount > 0)
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
                    MessageBox.Show("Error checking exam name: " + ex.Message);
                    return false;
                }
            }
        }
        private string GetExamType(string examName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT EXAM_TYPE FROM EXAMS WHERE EXAM_NAME = @examName";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@examName", examName);
                    object result = command.ExecuteScalar();

                    if (result != null)
                    {
                        return result.ToString();
                    }
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Error retrieving exam type: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            return string.Empty;
        }
        private void UpdateWelcomeText()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string getNameQuery = "SELECT NAME FROM STUDENTS WHERE STUDENT_ID = @studentId";
                    SqlCommand updateWelcomeTextCommand = new SqlCommand(getNameQuery, connection);
                    updateWelcomeTextCommand.Parameters.AddWithValue("@studentId", studentId);
                    string studentName = (string)updateWelcomeTextCommand.ExecuteScalar();
                    string welcomeText = $"Welcome, {studentName}!";
                    labelWelcome.Text = welcomeText;
                    labelWelcome.Location = new Point(this.ClientSize.Width - labelWelcome.Width - 10, 10);
                }
                catch (SqlException ex)
                {
                    labelWelcome.Text = "Welcome, Student!";
                    return;
                }
            }
        }
        private void btnClear_Click(object sender, EventArgs e)
        {
            textBoxExamName.Clear();
            textBoxTrueMath.Clear();
            textBoxFalseMath.Clear();
            textBoxBlankMath.Clear();
            textBoxTrueScience.Clear();
            textBoxFalseScience.Clear();
            textBoxBlankScience.Clear();
            textBoxTrueTurkish.Clear();
            textBoxFalseTurkish.Clear();
            textBoxBlankTurkish.Clear();
            textBoxTrueHistory.Clear();
            textBoxFalseHistory.Clear();
            textBoxBlankHistory.Clear();
            textBoxTrueReligion.Clear();
            textBoxFalseReligion.Clear();
            textBoxBlankReligion.Clear();
            textBoxTrueEnglish.Clear();
            textBoxFalseEnglish.Clear();
            textBoxBlankEnglish.Clear();
            dataGridViewResults.DataSource = null;
        }
        private void btnShowPanel_Click(object sender, EventArgs e)
        {
            if (studentReportPanel == null || studentReportPanel.IsDisposed)
            {
                studentReportPanel = new StudentReportPanel(studentId);
                studentReportPanel.Location = new Point(this.Location.X + this.Width, this.Location.Y);
                studentReportPanel.Show();
                this.LocationChanged += StudentPanel_ReportPanel_LocationChanged;
                this.FormClosed += StudentPanel_ReportPanel_FormClosed;
            }
            else
            {
                studentReportPanel.BringToFront();
            }
        }
        private void StudentPanel_ReportPanel_LocationChanged(object sender, EventArgs e)
        {
            if (studentReportPanel != null && !studentReportPanel.IsDisposed)
            {
                studentReportPanel.Location = new Point(this.Location.X + this.Width, this.Location.Y);
            }
        }
        private void StudentPanel_ReportPanel_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (studentReportPanel != null && !studentReportPanel.IsDisposed)
            {
                studentReportPanel.Close();
            }
        }
    }
}

