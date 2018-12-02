using System;
using System.IO;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ShinyCounter {
    public partial class Form1 : Form {
        public static int KEY_LEFT_ALT = 164;
        public static int KEY_PRESS = KEY_LEFT_ALT;
        public static int TIMER_SECS = 0;
        public static bool TIMER_STARTED = false;
        public static bool CHECK_NEXT_KEY = false;
        public static FileInfo[] spriteImages = null;

        public static double[,] rates =
        {
            //default   //lure      //charm     //lure+charm
            { 4096,     2048,       1365.3,     1024 },
            { 1024,     819.2,      682.6,      585.14 },
            { 512,      455.1,      409.6,      372.36 },
            { 341.3,    315.08,     292.57,     273.07 },
        };

        public Form1() {
            InitializeComponent();

            spriteImages = new DirectoryInfo(Path.GetDirectoryName(Application.ExecutablePath) + @"\images\").GetFiles("*.png");

            textBox1.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            textBox1.AutoCompleteSource = AutoCompleteSource.CustomSource;
            AutoCompleteStringCollection col = new AutoCompleteStringCollection();
            foreach(FileInfo file in spriteImages)
                col.Add(file.Name);
            textBox1.AutoCompleteCustomSource = col;
        }

        private void Form1_Load(object sender, EventArgs e) {
            Hook.CreateHook(KeyReader);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
            Hook.DestroyHook();
        }

        private void KeyReader(IntPtr wParam, IntPtr lParam) {
            int keyPressed = Marshal.ReadInt32(lParam);

            if(CHECK_NEXT_KEY) {
                KEY_PRESS = keyPressed;
                CHECK_NEXT_KEY = false;
            }

            else if(keyPressed == KEY_PRESS) {
                label3.Text = String.Format("{0}", Convert.ToInt32(label3.Text) + 1);
            }
        }

        private void LoadSprite() {
            string spriteName = textBox1.Text;
            if (spriteName.Contains(".png") == false)
                spriteName += ".png";

            foreach (FileInfo file in spriteImages) {
                if(file.Name.Equals(spriteName)) {
                    pictureBox1.Image = Image.FromFile(file.FullName);
                    pictureBox1.Visible = true;
                    return;
                }
            }

            pictureBox1.Visible = false;
        }

        private void UpdateOdds() {
            bool hasLure = checkBox1.Checked;
            bool hasCharm = checkBox2.Checked;
            int comboCount = Convert.ToInt32(label4.Text);
            double rate = 0.0;

            int row = 0;
            if (comboCount > 30) row = 3;
            else if (comboCount > 20) row = 2;
            else if (comboCount > 10) row = 1;
            else row = 0;

            if (hasLure && hasCharm) {
                rate = rates[row, 3];
            }

            else if (hasCharm) {
                rate = rates[row, 2];
            }

            else if(hasLure) {
                rate = rates[row, 1];
            }

            else {
                rate = rates[row, 0];
            }

            label1.Text = String.Format("Shiny Chance: 1 / {0}", rate);
        }

        private void timer1_Tick(object sender, EventArgs e) {
            TIMER_SECS++;

            TimeSpan t = TimeSpan.FromSeconds(TIMER_SECS);
            string answer = string.Format("{0:D2}h:{1:D2}m:{2:D2}s", t.Hours, t.Minutes, t.Seconds);

            label6.Text = answer;
        }

        private void CheckTimer() {
            int catchCombo = Convert.ToInt32(label4.Text);

            if (catchCombo < 31) {
                timer1.Stop();
                TIMER_SECS = 0;
                label6.Text = "00h:00m:00s";
                TIMER_STARTED = false;
            }

            else if (!TIMER_STARTED) {
                timer1.Start();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e) {
            LoadSprite();
        }

        private void textBox1_Leave(object sender, EventArgs e) {
            LoadSprite();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            System.Diagnostics.Process.Start("https://twitter.com/Matrix");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            System.Diagnostics.Process.Start("https://twitter.com/Bxcon");
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            System.Diagnostics.Process.Start("https://discord.gg/bhegTr");
        }

        private void button1_Click(object sender, EventArgs e) {
            Properties.Settings.Default.KeyBind = KEY_PRESS;
            Properties.Settings.Default.Counter = Convert.ToInt32(label3.Text);
            Properties.Settings.Default.CatchCombo = Convert.ToInt32(label4.Text);
            Properties.Settings.Default.Lure = checkBox1.Checked;
            Properties.Settings.Default.Charm = checkBox2.Checked;
            Properties.Settings.Default.ComboTimer = label6.Text;
            Properties.Settings.Default.Pokemon = textBox1.Text;
            Properties.Settings.Default.TimerInt = TIMER_SECS;
            Properties.Settings.Default.Save();
        }

        private void button2_Click(object sender, EventArgs e) {
            KEY_PRESS = Properties.Settings.Default.KeyBind;
            label3.Text = Convert.ToString(Properties.Settings.Default.Counter);
            label4.Text = Convert.ToString(Properties.Settings.Default.CatchCombo);
            checkBox1.Checked = Properties.Settings.Default.Lure;
            checkBox2.Checked = Properties.Settings.Default.Charm;
            label6.Text = Properties.Settings.Default.ComboTimer;
            textBox1.Text = Properties.Settings.Default.Pokemon;
            TIMER_SECS = Properties.Settings.Default.TimerInt;

            UpdateOdds();
            CheckTimer();
            LoadSprite();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e) {
            UpdateOdds();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e) {
            UpdateOdds();
        }

        private void button6_Click(object sender, EventArgs e) {
            int count = Convert.ToInt32(label4.Text) - 1;
            if (count < 0)
                count = 0;

            label4.Text = Convert.ToString(count);

            UpdateOdds();
            CheckTimer();
        }

        private void button7_Click(object sender, EventArgs e) {
            int count = Convert.ToInt32(label4.Text) + 1;
            label4.Text = Convert.ToString(count);

            UpdateOdds();
            CheckTimer();
        }

        private void button8_Click(object sender, EventArgs e) {
            label4.Text = "0";

            UpdateOdds();
            CheckTimer();
        }

        private void button4_Click(object sender, EventArgs e) {
            label3.Text = "0";
        }

        private void button5_Click(object sender, EventArgs e) {
            int count = Convert.ToInt32(label3.Text) - 1;
            if (count < 0)
                count = 0;

            label3.Text = Convert.ToString(count);
        }

        private void button3_Click(object sender, EventArgs e) {
            CHECK_NEXT_KEY = true;
        }

        private void button9_Click(object sender, EventArgs e) {
            KEY_PRESS = KEY_LEFT_ALT;
        }
    }
}
