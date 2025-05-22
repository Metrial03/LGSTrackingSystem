using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Configuration;

namespace LGSTrackingSystem
{
    public partial class AdminPanel : Form
    {
        string connectionString = ConfigurationManager.ConnectionStrings["LGSDB"].ConnectionString;
        private List<string> allStudentNames = new List<string>();
        private List<string> allExamNames = new List<string>();
        private AdminReportPanel adminReportPanel;
        private ExamListPanel examListPanel;
        public AdminPanel()
        {
            InitializeComponent();
            CoboxListExam();
            CoboxListStudent();
            textBoxTestStudentName.TextChanged += textBoxTestStudentName_TextChanged;
            textBoxExamName.TextChanged += textBoxExamName_TextChanged;

        }
        #region Student Management
        string gender => radioMan.Checked == false && radioWoman.Checked == false ? "" : (radioWoman.Checked ? "Woman" : "Man");
        private void btnAddStudent_Click(object sender, EventArgs e)
        {
            int studentId = getStudentIdFromName(textBoxTestStudentName.Text);
            bool checkStudentID = CheckStudentID(textBoxStudentNumber.Text);
            bool checkUsername = CheckUsername(false);

            if (checkStudentID)
            {
                MessageBox.Show("Student Number already exists.");
            }
            else if (checkUsername)
            {
                MessageBox.Show("Username already exists.");
            }

            if (Validation() == false || checkUsername || checkStudentID)
            {
                return;
            }
            AddStudent();
            CoboxListStudent();
        }
        private void btnUpdateStudent_Click(object sender, EventArgs e)
        {
            int studentId = getStudentIdFromName(textBoxTestStudentName.Text);
            bool checkUsername = CheckUsername(true);
            bool checkStudentID = CheckStudentID(textBoxStudentNumber.Text);
            if (checkStudentID == false)
            {
                MessageBox.Show("Student Number does not exist.");
            }
            else if (checkUsername)
            {
                MessageBox.Show("Username already exists.");
            }
            if (checkStudentID == false || checkUsername)
            {
                return;
            }
            UpdateStudent();
            CoboxListStudent();
        }
        private void btnDeleteStudent_Click(object sender, EventArgs e)
        {
            bool checkStudentID = CheckStudentID(textBoxStudentNumber.Text);
            if (checkStudentID == false)
            {
                MessageBox.Show("Student number does not exist.");
                return;
            }
            DeleteStudent();
            CoboxListStudent();
        }
        private void btnListStudent_Click(object sender, EventArgs e)
        {
            ListStudent();
        }
        private void AddStudent()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string addUserQuery = "INSERT INTO Users (USERNAME, PASSWORD, USERTYPE) VALUES (@username, @password, 'Student'); SELECT SCOPE_IDENTITY();";
                    SqlCommand addUserCommand = new SqlCommand(addUserQuery, connection);
                    addUserCommand.Parameters.AddWithValue("@username", textBoxUsername.Text);
                    addUserCommand.Parameters.AddWithValue("@password", textBoxPassword.Text);
                    int userId = Convert.ToInt32(addUserCommand.ExecuteScalar());

