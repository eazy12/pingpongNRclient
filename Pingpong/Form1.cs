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
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(delegate {
                    Text = String.Format("Ping Pong score: {0} - {1}", player.Score, player2.Score);
                }));
            }

        }

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
            gp.AddEllipse(0, 0, pic.Width, pic.Height);
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
