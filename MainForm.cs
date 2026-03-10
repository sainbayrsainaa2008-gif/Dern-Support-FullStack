using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace GradeCalculator
{
    public class MainForm : Form
    {
        private RadioButton rbU1Pass, rbU1Merit, rbU1Distinction, rbU1Refer;
        private RadioButton rbU3Pass, rbU3Merit, rbU3Distinction, rbU3Refer;
        private RadioButton rbU4Pass, rbU4Merit, rbU4Distinction, rbU4Refer;
        private RadioButton rbU6Pass, rbU6Merit, rbU6Distinction, rbU6Refer;

        private TextBox  txtName;
        private ComboBox cmbLevel;
        private Label    lblGrade;
        private Label    lblCategory;
        private Label    lblDBStatus;

        private const string ConnStr =
            "server=localhost;port=3306;database=programm;uid=root;password=;";

        public MainForm()
        {
            BuildUI();
            InitDB();
        }

        private void InitDB()
        {
            try
            {
                using (var root = new MySqlConnection("server=localhost;port=3306;uid=root;password=;"))
                {
                    root.Open();
                    var c = root.CreateCommand();
                    c.CommandText = "CREATE DATABASE IF NOT EXISTS programm CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;";
                    c.ExecuteNonQuery();
                }
                using (var con = new MySqlConnection(ConnStr))
                {
                    con.Open();
                    var c = con.CreateCommand();
                    c.CommandText = @"
                        CREATE TABLE IF NOT EXISTS results (
                            id        INT AUTO_INCREMENT PRIMARY KEY,
                            name      VARCHAR(100) NOT NULL,
                            level     VARCHAR(20)  NOT NULL,
                            unit1     VARCHAR(20)  NOT NULL,
                            unit3     VARCHAR(20)  NOT NULL,
                            unit4     VARCHAR(20)  NOT NULL,
                            unit6     VARCHAR(20)  NOT NULL,
                            grade     VARCHAR(20)  NOT NULL,
                            category  VARCHAR(100) NOT NULL,
                            saved_at  DATETIME     DEFAULT CURRENT_TIMESTAMP
                        );";
                    c.ExecuteNonQuery();
                }
                lblDBStatus.Text      = "🟢 MySQL: Холбогдсон  |  DB: programm";
                lblDBStatus.ForeColor = Color.LightGreen;
            }
            catch (Exception ex)
            {
                lblDBStatus.Text      = "🔴 MySQL: Холбогдохгүй байна";
                lblDBStatus.ForeColor = Color.OrangeRed;
                MessageBox.Show("XAMPP → MySQL → Start дарна уу!\n\nАлдаа: " + ex.Message,
                    "Database алдаа", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void SaveToDB(string name, string level,
            string u1, string u3, string u4, string u6,
            string grade, string category)
        {
            using var con = new MySqlConnection(ConnStr);
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = @"INSERT INTO results (name,level,unit1,unit3,unit4,unit6,grade,category)
                VALUES (@n,@l,@u1,@u3,@u4,@u6,@g,@c);";
            cmd.Parameters.AddWithValue("@n",  name);
            cmd.Parameters.AddWithValue("@l",  level);
            cmd.Parameters.AddWithValue("@u1", u1);
            cmd.Parameters.AddWithValue("@u3", u3);
            cmd.Parameters.AddWithValue("@u4", u4);
            cmd.Parameters.AddWithValue("@u6", u6);
            cmd.Parameters.AddWithValue("@g",  grade);
            cmd.Parameters.AddWithValue("@c",  category);
            cmd.ExecuteNonQuery();
        }

        private void BuildUI()
        {
            this.Text            = "Монгол Улсын Их Сургууль";
            this.Size            = new Size(1100, 430);
            this.BackColor       = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox     = false;
            this.StartPosition   = FormStartPosition.CenterScreen;
            this.Font            = new Font("Segoe UI", 9f);

            // ── Нэр + Level ───────────────────────────────────────────────
            Add(new Label { Text = "Enter your name and level:", Location = new Point(20, 22), AutoSize = true });
            txtName = new TextBox { Location = new Point(230, 19), Size = new Size(160, 22) };
            Add(txtName);
            cmbLevel = new ComboBox
            {
                Location = new Point(405, 19), Size = new Size(95, 22),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbLevel.Items.AddRange(new object[] { "Level 2", "Level 3", "Level 4", "Level 5" });
            cmbLevel.SelectedIndex = 1;
            Add(cmbLevel);

            // ── Unit мөр бүрийг тусдаа Panel-д байрлуулна ────────────────
            // Panel бүр өөрийн дотор радио бүлэгтэй тул хоорондоо нөлөөлөхгүй
            MakeRow(50,  "Enter marks for unit 1 information technology:",
                out rbU1Pass, out rbU1Merit, out rbU1Distinction, out rbU1Refer);
            MakeRow(100, "Enter marks for unit 3 website development:",
                out rbU3Pass, out rbU3Merit, out rbU3Distinction, out rbU3Refer);
            MakeRow(150, "Enter marks for unit 4 using social media in business:",
                out rbU4Pass, out rbU4Merit, out rbU4Distinction, out rbU4Refer);
            MakeRow(200, "Enter marks for unit 6 programming:",
                out rbU6Pass, out rbU6Merit, out rbU6Distinction, out rbU6Refer);

            // ── Товчнууд ──────────────────────────────────────────────────
            var btnCalc = Btn("click me",    Color.FromArgb(0, 102, 204), new Point(185, 258), 90);
            var btnView = Btn("📋 Бүх дүн",  Color.FromArgb(34, 139, 34), new Point(285, 258), 105);
            var btnDel  = Btn("🗑 Устгах",   Color.FromArgb(180, 40, 40), new Point(400, 258), 90);
            btnCalc.Click += OnCalc;
            btnView.Click += OnView;
            btnDel.Click  += OnDelete;
            Add(btnCalc); Add(btnView); Add(btnDel);

            lblGrade    = new Label { Text = "", Location = new Point(20, 300), AutoSize = true, Font = new Font("Segoe UI", 10f, FontStyle.Bold) };
            lblCategory = new Label { Text = "", Location = new Point(20, 324), AutoSize = true };
            Add(lblGrade);
            Add(lblCategory);

            // ── Лого ─────────────────────────────────────────────────────
            var pnl = new Panel
            {
                Location  = new Point(840, 0),
                Size      = new Size(252, 400),
                BackColor = Color.FromArgb(10, 30, 75)
            };
            Add(pnl);
            pnl.Controls.Add(new Label { Text = "МУИС", ForeColor = Color.White, Font = new Font("Georgia", 34f, FontStyle.Bold), Location = new Point(42, 30), AutoSize = true });
            pnl.Controls.Add(new Panel { Location = new Point(16, 92), Size = new Size(220, 3), BackColor = Color.White });
            pnl.Controls.Add(new Label { Text = "МОНГОЛ УЛСЫН", ForeColor = Color.White, Font = new Font("Arial", 11f, FontStyle.Bold), Location = new Point(16, 104), AutoSize = true });
            pnl.Controls.Add(new Label { Text = "ИХ СУРГУУЛЬ",  ForeColor = Color.White, Font = new Font("Arial", 11f, FontStyle.Bold), Location = new Point(26, 126), AutoSize = true });
            lblDBStatus = new Label { Text = "⏳ Холбож байна...", ForeColor = Color.Yellow, Font = new Font("Segoe UI", 8f), Location = new Point(10, 310), AutoSize = true };
            pnl.Controls.Add(lblDBStatus);
            pnl.Controls.Add(new Label { Text = "DB: programm  |  Table: results", ForeColor = Color.LightGray, Font = new Font("Segoe UI", 7.5f), Location = new Point(10, 335), AutoSize = true });
        }

        // ── Гол засвар: Panel ашиглан радио товчийг тусгаарлах ───────────
        // Windows Forms-д нэг ContainerControl дотор байгаа радио товчнууд
        // л хоорондоо бүлэгдэнэ. Тиймээс unit болгонд тусдаа Panel хийнэ.
        private void MakeRow(int y, string labelText,
            out RadioButton rPass, out RadioButton rMerit,
            out RadioButton rDist, out RadioButton rRefer)
        {
            // Label нь Form дээр шууд
            Add(new Label
            {
                Text     = labelText,
                Location = new Point(20, y + 12),
                AutoSize = true
            });

            // Panel — радио товчнуудыг тусгаарлах савлагаа
            var panel = new Panel
            {
                Location  = new Point(510, y),
                Size      = new Size(315, 40),
                BackColor = Color.White
            };

            rPass  = new RadioButton { Text = "PASS",        Location = new Point(0,   10), AutoSize = true };
            rMerit = new RadioButton { Text = "MERIT",       Location = new Point(75,  10), AutoSize = true };
            rDist  = new RadioButton { Text = "DISTINCTION", Location = new Point(150, 10), AutoSize = true };
            rRefer = new RadioButton { Text = "REFER",       Location = new Point(265, 10), AutoSize = true };

            panel.Controls.Add(rPass);
            panel.Controls.Add(rMerit);
            panel.Controls.Add(rDist);
            panel.Controls.Add(rRefer);

            Add(panel);
        }

        private Button Btn(string text, Color color, Point loc, int width)
        {
            var b = new Button
            {
                Text      = text,
                Location  = loc,
                Size      = new Size(width, 30),
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor    = Cursors.Hand,
                Font      = new Font("Segoe UI", 8.5f)
            };
            b.FlatAppearance.BorderSize = 0;
            return b;
        }

        private void Add(Control c) => this.Controls.Add(c);

        private void OnCalc(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text)
                || !Any(rbU1Pass,rbU1Merit,rbU1Distinction,rbU1Refer)
                || !Any(rbU3Pass,rbU3Merit,rbU3Distinction,rbU3Refer)
                || !Any(rbU4Pass,rbU4Merit,rbU4Distinction,rbU4Refer)
                || !Any(rbU6Pass,rbU6Merit,rbU6Distinction,rbU6Refer))
            {
                MessageBox.Show("Enter all values", "Invalid Input",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string u1 = Sel(rbU1Pass,rbU1Merit,rbU1Distinction,rbU1Refer);
            string u3 = Sel(rbU3Pass,rbU3Merit,rbU3Distinction,rbU3Refer);
            string u4 = Sel(rbU4Pass,rbU4Merit,rbU4Distinction,rbU4Refer);
            string u6 = Sel(rbU6Pass,rbU6Merit,rbU6Distinction,rbU6Refer);

            string grade, category;
            if (u1=="refer"||u3=="refer"||u4=="refer"||u6=="refer")
            { grade="refer"; category="Ineligible for internships and scholarships."; }
            else if (u1=="distinction"&&u3=="distinction"&&u4=="distinction"&&u6=="distinction")
            { grade="distinction"; category="You fall under category 1"; }
            else if (R(u1)>=2&&R(u3)>=2&&R(u4)>=2&&R(u6)>=2)
            { grade="merit"; category="You fall under category 2"; }
            else
            { grade="pass"; category="You fall under category 3"; }

            lblGrade.Text    = $"Your grade is {grade}";
            lblCategory.Text = category;

            try
            {
                SaveToDB(txtName.Text, cmbLevel.Text, u1, u3, u4, u6, grade, category);
                lblCategory.Text = category + "  ✅ Хадгалагдлаа";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Хадгалахад алдаа:\n" + ex.Message,
                    "DB Алдаа", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnView(object? sender, EventArgs e)
        {
            var frm = new Form
            {
                Text = "📋 Бүх оюутны дүн — programm.results",
                Size = new Size(1050, 480),
                StartPosition = FormStartPosition.CenterScreen,
                BackColor = Color.White
            };
            var grid = new DataGridView
            {
                Dock = DockStyle.Fill, ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White, BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
                    { BackColor = Color.FromArgb(240, 245, 255) }
            };
            frm.Controls.Add(grid);
            try
            {
                using var con = new MySqlConnection(ConnStr);
                con.Open();
                var da = new MySqlDataAdapter(
                    "SELECT id AS 'ID', name AS 'Нэр', level AS 'Level', " +
                    "unit1 AS 'Unit 1', unit3 AS 'Unit 3', unit4 AS 'Unit 4', unit6 AS 'Unit 6', " +
                    "grade AS 'Дүн', category AS 'Категори', saved_at AS 'Огноо' " +
                    "FROM results ORDER BY id DESC", con);
                var dt = new DataTable();
                da.Fill(dt);
                grid.DataSource = dt;
            }
            catch (Exception ex) { MessageBox.Show("DB алдаа: " + ex.Message); }
            frm.ShowDialog();
        }

        private void OnDelete(object? sender, EventArgs e)
        {
            if (MessageBox.Show("Сүүлийн бичлэгийг устгах уу?", "Устгах",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            try
            {
                using var con = new MySqlConnection(ConnStr);
                con.Open();
                var cmd = con.CreateCommand();
                cmd.CommandText = "DELETE FROM results ORDER BY id DESC LIMIT 1;";
                int rows = cmd.ExecuteNonQuery();
                MessageBox.Show(rows > 0 ? "✅ Устгагдлаа!" : "Устгах бичлэг байхгүй.");
            }
            catch (Exception ex) { MessageBox.Show("Устгахад алдаа: " + ex.Message); }
        }

        private bool Any(params RadioButton[] rbs)
        {
            foreach (var r in rbs) if (r.Checked) return true;
            return false;
        }
        private string Sel(RadioButton p, RadioButton m, RadioButton d, RadioButton r)
        {
            if (p.Checked) return "pass";
            if (m.Checked) return "merit";
            if (d.Checked) return "distinction";
            if (r.Checked) return "refer";
            return "";
        }
        private int R(string g) => g=="distinction"?3:g=="merit"?2:g=="pass"?1:0;
    }
}
