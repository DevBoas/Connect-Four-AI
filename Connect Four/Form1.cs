using Connect_Four.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;


// This is the code for your desktop app.
// Press Ctrl+F5 (or go to Debug > Start Without Debugging) to run your app.

namespace Connect_Four
{
    public partial class Form1 : Form
    {
        private int[][] jaggedArray3 =
        {
            new int[] { 0, 0, 0, 0, 0, 0 }, // stack 1
            new int[] { 0, 0, 0, 0, 0, 0 }, // stack 2
            new int[] { 0, 0, 0, 0, 0, 0 }, // stack 3
            new int[] { 0, 0, 0, 0, 0, 0 }, // stack 4
            new int[] { 0, 0, 0, 0, 0, 0 }, // stack 5
            new int[] { 0, 0, 0, 0, 0, 0 }, // stack 6
            new int[] { 0, 0, 0, 0, 0, 0 }, // stack 7
        };

        PictureBox ball = null;
        int loc = -1;
        int lastoffset = 0;
        int ballCounter = 0;
        int boardCounter = 0;
        int round = 0;

        public Form1()
        {
            InitializeComponent();
            CreateBall();
        }

        private void UpdateTurnLabel()
        {
            if (ballCounter % 2 == 0)
                Turn.Text = "Green's Turn!";
            else
                Turn.Text = "Yellow's Turn!";
        }

        private int TryMove(int index, int who, Boolean withSim)
        {
            int c = 0;
            int score = 0;

            //Horizontal score
            for (int j = 0; j < jaggedArray3[0].Length; j++)
            {
                c = 0;
                for (int i = 0; i < jaggedArray3.Length; i++)
                {
                    if (jaggedArray3[i][j] == who || (jaggedArray3[i][j] == 3 && withSim))
                    {
                        c++;
                        if (c > score)
                            score = c;
                    }
                    else
                        c = 0;
                }
            }
            return score;
        }

        private int StackHeight(int index)
        {
            int c = 0;
            for (int i = 0; i < jaggedArray3[index].Length; i++)
            {
                if (jaggedArray3[index][i] != 0)
                    c++;
                else
                    break;
            }
            return c;
        }

        private int Simulate(int index)
        {
            int Height = StackHeight(index);
            //MessageBox.Show("Going to simulate adding a ball to index " + index.ToString());
            //MessageBox.Show("Stack Height of index " + index.ToString() + " is " + Height.ToString());
            if (Height != 6)
            {
                jaggedArray3[index][Height] = 3;
                //MessageBox.Show("Index Height " + Height + " = 3 now");
                return 1;
            }
            return -1;
        }

        private void UnSimulate(int index)
        {
            int Height = StackHeight(index);
            jaggedArray3[index][Height - 1] = 0;
            //MessageBox.Show("Index Height " + (Height - 1) + " = 0 again");
        }

        private int getScore(int score, int index, int bestIndex)
        {
            int sim = Simulate(index);
            if (sim != -1)
            {
                int SimulationScoreEnemy = TryMove(index, 2, true);
                int SimulationScoreEnemyNoSim = TryMove(index, 2, false);
                int RealSimulationScoreEnemy = SimulationScoreEnemy - SimulationScoreEnemyNoSim;
                //MessageBox.Show("Enemy Sim Score for index" + index.ToString() + " = " + RealSimulationScoreEnemy);
                int SimlationScoreFriendly = TryMove(index, 1, true);
                //int SimlationScoreFriendlyNoSim = TryMove(index, 1, false);
                //int RealSimulationScoreFriendly = SimlationScoreFriendly - SimlationScoreFriendlyNoSim;
                //MessageBox.Show("Enemy Simulation score for index" + index.ToString() + " = " + SimulationScoreEnemy.ToString());
                //MessageBox.Show("Enemy Simulation score without simulation for index" + index.ToString() + " = " + SimulationScoreEnemyNoSim.ToString());
                //MessageBox.Show("Friendly Simulation score for index" + index.ToString() + " = " + SimlationScoreFriendly.ToString());

                //if (RealSimulationScoreFriendly >= RealSimulationScoreEnemy)
                    //RealSimulationScoreEnemy = RealSimulationScoreFriendly;

                if (SimulationScoreEnemy == 4)
                    RealSimulationScoreEnemy = 8888;

                if (SimlationScoreFriendly == 4)
                    RealSimulationScoreEnemy = 9999;

                if (RealSimulationScoreEnemy > score)
                {
                    score = RealSimulationScoreEnemy;
                    bestIndex = index;
                }

                UnSimulate(index);
            }
            if (index == (jaggedArray3.Length - 1))
            {
                //MessageBox.Show("Best score was" + score.ToString());
                return bestIndex;
            }
            index++;
            return getScore(score, index, bestIndex);
        }

