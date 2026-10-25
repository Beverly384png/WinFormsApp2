using System;
using System.Drawing;
using System.Windows.Forms;
using WinFormsApp2.Data;
using WinFormsApp2.Models;

namespace WinFormsApp2
{
    public partial class EditForm : Form
    {
        private readonly MaterialRepository _repository;
        private readonly int? _materialId;

        private ComboBox cmbType = null!;
        private TextBox txtName = null!;
        private NumericUpDown numPrice = null!;
        private TextBox txtUnit = null!;
        private NumericUpDown numPackQty = null!;
        private NumericUpDown numStockQty = null!;
        private NumericUpDown numMinQty = null!;

        public EditForm(MaterialRepository repository, int? materialId = null)
        {
            InitializeComponent();
            _repository = repository;
            _materialId = materialId;

            SetupUI();

            if (_materialId.HasValue)
            {
                this.Text = "Редактирование материала";
                LoadDataForEdit();
            }
            else
            {
                this.Text = "Добавление материала";
            }
        }

        private void SetupUI()
        {
            this.Size = new Size(400, 440);
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            Label lblType = new Label { Text = "Тип материала:", Location = new Point(20, 20), AutoSize = true };
            cmbType = new ComboBox { Location = new Point(180, 17), Width = 180, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbType.Items.AddRange(new object[] { "Воск", "Фитиль", "Ароматизатор", "Краситель", "Тара" });

            Label lblName = new Label { Text = "Наименование:", Location = new Point(20, 60), AutoSize = true };
            txtName = new TextBox { Location = new Point(180, 57), Width = 180 };

            Label lblPrice = new Label { Text = "Цена (руб):", Location = new Point(20, 100), AutoSize = true };
            numPrice = new NumericUpDown { Location = new Point(180, 97), Width = 180, DecimalPlaces = 2, Maximum = 1000000, Minimum = 0 };

            Label lblUnit = new Label { Text = "Ед. измерения:", Location = new Point(20, 140), AutoSize = true };
            txtUnit = new TextBox { Location = new Point(180, 137), Width = 180 };

            Label lblPack = new Label { Text = "Кол-во в упаковке:", Location = new Point(20, 180), AutoSize = true };
            numPackQty = new NumericUpDown { Location = new Point(180, 177), Width = 180, DecimalPlaces = 2, Maximum = 1000000, Minimum = 0 };

            Label lblStock = new Label { Text = "На складе:", Location = new Point(20, 220), AutoSize = true };
            numStockQty = new NumericUpDown { Location = new Point(180, 217), Width = 180, DecimalPlaces = 2, Maximum = 1000000, Minimum = 0 };

            Label lblMin = new Label { Text = "Минимальное кол-во:", Location = new Point(20, 260), AutoSize = true };
            numMinQty = new NumericUpDown { Location = new Point(180, 257), Width = 180, DecimalPlaces = 2, Maximum = 1000000, Minimum = 0 };

            Button btnSave = new Button { Text = "Сохранить", Location = new Point(170, 320), Size = new Size(85, 30) };
            btnSave.Click += BtnSave_Click;

            Button btnCancel = new Button { Text = "Назад", Location = new Point(265, 320), Size = new Size(85, 30) };
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            this.Controls.Add(lblType); this.Controls.Add(cmbType);
            this.Controls.Add(lblName); this.Controls.Add(txtName);
            this.Controls.Add(lblPrice); this.Controls.Add(numPrice);
            this.Controls.Add(lblUnit); this.Controls.Add(txtUnit);
            this.Controls.Add(lblPack); this.Controls.Add(numPackQty);
            this.Controls.Add(lblStock); this.Controls.Add(numStockQty);
            this.Controls.Add(lblMin); this.Controls.Add(numMinQty);
            this.Controls.Add(btnSave); this.Controls.Add(btnCancel);
        }

        private void LoadDataForEdit()
        {
            var material = _repository.GetMaterialById(_materialId!.Value);
            if (material != null)
            {
                txtName.Text = material.Name;
                numPrice.Value = material.Price;
                txtUnit.Text = material.MeasureUnit;
                numMinQty.Value = material.MinQty;
                numStockQty.Value = material.StockQty;
                numPackQty.Value = 1;

                if (!string.IsNullOrEmpty(material.MaterialType))
                {
                    cmbType.Text = material.MaterialType;
                }
            }
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtName.Text))
                {
                    MessageBox.Show("Поле 'Наименование' не может быть пустым. Пожалуйста, введите название.", "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var item = new MaterialListItem
                {
                    Id = _materialId ?? 0,
                    MaterialType = cmbType.Text,
                    Name = txtName.Text,
                    Price = numPrice.Value,
                    MeasureUnit = txtUnit.Text,
                    MinQty = numMinQty.Value,
                    StockQty = numStockQty.Value
                };

                _repository.SaveMaterial(item, _materialId.HasValue);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении:\n{ex.Message}\n\nПроверьте введенные данные и попробуйте снова.", "Ошибка сохранения", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}