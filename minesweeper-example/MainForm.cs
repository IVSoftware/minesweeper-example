using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace minesweeper_example
{
    public partial class MainForm : Form
    {
        enum SelectedImage{ None, Flag, Bomb, }

        Dictionary<SelectedImage, Image> Images = new Dictionary<SelectedImage, Image>
        {
            {  SelectedImage.None, null },
            {  SelectedImage.Flag, Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "flag.png")) },
            {  SelectedImage.Bomb, Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "bomb.png")) },
        };
        class MSButton : Button
        {
            public SelectedImage SelectedImage
            {
                get => _selectedImage;
                set
                {
                    if (!Equals(_selectedImage, value))
                    {
                        _selectedImage = value;
                        SelectedImageChanged?.Invoke(this, EventArgs.Empty);
                    }
                }
            }
            SelectedImage _selectedImage = 0;
            public event EventHandler SelectedImageChanged;

            // Cycle through the images
            protected override void OnClick(EventArgs e)
            {
                base.OnClick(e);
                SelectedImage = (SelectedImage)(((int)SelectedImage + 1) % 3);
            }
        }
        public MainForm()
        {
            InitializeComponent();
            const float DIM = 40F;
            tableLayoutPanel.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            tableLayoutPanel.RowCount = 10;
            tableLayoutPanel.RowStyles.Clear();
            tableLayoutPanel.ColumnCount = 10;
            tableLayoutPanel.ColumnStyles.Clear();
            for (int col = 0; col < 10; col++)
            {
                tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, DIM));
                for (int row = 0; row < 10; row++)
                {
                    tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, DIM));
                    var button = new MSButton
                    {
                        Anchor = (AnchorStyles)0xF, // All
                        BackgroundImageLayout = ImageLayout.Stretch,
                    };
                    button.SelectedImageChanged += (sender, e) =>
                    {
                        if(sender is MSButton msbutton)
                        {
                            msbutton.BackgroundImage = Images[msbutton.SelectedImage];
                        }
                    };
                    tableLayoutPanel.Controls.Add(button, col, row);
                }
            }
            // Size including 1 px borders.
            tableLayoutPanel.Width = (int)((DIM * 10) + 11);
            tableLayoutPanel.Height = (int)((DIM * 10) + 11);
        }
    }
}