        int ha = 0;
        private void ComputerMove(PictureBox newBall)
        {
            int GetPlay = getScore(0, 0, -1);
            //MessageBox.Show("The Best Play was at index" + GetPlay.ToString());

            int loc = GetPlay;
            if (GetPlay == -1)
            {
                Random rnd = new Random();
                int random = rnd.Next(0, 7);
                loc = random;
            }
            pictureBox1.Controls.Add(newBall);
            ballCounter++;
            //MessageBox.Show("H_Score = " + H_Score.ToString());
            newBall.Visible = true;
            int offset = 0;
            if (loc == 0 || loc == 1)
                offset = 1;
            else if (loc == 2 || loc == 3 || loc == 4)
                offset = 2;
            else if (loc == 6 || loc == 5)
                offset = 3;
            newBall.Location = new Point((pictureBox1.Location.X - offset) + (loc * 100), pictureBox1.Location.Y - 20);
            DropBall(newBall, loc);
            CheckGameState();
            if ((boardCounter != 0) && boardCounter == (jaggedArray3.Length * jaggedArray3[0].Length))
            {
                MessageBox.Show("Its a tie!");
                ResetBoard();
            }
            CreateBall();
        }

        private void DropBall(PictureBox KeepBall, int loc)
        {
            int height = InsertBall(loc);
            if (height == -1)
                return;
            boardCounter++;
            int offset = 0;
            offset = (height - 1) * 4;
            int goal = (pictureBox1.Size.Height - (KeepBall.Size.Height * height) - (14 * height) - offset);
            int steps = 16;
            while (KeepBall.Location.Y < goal)
            {
                int newY = KeepBall.Location.Y + (int)steps;
                if (newY > goal)
                    KeepBall.Location = new Point(KeepBall.Location.X, goal);
                else
                    KeepBall.Location = new Point(KeepBall.Location.X, KeepBall.Location.Y + steps);
                wait(1);
            }
        }

        private void CreateBall()
        {
            UpdateTurnLabel();
            PictureBox newBall = new PictureBox();
            newBall.Name = "Ball";
            newBall.SizeMode = PictureBoxSizeMode.AutoSize;
            if (ballCounter % 2 == 0)
            {
                newBall.Image = (Image)Resources.ResourceManager.GetObject("Green");
                newBall.MouseDown += PictureBox1_MouseDown;
                newBall.Visible = false;
                ball = newBall;
                pictureBox1.Controls.Add(newBall);
                ballCounter++;
                if ((loc == -1) || (CanPlace(loc) == -2))
                    return;
                newBall.Visible = true;
                newBall.Location = new Point((pictureBox1.Location.X - lastoffset) + (loc * 100), pictureBox1.Location.Y - 20);
            }
            else
            {
                newBall.MouseDown += PictureBox1_MouseDown;
                newBall.Image = (Image)Resources.ResourceManager.GetObject("Yellow");
                ComputerMove(newBall);
            }
            
        }

        private int CanPlace(int c)
        {
            if (c == -1 || (c != -1) && (c > 6))
                return -1;
            for (int i = 0; i < jaggedArray3[c].Length; i++)
                if (jaggedArray3[c][i] == 0)
                    return i;
            return -2;
        }

        private void PictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (ball == null)
                return;
            Control c = (sender as Control);
            int x = 0;
            int y = 0;
            if (c.Name =="pictureBox1")
            {
                x = pictureBox1.Location.X;
                y = pictureBox1.Location.Y;
            }
            if (e.Location.X >= pictureBox1.Location.X && e.Location.X <= pictureBox1.Location.X + pictureBox1.Size.Width)
                loc = (e.Location.X) / 100; 
            else
                loc = -1;
            if (CanPlace(loc) == -2 && ball != null)
            {
                ball.Visible = false;
                return;
            }
            if (loc != -1 && ball != null)
            {
                ball.Visible = true;
                int offset = 0;
                if (loc == 0 || loc == 1)
                    offset = 1;
                else if (loc == 2 || loc == 3 || loc == 4 )
                    offset = 2;
                else if (loc == 6 || loc == 5)
                    offset = 3;
                lastoffset = offset;
                ball.Location = new Point((pictureBox1.Location.X - offset) + (loc * 100), pictureBox1.Location.Y - 20);
            }
            else if (ball != null)
                ball.Visible = false;
            label1.Text = "loc "+ loc.ToString() + " MousePos X" + (e.Location.X + x).ToString() + " Y" + (e.Location.Y + y).ToString();
        }

        public static void wait(int milliseconds)
        {
            System.Windows.Forms.Timer timer1 = new System.Windows.Forms.Timer();
            if (milliseconds == 0 || milliseconds < 0) return;
            timer1.Interval = milliseconds;
            timer1.Enabled = true;
            timer1.Start();
            timer1.Tick += (s, e) =>
            {
                timer1.Enabled = false;
                timer1.Stop();
            };
            while (timer1.Enabled)
            {
                Application.DoEvents();
            }
        }

        private int InsertBall(int where)
        {
            if (where == -1 || (where != -1) && (where > 6))
                return -1;
            for (int i = 0; i < jaggedArray3[where].Length; i++)
            {
                if (jaggedArray3[where][i] == 0)
                {
                    if (ballCounter % 2 == 0)
                        jaggedArray3[where][i] = 1;
                    else
                        jaggedArray3[where][i] = 2;
                    return i+1;
                }
            }

            return -1;
        }

