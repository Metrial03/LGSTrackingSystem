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
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT USERTYPE FROM USERS WHERE USERNAME = @username AND PASSWORD = @password";
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
                        StudentPanel studentPanel = new StudentPanel();
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
    }
}
