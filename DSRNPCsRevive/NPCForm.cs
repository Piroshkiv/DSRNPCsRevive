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
                FillWeight = 50
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

            var npcList = _characters[_slotNumber].Npc.NPCData;

            foreach (var npc in npcList.Npcs)
            {
                dataGridView1.Rows.Add(npc.Name);
            }

            dataGridView1.CellContentClick += (s, e) =>
            {
                if (e.RowIndex < 0) return;
                string npcName = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
                var character = _characters[_slotNumber];

                try
                {
                    if (dataGridView1.Columns[e.ColumnIndex].Name == "Kill")
                        character.Npc.SetNpcAlive(npcName, false);
                    else if (dataGridView1.Columns[e.ColumnIndex].Name == "Revive")
                        character.Npc.SetNpcAlive(npcName, true);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}");
                }
            };

            dataGridView1.CellFormatting += (s, e) =>
            {
                if (e.RowIndex < 0) return;

                if (dataGridView1.Columns[e.ColumnIndex].Name == "Kill")
                {
                    e.CellStyle.BackColor = Color.DarkRed;
                    e.CellStyle.ForeColor = Color.White;
                }
                else if (dataGridView1.Columns[e.ColumnIndex].Name == "Revive")
                {
                    e.CellStyle.BackColor = Color.DarkBlue;
                    e.CellStyle.ForeColor = Color.White;
                }
            };
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
    }
}
