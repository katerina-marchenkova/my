using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Shuruev.StyleCop.CSharp.Properties;

namespace Shuruev.StyleCop.CSharp.Common
{
	/// <summary>
	/// Graphically displays an example for selected rule.
	/// </summary>
	public partial class DisplayExample : UserControl
	{
		private static readonly Color s_borderColor = Color.FromArgb(118, 118, 118);

		private Image m_sample;
		private string m_exampleUrl;

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public DisplayExample()
		{
			InitializeComponent();
			Clear();
		}

		#region Event handlers

		private void DisplayExample_SizeChanged(object sender, EventArgs e)
		{
			InvalidateAll();
		}

		private void DisplayExample_BackColorChanged(object sender, EventArgs e)
		{
			UpdateCurlSample();
			InvalidateAll();
		}

		private void pictureExample_Paint(object sender, PaintEventArgs e)
		{
			if (m_sample == null)
				UpdateCurlSample();

			if (m_sample != null)
			{
				PictureBox box = (PictureBox)sender;
				e.Graphics.DrawImage(m_sample, box.Width - 72, box.Height - 78);
			}
		}

		private void backgroundUpper_Paint(object sender, PaintEventArgs e)
		{
			PictureBox box = (PictureBox)sender;
			using (Pen pen = new Pen(s_borderColor))
			{
				e.Graphics.DrawLine(pen, 0, 0, box.Width, 0);
				e.Graphics.DrawLine(pen, 0, 0, 0, box.Height);
			}
		}

		private void backgroundLower_Paint(object sender, PaintEventArgs e)
		{
			PictureBox box = (PictureBox)sender;
			using (Pen pen = new Pen(s_borderColor))
			{
				e.Graphics.DrawLine(pen, 0, 0, 0, box.Height);
			}
		}

		private void borderBottomLeft_Paint(object sender, PaintEventArgs e)
		{
			using (Pen pen = new Pen(s_borderColor))
			{
				e.Graphics.DrawLine(pen, 0, 6, 32, 6);
				e.Graphics.DrawLine(pen, 0, 0, 0, 6);
			}
		}

		private void borderTopRight_Paint(object sender, PaintEventArgs e)
		{
			using (Pen pen = new Pen(s_borderColor))
			{
				e.Graphics.DrawLine(pen, 0, 0, 5, 0);
				e.Graphics.DrawLine(pen, 5, 0, 5, 32);
			}
		}

		private void linkDetails_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			if (!String.IsNullOrEmpty(m_exampleUrl))
				Process.Start(m_exampleUrl);
		}

		#endregion

		#region Service methods

		/// <summary>
		/// Updates curl sample using current backgorund color.
		/// </summary>
		private void UpdateCurlSample()
		{
			Point[] points = new Point[7];
			points[0] = new Point(0, 96);
			points[1] = new Point(0, 85);
			points[2] = new Point(16, 85);
			points[3] = new Point(85, 10);
			points[4] = new Point(85, 0);
			points[5] = new Point(96, 0);
			points[6] = new Point(96, 96);

			m_sample = new Bitmap(96, 96);
			using (SolidBrush brush = new SolidBrush(BackColor))
			{
				Graphics graphics = Graphics.FromImage(m_sample);
				graphics.FillPolygon(brush, points);
				graphics.DrawImage(Resources.CurlBottomRightTransparent, 0, 0);
			}
		}

		/// <summary>
		/// Invalidate all custom-drawn control areas.
		/// </summary>
		private void InvalidateAll()
		{
			pictureExample.Invalidate();
			backgroundUpper.Invalidate();
			backgroundLower.Invalidate();
			borderBottomLeft.Invalidate();
			borderTopRight.Invalidate();
		}

		#endregion

		#region Displaying examples

		/// <summary>
		/// Clears example.
		/// </summary>
		public void Clear()
		{
			pictureExample.Image = null;
			linkDetails.Enabled = false;
		}

		/// <summary>
		/// Displays specified example.
		/// </summary>
		public void Display(Image exampleImage, string exampleUrl)
		{
			m_exampleUrl = exampleUrl;

			pictureExample.Image = exampleImage;
			linkDetails.Enabled = true;
		}

		#endregion
	}
}
