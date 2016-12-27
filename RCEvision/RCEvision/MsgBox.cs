using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

namespace RCEvision
{
    public partial class MsgBox : Form
    {
        private const int CS_DROPSHADOW = 0x00020000;
        private static MsgBox msgBox;
        private Panel plHeader = new Panel();
        private Panel plFooter = new Panel();
        private Panel plIcon = new Panel();
        private PictureBox picIcon = new PictureBox();
        private FlowLayoutPanel flpButtons = new FlowLayoutPanel();
        private Label lblTitle;
        private Label lblMessage;
        private List<Button> buttonCollection = new List<Button>();
        private static DialogResult buttonResult = new DialogResult(); //Abort,cancel, Ignore, no, non, ok, retry
        private static Timer timer;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool MessageBeep(uint type);

        private MsgBox()
        {
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.BackColor = Color.FromArgb(45, 45, 48);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Width = 400;

            lblTitle = new Label();
            lblTitle.ForeColor = Color.White;
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 18);
            lblTitle.Dock = DockStyle.Top;
            lblTitle.Height = 50;

            lblMessage = new Label();
            lblMessage.ForeColor = Color.White;
            lblMessage.Font = new System.Drawing.Font("Segoe UI", 10);
            lblMessage.Dock = DockStyle.Fill;

            flpButtons.FlowDirection = FlowDirection.RightToLeft;
            flpButtons.Dock = DockStyle.Fill;

            plHeader.Dock = DockStyle.Fill;
            plHeader.Padding = new Padding(20);
            plHeader.Controls.Add(lblMessage);
            plHeader.Controls.Add(lblTitle);

            plFooter.Dock = DockStyle.Bottom;
            plFooter.Padding = new Padding(20);
            plFooter.BackColor = Color.FromArgb(37, 37, 38);
            plFooter.Height = 80;
            plFooter.Controls.Add(flpButtons);

            picIcon.Width = 32;
            picIcon.Height = 32;
            picIcon.Location = new Point(30, 50);

            plIcon.Dock = DockStyle.Left;
            plIcon.Padding = new Padding(20);
            plIcon.Width = 70;
            plIcon.Controls.Add(picIcon);

            this.Controls.Add(plHeader);
            this.Controls.Add(plIcon);
            this.Controls.Add(plFooter);
        }

        public static void Show(string message)
        {
            msgBox = new MsgBox();
            msgBox.lblMessage.Text = message;
            msgBox.ShowDialog();
            MessageBeep(0);
        }

        public static void Show(string message, string title)
        {
            msgBox = new MsgBox();
            msgBox.lblMessage.Text = message;
            msgBox.lblTitle.Text = title;
            msgBox.Size = MsgBox.MessageSize(message);
            msgBox.ShowDialog();
            MessageBeep(0);
        }

        public static DialogResult Show(string message, string title, Buttons buttons)
        {
            msgBox = new MsgBox();
            msgBox.lblMessage.Text = message;
            msgBox.lblTitle.Text = title;
            msgBox.plIcon.Hide();

            MsgBox.InitButtons(buttons);

            msgBox.Size = MsgBox.MessageSize(message);
            msgBox.ShowDialog();
            MessageBeep(0);
            return buttonResult;
        }

        public static DialogResult Show(string message, string title, Buttons buttons, Icon icon)
        {
            msgBox = new MsgBox();
            msgBox.lblMessage.Text = message;
            msgBox.lblTitle.Text = title;

            MsgBox.InitButtons(buttons);
            MsgBox.InitIcon(icon);

            msgBox.Size = MsgBox.MessageSize(message);
            msgBox.ShowDialog();
            MessageBeep(0);
            return buttonResult;
        }

        public static DialogResult Show(string message, string title, Buttons buttons, Icon icon, AnimateStyle style)
        {
            msgBox = new MsgBox();
            msgBox.lblMessage.Text = message;
            msgBox.lblTitle.Text = title;
            msgBox.Height = 0;

            MsgBox.InitButtons(buttons);
            MsgBox.InitIcon(icon);

            timer = new Timer();
            Size formSize = MsgBox.MessageSize(message);

            switch (style)
            {
                case MsgBox.AnimateStyle.SlideDown:
                    msgBox.Size = new Size(formSize.Width, 0);
                    timer.Interval = 1;
                    timer.Tag = new AnimateMsgBox(formSize, style);
                    break;

                case MsgBox.AnimateStyle.FadeIn:
                    msgBox.Size = formSize;
                    msgBox.Opacity = 0;
                    timer.Interval = 20;
                    timer.Tag = new AnimateMsgBox(formSize, style);
                    break;

                case MsgBox.AnimateStyle.ZoomIn:
                    msgBox.Size = new Size(formSize.Width + 100, formSize.Height + 100);
                    timer.Tag = new AnimateMsgBox(formSize, style);
                    timer.Interval = 1;
                    break;
            }

            timer.Tick += timer_Tick;
            timer.Start();

            msgBox.ShowDialog();
            MessageBeep(0);
            return buttonResult;
        }

