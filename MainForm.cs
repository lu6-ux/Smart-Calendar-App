using Microsoft.VisualBasic;
using SmartCalendarApp;
using System;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;

namespace Smart_Calendar_App
{
    public partial class MainForm : Form
    {
        private MonthCalendar monthCalendar1;
        private Timer reminderTimer;

        public MainForm()
        {
            InitializeComponent();
            InitializeMenu();
            InitializeMonthCalendar();
            this.Load += MainForm_Load;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                CreateEventsTable();
                LoadEvents();
                HighlightEventDates();
                ImproveGridUI();
                ToolTipsSetup();

                monthCalendar1.SetDate(DateTime.Today);

                reminderTimer = new Timer();
                reminderTimer.Interval = 60000;
                reminderTimer.Tick += ReminderTimer_Tick;
                reminderTimer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Startup error: " + ex.Message);
            }
        }

        private void InitializeMenu()
        {
            MenuStrip menuStrip = new MenuStrip();

            ToolStripMenuItem fileMenu = new ToolStripMenuItem("File");
            ToolStripMenuItem addItem = new ToolStripMenuItem("Add ");
            addItem.Click += AddItem_Click;
            ToolStripMenuItem editItem = new ToolStripMenuItem("Edit ");
            editItem.Click += EditItem_Click;
            ToolStripMenuItem deleteItem = new ToolStripMenuItem("Delete ");
            deleteItem.Click += DeleteItem_Click;
            ToolStripMenuItem logoutItem = new ToolStripMenuItem("Logout");
            logoutItem.Click += LogoutItem_Click;

            fileMenu.DropDownItems.Add(addItem);
            fileMenu.DropDownItems.Add(editItem);
            fileMenu.DropDownItems.Add(deleteItem);
            fileMenu.DropDownItems.Add(new ToolStripSeparator());
            fileMenu.DropDownItems.Add(logoutItem);

            ToolStripMenuItem helpMenu = new ToolStripMenuItem("Help");
            ToolStripMenuItem howToUseItem = new ToolStripMenuItem("How to Use");
            howToUseItem.Click += HowToUseItem_Click;
            helpMenu.DropDownItems.Add(howToUseItem);

            ToolStripMenuItem aboutMenu = new ToolStripMenuItem("About");
            aboutMenu.Click += AboutMenu_Click;

            menuStrip.Items.Add(fileMenu);
            menuStrip.Items.Add(helpMenu);
            menuStrip.Items.Add(aboutMenu);

            this.MainMenuStrip = menuStrip;
            this.Controls.Add(menuStrip);
        }

        private void AddItem_Click(object sender, EventArgs e)
        {
            btnAdd_Click(sender, e);
        }

        private void EditItem_Click(object sender, EventArgs e)
        {
            btnEdit_Click(sender, e);
        }

        private void DeleteItem_Click(object sender, EventArgs e)
        {
            btnDelete_Click(sender, e);
        }

