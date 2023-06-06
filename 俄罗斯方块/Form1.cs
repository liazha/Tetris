﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Tetris.Properties;
using System.Threading;
using System.Threading.Tasks;

namespace Tetris
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            levelForm = new Form2();
            overForm = new Form3();
            levelForm.MyRussia = MyRussia;
            MyRussia.overForm = overForm;
            MyRussia.Label_Linage = label3;//将label3控件加载到Russia类中
            MyRussia.Label_Fraction = label4;//将label4控件加载到Russia类中
            MyRussia.Label_Level = label6;//将label5控件加载到Russia类中
            MyRussia.backgroundMusic = this.backgroundMusic;
        }

        Form2 levelForm;
        Form3 overForm;
        Russia MyRussia = new Russia();//实例化Russia类，用于操作游戏
        Russia TemRussia = new Russia();//实例化Russia类，用于生成下一个方块样式
        public static int CakeNO = 0;//记录下一个方块样式的标识
        public static bool become = false;//判断是否生成下一个方块的样式
        public static bool isbegin = false;//判断当前游戏是否开始
        public bool ispause = true;//判断是否暂停游戏
        public System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        SoundPlayer backgroundMusic = new SoundPlayer(Resources.Tetris);//背景音乐
        public static int initInterval = 500;
        public static int minInterval = 100;


        [DllImport("winmm.dll")]
        private static extern long mciSendString(
            string command,      //MCI命令字符串
            string returnString, //存放反馈信息的缓冲区
            int returnSize,      //缓冲区的长度
            IntPtr hwndCallback  //回调窗口的句柄，一般为NULL
        );

        public static void playVoice(string file)
        {
            //使用委托的方式才能正常播放音效
            Thread td = new Thread((ThreadStart)delegate
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resource/" + file);
                mciSendString("open " + path + " type mpegvideo alias temp_alias", null, 0, IntPtr.Zero);
                mciSendString("play temp_alias wait", null, 0, IntPtr.Zero);
                mciSendString("close temp_alias ", null, 0, IntPtr.Zero);
            });
            td.SetApartmentState(ApartmentState.STA);
            td.IsBackground = true;
            td.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button3.Enabled = false;
            MyRussia.ConvertorClear();//清空整个控件
            MyRussia.firstPoi = new Point(140, 20);//设置方块的起始位置
            label3.Text = "0";//显示去除的行数
            label4.Text = "0";//显示分数
            
            timer1.Enabled = false;//停止计时
            timer1.Enabled = true;//开始计时
            Random rand = new Random();//实例化Random
            CakeNO = rand.Next(1, 8);//获取随机数
            MyRussia.CakeMode(CakeNO);//设置方块的样式
            MyRussia.Protract(panel1);//绘制组合方块
            beforehand();//生成下一个方块的样式
            MyRussia.PlaceInitialization();//初始化Random类中的信息
            isbegin = true;//判断是否开始
            ispause = true;
            MyRussia.timer = timer1;
            Task.Run(() =>
            {
                backgroundMusic.PlayLooping();
            });

            button2.Text = "暂停";
            ispause = true;
            textBox1.Focus();//获取焦点
        }

        /// <summary>
        /// 生成下一个方块的样式
        /// </summary>
        public void beforehand()
        {
            Graphics P3 = panel3.CreateGraphics();
            P3.FillRectangle(new SolidBrush(Color.Black), 0, 0, panel3.Width, panel3.Height);
            Random rand = new Random();//实例化Random
            CakeNO = rand.Next(1, 9);//获取随机数
            TemRussia.firstPoi = new Point(50, 30);//设置方块的起始位置
            TemRussia.CakeMode(CakeNO);//设置方块的样式
            TemRussia.Protract(panel3);//绘制组合方块
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (!isbegin)//如果没有开始游戏
                return;
            if (!ispause)//如果游戏暂停
                return;
            if (e.KeyCode == Keys.Up)//如果当前按下的是↑键
                MyRussia.MyConvertorMode();//变换当前方块的样式
            if (e.KeyCode == Keys.Down)//如果当前按下的是↓键
            {
                int interval = MyRussia.GetTimerInterval() - 200;
                timer1.Interval = interval > minInterval ? interval : minInterval;//增加下移的速度
                MyRussia.ConvertorMove(0);//方块下移
            }
            if (e.KeyCode == Keys.Left)//如果当前按下的是←键
                MyRussia.ConvertorMove(1);//方块左移
            if (e.KeyCode == Keys.Right)//如果当前按下的是→键
                MyRussia.ConvertorMove(2);//方块右移
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Interval = MyRussia.GetTimerInterval();
            MyRussia.ConvertorMove(0);//方块下移
            if (become)//如果显示新的方块
            {
                beforehand();//生成下一个方块
                become = false;
            }
            textBox1.Focus();//获取焦点
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (!isbegin)//如果游戏没有开始
                return;
            if (!ispause)//如果暂停游戏
                return;
            if (e.KeyCode == Keys.Down)//如果当前松开的是↓键
            {
                timer1.Interval = MyRussia.GetTimerInterval();//恢复下移的速度
            }
            playVoice("PressKey.wav");
            textBox1.Focus();//获取焦点
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (timer1.Enabled == true)
            {
                timer1.Stop();//暂停
                button2.Text = "继续";
                ispause = false;
                backgroundMusic.Stop();
                button3.Enabled = true;
                textBox1.Focus();//获取焦点
            }
            else
            {
                timer1.Start();//继续
                button2.Text = "暂停";
                ispause = true;
                backgroundMusic.PlayLooping();
                textBox1.Focus();//获取焦点
            }

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            if (isbegin)//如是游戏开始
            {
                //重绘背景上的方块
                for (int i = 0; i <= (panel1.Width / 20 - 1); i++)
                {
                    for (int j = 0; j <= (panel1.Height / 20 - 1); j++)
                    {
                        Rectangle rect = new Rectangle(i * 20 + 1, j * 20 + 1, 19, 19);//获取各方块的绘制区域
                        e.Graphics.FillRectangle(new SolidBrush(Russia.PlaceColor[i, j]), rect);//绘制方块
                    }
                }
            }
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {
            if (isbegin)//如果游戏开始
            {
                TemRussia.firstPoi = new Point(50, 30);//设置方块的起始位置
                TemRussia.CakeMode(CakeNO);//设置方块的样式
                TemRussia.Protract(panel3);//绘制组合方块
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (timer.Enabled == false)
            {
                levelForm.ShowDialog();
            }
        }
    }
}
