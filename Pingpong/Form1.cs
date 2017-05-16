using System;
using System.Runtime.Remoting;
using System.Drawing;
using System.Windows.Forms;
using gamelogic;
using System.Timers;
using System.Runtime.Remoting.Lifetime;

namespace Pingpong
{
    public partial class Form1 : Form
    {
        Game game;
        Player player;
        System.Timers.Timer TickTimer;

        public Form1()
        {
            try
            {
                InitializeComponent();
                this.KeyPreview = true;

                RemotingConfiguration.Configure("Pingpong.exe.config", false);
                
                game = (Game)Activator.GetObject(typeof(Game), "tcp://localhost:8000/Game/Gameee");
                player = game.Connect();

                ILease lease_1 = (ILease)game.GetLifetimeService();
                MyClientSponsor sponsor = new MyClientSponsor();
                lease_1.Register(sponsor);

                TickTimer = new System.Timers.Timer(5);
                TickTimer.Elapsed += onUpdateInfo;
                TickTimer.AutoReset = true;
                TickTimer.Enabled = true;
            }
            catch (System.Net.Sockets.SocketException e)
            {
                label_Start.Location = new Point(71, 217);
                label_Start.Text = "Сервер не обнаружен. Нажмите ESC для выхода.";
            }
        }

        public void onUpdateInfo(Object source, ElapsedEventArgs e)
        {
            try
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
                    label_Start.Invoke(new MethodInvoker(delegate
                    {
                        if (game.status != "playing")
                        {
                            label_Start.Show();
                        }
                        else
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
                            Text = Text.PadLeft(65);
                        }));
                    }
                }
            }
            catch (System.Net.Sockets.SocketException ee)
            {
                TickTimer.Enabled = false;
                TickTimer.AutoReset = false;
            }
            catch
            {
                game.Disconnect(player);
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
            try
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
                        if (game.getPlayer(1) == null)
                        {
                            game.SetStatus("waiting2player");
                            this.label_Start.Text = "Второй игрок не подключен";
                            this.label_Start.Visible = true;
                        }
                        else
                        {
                            game.SetStatus("playing");
                            this.label_Start.Visible = false;
                        }
                        break;
                    default:
                        if(this.label_Start.Text == "Сервер не обнаружен. Нажмите ESC для выхода.")
                        {
                            Close();
                        }
                        break;
                }
            }
            catch
            {
                Console.WriteLine("Here1");
                Close();
            }
        }
 
        private void Form1_Load(object sender, EventArgs e)
        {
            pb_Ball.Location = new Point(game.Ball.X, game.Ball.Y);
            CircleThis(pb_Ball);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.label_Start.Text != "Сервер не обнаружен. Нажмите ESC для выхода.")
            {
                game.SetStatus("finish");
                game.Disconnect(player);
            }
        }

    }

    public class MyClientSponsor : MarshalByRefObject, ISponsor
    {
        private DateTime lastRenewal;

        public MyClientSponsor()
        {
            Console.WriteLine("MyClientSponsor.ctor called");
            lastRenewal = DateTime.Now;
        }

        public TimeSpan Renewal(ILease lease)
        {
            Console.WriteLine("I've been asked to renew the lease.");
            Console.WriteLine("Time since last renewal:" + (DateTime.Now - lastRenewal).ToString());

            lastRenewal = DateTime.Now;
            return TimeSpan.FromSeconds(20);
        }
    }
}
