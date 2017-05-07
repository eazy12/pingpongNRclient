using RotatePictureBox;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Pingpong.Properties;
using gamelogic;

namespace Pingpong
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.KeyPreview = true;
            //Dictionary<string, string> pro = new Dictionary<string, string>();
            //pro["name"] = "TCP Channel Binary";
            //pro["priority"] = "17";
            //pro["port"] = "8086";
            //BinaryClientFormatterSinkProvider snkPrvd2 = new BinaryClientFormatterSinkProvider();
            //TcpClientChannel Channel = new TcpClientChannel(pro, snkPrvd2);

            //ChannelServices.RegisterChannel(Channel, true);

            RemotingConfiguration.Configure("Pingpong.exe.config", false);
            game = new Game();
            //Game game = (Game)Activator.GetObject(typeof(Game), "tcp://localhost:8086/Game");
            Player p = game.Connect();

            game.UpdateInfoHandle += new UpdateInfoEvent(onUpdateInfo);
        }

        private void onUpdateInfo(UpdateInfo updateInfo)
        {
            Console.WriteLine(updateInfo.getS());
        }


        PictureBox[] Score_Player = new PictureBox[5];  
        PictureBox[] Score_Enemy = new PictureBox[5];   
        Color ScoreColor = Color.Silver;                //Just to set the background color of the scoreboxes
        Random rng = new Random();                      //If you change this, change it from the design page too
        Boolean Player_Up, Player_Down = false;         //Booleans to see if player is going up or down
        Boolean BallGoingLeft = false;                   //Is the ball going left or right?
        Boolean GameOn = false;                         //Is the game on or paused
        Game game;

        int Speed_Player;                           //Dont change these, change them from the settings page
        int Speed_Enemy;                            
        int BallSpeed;
        int BallForce;
        int Round = 0;

       

        public void PaintBox(int X, int Y, int W, int H, Color C)
        {
            PictureBox Temp = new PictureBox();
            Temp.BackColor = C;
            Temp.Size = new Size(W, H);
            Temp.Location = new Point(X, Y);
            WorldFrame.Controls.Add(Temp);
        }

     

        

       

        public void CircleThis(PictureBox pic)  //Just a function to redraw the ball into a circle.
        {
            System.Drawing.Drawing2D.GraphicsPath gp = new System.Drawing.Drawing2D.GraphicsPath();
            gp.AddEllipse(0, 0, pic.Width - 3, pic.Height - 3);
            Region rg = new Region(gp);
            pic.Region = rg;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            game.changePosition();
            return;
            switch (e.KeyCode)      //Regular key input, if press the right keys it moves in its direction
            {
                case Keys.W:
                case Keys.Up:
                    Player_Down = false;
                    Player_Up = true;
                    break;
                case Keys.S:
                case Keys.Down:
                    Player_Up = false;
                    Player_Down = true;
                    break;
                case Keys.Space:    //If hit space it starts the game,
                    GameOn = true;
                    ///RandomStart(BallGoingLeft);
                    label_Start.Visible = false;
                    break;
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                case Keys.Up:
                    Player_Up = false;
                    break;
                case Keys.S:
                case Keys.Down:
                    Player_Down = false;
                    break;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            return;
            for (int i = 0; i < 5; i++)
            {
                //Score_Player[i] = PicID(i + 1);         //Adds the "score" pictureboxes to an array each
                //Score_Enemy[i] = PicID(i + 1, true);
            }
            CircleThis(pb_Ball); 
            pb_Ball.Location = new Point(208, rng.Next(10, 190));   // Moves the ball in place

            BallGoingLeft = Convert.ToBoolean(rng.Next(0, 1));

        }  

        public void AddScore(PictureBox[] Arr)
        {
            for (int i = 0; i < Arr.Length; i++)
            {   //Goes through the entire array, checks where the first "non black" box is
                if (Arr[i].BackColor == ScoreColor)
                {   //And then changes it to black
                    Arr[i].BackColor = Color.Black;
                    break;
                }
            }

            if (Arr[4].BackColor == Color.Black)
            {   //If they all are black, game ends.
                GameOn = false;
                label_Start.Visible = true;
                RestoreScore();
                pb_Ball.Location = new Point(208, rng.Next(10, 190));
                pb_Player.Location = new Point(3, 67);
                pb_Enemy.Location = new Point(409, 67);
                Round = 0;
                label_Time.Visible = false;
            }
        }

        public void RestoreScore()
        {
            for (int i = 0; i <= 5; i++)
            {   //Resets all the score boxes to their original color
                //PicID(i).BackColor = ScoreColor;
                //PicID(i, true).BackColor = ScoreColor;
            }
        }  

        //delete
        private void timer_Enemy_Tick(object sender, EventArgs e)
        {
            return;
            if (GameOn) //Timer to move the Enemy
            {   //Always tries to be in the middle
                if (pb_Enemy.Location.Y + 28 < pb_Ball.Location.Y)
                {   //Which is around 28 pixels below its Y coordinate
                    pb_Enemy.Top += Speed_Enemy;
                }
                else
                {
                    pb_Enemy.Top -= Speed_Enemy;
                }
            }
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            return;
            SettingsForm sF = new SettingsForm();
            sF.Show();
        }

        private void timer_Sec_Tick(object sender, EventArgs e)
        {
            return;
            if (GameOn)
            {
                Round++;
                label_Time.Visible = true;

                TimeSpan time = TimeSpan.FromSeconds(Round);

                string str = time.ToString(@"mm\:ss");
                label_Time.Text = "Time: " + str;
            }
        }


        private void timer_Moveball_Tick(object sender, EventArgs e)
        {

        }


    }
}