                    string addStudentQuery = "INSERT INTO Students (STUDENT_NUMBER, NAME, SURNAME, GENDER, EMAIL, ADDRESS, USER_ID) VALUES (@studentNumber, @name, @surname, @gender, @email, @address, @userId)";
                    SqlCommand addStudentCommand = new SqlCommand(addStudentQuery, connection);
                    addStudentCommand.Parameters.AddWithValue("@studentNumber", textBoxStudentNumber.Text);
                    addStudentCommand.Parameters.AddWithValue("@name", textBoxName.Text);
                    addStudentCommand.Parameters.AddWithValue("@surname", textBoxSurname.Text);
                    addStudentCommand.Parameters.AddWithValue("@email", textBoxEmail.Text);
                    addStudentCommand.Parameters.AddWithValue("@address", textBoxAdress.Text);
                    addStudentCommand.Parameters.AddWithValue("@gender", gender);
                    addStudentCommand.Parameters.AddWithValue("@userId", userId);
                    addStudentCommand.ExecuteNonQuery();
                    MessageBox.Show("Student added successfully.");
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Error adding user: " + ex.Message);
                    return;
                }
            }
        }
        private void UpdateStudent()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Build dynamic update for Users table
                    List<string> userUpdates = new List<string>();
                    SqlCommand updateUserCommand = new SqlCommand();
                    updateUserCommand.Connection = connection;

                    if (!string.IsNullOrWhiteSpace(textBoxUsername.Text))
                    {
                        userUpdates.Add("USERNAME = @username");
                        updateUserCommand.Parameters.AddWithValue("@username", textBoxUsername.Text);
                    }
                    if (!string.IsNullOrWhiteSpace(textBoxPassword.Text))
                    {
                        userUpdates.Add("PASSWORD = @password");
                        updateUserCommand.Parameters.AddWithValue("@password", textBoxPassword.Text);
                    }

                    if (userUpdates.Count > 0)
                    {
                        string updateUserQuery = $"UPDATE Users SET {string.Join(", ", userUpdates)} WHERE ID = (SELECT USER_ID FROM Students WHERE STUDENT_NUMBER = @studentNumber)";
                        updateUserCommand.CommandText = updateUserQuery;
                        updateUserCommand.Parameters.AddWithValue("@studentNumber", textBoxStudentNumber.Text);
                        updateUserCommand.ExecuteNonQuery();
                    }

                    // Build dynamic update for Students table
                    List<string> studentUpdates = new List<string>();
                    SqlCommand updateStudentCommand = new SqlCommand();
                    updateStudentCommand.Connection = connection;

                    if (!string.IsNullOrWhiteSpace(textBoxName.Text))
                    {
                        studentUpdates.Add("NAME = @name");
                        updateStudentCommand.Parameters.AddWithValue("@name", textBoxName.Text);
                    }
                    if (!string.IsNullOrWhiteSpace(textBoxSurname.Text))
                    {
                        studentUpdates.Add("SURNAME = @surname");
                        updateStudentCommand.Parameters.AddWithValue("@surname", textBoxSurname.Text);
                    }
                    if (!string.IsNullOrWhiteSpace(textBoxEmail.Text))
                    {
                        studentUpdates.Add("EMAIL = @email");
                        updateStudentCommand.Parameters.AddWithValue("@email", textBoxEmail.Text);
                    }
                    if (!string.IsNullOrWhiteSpace(textBoxAdress.Text))
                    {
                        studentUpdates.Add("ADDRESS = @address");
                        updateStudentCommand.Parameters.AddWithValue("@address", textBoxAdress.Text);
                    }
                    if (radioMan.Checked || radioWoman.Checked)
                    {
                        studentUpdates.Add("GENDER = @gender");
                        updateStudentCommand.Parameters.AddWithValue("@gender", gender);
                    }

                    if (studentUpdates.Count > 0)
                    {
                        string updateStudentQuery = $"UPDATE Students SET {string.Join(", ", studentUpdates)} WHERE STUDENT_NUMBER = @studentNumber";
                        updateStudentCommand.CommandText = updateStudentQuery;
                        updateStudentCommand.Parameters.AddWithValue("@studentNumber", textBoxStudentNumber.Text);
                        updateStudentCommand.ExecuteNonQuery();
                    }

                    MessageBox.Show("Student updated successfully.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error updating user: " + ex.Message);
                }
            }
        }
        private void DeleteStudent()
        {
            var result = MessageBox.Show("Are you sure you want to delete this student and their associated results?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.No)
            {
                return;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string getIdsQuery = "SELECT STUDENT_ID, USER_ID FROM STUDENTS WHERE STUDENT_NUMBER = @studentNumber";
                    SqlCommand getIdsCommand = new SqlCommand(getIdsQuery, connection);
                    getIdsCommand.Parameters.AddWithValue("@studentNumber", textBoxStudentNumber.Text);
                    using (SqlDataReader reader = getIdsCommand.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            MessageBox.Show("Student not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        int studentId = reader.GetInt32(0);
                        int userId = reader.GetInt32(1);

                        reader.Close();

                        string deleteResultsQuery = "DELETE FROM RESULTS WHERE STUDENT_ID = @studentId";
                        SqlCommand deleteResultsCommand = new SqlCommand(deleteResultsQuery, connection);
                        deleteResultsCommand.Parameters.AddWithValue("@studentId", studentId);

                        string deleteStudentQuery = "DELETE FROM STUDENTS WHERE STUDENT_ID = @studentId";
                        SqlCommand deleteStudentCommand = new SqlCommand(deleteStudentQuery, connection);
                        deleteStudentCommand.Parameters.AddWithValue("@studentId", studentId);
                        deleteStudentCommand.ExecuteNonQuery();

                        string deleteUserQuery = "DELETE FROM USERS WHERE ID = @userId";
                        SqlCommand deleteUserCommand = new SqlCommand(deleteUserQuery, connection);
                        deleteUserCommand.Parameters.AddWithValue("@userId", userId);
                        deleteUserCommand.ExecuteNonQuery();

                        MessageBox.Show("Student and their associated results deleted successfully.");
                    }
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Error deleting student: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void ListStudent()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string listStudentQuery = @"
                SELECT 
                    S.STUDENT_NUMBER AS [Student Number], 
                    S.NAME AS [Name], 
                    S.SURNAME AS [Surname], 
                    S.GENDER AS [Gender], 
                    S.EMAIL AS [Email], 
                    S.ADDRESS AS [Address],
                    U.USERNAME AS [Username],
                    U.PASSWORD AS [Password]
                FROM Students S
                INNER JOIN Users U ON S.USER_ID = U.ID
                WHERE 1=1";

                    if (!string.IsNullOrEmpty(textBoxStudentNumber.Text))
                    {
                        listStudentQuery += " AND S.STUDENT_NUMBER = @studentNumber";
                    }
                    if (!string.IsNullOrEmpty(textBoxName.Text))
                    {
                        listStudentQuery += " AND S.NAME LIKE @name";
                    }
                    if (!string.IsNullOrEmpty(textBoxSurname.Text))
                    {
                        listStudentQuery += " AND S.SURNAME LIKE @surname";
                    }
                    if (!string.IsNullOrEmpty(textBoxEmail.Text))
                    {
                        listStudentQuery += " AND S.EMAIL LIKE @email";
                    }
                    if (!string.IsNullOrEmpty(gender))
                    {
                        listStudentQuery += " AND S.GENDER = @gender";
                    }
                    if (!string.IsNullOrEmpty(textBoxAdress.Text))
                    {
                        listStudentQuery += " AND S.ADDRESS LIKE @address";
                    }

                    SqlCommand listStudentCommand = new SqlCommand(listStudentQuery, connection);
                    if (!string.IsNullOrEmpty(textBoxStudentNumber.Text))
                    {
                        if (int.TryParse(textBoxStudentNumber.Text, out int studentID))
                        {
                            listStudentCommand.Parameters.AddWithValue("@studentNumber", studentID);
                        }
                        else
                        {
                            MessageBox.Show("Student ID must be a valid number.");
                            return;
                        }
                    }
                    if (!string.IsNullOrEmpty(textBoxName.Text))
                    {
                        listStudentCommand.Parameters.AddWithValue("@name", "%" + textBoxName.Text + "%");
                    }
                    if (!string.IsNullOrEmpty(textBoxSurname.Text))
                    {
                        listStudentCommand.Parameters.AddWithValue("@surname", "%" + textBoxSurname.Text + "%");
                    }
                    if (!string.IsNullOrEmpty(textBoxEmail.Text))
                    {
                        listStudentCommand.Parameters.AddWithValue("@email", "%" + textBoxEmail.Text + "%");
                    }
                    if (!string.IsNullOrEmpty(gender))
                    {
                        listStudentCommand.Parameters.AddWithValue("@gender", gender);
                    }
                    if (!string.IsNullOrEmpty(textBoxAdress.Text))
                    {
                        listStudentCommand.Parameters.AddWithValue("@address", "%" + textBoxAdress.Text + "%");
                    }

                    SqlDataAdapter adapter = new SqlDataAdapter(listStudentCommand);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    dataGridViewStudents.DataSource = dataTable;
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Error listing students: " + ex.Message);
                }
            }
        }
        private bool Validation()
        {
            if (string.IsNullOrEmpty(textBoxStudentNumber.Text) || string.IsNullOrEmpty(textBoxName.Text) || string.IsNullOrEmpty(textBoxSurname.Text) || string.IsNullOrEmpty(textBoxEmail.Text) ||
                string.IsNullOrEmpty(textBoxAdress.Text) || string.IsNullOrEmpty(textBoxUsername.Text) || string.IsNullOrEmpty(textBoxPassword.Text) || radioWoman.Checked == false && radioMan.Checked == false)
            {
                MessageBox.Show("Please fill in all fields.");
                return false;
            }
            else if (int.TryParse(textBoxStudentNumber.Text, out int studentID) == false)
            {
                MessageBox.Show("Student ID must be a number.");
                return false;
            }
            else if (textBoxUsername.Text.Length < 5)
            {
                MessageBox.Show("Username must be at least 5 characters long.");
                return false;
            }
            else if (textBoxPassword.Text.Length < 8)
            {
                MessageBox.Show("Password must be at least 8 characters long.");
                return false;
            }
            else if (textBoxPassword.Text.Any(char.IsDigit) == false)
            {
                MessageBox.Show("Password must contain at least one digit.");
                return false;
            }
            else if (textBoxPassword.Text.Any(char.IsLetter) == false)
            {
                MessageBox.Show("Password must contain at least one letter.");
                return false;
            }
            else if (!ValidateEmail())
            {
                return false;
            }

            return true;
        }
        private bool ValidateEmail()
        {
            var validDomains = new List<string> { "@gmail.com", "@yahoo.com", "@outlook.com" };

            if (!IsValidEmailDomain(textBoxEmail.Text, validDomains))
            {
                MessageBox.Show("Email must end with a valid domain (e.g., @gmail.com, @yahoo.com, @outlook.com).", "Invalid Email", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxEmail.Focus();
                return false;
            }

            return true;
        }
        private bool IsValidEmailDomain(string email, List<string> validDomains)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            foreach (var domain in validDomains)
            {
                if (email.EndsWith(domain, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
        #endregion
        #region management panel
        private void btnAddExam_Click(object sender, EventArgs e)
        {
            if (ExistingExamName(textBoxMngExamName.Text) == false)
            {
                MessageBox.Show("Exam name already exists.");
                return;
            }
            AddExam();
            CoboxListExam();
            if (examListPanel != null && !examListPanel.IsDisposed)
            {
                examListPanel.Invoke(new Action(() => examListPanel.GetType().GetMethod("listExams", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke(examListPanel, null)));
            }
        }
        private void btnDeleteExam_Click(object sender, EventArgs e)
        {
            bool checkExamName = ExistingExamName(textBoxMngExamName.Text);
            if (checkExamName)
            {
                MessageBox.Show("Exam does not exist.");
                return;
            }
            DeleteExam();
            CoboxListExam();
            if (examListPanel != null && !examListPanel.IsDisposed)
            {
                examListPanel.Invoke(new Action(() => examListPanel.GetType().GetMethod("listExams", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke(examListPanel, null)));
            }
        }
        private void btnAddResult_Click(object sender, EventArgs e)
        {
            string examName = cbExamList.Text;
            string studentName = cbStudentList.Text;
            bool validation = ResultValidation();
            bool existResult = checkExistResult(studentName, examName);
            int studentId = getStudentIdFromName(studentName);
            int examId = getExamId();
            if (cbExamList.SelectedIndex == -1 || cbStudentList.SelectedIndex == -1)
            {
                MessageBox.Show("Please select an exam and a student.");
                return;
            }
            if (existResult == false)
            {
                MessageBox.Show("Result already exists.");
                return;
            }
            if (validation == false)
            {
                return;
            }
            if (studentId == -1 || examId == -1)
            {
                MessageBox.Show("Student or exam not found.");
                return;
            }
            try
            {
                AddResult(studentId, examId, "Mathematics", Convert.ToInt32(textBoxTrueMath.Text),
                    Convert.ToInt32(textBoxFalseMath.Text), Convert.ToInt32(textBoxBlankMath.Text));
                AddResult(studentId, examId, "Science", Convert.ToInt32(textBoxTrueScience.Text),
                    Convert.ToInt32(textBoxFalseScience.Text), Convert.ToInt32(textBoxBlankScience.Text));
                AddResult(studentId, examId, "Turkish", Convert.ToInt32(textBoxTrueTurkish.Text),
                    Convert.ToInt32(textBoxFalseTurkish.Text), Convert.ToInt32(textBoxBlankTurkish.Text));
                AddResult(studentId, examId, "History", Convert.ToInt32(textBoxTrueHistory.Text),
                    Convert.ToInt32(textBoxFalseHistory.Text), Convert.ToInt32(textBoxBlankHistory.Text));
                AddResult(studentId, examId, "Religion", Convert.ToInt32(textBoxTrueReligion.Text),
                    Convert.ToInt32(textBoxFalseReligion.Text), Convert.ToInt32(textBoxBlankReligion.Text));
                AddResult(studentId, examId, "English", Convert.ToInt32(textBoxTrueEnglish.Text),
                    Convert.ToInt32(textBoxFalseEnglish.Text), Convert.ToInt32(textBoxBlankEnglish.Text));
                MessageBox.Show("Result added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch
            {
                MessageBox.Show("Something went wrong.");
            }

        }
        private void btnRemoveResult_Click(object sender, EventArgs e)
        {
            string studentName = cbStudentList.Text;
            int studentId = getStudentIdFromName(studentName);
            bool checkResult = checkExistResult(cbStudentList.Text, cbExamList.Text);
            if (cbExamList.SelectedIndex == -1 || cbStudentList.SelectedIndex == -1)
            {
                MessageBox.Show("Please select an exam and a student.");
                return;
            }
            if (checkResult)
            {
                MessageBox.Show("Result does not exist.");
                return;
            }
            DeleteResult();
        }
        private void btnShowResults_Click(object sender, EventArgs e)
        {
            ShowResult();
        }
        private void btnTestHistory_Click(object sender, EventArgs e)
        {
            ShowTestHistory();
        }
        private void btnClear_Click(object sender, EventArgs e)
        {
            clearTextBoxesManagement();
        }
        #endregion
        private bool CheckUsername(bool isUpdate = false)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string checkUserQuery;

                    if (isUpdate)
                    {
                        checkUserQuery = @"
                    SELECT COUNT(*) 
                    FROM USERS 
                    WHERE USERNAME = @username 
                    AND ID != (SELECT USER_ID FROM Students WHERE STUDENT_NUMBER = @studentNumber)";
                    }
                    else
                    {
                        checkUserQuery = "SELECT COUNT(*) FROM USERS WHERE USERNAME = @username";
                    }

                    SqlCommand checkUserCommand = new SqlCommand(checkUserQuery, connection);
                    checkUserCommand.Parameters.AddWithValue("@username", textBoxUsername.Text);

                    if (isUpdate)
                    {
                        checkUserCommand.Parameters.AddWithValue("@studentNumber", textBoxStudentNumber.Text);
                    }

                    int userCount = (int)checkUserCommand.ExecuteScalar();
                    return userCount > 0;
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Error checking username: " + ex.Message);
                    return false;
                }
            }
        }
        private bool CheckStudentID(string studentNumber)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string checkIDQuery = "SELECT COUNT(*) FROM STUDENTS WHERE STUDENT_NUMBER = @studentNumber";
                    SqlCommand checkIDCommand = new SqlCommand(checkIDQuery, connection);
                    checkIDCommand.Parameters.AddWithValue("@studentNumber", studentNumber);
                    int idCount = (int)checkIDCommand.ExecuteScalar();
                    if (idCount > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Error checking Student ID: " + ex.Message);
                    return false;
                }
            }
        }
        private bool ExistingExamName(string textBoxName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string checkExamNameQuery = "SELECT COUNT(*) FROM EXAMS WHERE EXAM_NAME = @examName";
                    SqlCommand checkExamNameCommand = new SqlCommand(checkExamNameQuery, connection);
                    checkExamNameCommand.Parameters.AddWithValue("@examName", textBoxName);
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
        private void AddExam()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string addExamQuery = "INSERT INTO EXAMS (EXAM_NAME, EXAM_TYPE) VALUES (@examName , @examType)";
                    SqlCommand addExamCommand = new SqlCommand(addExamQuery, connection);
                    addExamCommand.Parameters.AddWithValue("@examName", textBoxMngExamName.Text);
                    addExamCommand.Parameters.AddWithValue("@examType", "Official");
                    addExamCommand.ExecuteNonQuery();
                    MessageBox.Show("Exam added successfully.");

                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Error adding exam: " + ex.Message);
                    return;
                }
            }
        }
        private void DeleteExam()
        {
            var result = MessageBox.Show("Are you sure you want to delete this exam?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.No)
            {
                return;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string deleteResultsQuery = "DELETE FROM RESULTS WHERE EXAM_ID = (SELECT EXAM_ID FROM EXAMS WHERE EXAM_NAME = @examName)";
                    SqlCommand deleteResultsCommand = new SqlCommand(deleteResultsQuery, connection);
                    deleteResultsCommand.Parameters.AddWithValue("@examName", textBoxMngExamName.Text);
                    deleteResultsCommand.ExecuteNonQuery();

                    string deleteExamQuery = "DELETE FROM EXAMS WHERE EXAM_NAME = @examName";
                    SqlCommand deleteExamCommand = new SqlCommand(deleteExamQuery, connection);
                    deleteExamCommand.Parameters.AddWithValue("@examName", textBoxMngExamName.Text);
                    deleteExamCommand.ExecuteNonQuery();

                    MessageBox.Show("Exam deleted successfully.");
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Error deleting exam: " + ex.Message);
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
                    string query = @"
                SELECT 
                    (S.NAME + ' ' + S.SURNAME) AS [Student], 
                    E.EXAM_NAME AS [Exam Name], 
                    R.SUBJECT_NAME AS [Subject Name], 
                    R.TRUE_COUNT AS [True Count], 
                    R.FALSE_COUNT AS [False Count], 
                    R.BLANK_COUNT AS [Blank Count]
                FROM RESULTS R
                INNER JOIN STUDENTS S ON R.STUDENT_ID = S.STUDENT_ID
                INNER JOIN EXAMS E ON R.EXAM_ID = E.EXAM_ID
                WHERE (S.NAME + ' ' + S.SURNAME) = @studentName
                  AND E.EXAM_NAME = @examName";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@studentName", cbStudentList.Text);
                    command.Parameters.AddWithValue("@examName", cbExamList.Text);

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
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
        private void AddResult(int studentId, int examId, string subjectName, int trueCount, int falseCount, int blankCount)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
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
                    int studentId = getStudentIdFromName(cbStudentList.Text);
                    string deleteResultQuery = "DELETE FROM RESULTS WHERE STUDENT_ID = @studentId AND EXAM_ID = (SELECT EXAM_ID FROM EXAMS WHERE EXAM_NAME = @examName)";
                    SqlCommand deleteResultCommand = new SqlCommand(deleteResultQuery, connection);
                    deleteResultCommand.Parameters.AddWithValue("@studentId", studentId);
                    deleteResultCommand.Parameters.AddWithValue("@examName", cbExamList.Text);
                    deleteResultCommand.ExecuteNonQuery();

                    MessageBox.Show("Result deleted successfully.");
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Error deleting result: " + ex.Message);
                }
            }
        }
        private void ShowTestHistory()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string selectedStudent = cbStudentList.SelectedIndex != -1 ? cbStudentList.Text : null;
                    string selectedExam = cbExamList.SelectedIndex != -1 ? cbExamList.Text : null;

                    string query = @"
                SELECT 
                    (S.NAME + ' ' + S.SURNAME) AS [Student], 
                    E.EXAM_NAME AS [Exam Name], 
                    E.EXAM_TYPE AS [Exam Type],
                    SUM(R.TRUE_COUNT) AS [True], 
                    SUM(R.FALSE_COUNT) AS [False], 
                    SUM(R.BLANK_COUNT) AS [Blank]
                FROM RESULTS R
                INNER JOIN STUDENTS S ON R.STUDENT_ID = S.STUDENT_ID
                INNER JOIN EXAMS E ON R.EXAM_ID = E.EXAM_ID
                WHERE 1=1
            ";

                    if (!string.IsNullOrEmpty(selectedStudent))
                    {
                        query += " AND (S.NAME + ' ' + S.SURNAME) = @studentName";
                    }
                    if (!string.IsNullOrEmpty(selectedExam))
                    {
                        query += " AND E.EXAM_NAME = @examName";
                    }

                    query += @"
                GROUP BY S.NAME, S.SURNAME, E.EXAM_NAME, E.EXAM_TYPE
            ";

                    SqlCommand command = new SqlCommand(query, connection);

                    if (!string.IsNullOrEmpty(selectedStudent))
                    {
                        command.Parameters.AddWithValue("@studentName", selectedStudent);
                    }
                    if (!string.IsNullOrEmpty(selectedExam))
                    {
                        command.Parameters.AddWithValue("@examName", selectedExam);
                    }

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
        private void clearTextBoxesManagement()
        {
            textBoxMngExamName.Clear();
            textBoxTestStudentName.Clear();
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
            cbStudentList.SelectedIndex = -1;
            cbExamList.SelectedIndex = -1;
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
        #region Filtering listing
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

        #endregion

        #region Panels
        private void btnOpenExamPanel_Click(object sender, EventArgs e)
        {
            if (examListPanel == null || examListPanel.IsDisposed)
            {
                examListPanel = new ExamListPanel();
                examListPanel.Location = new Point(this.Location.X - examListPanel.Width, this.Location.Y);
                examListPanel.Show();
                this.LocationChanged += AdminPanel_LocationChanged;
                this.FormClosed += AdminPanel_FormClosed;
            }
            else
            {
                examListPanel.BringToFront();
            }
        }
        private void AdminPanel_LocationChanged(object sender, EventArgs e)
        {
            if (examListPanel != null && !examListPanel.IsDisposed)
            {
                examListPanel.Location = new Point(this.Location.X - examListPanel.Width, this.Location.Y);
            }
        }
        private void AdminPanel_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (examListPanel != null && !examListPanel.IsDisposed)
            {
                examListPanel.Close();
            }
        }
        private void btnClearStudent_Click(object sender, EventArgs e)
        {
            textBoxStudentNumber.Clear();
            textBoxName.Clear();
            textBoxSurname.Clear();
            textBoxEmail.Clear();
            textBoxAdress.Clear();
            textBoxUsername.Clear();
            textBoxPassword.Clear();
            radioMan.Checked = false;
            radioWoman.Checked = false;
            dataGridViewStudents.DataSource = null;
        }
        private void btnShowPanel_Click(object sender, EventArgs e)
        {
            if (adminReportPanel == null || adminReportPanel.IsDisposed)
            {
                adminReportPanel = new AdminReportPanel();
                adminReportPanel.Location = new Point(this.Location.X + this.Width, this.Location.Y);
                adminReportPanel.Show();
                this.LocationChanged += AdminPanel_ReportPanel_LocationChanged;
                this.FormClosed += AdminPanel_ReportPanel_FormClosed;
            }
            else
            {
                adminReportPanel.BringToFront();
            }
        }
        private void AdminPanel_ReportPanel_LocationChanged(object sender, EventArgs e)
        {
            if (adminReportPanel != null && !adminReportPanel.IsDisposed)
            {
                adminReportPanel.Location = new Point(this.Location.X + this.Width, this.Location.Y);
            }
        }
        private void AdminPanel_ReportPanel_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (adminReportPanel != null && !adminReportPanel.IsDisposed)
            {
                adminReportPanel.Close();
            }
        }
        #endregion
    }
}
