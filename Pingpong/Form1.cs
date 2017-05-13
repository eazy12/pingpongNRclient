using RotatePictureBox;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels.Ipc;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Pingpong.Properties;
using System.Runtime.Serialization.Formatters;
using gamelogic;
using System.Runtime.Remoting.Channels.Http;
using System.Runtime.Remoting.Lifetime;

namespace Pingpong
{
    public partial class Form1 : Form
    {
        
        Player player;
        Player player2;
        Game game;

        public Form1()
        {
            InitializeComponent();
            this.KeyPreview = true;
            //Dictionary<string, string> pro = new Dictionary<string, string>();
            //pro["name"] = "TCP Channel Binary KEK";
            //pro["priority"] = "17";
            //pro["port"] = "9000";
            //BinaryClientFormatterSinkProvider snkPrvd2 = new BinaryClientFormatterSinkProvider();
            ////snkPrvd2.TypeFilterLevel = TypeFilterLevel.Full;
            //TcpClientChannel Channel = new TcpClientChannel(pro, snkPrvd2);
            //ChannelServices.RegisterChannel(Channel, false);

            
            RemotingConfiguration.Configure("Pingpong.exe.config", false);
            game = new Game();
            //game = (Game)Activator.GetObject(typeof(Game), "tcp://localhost:9000/Game/Gameee");
            player = game.Connect();
            player2 = game.Connect();

            game.UpdateInfoHandle += new UpdateInfoEvent(onUpdateInfo);
        }

        private void onUpdateInfo(UpdateInfo updateInfo)
        {
            if (pb_Player.InvokeRequired)
            {
                pb_Player.Invoke(new MethodInvoker(delegate { pb_Player.Location = new Point(player.X, player.Y); }));
            }
            if (pb_Enemy.InvokeRequired)
            {
                pb_Enemy.Invoke(new MethodInvoker(delegate { pb_Enemy.Location = new Point(player2.X, player2.Y); }));
            }
            if (pb_Ball.InvokeRequired)
            {
                pb_Ball.Invoke(new MethodInvoker(delegate { pb_Ball.Location = new Point(game.Ball.X, game.Ball.Y); }));
            }
        }

        // Это, вроде, для очков
        PictureBox[] Score_Player = new PictureBox[5];  
        PictureBox[] Score_Enemy = new PictureBox[5];   
        Color ScoreColor = Color.Silver;

        public void PaintBox(int X, int Y, int W, int H, Color C)
        {
            //PictureBox Temp = new PictureBox();
            //Temp.BackColor = C;
            //Temp.Size = new Size(W, H);
            //Temp.Location = new Point(X-W/2, Y-H/2);
            //WorldFrame.Controls.Add(Temp);
        }

        public void CircleThis(PictureBox pic)
        {
            System.Drawing.Drawing2D.GraphicsPath gp = new System.Drawing.Drawing2D.GraphicsPath();
            gp.AddEllipse(pic.Width/2, pic.Height/2, pic.Width, pic.Height);
            Region rg = new Region(gp);
            pic.Region = rg;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)      //Regular key input, if press the right keys it moves in its direction
            {
                case Keys.Up:
                    player.ChangePosition("up");
                    break;
                case Keys.Down:
                    player.ChangePosition("down");
                    break;
                case Keys.W:
                    player2.ChangePosition("up");
                    break;
                case Keys.S:
                    player2.ChangePosition("down");
                    break;
                case Keys.Space:    //If hit space it starts the game,
                    game.SetStatus("playing");
                    break;
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            pb_Ball.Location = new Point(game.Ball.X, game.Ball.Y);
            //CircleThis(pb_Ball);
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
                
                label_Start.Visible = true;
                RestoreScore();
                pb_Ball.Location = new Point(208, 80);
                pb_Player.Location = new Point(8, 67);
                pb_Enemy.Location = new Point(409, 67);
                
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
            //if (GameOn)
            //{
            //    Round++;
            //    label_Time.Visible = true;

            //    TimeSpan time = TimeSpan.FromSeconds(Round);

            //    string str = time.ToString(@"mm\:ss");
            //    label_Time.Text = "Time: " + str;
            //}
        }


        private void timer_Moveball_Tick(object sender, EventArgs e)
        {

        }


    }
}
