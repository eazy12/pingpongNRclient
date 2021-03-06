﻿using System;
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

        bool isFirstHitSpace = true;

        public Form1()
        {
            try
            {
                InitializeComponent();
                this.KeyPreview = true;

                RemotingConfiguration.Configure("Pingpong.exe.config", false);

                //ILease lease_1 = (ILease)game.GetLifetimeService();
                //MyClientSponsor sponsor = new MyClientSponsor();
                //lease_1.Register(sponsor);

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
                    Player pl = game.getPlayer(0);
                    if (pl != null)
                   {
                        pb_Player.Invoke(new MethodInvoker(delegate { pb_Player.Location = new Point(pl.X, pl.Y); }));
                    }
                }
               

                if (pb_Enemy.InvokeRequired)
                {
                    Player pl = game.getPlayer(1);
                    if (pl != null)
                    {
                        pb_Enemy.Invoke(new MethodInvoker(delegate { pb_Enemy.Location = new Point(pl.X, pl.Y); }));
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

                if (menuStrip1.InvokeRequired)
                {
                    menuStrip1.Invoke(new MethodInvoker(delegate
                    {
                        if (game.status != "playing")
                        {
                            menuStrip1.Show();
                        }
                        else
                        {
                            menuStrip1.Hide();
                        }
                    }));
                }

                if (InvokeRequired)
                {
                    Player pl1 = game.getPlayer(0);
                    Player pl2 = game.getPlayer(1);

                    if (pl1 != null && pl2 != null)
                    {
                        Invoke(new MethodInvoker(delegate
                        {
                            Text = String.Format("Ping Pong score: {0} - {1}", pl1.Score, pl2.Score);
                            //Text = Text.PadLeft(65);
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
                        if (!isFirstHitSpace)
                        {
                            player.ChangePosition("up");
                        }
                        break;
                    case Keys.Down:
                        if (!isFirstHitSpace)
                        {
                            player.ChangePosition("down");
                        }
                        break;
                    case Keys.Space:
                        try
                        {
                            if (isFirstHitSpace)
                            {
                                game = (Game)Activator.GetObject(typeof(Game), "tcp://" + FormConnection.GetAddress() + ":8000/Game/Gameee");
                                Console.WriteLine(FormConnection.GetAddress() );
                                player = game.Connect(FormNickname.GetNickName());
                                TickTimer = new System.Timers.Timer(5);
                                TickTimer.Elapsed += onUpdateInfo;
                                TickTimer.AutoReset = true;
                                TickTimer.Enabled = true;

                                pb_Ball.Location = new Point(game.Ball.X, game.Ball.Y);
                                CircleThis(pb_Ball);
                            }
                       
                        isFirstHitSpace = false;
                        
                        if (game.getPlayer(1) == null)
                        {
                            game.SetStatus("waiting2player");
                            this.label_Start.Text = "Второй игрок не подключен";
                        }
                        else
                        {
                            game.SetStatus("playing");
                        }
                        
                        }
                        catch
                        {
                            Console.WriteLine(isFirstHitSpace);
                            label_Start.Text = "Сервер не обнаружен. Нажмите ESC для выхода.";
                        }
                        break;
                    case Keys.Escape:
                        if(this.label_Start.Text == "Сервер не обнаружен. Нажмите ESC для выхода.")
                        {
                            Close();
                        }
                        break;
                }
            }
            catch
            {
                Console.WriteLine(isFirstHitSpace);
                Close();
            }
        }
 
        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.label_Start.Text != "Сервер не обнаружен. Нажмите ESC для выхода.")
            {
                game.SetStatus("finish");
                game.Disconnect(player);
            }
        }

        private void подключениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormConnection form4 = new FormConnection();
            form4.ShowDialog();
        }

        private void имяИгрокаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormNickname form5 = new FormNickname();
            form5.ShowDialog();
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
