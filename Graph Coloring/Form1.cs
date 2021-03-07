using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Graph_Coloring
{
    public partial class Form1 : Form
    {
        void solve()
        {
            // reset the color list
            graph = new List<int>();
            for (int i = 0; i < pos.Count; i++) graph.Add(0);


            //creates a array with all connections each node have
            List<int>[] con2 = new List<int>[graph.Count()];

            for (int i = 0; i < con2.Length; i++)
                con2[i] = new List<int>();

            for (int i = 0; i < con.Count; i++)
            {
                con2[con[i][1]].Add(con[i][0]);
                con2[con[i][0]].Add(con[i][1]);
            }



            //color the graph
            bool busy = true;
            int c = 0;
            while (busy)
            {
                int[] triangles = new int[graph.Count()];

                for (int i = 0; i < triangles.Count(); i++)
                {
                    for (int j = 0; j < con2[i].Count(); j++)
                    {
                        for (int k = 0; k < con2[con2[i][j]].Count(); k++)
                        {
                            if (con2[con2[i][j]][k] == i) continue;
                            for (int l = 0; l < con2[con2[con2[i][j]][k]].Count(); l++)
                            {
                                if (con2[con2[con2[i][j]][k]][l] == i)
                                {
                                    triangles[i]++;
                                }
                            }
                                
                        }
                    }
                }
                List<int> priority = new List<int>();
                List<int> priority_id = new List<int>();

                for (int i = 0; i < triangles.Count(); i++)
                {
                    var m = priority.Mind(triangles[i]);
                    priority.Match(m, triangles[i]);
                    priority_id.Match(m, i);
                }

                
                busy = false;

                for (int i = priority_id.Count() - 1; i >= 0; i--)
                {
                    active(priority_id[i]);
                    priority = new List<int>();
                    priority_id = new List<int>();
                    for (int j = 0; j < triangles.Count(); j++)
                    {
                        var mi = priority.Mind(triangles[j]);
                        priority.Match(mi, triangles[j]);
                        priority_id.Match(mi, j);
                    }
                }
                void active(int m)
                {
                    if (graph[m] == c)
                    {
                        List<int> pass = new List<int>();
                        for (int j = 0; j < con2[m].Count(); j++)
                        {
                            if (graph[con2[m][j]] == c)
                            {
                                con2[con2[m][j]].Remove(m);
                                for (int k = 0; k < con2[m].Count(); k++)
                                {
                                    for (int l = 0; l < con2[con2[m][k]].Count(); l++)
                                    {
                                        if (con2[con2[m][k]][l] == con2[m][j])
                                            triangles[con2[m][j]]--;
                                    }
                                }
                                graph[con2[m][j]]++;
                                pass.Add(con2[m][j]);
                                busy = true;
                            }
                        }
                        List<int> active_priority = new List<int>();
                        List<int> active_priority_id = new List<int>();
                        for (int j = 0; j < pass.Count(); j++)
                        {
                            var mi = active_priority.Mind(triangles[pass[j]]);
                            active_priority.Match(mi, triangles[pass[j]]);
                            active_priority_id.Match(mi, j);
                        }
                        for (int j = active_priority_id.Count()-1; j >= 0; j--)
                        {
                            passive(active_priority_id[j]);
                        }
                    }
                }
                void passive(int m)
                {
                    List<int> passive_priority = new List<int>();
                    List<int> passive_priority_id = new List<int>();
                    for (int i = 0; i < con2[m].Count(); i++)
                    {
                        var mi = passive_priority.Mind(triangles[con2[m][i]]);
                        passive_priority.Match(mi, triangles[con2[m][i]]);
                        passive_priority_id.Match(mi, i);
                    }
                    for (int i = passive_priority_id.Count()-1; i >= 0 ; i--)
                    {
                        active(passive_priority_id[i]);
                    }
                }
                max = c;
                true_max = c;
                c++;
            }
        }
        public Form1()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
            InitializeComponent();
            Timer aTimer = new Timer();
            aTimer.Tick += new EventHandler(OnTimedEvent);
            aTimer.Interval = 1;
            aTimer.Enabled = true;
            aTimer.Start();
            if (!File.Exists("save.bin"))
                using (var a = File.Create("save.bin"))
                using (var b = new BinaryWriter(a))
                {
                    b.Write(0);
                    b.Write(0);
                }
            load();

        }
        void save()
        {
            using (var a = File.OpenWrite("save.bin"))
            using (var b = new BinaryWriter(a))
            {
                b.Write(pos.Count());
                foreach (var i in pos)
                {
                    b.Write(i.X);
                    b.Write(i.Y);
                }
                b.Write(con.Count());
                foreach (var i in con)
                {
                    b.Write(i[0]);
                    b.Write(i[1]);
                }

            }
        }
        void load()
        {
            pos = new List<Point>();
            con = new List<int[]>();
            using (var a = File.OpenRead("save.bin"))
            using (var b = new BinaryReader(a))
            {
                var len = b.ReadInt32();
                for (int i = 0; i < len; i++)
                {
                    pos.Add(new Point(b.ReadInt32(), b.ReadInt32()));
                }
                len = b.ReadInt32();
                for (int i = 0; i < len; i++)
                {
                    con.Add(new int[] { b.ReadInt32(), b.ReadInt32() });
                }
            }
            solve();
        }

        //data
        Brush[] color =
        {
                Brushes.Red,
                Brushes.Blue,
                Brushes.Lime,
                Brushes.Cyan,
                Brushes.Yellow,
                Brushes.Magenta,
                Brushes.Orange,
                Brushes.RoyalBlue,
                Brushes.Purple,
                Brushes.Gray,
                Brushes.Black,
                Brushes.White,
        };
        List<int> graph;
        List<Point> pos;
        List<int[]> con;
        int cap(int v, int max) => v > max ? max : v;
        private void Form1_Paint(object sender, PaintEventArgs e)
        {

            var g = e.Graphics;
            g.FillRectangle(Brushes.Black, new Rectangle(0, 0, 640, 480));

            Point m = new Point(Cursor.Position.X - Left - 8, Cursor.Position.Y - Top - 30);

            if (drag)
            {
                pos[drag_id] = new Point(m.X + drag_relative.X, m.Y + drag_relative.Y);
            }
            if (connect)
            {
                g.DrawLine(Pens.White, pos[drag_id], m);
            }
            for (int i = 0; i < con.Count; i++)
            {
                g.DrawLine(Pens.White, pos[con[i][0]], pos[con[i][1]]);
            }
            for (int i = 0; i < pos.Count; i++)
            {
                var ret = new Rectangle(pos[i].X - 16 / 2, pos[i].Y - 16 / 2, 16, 16);
                g.FillEllipse(color[cap(graph[i],max)], ret);
                g.DrawEllipse(Pens.White, ret);
                //g.DrawString(i.ToString(),SystemFonts.CaptionFont, Brushes.Cyan, pos[i].X+8, pos[i].Y+8);
            }
        }
        bool drag = false;
        int drag_id;
        void OnTimedEvent(object source, EventArgs e)
        {
            Refresh();
        }
        int max = 0;
        int true_max = 0;
        bool dist(int d, int i, int x, int y)
            => (Math.Pow(pos[i].X - x, 2) + Math.Pow(pos[i].Y - y, 2)) < (d * d);
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            int x = Cursor.Position.X - Left - 8;
            int y = Cursor.Position.Y - Top - 30;
            void create()
            {
                pos.Add(new Point(x, y));
                graph.Add(0);
                drag = true;
                int cur = pos.Count() - 1;
                drag_relative.X = 0;
                drag_relative.Y = 0;
                drag_id = cur;
                solve();
            }
            switch (e.Button)
            {
                case MouseButtons.Left:

                    for (int i = 0; i < pos.Count; i++)
                    {
                        if (dist(16, i, x, y))
                        {
                            drag = true;
                            drag_id = i;
                            drag_relative.X = pos[i].X - x;
                            drag_relative.Y = pos[i].Y - y;
                            return;
                        }
                    }
                    create();

                    break;
                case MouseButtons.Middle:

                    for (int i = 0; i < pos.Count; i++)
                    {
                        if (dist(16, i, x, y))
                        {
                            connect = true;
                            drag_id = i;
                            return;
                        }

                    }
                    create();

                    break;
                case MouseButtons.Right:
                    for (int i = 0; i < pos.Count; i++)
                    {
                        if (dist(16, i, x, y))
                        {
                            for (int j = 0; j < con.Count;)
                            {
                                var jj = con[j];
                                var a = jj[0].CompareTo(i);
                                var b = jj[1].CompareTo(i);
                                if (a == 0 || b == 0) con.RemoveAt(j);
                                else
                                {
                                    if (a > 0) jj[0]--;
                                    if (b > 0) jj[1]--;
                                    j++;
                                }
                            }
                            pos.RemoveAt(i);
                            solve();
                            return;
                        }
                    }
                    break;
            }

        }
        bool connect = false;
        Point drag_relative;

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            int x = Cursor.Position.X - Left - 8;
            int y = Cursor.Position.Y - Top - 30;
            drag = false;
            if (connect)
            {
                for (int i = 0; i < pos.Count; i++)
                {
                    if (dist(16, i, x, y))
                    {
                        if (drag_id == i) continue;
                        con.Add(new int[] { drag_id, i });
                        solve();
                        connect = false;
                        return;
                    }
                }
                connect = false;
                pos.Add(new Point(x, y));
                graph.Add(0);
                int cur = pos.Count() - 1;
                drag_relative.X = 0;
                drag_relative.Y = 0;
                con.Add(new int[] { cur, drag_id });
                solve();
            }

        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F5: save(); break;
                case Keys.F9: load(); break;
                case Keys.Right: max=max<true_max?max+1:true_max; Refresh(); break;
                case Keys.Left: max=max>0?max-1:0; Refresh(); break;
                
            }
        }
    }
}
