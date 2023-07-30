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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using static twitter.Main;

namespace twitter
{
    public partial class MainGeneral : Form
    {

        private string _username;
        private int _session_id;
        private int _user_id;
        // private int username;

        public MainGeneral(string username, int session_id)
        {
            InitializeComponent();
            _username = username;
            _session_id = session_id;


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


            ShowTweets();


            label5.Text = "Bienvenido, " + _username;

        }

        private void label3_Click(object sender, EventArgs e)
        {
            this.Hide();
            Login Lgform = new Login();
            Lgform.Show();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            this.Hide();
            Setting_account Settingform = new Setting_account(_username, _session_id);
            Settingform.Show();
        }

        private void Description2_Click(object sender, EventArgs e)
        {
            Main mainGeneral = new Main(_username, _session_id);
            this.Hide();
            mainGeneral.Show();
        }

        void ShowTweets()
        {

            DB db = new DB();
            MySqlConnection connec = db.getConnection();
            connec.Open();
            List<Post> userTweets = new List<Post>();

            MySqlCommand connection_tweet = new MySqlCommand("SELECT tweet_entity.*, users.username FROM tweet_entity INNER JOIN users ON tweet_entity.user_id = users.ID ", connec);
            connection_tweet.Parameters.AddWithValue("@user_id", _user_id);
            using (MySqlDataReader reader = connection_tweet.ExecuteReader())
            {
                while (reader.Read())
                {
                    string username1 = reader.GetString("username");
                    string tweet_one = reader.GetString("text");
                    DateTime createdAt = reader.GetDateTime("created_at");
                    int likes = reader.GetInt32("likes");
                    Post tweet = new Post(username1, tweet_one, createdAt, likes);
                    userTweets.Add(tweet);
                }
                reader.Close();
            }
            DisplayPosts(userTweets);
            textBox1.Text = string.Empty;
            connec.Close();
        }

        private void rjButtonRefresh_Click(object sender, EventArgs e)
        {
            DB db = new DB();
            MySqlConnection connec = db.getConnection();
            
            connec.Open();
            List<Post> userTweets = new List<Post>();

            string filterName = textBox1.Text;
            MySqlCommand connection_tweet = new MySqlCommand("SELECT tweet_entity.*, users.username FROM tweet_entity INNER JOIN users ON tweet_entity.user_id = users.ID WHERE users.username LIKE @username", connec);
            connection_tweet.Parameters.AddWithValue("@username", "%" + filterName + "%");
            using (MySqlDataReader reader = connection_tweet.ExecuteReader())
            {
                while (reader.Read())
                {
    
                    string username = reader.GetString("username");
                    string tweet_one = reader.GetString("text");
                    // string imageUrl = reader.GetString("image_url");
                    DateTime createdAt = reader.GetDateTime("created_at");
                    int likes = reader.GetInt32("likes");
                    Post tweet = new Post(username, tweet_one, createdAt, likes);
                    userTweets.Add(tweet);
                }
                reader.Close();
            }

            DisplayPosts(userTweets);

            textBox1.Text = string.Empty;
            connec.Close();
        }

        private void DisplayPosts(List<Post> userTweets)
        {
  
            listBoxControl1.DataSource = userTweets;
            listBoxControl1.DisplayMember = "Text";
        }

        private void label8_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            String fname = textBox1.Text;

            if (fname.ToString().Trim().Equals("Buscar"))
            {
                textBox1.Text = "";
                textBox1.ForeColor = Color.Gray;
            }
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            String fname = textBox1.Text;
            if (fname.ToString().Trim().Equals("Buscar") || fname.Trim().Equals(""))
            {
                textBox1.Text = "Buscar";
                textBox1.ForeColor = Color.Gray;
            }
        }

    }
}
