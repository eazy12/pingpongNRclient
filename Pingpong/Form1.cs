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
        Game game;
        Player player;
        Player player2;

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
            switch (e.KeyCode)   
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
                case Keys.Space:    
                    game.SetStatus("playing");
                    label_Start.Hide();
                    break;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            pb_Ball.Location = new Point(game.Ball.X, game.Ball.Y);
            CircleThis(pb_Ball);
        }  
    }
}
