One possible remedy for **Why does my winform button.BackgroundImage value come out as System.Drawing.Bitmap** is to make sure the image can be located. What you're doing doesn't seem that far off the mark in terms of loading the image from file, but you might want to streamline the process with some suggestions:

[![sample game board][1]][1]


One straightforward way to ensure the files are located is to mark them to be copied to the output directory.

[![Copy to output directory][2]][2]

Then you can make a dictionary (or an `ImageList`) by loading from a location that is known by app at runtime.

```
enum SelectedImage{ None, Flag, Bomb, }

Dictionary<SelectedImage, Image> Images = new Dictionary<SelectedImage, Image>
{
    {  SelectedImage.None, null },
    {  SelectedImage.Flag, Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "flag.png")) },
    {  SelectedImage.Bomb, Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "bomb.png")) },
};
```

___

**Inherit Button**

It might make things easier if you make a custom button class that is aware of the `SelectedImage` enum and can fire an event when that property changes.

```
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
```

**Game Board**

Now you can consume the custom button class using a `TableLayoutPanel` (with your chosen dimensions) for a gameboard.

```
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
```

  [1]: https://i.stack.imgur.com/vVWHy.png
  [2]: https://i.stack.imgur.com/T5Pdq.png