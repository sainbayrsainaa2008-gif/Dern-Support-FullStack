using System;
using System.Drawing;
using System.Windows.Forms;

namespace GradeCalculator
{
    public class MainForm : Form
    {
        // Radio buttons for 4 units
        private RadioButton rbU1Pass, rbU1Merit, rbU1Distinction, rbU1Refer;
        private RadioButton rbU3Pass, rbU3Merit, rbU3Distinction, rbU3Refer;
        private RadioButton rbU4Pass, rbU4Merit, rbU4Distinction, rbU4Refer;
        private RadioButton rbU6Pass, rbU6Merit, rbU6Distinction, rbU6Refer;

        private TextBox txtName;
        private ComboBox cmbLevel;
        private Label lblGrade;
        private Label lblCategory;

        public MainForm()
        {
            BuildUI();
        }

        private void BuildUI()
        {
            this.Text            = "Монгол Улсын Их Сургууль";
            this.Size            = new Size(1100, 360);
            this.BackColor       = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox     = false;
            this.StartPosition   = FormStartPosition.CenterScreen;
            this.Font            = new Font("Segoe UI", 9f);

            // ── Row 0: Name + Level ───────────────────────────────────────
            Add(new Label { Text = "Enter your name and level:", Location = new Point(20, 22), AutoSize = true });

            txtName = new TextBox { Location = new Point(240, 19), Size = new Size(160, 22) };
            Add(txtName);

            cmbLevel = new ComboBox { Location = new Point(415, 19), Size = new Size(95, 22), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbLevel.Items.AddRange(new object[] { "Level 2", "Level 3", "Level 4", "Level 5" });
            cmbLevel.SelectedIndex = 1;
            Add(cmbLevel);

            // ── Unit rows ─────────────────────────────────────────────────
            MakeRow(58,  "Enter marks for unit 1 information technology:",    out rbU1Pass, out rbU1Merit, out rbU1Distinction, out rbU1Refer);
            MakeRow(90,  "Enter marks for unit 3 website development:",        out rbU3Pass, out rbU3Merit, out rbU3Distinction, out rbU3Refer);
            MakeRow(122, "Enter marks for unit 4 using social media in business:", out rbU4Pass, out rbU4Merit, out rbU4Distinction, out rbU4Refer);
            MakeRow(154, "Enter marks for unit 6 programming:",                out rbU6Pass, out rbU6Merit, out rbU6Distinction, out rbU6Refer);

            // ── Click me button ───────────────────────────────────────────
            var btn = new Button
            {
                Text      = "click me",
                Location  = new Point(390, 195),
                Size      = new Size(88, 30),
                BackColor = Color.FromArgb(0, 102, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor    = Cursors.Hand,
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.Click += OnClick;
            Add(btn);

            // ── Result labels ─────────────────────────────────────────────
            lblGrade    = new Label { Text = "", Location = new Point(20, 238), AutoSize = true, Font = new Font("Segoe UI", 9f) };
            lblCategory = new Label { Text = "", Location = new Point(20, 260), AutoSize = true, Font = new Font("Segoe UI", 9f) };
            Add(lblGrade);
            Add(lblCategory);

            // ── Logo panel (right side dark blue) ────────────────────────
            var logo = new Panel { Location = new Point(808, 8), Size = new Size(268, 285), BackColor = Color.FromArgb(10, 30, 75) };
            Add(logo);

            logo.Controls.Add(new Label
            {
                Text      = "МУИС",
                ForeColor = Color.White,
                Font      = new Font("Georgia", 36f, FontStyle.Bold),
                Location  = new Point(45, 25),
                AutoSize  = true,
            });
            logo.Controls.Add(new Panel { Location = new Point(16, 90), Size = new Size(236, 3), BackColor = Color.White });
            logo.Controls.Add(new Label { Text = "МОНГОЛ УЛСЫН",  ForeColor = Color.White, Font = new Font("Arial", 12f, FontStyle.Bold), Location = new Point(16, 103), AutoSize = true });
            logo.Controls.Add(new Label { Text = "ИХ СУРГУУЛЬ",  ForeColor = Color.White, Font = new Font("Arial", 12f, FontStyle.Bold), Location = new Point(28, 127), AutoSize = true });
        }

        // ── Helper: add one unit row ──────────────────────────────────────
        private void MakeRow(int y, string text,
            out RadioButton rPass, out RadioButton rMerit,
            out RadioButton rDist,  out RadioButton rRefer)
        {
            Add(new Label { Text = text, Location = new Point(20, y + 3), AutoSize = true });
            rPass  = new RadioButton { Text = "PASS",        Location = new Point(490, y), AutoSize = true };
            rMerit = new RadioButton { Text = "MERIT",       Location = new Point(563, y), AutoSize = true };
            rDist  = new RadioButton { Text = "DISTINCTION", Location = new Point(640, y), AutoSize = true };
            rRefer = new RadioButton { Text = "REFER",       Location = new Point(768, y), AutoSize = true };
            Add(rPass); Add(rMerit); Add(rDist); Add(rRefer);
        }

        private void Add(Control c) => this.Controls.Add(c);

        // ── Grade logic ───────────────────────────────────────────────────
        private void OnClick(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text)
                || !Any(rbU1Pass, rbU1Merit, rbU1Distinction, rbU1Refer)
                || !Any(rbU3Pass, rbU3Merit, rbU3Distinction, rbU3Refer)
                || !Any(rbU4Pass, rbU4Merit, rbU4Distinction, rbU4Refer)
                || !Any(rbU6Pass, rbU6Merit, rbU6Distinction, rbU6Refer))
            {
                MessageBox.Show("Enter all values", "Invalid Input",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string u1 = Sel(rbU1Pass, rbU1Merit, rbU1Distinction, rbU1Refer);
            string u3 = Sel(rbU3Pass, rbU3Merit, rbU3Distinction, rbU3Refer);
            string u4 = Sel(rbU4Pass, rbU4Merit, rbU4Distinction, rbU4Refer);
            string u6 = Sel(rbU6Pass, rbU6Merit, rbU6Distinction, rbU6Refer);

            if (u1=="refer" || u3=="refer" || u4=="refer" || u6=="refer")
            {
                lblGrade.Text    = "Your grade is refer";
                lblCategory.Text = "Ineligible for internships and scholarships.";
                return;
            }
            if (u1=="distinction" && u3=="distinction" && u4=="distinction" && u6=="distinction")
            {
                lblGrade.Text    = "Your grade is distinction";
                lblCategory.Text = "You fall under category 1";
                return;
            }
            if (R(u1)>=2 && R(u3)>=2 && R(u4)>=2 && R(u6)>=2)
            {
                lblGrade.Text    = "Your grade is merit";
                lblCategory.Text = "You fall under category 2";
                return;
            }
            lblGrade.Text    = "Your grade is pass";
            lblCategory.Text = "You fall under category 3";
        }

        private bool Any(params RadioButton[] rbs) { foreach (var r in rbs) if (r.Checked) return true; return false; }
        private string Sel(RadioButton p, RadioButton m, RadioButton d, RadioButton r)
        {
            if (p.Checked) return "pass";
            if (m.Checked) return "merit";
            if (d.Checked) return "distinction";
            if (r.Checked) return "refer";
            return "";
        }
        private int R(string g) => g=="distinction"?3 : g=="merit"?2 : g=="pass"?1 : 0;
    }
}
