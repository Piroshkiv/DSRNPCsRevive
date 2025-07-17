using DSRSave;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DSRNPCsRevive
{
    public partial class NPCForm : Form
    {
        private string _savePath;
        private int _slotNumber;
        private List<Character> _characters;
        private List<Npc> _npcSource = new();

        public NPCForm(string savePath, int slotNumber)
        {
            InitializeComponent();
            _savePath = savePath;
            _slotNumber = slotNumber;
            _characters = DSRSaveEditor.ReadSave(savePath).ToList();

            InitializeNpcGrid();
        }

        private void InitializeNpcGrid()
        {
            dataGridView1.Columns.Clear();
            dataGridView1.Rows.Clear();
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            var nameColumn = new DataGridViewTextBoxColumn
            {
                Name = "Name",
                HeaderText = "NPC Name",
                FillWeight = 50,
                ReadOnly = true
            };

            var killButton = new DataGridViewButtonColumn
            {
                Name = "Kill",
                HeaderText = "Kill",
                Text = "Kill",
                UseColumnTextForButtonValue = true,
                FillWeight = 25
            };

            var reviveButton = new DataGridViewButtonColumn
            {
                Name = "Revive",
                HeaderText = "Revive",
                Text = "Revive",
                UseColumnTextForButtonValue = true,
                FillWeight = 25
            };

            dataGridView1.Columns.Add(nameColumn);
            dataGridView1.Columns.Add(killButton);
            dataGridView1.Columns.Add(reviveButton);

            _npcSource = _characters[_slotNumber].Npc.NPCData.Npcs;

            foreach (var npc in _npcSource)
            {
                int rowIndex = dataGridView1.Rows.Add(npc.Name);
                dataGridView1.Rows[rowIndex].Tag = npc;
            }

            dataGridView1.CellContentClick += (s, e) =>
            {
                if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

                if (dataGridView1.Columns[e.ColumnIndex] is DataGridViewButtonColumn)
                {
                    string npcName = dataGridView1.Rows[e.RowIndex].Cells[0].Value?.ToString();
                    if (string.IsNullOrEmpty(npcName)) return;

                    var character = _characters[_slotNumber];

                    try
                    {
                        if (dataGridView1.Columns[e.ColumnIndex].Name == "Kill")
                        {
                            character.Npc.SetNpcAlive(npcName, false);
                            UpdateRowAppearance(e.RowIndex, false);
                        }
                        else if (dataGridView1.Columns[e.ColumnIndex].Name == "Revive")
                        {
                            character.Npc.SetNpcAlive(npcName, true);
                            UpdateRowAppearance(e.RowIndex, true);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            };

            dataGridView1.CellFormatting += (s, e) =>
            {
                if (e.RowIndex < 0) return;

                if (dataGridView1.Columns[e.ColumnIndex].Name == "Kill")
                {
                    e.CellStyle.BackColor = Color.Red;
                    e.CellStyle.ForeColor = Color.White;
                    e.CellStyle.SelectionBackColor = Color.DarkRed;
                    e.CellStyle.SelectionForeColor = Color.White;
                    e.CellStyle.Font = new Font(dataGridView1.Font, FontStyle.Bold);
                }
                else if (dataGridView1.Columns[e.ColumnIndex].Name == "Revive")
                {
                    e.CellStyle.BackColor = Color.Blue;
                    e.CellStyle.ForeColor = Color.White;
                    e.CellStyle.SelectionBackColor = Color.DarkBlue;
                    e.CellStyle.SelectionForeColor = Color.White;
                    e.CellStyle.Font = new Font(dataGridView1.Font, FontStyle.Bold);
                }
                else if (dataGridView1.Columns[e.ColumnIndex].Name == "Name")
                {
                    e.CellStyle.BackColor = Color.White;
                    e.CellStyle.ForeColor = Color.Black;
                    e.CellStyle.SelectionBackColor = Color.LightBlue;
                    e.CellStyle.SelectionForeColor = Color.Black;
                }
            };
        }

        private void UpdateRowAppearance(int rowIndex, bool isAlive)
        {
            if (rowIndex < 0 || rowIndex >= dataGridView1.Rows.Count) return;

            var row = dataGridView1.Rows[rowIndex];

            if (isAlive)
            {
                row.DefaultCellStyle.BackColor = Color.White;
                row.DefaultCellStyle.ForeColor = Color.Black;
            }
            else
            {
                row.DefaultCellStyle.BackColor = Color.LightGray;
                row.DefaultCellStyle.ForeColor = Color.DarkGray;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _characters.WriteSave(_savePath);
            DialogResult = DialogResult.OK;
            Close();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string query = textBox1.Text.Trim().ToLower();

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Tag is not Npc npc)
                {
                    row.Visible = true;
                    continue;
                }

                string name = npc.Name.ToLower();

                bool isMatch = name.Contains(query) ||
                               query.Contains(name);

                row.Visible = string.IsNullOrWhiteSpace(query) || isMatch;
            }
        }
    }
}
