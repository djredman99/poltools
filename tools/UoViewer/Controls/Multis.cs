/***************************************************************************
 *
 * $Author: Turley
 * 
 * "THE BEER-WARE LICENSE"
 * As long as you retain this notice you can do whatever you want with 
 * this stuff. If we meet some day, and you think this stuff is worth it,
 * you can buy me a beer in return.
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Ultima;
using System.IO;
using System.Drawing.Imaging;

namespace Controls
{
    public partial class Multis : UserControl
    {
        public Multis()
        {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
        }

        private void OnLoad(object sender, EventArgs e)
        {
            TreeViewMulti.BeginUpdate();
            TreeViewMulti.Nodes.Clear();
            for (int i = 0; i <= 0x4000; i++)
            {
                MultiComponentList multi = Ultima.Multis.GetComponents(i);
                if (multi != MultiComponentList.Empty)
                {
                    TreeNode node = new TreeNode(String.Format("{0,5} (0x{1:X})", i, i));
                    node.Tag = multi;
                    node.Name = i.ToString();
                    TreeViewMulti.Nodes.Add(node);
                }
            }
            TreeViewMulti.EndUpdate();
            TreeViewMulti.SelectedNode = TreeViewMulti.Nodes[0];
        }

        private void afterSelect_Multi(object sender, TreeViewEventArgs e)
        {
            HeightChangeMulti.Maximum = ((MultiComponentList)TreeViewMulti.SelectedNode.Tag).maxHeight;
            toolTip.SetToolTip(HeightChangeMulti, String.Format("MaxHeight: {0}", HeightChangeMulti.Maximum - HeightChangeMulti.Value));
            StatusMultiText.Text = String.Format("Size: {0},{1} MaxHeight: {2}",
                                               ((MultiComponentList)TreeViewMulti.SelectedNode.Tag).Width,
                                               ((MultiComponentList)TreeViewMulti.SelectedNode.Tag).Height,
                                               ((MultiComponentList)TreeViewMulti.SelectedNode.Tag).maxHeight);
            ChangeComponentList((MultiComponentList)TreeViewMulti.SelectedNode.Tag);
            MultiPictureBox.Refresh();
        }

        private void onPaint_MultiPic(object sender, PaintEventArgs e)
        {
            int h = HeightChangeMulti.Maximum - HeightChangeMulti.Value;
            Bitmap m_MainPicture_Multi  = ((MultiComponentList)TreeViewMulti.SelectedNode.Tag).GetImage(h);
            Point location = Point.Empty;
            Size size = Size.Empty;
            Rectangle destRect = Rectangle.Empty;
            size = MultiPictureBox.Size;
            if ((m_MainPicture_Multi.Height < size.Height) && (m_MainPicture_Multi.Width < size.Width))
            {
                location.X = (MultiPictureBox.Width - m_MainPicture_Multi.Width) / 2;
                location.Y = (MultiPictureBox.Height - m_MainPicture_Multi.Height) / 2;
                destRect = new Rectangle(location, m_MainPicture_Multi.Size);
            }
            else if (m_MainPicture_Multi.Height < size.Height)
            {
                location.X = 0;
                location.Y = (MultiPictureBox.Height - m_MainPicture_Multi.Height) / 2;
                if (m_MainPicture_Multi.Width > size.Width)
                    destRect = new Rectangle(location, new Size(size.Width, m_MainPicture_Multi.Height));
                else
                    destRect = new Rectangle(location, m_MainPicture_Multi.Size);
            }
            else if (m_MainPicture_Multi.Width < size.Width)
            {
                location.X = (MultiPictureBox.Width - m_MainPicture_Multi.Width) / 2;
                location.Y = 0;
                if (m_MainPicture_Multi.Height > size.Height)
                    destRect = new Rectangle(location, new Size(m_MainPicture_Multi.Width, size.Height));
                else
                    destRect = new Rectangle(location, m_MainPicture_Multi.Size);
            }
            else
                destRect = new Rectangle(new Point(0, 0), size);


            e.Graphics.DrawImage(m_MainPicture_Multi, destRect, 0, 0, m_MainPicture_Multi.Width, m_MainPicture_Multi.Height, System.Drawing.GraphicsUnit.Pixel);
            
        }

        private void onValue_HeightChangeMulti(object sender, EventArgs e)
        {
            toolTip.SetToolTip(HeightChangeMulti, String.Format("MaxHeight: {0}", HeightChangeMulti.Maximum - HeightChangeMulti.Value));
            MultiPictureBox.Refresh();
        }

        private void ChangeComponentList(MultiComponentList multi)
        {
            MultiComponentBox.Clear();
            for (int x = 0; x < multi.Width; ++x)
            {
                for (int y = 0; y < multi.Height; ++y)
                {
                    Tile[] tiles = multi.Tiles[x][y];
                    for (int i = 0; i < tiles.Length; ++i)
                    {
                        MultiComponentBox.AppendText(String.Format("0x{0:X4} {1,3} {2,3} {3,2}\n", tiles[i].ID - 0x4000, x, y, tiles[i].Z));

                    }
                }
            }
        }

        private void extract_Image_Click(object sender, EventArgs e)
        {
            string path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            string FileName = Path.Combine(path, String.Format("Multi 0x{0:X}.jpg", int.Parse(TreeViewMulti.SelectedNode.Name)));
            int h = HeightChangeMulti.Maximum - HeightChangeMulti.Value;
            Bitmap bit = ((MultiComponentList)TreeViewMulti.SelectedNode.Tag).GetImage(h);
            bit.Save(FileName, ImageFormat.Jpeg);

        }
    }
}