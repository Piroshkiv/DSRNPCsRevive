using DSRSave;

namespace DSRNPCsRevive
{
    public partial class Form1 : Form
    {
        private string _savePath;
        private Setting _settings;

        public Form1()
        {
            InitializeComponent();
            LoadSettings();
        }

        private void LoadSettings()
        {
            _settings = SettingHelper.LoadSettings();

            if (!string.IsNullOrEmpty(_settings.SaveFilePath) && File.Exists(_settings.SaveFilePath))
            {
                _savePath = _settings.SaveFilePath;
                LoadSaveFile(_savePath);
            }
        }

        private void SaveSettings()
        {
            _settings.SaveFilePath = _savePath;
            SettingHelper.SaveSettings(_settings);
        }

        private void LoadSaveFile(string filePath)
        {
            try
            {
                string destPath = Path.Combine(Application.StartupPath, Path.GetFileName(filePath));
                File.Copy(filePath, destPath, true);

                var characters = DSRSaveEditor.ReadSave(destPath).ToList();
                FillDataGridView(characters);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to read file: {ex.Message}");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            if (!string.IsNullOrEmpty(_settings.SaveFilePath))
            {
                ofd.InitialDirectory = Path.GetDirectoryName(_settings.SaveFilePath);
            }

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                _savePath = ofd.FileName;
                LoadSaveFile(_savePath);
                SaveSettings();
            }
        }

        private void FillDataGridView(List<Character> characters)
        {
            dataGridView1.Columns.Clear();
            dataGridView1.Rows.Clear();

            var colSlot = new DataGridViewTextBoxColumn
            {
                Name = "Slot",
                HeaderText = "Slot",
                Width = 150
            };
            var colName = new DataGridViewTextBoxColumn
            {
                Name = "Name",
                HeaderText = "Name",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            };
            var colButton = new DataGridViewButtonColumn
            {
                Name = "Action",
                HeaderText = "",
                Text = "Edit",
                UseColumnTextForButtonValue = true,
                Width = 150
            };

            dataGridView1.Columns.AddRange(colSlot, colName, colButton);

            foreach (var c in characters.SkipLast(1))
            {
                string displayName = c.IsEmpty ? "EMPTY" : c.General.Name1;
                int rowIndex = dataGridView1.Rows.Add(c.SlotNumber.ToString(), displayName);

                if (c.IsEmpty)
                {
                    var row = dataGridView1.Rows[rowIndex];
                    row.Cells[1].Style.ForeColor = Color.Gray;
                    row.Cells[2] = new DataGridViewTextBoxCell();
                    row.Cells[2].Value = "";
                    row.ReadOnly = true;
                }
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex == dataGridView1.RowCount - 1 || e.ColumnIndex != 2) return;

            var name = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
            if (name == "EMPTY") return;

            int slotNumber = int.Parse(dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString());
            var form = new NPCForm(_savePath, slotNumber);
            form.ShowDialog();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            SaveSettings();
            base.OnFormClosed(e);
        }
    }
}
