using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Shuruev.StyleCop.CSharp
{
	/// <summary>
	/// Simple text box with highlighting support.
	/// </summary>
	[Description("Simple text box with highlighting support.")]
	public partial class HighlightTextBox : UserControl
	{
		private Color m_defaultColor = Color.Black;
		private bool m_readOnly = false;

		private RichTextBox m_active = null;
		private RichTextBox m_buffer = null;
		private bool m_busy = false;

		public HighlightTextBox()
		{
			InitializeComponent();

			m_active = richA;
			m_buffer = richB;

			rich_TextChanged(null, EventArgs.Empty);
		}

		#region Events

		/// <summary>
		/// Occurs when control must highlight its content.
		/// </summary>
		[Description("Occurs when control must highlight its content.")]
		public event ControlEventHandler Highlight;

		/// <summary>
		/// Raises Highlight event.
		/// </summary>
		protected virtual void OnHighlight(ControlEventArgs e)
		{
			if (Highlight != null)
				Highlight(this, e);
		}

		#endregion

		#region Properties

		/// <summary>
		/// Color that will be used for displaying content.
		/// </summary>
		[Description("Color that will be used for displaying content.")]
		[DefaultValue(typeof(Color), "Black")]
		public Color DefaultColor
		{
			get { return m_defaultColor; }
			set
			{
				m_defaultColor = value;

				rich_TextChanged(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether text in the control is read-only.
		/// </summary>
		[Description("Gets or sets a value indicating whether text in the control is read-only.")]
		[DefaultValue(false)]
		public bool ReadOnly
		{
			get { return m_readOnly; }
			set
			{
				m_readOnly = value;

				m_active.ReadOnly = value;
				m_buffer.ReadOnly = value;

				rich_TextChanged(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Gets underlying text box control.
		/// </summary>
		public RichTextBox RichTextBox
		{
			get { return m_active; }
		}

		#endregion

		#region Event handlers

		private void rich_TextChanged(object sender, EventArgs e)
		{
			if (m_busy)
				return;

			m_busy = true;

			m_buffer.Text = m_active.Text;

			m_buffer.SelectAll();
			m_buffer.SelectionColor = m_defaultColor;

			ControlEventArgs args = new ControlEventArgs(m_buffer);
			OnHighlight(args);

			m_buffer.SelectionStart = m_active.SelectionStart;
			m_buffer.SelectionLength = m_active.SelectionLength;

			SCROLLINFO si = new SCROLLINFO();
			si.cbSize = (uint)Marshal.SizeOf(typeof(SCROLLINFO));
			si.fMask = SIF_POS;
			GetScrollInfo(m_active.Handle, (int)SB_VERT, ref si);
			SetScrollInfo(m_buffer.Handle, (int)SB_VERT, ref si, true);

			uint wparam = ((uint)si.nPos << 16) + SB_THUMBPOSITION;
			SendMessage(m_buffer.Handle, WM_VSCROLL, wparam, 0);
			SendMessage(m_buffer.Handle, WM_VSCROLL, SB_ENDSCROLL, 0);

			m_busy = false;

			RichTextBox temp = m_active;
			m_active = m_buffer;
			m_buffer = temp;

			m_active.BringToFront();
			m_active.Show();
			m_buffer.Hide();
			m_active.Focus();
		}

		#endregion

		#region Native methods

		private const uint WM_HSCROLL = 0x0114;
		private const uint WM_VSCROLL = 0x0115;

		[DllImport("User32.dll")]
		private static extern ulong SendMessage(
			IntPtr hWnd,
			uint msg,
			uint wParam,
			uint lParam);

		private const uint SB_LINEUP = 0;
		private const uint SB_LINELEFT = 0;
		private const uint SB_LINEDOWN = 1;
		private const uint SB_LINERIGHT = 1;
		private const uint SB_PAGEUP = 2;
		private const uint SB_PAGELEFT = 2;
		private const uint SB_PAGEDOWN = 3;
		private const uint SB_PAGERIGHT = 3;
		private const uint SB_THUMBPOSITION = 4;
		private const uint SB_THUMBTRACK = 5;
		private const uint SB_TOP = 6;
		private const uint SB_LEFT = 6;
		private const uint SB_BOTTOM = 7;
		private const uint SB_RIGHT = 7;
		private const uint SB_ENDSCROLL = 8;

		private const uint SB_HORZ = 0;
		private const uint SB_VERT = 1;
		private const uint SB_CTL = 2;
		private const uint SB_BOTH = 3;

		private const uint SIF_RANGE = 0x0001;
		private const uint SIF_PAGE = 0x0002;
		private const uint SIF_POS = 0x0004;
		private const uint SIF_DISABLENOSCROLL = 0x0008;
		private const uint SIF_TRACKPOS = 0x0010;
		private const uint SIF_ALL = (SIF_RANGE | SIF_PAGE | SIF_POS | SIF_TRACKPOS);

		private struct SCROLLINFO
		{
			public uint cbSize;
			public uint fMask;
			public int nMin;
			public int nMax;
			public uint nPage;
			public int nPos;
			public int nTrackPos;
		}

		[DllImport("User32.dll")]
		private static extern bool GetScrollInfo(
			IntPtr hwnd,
			int fnBar,
			ref SCROLLINFO lpsi);

		[DllImport("User32.dll")]
		private static extern bool SetScrollInfo(
			IntPtr hwnd,
			int fnBar,
			ref SCROLLINFO lpsi,
			bool fRedraw);

		#endregion
	}
}