        private void LogoutItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("You have been logged out.");
            this.Close();
        }

        private void HowToUseItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "How to Use Smart Calendar:\n\n" +
                "1. Go to File → Add Event to create a new event.\n" +
                "2. Select a date from the calendar to view events.\n" +
                "3. Use File → Edit Event to change an event.\n" +
                "4. Use File → Delete Event to remove an event.\n" +
                "5. Use File → Logout to exit the application.",
                "How to Use");
        }

        private void AboutMenu_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "Smart Calendar Application Version 1.0\nDeveloped by Lakshana ",
                "About");
        }

        private void CreateEventsTable()
        {
            using (var conn = Database.GetConnection())
            {
                string table = @"CREATE TABLE IF NOT EXISTS Events(
                                    EventID INTEGER PRIMARY KEY AUTOINCREMENT,
                                    Title TEXT,
                                    EventDate TEXT,
                                    EventTime TEXT,
                                    Category TEXT,
                                    Priority TEXT
                                )";
                new SQLiteCommand(table, conn).ExecuteNonQuery();
            }
        }

        private void HighlightEventDates()
        {
            if (monthCalendar1 == null) return;

            monthCalendar1.RemoveAllBoldedDates();

            using (var conn = Database.GetConnection())
            {
                string query = "SELECT DISTINCT EventDate FROM Events";
                var cmd = new SQLiteCommand(query, conn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if (DateTime.TryParse(reader["EventDate"].ToString(), out DateTime dt))
                    {
                        monthCalendar1.AddBoldedDate(dt);
                    }
                }
            }

            monthCalendar1.UpdateBoldedDates();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string title = Interaction.InputBox("Enter Event Title:");
            string date = Interaction.InputBox("Enter Event Date (YYYY-MM-DD):");
            string time = Interaction.InputBox("Enter Event Time (e.g. 2:30 PM):");

            if (!DateTime.TryParse($"{date} {time}", out DateTime fullDate))
            {
                MessageBox.Show("Invalid date/time format!");
                return;
            }

            using (var conn = Database.GetConnection())
            {
                string sql = @"INSERT INTO Events (Title, EventDate, EventTime, Category, Priority)
                               VALUES (@t,@d,@ti,'General','Normal')";
                var cmd = new SQLiteCommand(sql, conn);
                cmd.Parameters.AddWithValue("@t", title);
                cmd.Parameters.AddWithValue("@d", date);
                cmd.Parameters.AddWithValue("@ti", fullDate.ToString("HH:mm"));
                cmd.ExecuteNonQuery();
            }

            LoadEvents();
            HighlightEventDates();
            MessageBox.Show("Event Added!");
        }

        private void LoadEvents()
        {
            using (var conn = Database.GetConnection())
            {
                string query = "SELECT * FROM Events";
                var adapter = new SQLiteDataAdapter(query, conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dataGridView1.DataSource = dt;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null) return;

            int id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["EventID"].Value);

            using (var conn = Database.GetConnection())
            {
                var cmd = new SQLiteCommand("DELETE FROM Events WHERE EventID=@id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }

            LoadEvents();
            HighlightEventDates();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null) return;

            int id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["EventID"].Value);

            string title = Interaction.InputBox("Edit Title:");
            string date = Interaction.InputBox("Edit Date (YYYY-MM-DD):");
            string time = Interaction.InputBox("Edit Time (e.g. 2:30 PM):");

            if (!DateTime.TryParse($"{date} {time}", out DateTime fullDate))
            {
                MessageBox.Show("Invalid date/time format!");
                return;
            }

            using (var conn = Database.GetConnection())
            {
                var cmd = new SQLiteCommand(
                    "UPDATE Events SET Title=@t, EventDate=@d, EventTime=@ti WHERE EventID=@id", conn);

                cmd.Parameters.AddWithValue("@t", title);
                cmd.Parameters.AddWithValue("@d", date);
                cmd.Parameters.AddWithValue("@ti", fullDate.ToString("HH:mm"));
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }

            LoadEvents();
            HighlightEventDates();
        }

        private void InitializeMonthCalendar()
        {
            monthCalendar1 = new MonthCalendar();
            monthCalendar1.Dock = DockStyle.Left;
            monthCalendar1.MaxSelectionCount = 1;
            monthCalendar1.DateChanged += monthCalendar1_DateChanged;
            monthCalendar1.SetDate(DateTime.Today);
            this.Controls.Add(monthCalendar1);
        }

        private void monthCalendar1_DateChanged(object sender, DateRangeEventArgs e)
        {
            string selectedDate = e.Start.ToString("yyyy-MM-dd");
            FilterEventsByDate(selectedDate);
        }

        private void FilterEventsByDate(string selectedDate)
        {
            using (var conn = Database.GetConnection())
            {
                string query = "SELECT * FROM Events WHERE EventDate=@d";
                var adapter = new SQLiteDataAdapter(query, conn);
                adapter.SelectCommand.Parameters.AddWithValue("@d", selectedDate);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dataGridView1.DataSource = dt;
            }
        }

        private void ImproveGridUI()
        {
            dataGridView1.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 10, FontStyle.Bold);
        }

        private void ToolTipsSetup()
        {
            ToolTip tip = new ToolTip();
            tip.SetToolTip(btnAdd, "Add a new event");
            tip.SetToolTip(btnEdit, "Edit selected event");
            tip.SetToolTip(btnDelete, "Delete selected event");
        }

        private void ReminderTimer_Tick(object sender, EventArgs e)
        {
        }
    }
}
