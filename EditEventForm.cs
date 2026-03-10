using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace Smart_Calendar_App
{
    public partial class EditEventForm : Form

    {
        public string EventTitle { get; set; }
        public string EventDate { get; set; }
        public string EventTime { get; set; }
        public EditEventForm(string title, string date, string time)
        {
            InitializeComponent();

            txtTitle.Text = title;
            txtDate.Text = date;
            txtTime.Text = time;
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            @EventTitle = txtTitle.Text;
            @EventDate = txtDate.Text;
            @EventTime = txtTime.Text;

            
            this.DialogResult = DialogResult.OK;
        
    }

        private void EditEventForm_Load(object sender, EventArgs e)
        {

        }
    }
}
