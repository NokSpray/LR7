using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace LR7
{
    // Flyweight
    public class Snowflake
    {
        public string Shape { get; }
        public Color Color { get; }
        public int Size { get; }

        public Snowflake(string shape, Color color, int size)
        {
            Shape = shape;
            Color = color;
            Size = size;
        }

        public void Draw(Graphics g, float x, float y)
        {
            using (Brush brush = new SolidBrush(Color))
            {
                g.FillEllipse(brush, x, y, Size, Size);
            }
        }
    }

    // Flyweight Factory
    public class SnowflakeFactory
    {
        private readonly Dictionary<string, Snowflake> _snowflakes = new();

        public Snowflake GetSnowflake(string shape, Color color, int size)
        {
            string key = $"{shape}_{color}_{size}";

            if (!_snowflakes.ContainsKey(key))
            {
                _snowflakes[key] = new Snowflake(shape, color, size);
            }

            return _snowflakes[key];
        }
    }

    // Snowflake Instance
    public class SnowflakeInstance
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Speed { get; set; }
        public Snowflake Snowflake { get; }

        public SnowflakeInstance(float x, float y, float speed, Snowflake snowflake)
        {
            X = x;
            Y = y;
            Speed = speed;
            Snowflake = snowflake;
        }
    }

    // MainForm
    public partial class Form1 : Form
    {

        private readonly SnowflakeFactory _factory = new();
        private readonly List<SnowflakeInstance> _snowflakes = new();

        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            _timer.Interval = 16; // ~60 FPS
            _timer.Tick += (s, e) => UpdateSnow();
            _timer.Start();
            GenerateSnowflakes();
        }
        private void btnStartStop_Click(object sender, EventArgs e)
        {
            _timer.Enabled = !_timer.Enabled;
        }

        private void GenerateSnowflakes()
        {
            Random rand = new();
            for (int i = 0; i < 100; i++)
            {
                var snowflake = _factory.GetSnowflake(
                    "Circle",
                    Color.FromArgb(rand.Next(150, 255), 255, 255, 255),
                    rand.Next(5, 15)
                );
                _snowflakes.Add(new SnowflakeInstance(
                    rand.Next(0, this.ClientSize.Width),
                    rand.Next(-500, 0),
                    (float)rand.NextDouble() * 3 + 1,
                    snowflake
                ));
            }
        }

        private void UpdateSnow()
        {
            foreach (var snowflake in _snowflakes)
            {
                snowflake.Y += snowflake.Speed;
                if (snowflake.Y > this.ClientSize.Height)
                    snowflake.Y = -10; // Reset to the top
            }
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            foreach (var snowflake in _snowflakes)
            {
                snowflake.Snowflake.Draw(e.Graphics, snowflake.X, snowflake.Y);
            }
        }
    }
}

