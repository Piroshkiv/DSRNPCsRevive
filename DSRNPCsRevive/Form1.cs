using DSRSave;

namespace DSRNPCsRevive
{
    public partial class Form1 : Form
    {
        private string _savePath;
        public Form1()
        {
            InitializeComponent();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                _savePath = ofd.FileName;

                string sourcePath = ofd.FileName;
                string destPath = Path.Combine(Application.StartupPath, Path.GetFileName(sourcePath));
                File.Copy(sourcePath, destPath, true);

                try
                {
                    var characters = DSRSaveEditor.ReadSave(destPath).ToList();
                    FillDataGridView(characters);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to read file: {ex.Message}");
                }
            }
        }

        private void FillDataGridView(List<Character> characters)
        {
            dataGridView1.Columns.Clear();
            dataGridView1.Rows.Clear();

            // Add columns: Slot, Name, Action
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
                    row.Cells[2] = new DataGridViewTextBoxCell(); // Remove button
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
            if (e.RowIndex < 0 || e.RowIndex == dataGridView1.RowCount - 1 || e.ColumnIndex != 2) return; // Only "Open" button

            var name = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
            if (name == "EMPTY") return;

            int slotNumber = int.Parse(dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString());

            var form = new NPCForm(_savePath, slotNumber);
            form.ShowDialog();

        }
    }
}
