using Microsoft.Data.SqlClient;


namespace LGSTrackingSystem
{
    public partial class LoginPage : Form
    {
        string connectionString = "Data Source=ERAY\\SQLEXPRESS;Initial Catalog=LGSDB;Integrated Security=True;TrustServerCertificate=true";
        public LoginPage()
        {
            InitializeComponent();
        }

        private void checkBoxShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            textBoxPassword.UseSystemPasswordChar = !checkBoxShowPassword.Checked;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxUsername.Text) || string.IsNullOrEmpty(textBoxPassword.Text))
            {
                MessageBox.Show("Please enter both username and password.");
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"
                SELECT U.USERTYPE, S.STUDENT_ID 
                FROM USERS U
                LEFT JOIN STUDENTS S ON U.ID = S.USER_ID
                WHERE U.USERNAME = @username AND U.PASSWORD = @password";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@username", textBoxUsername.Text);
                    command.Parameters.AddWithValue("@password", textBoxPassword.Text);
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        string userType = reader["USERTYPE"].ToString();
                        if (userType == "Admin")
                        {
                            AdminPanel adminPanel = new AdminPanel();
                            adminPanel.Show();
                            this.Hide();
                        }
                        else if (userType == "Student")
                        {
                            int studentId = Convert.ToInt32(reader["STUDENT_ID"]);
                            StudentPanel studentPanel = new StudentPanel(studentId);
                            studentPanel.Show();
                            this.Hide();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Invalid username or password.");
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"A database error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
