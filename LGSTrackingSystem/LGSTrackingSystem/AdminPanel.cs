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

namespace LGSTrackingSystem
{
    public partial class AdminPanel : Form
    {
        string connectionString = "Data Source=ERAY\\SQLEXPRESS;Initial Catalog=LGSDB;Integrated Security=True;TrustServerCertificate=true";
        public AdminPanel()
        {
            InitializeComponent();
        }
        #region Student Management
        string gender => radioMan.Checked == false && radioWoman.Checked == false ? "" : (radioWoman.Checked ? "Woman" : "Man");
        private void btnAddStudent_Click(object sender, EventArgs e)
        {
            bool checkStudentID = CheckStudentID(textBoxStudentID.Text);
            bool checkUsername = CheckUsername(false);

            if (checkStudentID)
            {
                MessageBox.Show("Student ID already exists.");
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
        }
        private void btnUpdateStudent_Click(object sender, EventArgs e)
        {
            bool checkStudentID = CheckStudentID(textBoxStudentID.Text);
            bool checkUsername = CheckUsername(true);
            if (checkStudentID == false)
            {
                MessageBox.Show("Student ID does not exist.");
            }
            else if (checkUsername)
            {
                MessageBox.Show("Username already exists.");
            }
            if (Validation() == false || checkStudentID == false || checkUsername)
            {
                return;
            }
            UpdateStudent();
        }
        private void btnDeleteStudent_Click(object sender, EventArgs e)
        {
            bool checkStudentID = CheckStudentID(textBoxStudentID.Text);
            if (checkStudentID == false)
            {
                MessageBox.Show("Student ID does not exist.");
                return;
            }
            DeleteStudent();
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

                    string addStudentQuery = "INSERT INTO Students (STUDENT_ID, NAME, SURNAME, GENDER, EMAIL, ADDRESS, USER_ID) VALUES (@studentID, @name, @surname, @gender, @email, @address, @userId)";
                    SqlCommand addStudentCommand = new SqlCommand(addStudentQuery, connection);
                    addStudentCommand.Parameters.AddWithValue("@studentID", textBoxStudentID.Text);
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
                    string updateUserQuery = "UPDATE Users SET USERNAME = @username, PASSWORD = @password WHERE ID = (SELECT USER_ID FROM Students WHERE STUDENT_ID = @studentID)";
                    SqlCommand updateUserCommand = new SqlCommand(updateUserQuery, connection);
                    updateUserCommand.Parameters.AddWithValue("@username", textBoxUsername.Text);
                    updateUserCommand.Parameters.AddWithValue("@password", textBoxPassword.Text);
                    updateUserCommand.Parameters.AddWithValue("@studentID", textBoxStudentID.Text);
                    updateUserCommand.ExecuteNonQuery();

                    string updateStudentQuery = "UPDATE Students SET NAME = @name, SURNAME = @surname, GENDER = @gender, EMAIL = @email, ADDRESS = @address WHERE STUDENT_ID = @studentID";
                    SqlCommand updateStudentCommand = new SqlCommand(updateStudentQuery, connection);
                    updateStudentCommand.Parameters.AddWithValue("@name", textBoxName.Text);
                    updateStudentCommand.Parameters.AddWithValue("@surname", textBoxSurname.Text);
                    updateStudentCommand.Parameters.AddWithValue("@email", textBoxEmail.Text);
                    updateStudentCommand.Parameters.AddWithValue("@gender", gender);
                    updateStudentCommand.Parameters.AddWithValue("@address", textBoxAdress.Text);
                    updateStudentCommand.Parameters.AddWithValue("@studentID", textBoxStudentID.Text);
                    updateStudentCommand.ExecuteNonQuery();
                    MessageBox.Show("Student updated successfully.");
                }
                catch
                {
                    MessageBox.Show("Error updating user.");
                    return;
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

                    string getUserIdQuery = "SELECT USER_ID FROM Students WHERE STUDENT_ID = @studentID";
                    SqlCommand getUserIdCommand = new SqlCommand(getUserIdQuery, connection);
                    getUserIdCommand.Parameters.AddWithValue("@studentID", textBoxStudentID.Text);
                    object userIdObj = getUserIdCommand.ExecuteScalar();

                    if (userIdObj == null)
                    {
                        MessageBox.Show("Student not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    int userId = Convert.ToInt32(userIdObj);

                    string deleteResultsQuery = "DELETE FROM RESULTS WHERE STUDENT_ID = @studentID";
                    SqlCommand deleteResultsCommand = new SqlCommand(deleteResultsQuery, connection);
                    deleteResultsCommand.Parameters.AddWithValue("@studentID", textBoxStudentID.Text);
                    deleteResultsCommand.ExecuteNonQuery();

                    string deleteStudentQuery = "DELETE FROM Students WHERE STUDENT_ID = @studentID";
                    SqlCommand deleteStudentCommand = new SqlCommand(deleteStudentQuery, connection);
                    deleteStudentCommand.Parameters.AddWithValue("@studentID", textBoxStudentID.Text);
                    deleteStudentCommand.ExecuteNonQuery();

                    string deleteUserQuery = "DELETE FROM Users WHERE ID = @userId";
                    SqlCommand deleteUserCommand = new SqlCommand(deleteUserQuery, connection);
                    deleteUserCommand.Parameters.AddWithValue("@userId", userId);
                    deleteUserCommand.ExecuteNonQuery();

                    MessageBox.Show("Student and their associated results deleted successfully.");
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
                    string listStudentQuery = "SELECT STUDENT_ID AS [Student ID], NAME AS [Name], SURNAME AS [Surname], GENDER AS [Gender], EMAIL AS [Email], ADDRESS AS [Address] FROM Students WHERE 1=1";
                    if (!string.IsNullOrEmpty(textBoxStudentID.Text))
                    {
                        listStudentQuery += " AND STUDENT_ID = @studentID";
                    }
                    if (!string.IsNullOrEmpty(textBoxName.Text))
                    {
                        listStudentQuery += " AND NAME LIKE @name";
                    }
                    if (!string.IsNullOrEmpty(textBoxSurname.Text))
                    {
                        listStudentQuery += " AND SURNAME LIKE @surname";
                    }
                    if (!string.IsNullOrEmpty(textBoxEmail.Text))
                    {
                        listStudentQuery += " AND EMAIL LIKE @email";
                    }
                    if (!string.IsNullOrEmpty(gender))
                    {
                        listStudentQuery += " AND GENDER = @gender";
                    }
                    if (!string.IsNullOrEmpty(textBoxAdress.Text))
                    {
                        listStudentQuery += " AND ADDRESS LIKE @address";
                    }

                    SqlCommand listStudentCommand = new SqlCommand(listStudentQuery, connection);
                    if (!string.IsNullOrEmpty(textBoxStudentID.Text))
                    {
                        if (int.TryParse(textBoxStudentID.Text, out int studentID))
                        {
                            listStudentCommand.Parameters.AddWithValue("@studentID", studentID);
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
            if (string.IsNullOrEmpty(textBoxStudentID.Text) || string.IsNullOrEmpty(textBoxName.Text) || string.IsNullOrEmpty(textBoxSurname.Text) || string.IsNullOrEmpty(textBoxEmail.Text) ||
                string.IsNullOrEmpty(textBoxAdress.Text) || string.IsNullOrEmpty(textBoxUsername.Text) || string.IsNullOrEmpty(textBoxPassword.Text) || radioWoman.Checked == false && radioMan.Checked == false)
            {
                MessageBox.Show("Please fill in all fields.");
                return false;
            }
            else if (int.TryParse(textBoxStudentID.Text, out int studentID) == false)
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
        }
        private void btnAddResult_Click(object sender, EventArgs e)
        {
            bool validation = ResultValidation();
            bool studentIDCheck = CheckStudentID(textBoxTestStudentID.Text);
            bool examNameCheck = ExistingExamName(textBoxExamName.Text);
            bool existResult = checkExistResult(textBoxTestStudentID.Text, textBoxExamName.Text);

            if (examNameCheck)
            {
                MessageBox.Show("Exam does not exist.");
            }

            if (existResult == false)
            {
                MessageBox.Show("Result already exists.");
            }

            if (validation == false || studentIDCheck == false || examNameCheck || existResult == false)
            {
                return;
            }
            try
            {
                AddResult(textBoxTestStudentID.Text, textBoxExamName.Text, "Mathematics", Convert.ToInt32(textBoxTrueMath.Text),
                    Convert.ToInt32(textBoxFalseMath.Text), Convert.ToInt32(textBoxBlankMath.Text));
                AddResult(textBoxTestStudentID.Text, textBoxExamName.Text, "Science", Convert.ToInt32(textBoxTrueScience.Text),
                    Convert.ToInt32(textBoxFalseScience.Text), Convert.ToInt32(textBoxBlankScience.Text));
                AddResult(textBoxTestStudentID.Text, textBoxExamName.Text, "Turkish", Convert.ToInt32(textBoxTrueTurkish.Text),
                    Convert.ToInt32(textBoxFalseTurkish.Text), Convert.ToInt32(textBoxBlankTurkish.Text));
                AddResult(textBoxTestStudentID.Text, textBoxExamName.Text, "History", Convert.ToInt32(textBoxTrueHistory.Text),
                    Convert.ToInt32(textBoxFalseHistory.Text), Convert.ToInt32(textBoxBlankHistory.Text));
                AddResult(textBoxTestStudentID.Text, textBoxExamName.Text, "Religion", Convert.ToInt32(textBoxTrueReligion.Text),
                    Convert.ToInt32(textBoxFalseReligion.Text), Convert.ToInt32(textBoxBlankReligion.Text));
                AddResult(textBoxTestStudentID.Text, textBoxExamName.Text, "English", Convert.ToInt32(textBoxTrueEnglish.Text),
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
            bool studentIDCheck = CheckStudentID(textBoxTestStudentID.Text);
            bool checkExamName = ExistingExamName(textBoxExamName.Text);
            bool checkResult = checkExistResult(textBoxTestStudentID.Text, textBoxExamName.Text);

            if (checkExamName)
            {
                MessageBox.Show("Exam does not exist.");
            }
            if (checkResult)
            {
                MessageBox.Show("Result does not exist.");
            }
            if (studentIDCheck == false || checkExamName || checkResult)
            {
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
                    AND ID != (SELECT USER_ID FROM Students WHERE STUDENT_ID = @studentID)";
                    }
                    else
                    {
                        checkUserQuery = "SELECT COUNT(*) FROM USERS WHERE USERNAME = @username";
                    }

                    SqlCommand checkUserCommand = new SqlCommand(checkUserQuery, connection);
                    checkUserCommand.Parameters.AddWithValue("@username", textBoxUsername.Text);

                    if (isUpdate)
                    {
                        checkUserCommand.Parameters.AddWithValue("@studentID", textBoxStudentID.Text);
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
        private bool CheckStudentID(string textBoxName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string checkIDQuery = "SELECT COUNT(*) FROM STUDENTS WHERE STUDENT_ID = @studentID";
                    SqlCommand checkIDCommand = new SqlCommand(checkIDQuery, connection);
                    checkIDCommand.Parameters.AddWithValue("@studentID", textBoxName);
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
                    string showResultQuery = @"
                SELECT 
                    STUDENT_ID AS [Student ID], 
                    (SELECT EXAM_NAME FROM EXAMS WHERE EXAMS.EXAM_ID = RESULTS.EXAM_ID) AS [Exam Name], 
                    SUBJECT_NAME AS [Subject Name], 
                    TRUE_COUNT AS [True Count], 
                    FALSE_COUNT AS [False Count], 
                    BLANK_COUNT AS [Blank Count]
                FROM RESULTS
                WHERE STUDENT_ID = @studentID 
                AND EXAM_ID = (SELECT EXAM_ID FROM EXAMS WHERE EXAM_NAME = @examName)";

                    SqlCommand showResultCommand = new SqlCommand(showResultQuery, connection);
                    showResultCommand.Parameters.AddWithValue("@studentID", textBoxTestStudentID.Text);
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
        private void AddResult(string studentId, string examName, string subjectName, int trueCount, int falseCount, int blankCount)
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
                    deleteResultCommand.Parameters.AddWithValue("@studentId", textBoxTestStudentID.Text);
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
        private void ShowTestHistory()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = @"
                SELECT 
                    STUDENT_ID AS [Student ID], 
                    (SELECT EXAM_NAME FROM EXAMS WHERE EXAMS.EXAM_ID = RESULTS.EXAM_ID) AS [Exam Name], 
                    (SELECT EXAM_TYPE FROM EXAMS WHERE EXAMS.EXAM_ID = RESULTS.EXAM_ID) AS [Exam Type],
                    SUM(TRUE_COUNT) AS [True], 
                    SUM(FALSE_COUNT) AS [False], 
                    SUM(BLANK_COUNT) AS [Blank]
                FROM RESULTS
                GROUP BY STUDENT_ID, EXAM_ID";
                    SqlCommand command = new SqlCommand(query, connection);
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
        private bool checkExistResult(string studentId, string examName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
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
    }
}