        static void timer_Tick(object sender, EventArgs e)
        {
            Timer timer = (Timer)sender;
            AnimateMsgBox animate = (AnimateMsgBox)timer.Tag;

            switch (animate.Style)
            {
                case MsgBox.AnimateStyle.SlideDown:
                    if (msgBox.Height < animate.FormSize.Height)
                    {
                        msgBox.Height += 17;
                        msgBox.Invalidate();
                    }
                    else
                    {
                        MsgBox.timer.Stop();
                        MsgBox.timer.Dispose();
                    }
                    break;

                case MsgBox.AnimateStyle.FadeIn:
                    if (msgBox.Opacity < 1)
                    {
                        msgBox.Opacity += 0.1;
                        msgBox.Invalidate();
                    }
                    else
                    {
                        MsgBox.timer.Stop();
                        MsgBox.timer.Dispose();
                    }
                    break;

                case MsgBox.AnimateStyle.ZoomIn:
                    if (msgBox.Width > animate.FormSize.Width)
                    {
                        msgBox.Width -= 17;
                        msgBox.Invalidate();
                    }
                    if (msgBox.Height > animate.FormSize.Height)
                    {
                        msgBox.Height -= 17;
                        msgBox.Invalidate();
                    }
                    break;
            }
        }

        private static void InitButtons(Buttons buttons)
        {
            switch (buttons)
            {
                case MsgBox.Buttons.AbortRetryIgnore:
                    msgBox.InitAbortRetryIgnoreButtons();
                    break;

                case MsgBox.Buttons.OK:
                    msgBox.InitOKButton();
                    break;

                case MsgBox.Buttons.OKCancel:
                    msgBox.InitOKCancelButtons();
                    break;

                case MsgBox.Buttons.RetryCancel:
                    msgBox.InitRetryCancelButtons();
                    break;

                case MsgBox.Buttons.YesNo:
                    msgBox.InitYesNoButtons();
                    break;

                case MsgBox.Buttons.YesNoCancel:
                    msgBox.InitYesNoCancelButtons();
                    break;
            }

            foreach (Button btn in msgBox.buttonCollection)
            {
                btn.ForeColor = Color.FromArgb(170, 170, 170);
                btn.Font = new System.Drawing.Font("Segoe UI", 8);
                btn.Padding = new Padding(3);
                btn.FlatStyle = FlatStyle.Flat;
                btn.Height = 30;
                btn.FlatAppearance.BorderColor = Color.FromArgb(99, 99, 98);

                msgBox.flpButtons.Controls.Add(btn);
            }
        }

        private static void InitIcon(Icon icon)
        {
            switch (icon)
            {
                case MsgBox.Icon.Application:
                    msgBox.picIcon.Image = SystemIcons.Application.ToBitmap();
                    break;

                case MsgBox.Icon.Exclamation:
                    msgBox.picIcon.Image = SystemIcons.Exclamation.ToBitmap();
                    break;

                case MsgBox.Icon.Error:
                    msgBox.picIcon.Image = SystemIcons.Error.ToBitmap();
                    break;

                case MsgBox.Icon.Info:
                    msgBox.picIcon.Image = SystemIcons.Information.ToBitmap();
                    break;

                case MsgBox.Icon.Question:
                    msgBox.picIcon.Image = SystemIcons.Question.ToBitmap();
                    break;

                case MsgBox.Icon.Shield:
                    msgBox.picIcon.Image = SystemIcons.Shield.ToBitmap();
                    break;

                case MsgBox.Icon.Warning:
                    msgBox.picIcon.Image = SystemIcons.Warning.ToBitmap();
                    break;
            }
        }

        private void InitAbortRetryIgnoreButtons()
        {
            Button btnAbort = new Button();
            btnAbort.Text = "Abort";
            btnAbort.Click += ButtonClick;

            Button btnRetry = new Button();
            btnRetry.Text = "Retry";
            btnRetry.Click += ButtonClick;

            Button btnIgnore = new Button();
            btnIgnore.Text = "Ignore";
            btnIgnore.Click += ButtonClick;

            this.buttonCollection.Add(btnAbort);
            this.buttonCollection.Add(btnRetry);
            this.buttonCollection.Add(btnIgnore);
        }

