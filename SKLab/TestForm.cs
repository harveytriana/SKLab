// Sample by: Harbey Triana @harveytriana@gmail.com
//
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using SkiaSharp;

namespace SKLab
{
    /// <summary>
    /// smart plot learning SKLab
    /// </summary>
    public partial class TestForm : Form
    {
        float _w = 220, _h = 120;    // plot size in pixels
        float _left = 15, _top = 15; // plot offset in pixels
        float _x1 = -4, _x2 = 8;     // x axis cartesian
        float _y1 = -2, _y2 = 2;     // y axis cartesian
        float _stepX = 0.5F;         // x points step cartesian

        // pixel from cartesian    
        float PixelFromX(float x) => _w * (x - _x1) / (_x2 - _x1) + _left;
        float PixelFromY(float y) => _h * (y - _y1) / (_y2 - _y1) + _top;

        public TestForm()
        {
            InitializeComponent();
            SKPlot(pictureBox1);
            FKPlot(pictureBox2);
        }

        /// <summary>
        /// Drawing with SkiaSharp
        /// </summary>
        private void SKPlot(PictureBox p)
        {
            using (var surface = SKSurface.Create(new SKImageInfo(p.Width, p.Height))) {
                SKCanvas g = surface.Canvas;

                // curve points
                var points = new List<SKPoint>();
                for (float x = _x1; x <= _x2; x += _stepX) {
                    points.Add(new SKPoint
                    {
                        X = PixelFromX(x),
                        Y = PixelFromY((float)Math.Sin(x))
                    });
                }

                // draw it
                g.Clear(SKColors.White);

                using (var pen = new SKPaint()) {
                    // curve style
                    pen.Style = SKPaintStyle.Stroke;
                    pen.IsAntialias = true;
                    pen.Color = SKColors.CornflowerBlue;
                    pen.StrokeWidth = 2;

                    // draw curve
                    var splitLine = SmootherLine.CreateSpline(points.ToArray());
                    g.DrawPath(splitLine, pen);

                    // border
                    pen.IsAntialias = false;
                    pen.StrokeWidth = 1;
                    pen.Color = SKColors.LightGray;
                    g.DrawRect(_left, _top, _w, _h, pen);
                }
                // display the result in PictureBox
                using (SKImage image = surface.Snapshot())
                using (SKData data = image.Encode(SKEncodedImageFormat.Png, 100))
                using (MemoryStream mStream = new MemoryStream(data.ToArray())) {
                    p.Image = new Bitmap(mStream, false);
                }
            }
        }

        /// <summary>
        /// Drawing with .NET Framework
        /// </summary>
        void FKPlot(PictureBox p)
        {
            var canvas = new Bitmap(p.ClientSize.Width, p.ClientSize.Height);

            // curve points
            var points = new List<PointF>();
            for (float x = _x1; x <= _x2; x += _stepX) {
                points.Add(new PointF
                {
                    X = PixelFromX(x),
                    Y = PixelFromY((float)Math.Sin(x))
                });
            }
            // draw it
            using (var g = Graphics.FromImage(canvas)) {
                g.Clear(Color.White);

                // draw curve
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using (var pen = new Pen(Color.CornflowerBlue, 2)) {
                    g.DrawCurve(pen, points.ToArray());
                }
                g.DrawRectangle(Pens.LightGray, _left, _top, (int)_w, (int)_h);
            }
            // display the result
            p.Image = canvas;
        }
    }
}
