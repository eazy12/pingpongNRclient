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
using System.Timers;

namespace Pingpong
{
    public partial class Form1 : Form
    {
        Game game;
        Player player;

        static void ShowChannelProperties(IChannelReceiver channel)
        {
            Console.WriteLine("Name: " + channel.ChannelName);
            Console.WriteLine("Priority: " + channel.ChannelPriority);

            ChannelDataStore data = (ChannelDataStore)channel.ChannelData;
            if (data != null)
            {
                foreach (string uri in data.ChannelUris)
                {
                    Console.WriteLine("URI: " + uri);
                }
            }
        }
        public static void ShowWellKnownServiceTypes()

        {

            WellKnownServiceTypeEntry[] entries =

                RemotingConfiguration.GetRegisteredWellKnownServiceTypes();

            foreach (var entry in entries)

            {

                Console.WriteLine("Assembly: {0}", entry.AssemblyName);

                Console.WriteLine("Mode: {0}", entry.Mode);

                Console.WriteLine("URI: {0}", entry.ObjectUri);

                Console.WriteLine("Type: {0}", entry.TypeName);

            }

        }

        public Form1()
        {
            InitializeComponent();
            this.KeyPreview = true;

            RemotingConfiguration.Configure("Pingpong.exe.config", false);
            ShowWellKnownServiceTypes();
            foreach (var i in System.Runtime.Remoting.Channels.ChannelServices.RegisteredChannels)
            {
                Console.WriteLine(i.ChannelName, i);
            }

            game = (Game)Activator.GetObject(typeof(Game), "tcp://localhost:8000/Game/Gameee");
            player = game.Connect();

            System.Timers.Timer TickTimer = new System.Timers.Timer(5);
            TickTimer.Elapsed += onUpdateInfo;
            TickTimer.AutoReset = true;
            TickTimer.Enabled = true;
        }

        public void onUpdateInfo(Object source, ElapsedEventArgs e)
        {
            if (pb_Player.InvokeRequired)
            {
                if (game.getPlayer(0) != null)
                {
                    pb_Player.Invoke(new MethodInvoker(delegate { pb_Player.Location = new Point(game.getPlayer(0).X, game.getPlayer(0).Y); }));
                }
            }

            if (pb_Enemy.InvokeRequired)
            {
                if (game.getPlayer(1) != null)
                {
                    pb_Enemy.Invoke(new MethodInvoker(delegate { pb_Enemy.Location = new Point(game.getPlayer(1).X, game.getPlayer(1).Y); }));
                }
            }

            if (pb_Ball.InvokeRequired)
            {
                pb_Ball.Invoke(new MethodInvoker(delegate { pb_Ball.Location = new Point(game.Ball.X, game.Ball.Y); }));
            }

            if (label_Start.InvokeRequired)
            {
                label_Start.Invoke(new MethodInvoker(delegate {
                    if (game.status != "playing")
                    {
                        label_Start.Show();
                    } else
                    {
                        label_Start.Hide();
                    }
                }));
            }

            if (InvokeRequired)
            {
                if (game.getPlayer(0) != null && game.getPlayer(1) != null)
                {
                    Invoke(new MethodInvoker(delegate
                    {
                        Text = String.Format("Ping Pong score: {0} - {1}", game.getPlayer(0).Score, game.getPlayer(1).Score);
                    }));
                }
            }
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
                case Keys.Space:    
                    game.SetStatus("playing");
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
