using Smart_Calendar_App;
using System;
using System.Data.SQLite;
using System.Windows.Forms;

namespace SmartCalendarApp
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            txtUsername.Focus();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both username and password.", "Input Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (ValidateUser(username, password))
            {
                Session.CurrentUser = username;

                MessageBox.Show($"Welcome, {username}!", "Login Successful",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.Hide();
                using (MainForm mainForm = new MainForm())
                {
                    mainForm.ShowDialog();
                }
                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid username or password.", "Login Failed",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateUser(string username, string password)
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection("Data Source=smartcalendar.db;Version=3;"))
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM users WHERE Username=@username AND Password=@password";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@password", password);
                        object result = cmd.ExecuteScalar();
                        return (result != null && Convert.ToInt64(result) > 0);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database error: " + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            using (RegisterForm registerForm = new RegisterForm())
            {
                registerForm.ShowDialog();
            }
        }
    }
}
