using System;
using System.Drawing;
using System.Windows.Forms;
using WinFormsApp2.Data;

namespace WinFormsApp2
{
    public partial class Form1 : Form
    {
        private readonly MaterialRepository _repository;
        private FlowLayoutPanel flowMaterials;

        public Form1()
        {
            InitializeComponent();
            this.Text = "Список материалов";
            this.Icon = SystemIcons.Application;

            Panel topPanel = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.WhiteSmoke };

            PictureBox logo = new PictureBox { Location = new Point(10, 10), Size = new Size(40, 40), BackColor = Color.LightBlue };
            Label lblLogo = new Label { Text = "ЛОГО", Location = new Point(12, 20), AutoSize = true };

            Button btnAdd = new Button { Text = "Добавить материал", Location = new Point(60, 15), Size = new Size(150, 30) };
            btnAdd.Click += BtnAdd_Click;

            topPanel.Controls.Add(logo);
            topPanel.Controls.Add(lblLogo);
            topPanel.Controls.Add(btnAdd);

            flowMaterials = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoScroll = true, BackColor = Color.LightGray };

            this.Controls.Add(flowMaterials);
            this.Controls.Add(topPanel);

            _repository = new MaterialRepository();
            LoadMaterials();
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            using var editForm = new EditForm(_repository, null);
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                LoadMaterials();
            }
        }

        private void LoadMaterials()
        {
            try
            {
                var materials = _repository.GetMaterials();
                flowMaterials.Controls.Clear();

                foreach (var item in materials)
                {
                    Panel card = new Panel();
                    card.Size = new Size(400, 170);
                    card.BorderStyle = BorderStyle.FixedSingle;
                    card.Margin = new Padding(10);
                    card.BackColor = Color.White;
                    card.Cursor = Cursors.Hand;

                    EventHandler editClick = (s, e) =>
                    {
                        using var editForm = new EditForm(_repository, item.Id);
                        if (editForm.ShowDialog() == DialogResult.OK) LoadMaterials();
                    };
                    card.Click += editClick;

                    Label lblTitle = new Label { Text = $"{item.MaterialType} | {item.Name}", Font = new Font("Segoe UI", 10, FontStyle.Bold), Location = new Point(10, 10), AutoSize = true };
                    Label lblMin = new Label { Text = $"Минимальное количество: {item.MinQty} {item.MeasureUnit}", Location = new Point(10, 35), AutoSize = true };
                    Label lblStock = new Label { Text = $"Количество на складе: {item.StockQty} {item.MeasureUnit}", Location = new Point(10, 55), AutoSize = true };
                    if (item.StockQty < item.MinQty) lblStock.ForeColor = Color.Red;

                    Label lblPrice = new Label { Text = $"Цена: {item.Price:0.00} р / Единица измерения: {item.MeasureUnit}", Location = new Point(10, 75), AutoSize = true };
                    Label lblRequired = new Label { Text = $"Требуемое количество: {item.RequiredQty:0.00} {item.MeasureUnit}", Location = new Point(10, 95), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold) };

                    Button btnProducts = new Button { Text = "Используется в продукции", Location = new Point(10, 125), Size = new Size(200, 30) };
                    btnProducts.Click += (s, e) =>
                    {
                        var prods = _repository.GetProductsForMaterial(item.Id);
                        string msg = prods.Count > 0 ? string.Join("\n", prods) : "Материал не используется в продукции.";
                        MessageBox.Show(msg, "Список продукции", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    };

                    lblTitle.Click += editClick;
                    lblMin.Click += editClick;
                    lblStock.Click += editClick;
                    lblPrice.Click += editClick;
                    lblRequired.Click += editClick;

                    card.Controls.Add(lblTitle);
                    card.Controls.Add(lblMin);
                    card.Controls.Add(lblStock);
                    card.Controls.Add(lblPrice);
                    card.Controls.Add(lblRequired);
                    card.Controls.Add(btnProducts);

                    flowMaterials.Controls.Add(card);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}