        private void ResetBoard()
        {
            round++;
            for (int i = 0; i < jaggedArray3.Length; i++)
                for (int j = 0; j < jaggedArray3[i].Length; j++)
                    jaggedArray3[i][j] = 0;
            boardCounter = 0;
            pictureBox1.Controls.Clear();
            ball = null;
            loc = -1;
            lastoffset = 0;
            ballCounter = round;
        }

        private void CheckGameState()
        {
            int lastInt;
            int c;
            int winner = 0;
            string how = "";

            //Horizontal
            for (int j = 0; j < jaggedArray3[0].Length; j++)
            {
                lastInt = -1;
                c = 1;
                for (int i = 0; i < jaggedArray3.Length; i++)
                {
                    if (jaggedArray3[i][j] != 0)
                    {
                        if (lastInt == jaggedArray3[i][j])
                            c++;
                        else
                            c = 1;
                        lastInt = jaggedArray3[i][j];
                    }
                    else
                    {
                        lastInt = jaggedArray3[i][j];
                        c = 1;
                    }
                    if (c == 4)
                    {
                        winner = 1;
                        //how = "Winner horizontal";
                    }
                }
            }
            //Vertical
            for (int i = 0; i < jaggedArray3.Length; i++)
            {
                lastInt = -1;
                c = 1;
                for (int j = 0; j < jaggedArray3[i].Length; j++)
                {
                    if (jaggedArray3[i][j] != 0)
                    {
                        if (lastInt == jaggedArray3[i][j])
                            c++;
                        else
                            c = 1;
                        lastInt = jaggedArray3[i][j];
                    }
                    else
                        c = 1;
                    if (c == 4)
                    {
                        winner = 1;//MessageBox.Show("Winner Vertical");
                        //how = "Winner Vertical";
                    }
                }
            }
            //diagonal
            for (int y = 0; y < jaggedArray3.Length; y++)
            {
                for (int x = 0; x < jaggedArray3[y].Length; x++)
                {
                    lastInt = -1;
                    c = 1;
                    int y_end_up = y + 3;
                    int x_end_up = x + 3;
                    int z = x;
                    if (y_end_up < jaggedArray3.Length && x_end_up < jaggedArray3[y].Length)
                    {
                        for (int i = y; i <= y_end_up; i++)
                        {
                            if (jaggedArray3[i][z] != 0)
                            {
                                if (lastInt == jaggedArray3[i][z])
                                    c++;
                                else
                                    c = 1;
                                lastInt = jaggedArray3[i][z];
                                z++;
                            }
                            else
                                break;
                        }
                        if (c == 4)
                        {
                            winner = 1; //MessageBox.Show("Winner diagonal up");
                            //how = "Winner diagonal up";
                        }
                    }
                    lastInt = -1;
                    c = 1;
                    int x_end_down = y + 3;
                    int y_end_down = x - 3;
                    if (y_end_down >= 0 && x_end_down < jaggedArray3.Length) 
                    {
                        z = x;
                        for (int i = y; i <= x_end_down; i++)
                        {
                            if (jaggedArray3[i][z] != 0)
                            {
                                if (lastInt == jaggedArray3[i][z])
                                    c++;
                                else
                                    c = 1;
                                if (c == 4)
                                {
                                    winner = 1;//MessageBox.Show("Winner diagonal down");
                                    //how = "Winner diagonal down";
                                }
                                lastInt = jaggedArray3[i][z];
                                z--;
                            }
                            else
                                break;
                        }
                    }
                }
            }
            if (winner == 1)
            {
                if (ballCounter % 2 == 0)
                {
                    MessageBox.Show(how + "Yellow Wins this round!");
                    int score = Convert.ToInt32(Score2.Text.Substring(8));
                    score++;
                    Score2.Text = Score2.Text.Substring(0, 8) + score.ToString();
                }
                else
                {
                    MessageBox.Show(how + "Green Wins this round!");
                    int score = Convert.ToInt32(Score1.Text.Substring(7));
                    score++;
                    Score1.Text = Score1.Text.Substring(0, 7) + score.ToString();
                }
                ResetBoard();
            }
        }

        private void PictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (ball == null)
                return;
            int height = InsertBall(loc);
            if (height == -1)
                return;
            boardCounter++;
            PictureBox KeepBall = ball;
            int offset = 0;
            ball = null;
            offset = (height - 1) * 4;
            int goal = (pictureBox1.Size.Height - (KeepBall.Size.Height * height) - (14 * height) - offset);
            int steps = 16;
            while (KeepBall.Location.Y < goal)
            {
                int newY = KeepBall.Location.Y + (int)steps;
                if (newY > goal)
                    KeepBall.Location = new Point(KeepBall.Location.X, goal);
                else
                    KeepBall.Location = new Point(KeepBall.Location.X, KeepBall.Location.Y + steps);
                wait(1);
            }
            CheckGameState();
            if ((boardCounter != 0) && boardCounter == (jaggedArray3.Length * jaggedArray3[0].Length))
            {
                MessageBox.Show("Its a tie!");
                ResetBoard();
            }
            CreateBall();
        }
    }
}
