using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;

using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace twitter
{
    public partial class Setting_account : Form
    {

        private string _username;
        private int _session_id;
        private int _user_id;
        
        public Setting_account(string username, int session_id)
        {
            InitializeComponent();

            _username = username;
            _session_id = session_id;
            // Connect to the database
            DB db = new DB();
            MySqlConnection connection = db.getConnection();
   

            connection.Open();
            MySqlCommand command = new MySqlCommand("SELECT user_id FROM user_session WHERE session_id = @session_id", connection);
            command.Parameters.AddWithValue("@session_id", session_id);
            int user_id = Convert.ToInt32(command.ExecuteScalar());
            _user_id = user_id;

            // Retrieve the profile URL of the user from the users table
            command = new MySqlCommand("SELECT profile_url FROM users WHERE ID = @user_id", connection);
            command.Parameters.AddWithValue("@user_id", user_id);
            string profileUrl = command.ExecuteScalar()?.ToString();
            connection.Close();


            // Display the profile picture in pictureBox2
            if (!string.IsNullOrEmpty(profileUrl))
            {
                pictureBox3.ImageLocation = profileUrl;
            }




            label1.Text = "Hola, " + _username;

        }

        private void label3_Click(object sender, EventArgs e)
        {
            this.Hide();
            Login LogForm = new Login();
            LogForm.Show();
        }

        private void label2_Click(object sender, EventArgs e)
        {
            MainGeneral mainGeneral = new MainGeneral(_username, _session_id);
            this.Hide();
            mainGeneral.Show();
        }

        private void label8_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Description2_Click(object sender, EventArgs e)
        {
            Main mainGeneral = new Main(_username, _session_id);
            this.Hide();
            mainGeneral.Show();
        }


        private void rjButton1_Click(object sender, EventArgs e)
        {
            DB db = new DB();

            string username = textBox2.Text.Trim();
            string password = textBox1.Text.Trim();
            string newPassword = textBox3.Text.Trim();

            DataTable table = new DataTable();
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            MySqlCommand command = new MySqlCommand("SELECT `ID`, `salt`, `password` FROM `users` WHERE `ID` = @userID", db.getConnection());
            command.Parameters.AddWithValue("@userID", _user_id);
            adapter.SelectCommand = command;
            adapter.Fill(table);

            // Check if the user exists or not
            if (table.Rows.Count > 0)
            {
                int user_id = Convert.ToInt32(table.Rows[0]["ID"]);
                byte[] salt = (byte[])table.Rows[0]["salt"];
                byte[] encryptedPassword = (byte[])table.Rows[0]["password"];

                // Encrypt the entered password using the same method as login
                PasswordDeriveBytes pdb = new PasswordDeriveBytes(password, salt);
                byte[] enteredPasswordHash = pdb.GetBytes(256 / 8);

                // Compare the encrypted passwords
                if (encryptedPassword.SequenceEqual(enteredPasswordHash))
                {
                    // Password is correct, update the user's password

                    // Hash the new password using the existing salt
                    PasswordDeriveBytes newPdb = new PasswordDeriveBytes(newPassword, salt);
                    byte[] newHashedPassword = newPdb.GetBytes(256 / 8);

                    MySqlConnection connection = db.getConnection();
                    connection.Open();

                    MySqlCommand updateCommand = new MySqlCommand("UPDATE `users` SET `username` = @newUsername, `password` = @newPassword WHERE `ID` = @userId", connection);
                    updateCommand.Parameters.AddWithValue("@newUsername", username);
                    updateCommand.Parameters.AddWithValue("@newPassword", newHashedPassword);
                    updateCommand.Parameters.AddWithValue("@userId", user_id);
                    updateCommand.ExecuteNonQuery();

                    connection.Close();

                    MessageBox.Show("Password updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Incorrect password. Please try again.", "Invalid Password", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("User does not exist. Please enter a valid username.", "Invalid User", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }

        //** once you click on the textbox, It will erase the tag, and will change the color of your text.
        private void textBox2_Enter(object sender, EventArgs e)
        {
            String fname = textBox2.Text;

            if (fname.ToString().Trim().Equals("User"))
            {
                textBox2.Text = "";
                textBox2.ForeColor = Color.Gray;
            }
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            String fname = textBox2.Text;
            if (fname.ToString().Trim().Equals("User") || fname.Trim().Equals(""))
            {
                textBox2.Text = "User";
                textBox2.ForeColor = Color.Gray;
            }
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            String fname = textBox1.Text;

            if (fname.ToString().Trim().Equals("Password"))
            {
                textBox1.Text = "";
                textBox1.ForeColor = Color.Gray;
            }
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            String fname = textBox1.Text;
            if (fname.ToString().Trim().Equals("Password") || fname.Trim().Equals(""))
            {
                textBox1.Text = "Password";
                textBox1.ForeColor = Color.Gray;
            }
        }

        private void textBox3_Enter(object sender, EventArgs e)
        {

            String fname = textBox3.Text;

            if (fname.ToString().Trim().Equals("New Password"))
            {
                textBox3.Text = "";
                textBox3.ForeColor = Color.Gray;
            }
        }

        private void textBox3_Leave(object sender, EventArgs e)
        {
            String fname = textBox3.Text;
            if (fname.ToString().Trim().Equals("New Password") || fname.Trim().Equals(""))
            {
                textBox3.Text = "New Password";
                textBox3.ForeColor = Color.Gray;
            }
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }
    }
}
