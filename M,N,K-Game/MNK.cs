using System;
using System.Windows.Forms;
using System.Drawing;
using MNK_Game.Properties;
using System.Text.RegularExpressions;
using System.Drawing.Drawing2D;

namespace MNK_Game
{
    enum eMark
    {
        O, X
    }

    class MNK
    {
        private static readonly int msCellWidth = 70;
        private static readonly int msCellHeight = 70;
        
        private Panel mPanel;
        private ImageList mImages;

        private int mNumberOfClick;
        private int[,] mMarks;

        public Form GameForm { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
        public int NumberOfMarks { get; set; }

        public MNK(Form f, int row, int column, int numberOfMarks)
        {
            GameForm = f;
            Row = row;
            Column = column;
            NumberOfMarks = numberOfMarks;
        }

        public void MakeGrid()
        {
            PictureBox cell;

            mPanel = new Panel();
            mPanel.Size = new Size(msCellWidth * Column, msCellHeight * Row);
            mPanel.Location = new Point(50, 100);

            mMarks = new int[Row, Column];
            
            for (int i = 0; i < Row; i++)
            {
                for (int k = 0; k < Column; k++)
                {
                    mMarks[i, k] = -1;

                    cell = new PictureBox()
                    {
                        BorderStyle = BorderStyle.FixedSingle,
                        Name = "pictureBox" + i + "," + k,
                        Width = msCellWidth,
                        Height = msCellHeight,
                        Location = new Point(msCellWidth * k, msCellHeight * i)
                    };
                    cell.Click += cell_Click;
                    mPanel.Controls.Add(cell);
                }
            }

            getImages();           
            GameForm.Controls.Add(mPanel);
        }

        private void cell_Click(object sender, EventArgs e)
        {
            PictureBox pb = sender as PictureBox;
            MatchCollection rowColumn = Regex.Matches(pb.Name, @"\d+");
            int row = int.Parse(rowColumn[0].ToString());
            int column = int.Parse(rowColumn[1].ToString());

            if (pb.Image == null)
            {
                mNumberOfClick++;

                switch (mNumberOfClick % 2)
                {
                    case (int)eMark.O:
                        pb.Image = mImages.Images[0];
                        mMarks[row, column] = 0;
                        break;
                    case (int)eMark.X:
                        pb.Image = mImages.Images[1];
                        mMarks[row, column] = 1;
                        break;
                }
            }

            if (mNumberOfClick >= (NumberOfMarks * 2) - 1)
                checkMarks(row, column, (eMark)mMarks[row, column]);
        }

        private void checkMarks(int boxRow, int boxColumn, eMark mark)
        {
            int i = 0;
            int marksInRow = 0;

            // Check 4 lines that the clicked cell belongs to in different direction
            while (i < 4)
            {
                marksInRow = 1;

                switch (i)
                {
                    case 0:
                        for (int k = 1; (boxRow - k >= 0) && (boxColumn - k >= 0); k++)
                        {
                            if (mMarks[boxRow - k, boxColumn - k] == (int)mark)
                                marksInRow++;
                            else
                                break;
                        }
                        for (int k = 1; (boxRow + k < Row) && (boxColumn + k < Column); k++)
                        {
                            if (mMarks[boxRow + k, boxColumn + k] == (int)mark)
                                marksInRow++;
                            else
                                break;
                        }
                        break;
                    case 1:
                        for (int k = 1; boxRow - k >= 0; k++)
                        {
                            if (mMarks[boxRow - k, boxColumn] == (int)mark)
                                marksInRow++;
                            else
                                break;
                        }
                        for (int k = 1; boxRow + k < Row; k++)
                        {
                            if (mMarks[boxRow + k, boxColumn] == (int)mark)
                                marksInRow++;
                            else
                                break;
                        }
                        break;
                    case 2:
                        for (int k = 1; (boxRow - k >= 0) && (boxColumn + k < Column); k++)
                        {
                            if (mMarks[boxRow - k, boxColumn + k] == (int)mark)
                                marksInRow++;
                            else
                                break;
                        }
                        for (int k = 1; (boxRow + k < Row) && (boxColumn - k >= 0); k++)
                        {
                            if (mMarks[boxRow + k, boxColumn - k] == (int)mark)
                                marksInRow++;
                            else
                                break;
                        }
                        break;
                    case 3:
                        for (int k = 1; boxColumn - k >= 0; k++)
                        {
                            if (mMarks[boxRow, boxColumn - k] == (int)mark)
                                marksInRow++;
                            else
                                break;
                        }
                        for (int k = 1; boxColumn + k < Column; k++)
                        {
                            if (mMarks[boxRow, boxColumn + k] == (int)mark)
                                marksInRow++;
                            else
                                break;
                        }
                        break;
                }

                // Check if there is winner
                if (marksInRow == NumberOfMarks)
                {
                    MessageBox.Show(mark.ToString() + " wins!");
                    endGame();
                    break;
                }
                i++;
            }

            if (mNumberOfClick == Row * Column && marksInRow != NumberOfMarks)
            {
                MessageBox.Show("Draw!");
                endGame();
            }
        }

        /// <summary>
        /// Make image fit into a size with width and height
        /// </summary>
        /// <param name="originalImage"> image to convert </param>
        /// <returns></returns>
        private Image qualifyOfImage(Image originalImage)
        {
            // It makes an image size small
            Bitmap convertedImage = new Bitmap(msCellWidth, msCellHeight);

            // It gets the small image in high quality
            using (Graphics graphics = Graphics.FromImage(convertedImage))
            {
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.DrawImage(originalImage, 0, 0, msCellWidth, msCellHeight);
            }

            // The image is passed to a transparency method
            return convertedImage;
        }

        /// <summary>
        /// Make images fit into a size and add them into imageList
        /// </summary>
        private void getImages()
        {
            mImages = new ImageList()
            {
                ImageSize = new Size(msCellWidth, msCellHeight)
            };
            mImages.Images.Add(qualifyOfImage(Resources.O));
            mImages.Images.Add(qualifyOfImage(Resources.X));
        }

        /// <summary>
        /// End game
        /// </summary>
        private void endGame()
        {
            GameForm.Controls.Remove(mPanel);
            MakeGrid();
            mNumberOfClick = 0;
        }
    }
}