        private void InitOKButton()
        {
            Button btnOK = new Button();
            btnOK.Text = "OK";
            btnOK.Click += ButtonClick;

            this.buttonCollection.Add(btnOK);
        }

        private void InitOKCancelButtons()
        {
            Button btnOK = new Button();
            btnOK.Text = "OK";
            btnOK.Click += ButtonClick;

            Button btnCancel = new Button();
            btnCancel.Text = "Cancel";
            btnCancel.Click += ButtonClick;


            this.buttonCollection.Add(btnOK);
            this.buttonCollection.Add(btnCancel);
        }

        private void InitRetryCancelButtons()
        {
            Button btnRetry = new Button();
            btnRetry.Text = "OK";
            btnRetry.Click += ButtonClick;

            Button btnCancel = new Button();
            btnCancel.Text = "Cancel";
            btnCancel.Click += ButtonClick;


            this.buttonCollection.Add(btnRetry);
            this.buttonCollection.Add(btnCancel);
        }

        private void InitYesNoButtons()
        {
            Button btnYes = new Button();
            btnYes.Text = "Yes";
            btnYes.Click += ButtonClick;

            Button btnNo = new Button();
            btnNo.Text = "No";
            btnNo.Click += ButtonClick;


            this.buttonCollection.Add(btnYes);
            this.buttonCollection.Add(btnNo);
        }

        private void InitYesNoCancelButtons()
        {
            Button btnYes = new Button();
            btnYes.Text = "Abort";
            btnYes.Click += ButtonClick;

            Button btnNo = new Button();
            btnNo.Text = "Retry";
            btnNo.Click += ButtonClick;

            Button btnCancel = new Button();
            btnCancel.Text = "Cancel";
            btnCancel.Click += ButtonClick;

            this.buttonCollection.Add(btnYes);
            this.buttonCollection.Add(btnNo);
            this.buttonCollection.Add(btnCancel);
        }

        private static void ButtonClick(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            switch (btn.Text)
            {
                case "Abort":
                    buttonResult = DialogResult.Abort;
                    break;

                case "Retry":
                    buttonResult = DialogResult.Retry;
                    break;

                case "Ignore":
                    buttonResult = DialogResult.Ignore;
                    break;

                case "OK":
                    buttonResult = DialogResult.OK;
                    break;

                case "Cancel":
                    buttonResult = DialogResult.Cancel;
                    break;

                case "Yes":
                    buttonResult = DialogResult.Yes;
                    break;

                case "No":
                    buttonResult = DialogResult.No;
                    break;
            }

            msgBox.Dispose();
        }

        private static Size MessageSize(string message)
        {
            Graphics g = msgBox.CreateGraphics();
            int width = 450;
            int height = 230;

            SizeF size = g.MeasureString(message, new System.Drawing.Font("Segoe UI", 10));

            if (message.Length < 150)
            {
                if ((int)size.Width > 350)
                {
                    width = (int)size.Width;
                }
            }
            else
            {
                string[] groups = (from Match m in Regex.Matches(message, ".{1,180}") select m.Value).ToArray();
                int lines = groups.Length + 1;
                width = 700;
                height += (int)(size.Height + 10) * lines;
            }
            return new Size(width, height);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= CS_DROPSHADOW;
                return cp;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            Rectangle rect = new Rectangle(new Point(0, 0), new Size(this.Width - 1, this.Height - 1));
            Pen pen = new Pen(Color.FromArgb(0, 180, 251));

            g.DrawRectangle(pen, rect);
        }

        public enum Buttons
        {
            AbortRetryIgnore = 1,
            OK = 2,
            OKCancel = 3,
            RetryCancel = 4,
            YesNo = 5,
            YesNoCancel = 6
        }

        public enum Icon
        {
            Application = 1,
            Exclamation = 2,
            Error = 3,
            Warning = 4,
            Info = 5,
            Question = 6,
            Shield = 7,
            Search = 8
        }

        public enum AnimateStyle
        {
            SlideDown = 1,
            FadeIn = 2,
            ZoomIn = 3
        }
        class AnimateMsgBox
        {
            public Size FormSize;
            public MsgBox.AnimateStyle Style;

            public AnimateMsgBox(Size formSize, MsgBox.AnimateStyle style)
            {
                this.FormSize = formSize;
                this.Style = style;
            }
        }
    }
}
