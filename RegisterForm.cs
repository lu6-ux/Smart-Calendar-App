using SmartCalendarApp;
using System;
using System.Data.SQLite;
using System.Windows.Forms;

namespace Smart_Calendar_App
{
    public partial class RegisterForm : Form
    {
        public RegisterForm()
        {
            InitializeComponent();
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both username and password.", "Input Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SQLiteConnection conn = new SQLiteConnection("Data Source=smartcalendar.db;Version=3;"))
                {
                    conn.Open();

                   
                    string createTableQuery = @"
                        CREATE TABLE IF NOT EXISTS users (
                            UserID INTEGER PRIMARY KEY AUTOINCREMENT,
                            Username TEXT NOT NULL UNIQUE,
                            Password TEXT NOT NULL
                        );";

                    using (SQLiteCommand cmd = new SQLiteCommand(createTableQuery, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                  
                    string checkQuery = "SELECT COUNT(*) FROM users WHERE Username = @username";
                    using (SQLiteCommand checkCmd = new SQLiteCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@username", username);
                        long userExists = (long)checkCmd.ExecuteScalar();

                        if (userExists > 0)
                        {
                            MessageBox.Show("Username already exists. Try a different one.",
                                            "Registration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }

                   
                    string insertQuery = "INSERT INTO users (Username, Password) VALUES (@username, @password)";
                    using (SQLiteCommand insertCmd = new SQLiteCommand(insertQuery, conn))
                    {
                        insertCmd.Parameters.AddWithValue("@username", username);
                        insertCmd.Parameters.AddWithValue("@password", password);
                        insertCmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Registration successful! Please log in now.",
                                "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                
                LoginForm loginForm = new LoginForm();
                loginForm.Show();

                this.Hide(); 
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred during registration: {ex.Message}",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtUsername_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void txtUsername_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void RegisterForm_Load(object sender, EventArgs e)
        {

        }
    }
}