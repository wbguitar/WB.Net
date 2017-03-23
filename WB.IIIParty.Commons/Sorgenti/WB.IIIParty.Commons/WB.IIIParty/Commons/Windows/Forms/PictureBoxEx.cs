// ------------------------------------------------------------------------
//Società: WB IIIParty
//Anno: 2008 
//Progetto: AMIL5
//Autore: Acquisti Leonardo
//Nome modulo software: WB.IIIParty.Commons.dll
//Data ultima modifica: $LastChangedDate: 2011-10-20 10:21:02 +0200 (gio, 20 ott 2011) $
//Versione: $Rev: 43 $
// ------------------------------------------------------------------------

using System;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Drawing2D;

namespace WB.IIIParty.Commons.Windows.Forms
{
    /// <summary>
    /// Implementa una PictureBox avanzata
    /// </summary>
    public class PictureBoxEx : Control
    {
        #region Local Variables

        Image image = null;
        float opacity = 1.0F;
        bool verticalFlip = false;
        bool horizontalFlip = false;
        int rotationAngle = 0;
        bool stretchImage=false;
        bool centerImage=false;
        bool renderImage =true;
        Image paintImage=null;
        InterpolationMode interpolationMode = InterpolationMode.Default;

        #endregion

        #region Constructor

        /// <summary>
        /// 
        /// </summary>
        public PictureBoxEx()
        {

        }
        #endregion

        #region Properties

        /// <summary>
        /// Stretch dell'immagine
        /// </summary>
        public bool RenderImage
        {
            get
            {
                return renderImage;
            }
            set
            {
                renderImage = value;

                InvalidateEx();
            }
        }

        /// <summary>
        /// Stretch dell'immagine
        /// </summary>
        public bool StretchImage
        {
            get
            {
                return stretchImage;
            }
            set
            {
                stretchImage = value;

                InvalidateEx();
            }
        }

        /// <summary>
        /// Stretch dell'immagine
        /// </summary>
        public bool CenterImage
        {
            get
            {
                return centerImage;
            }
            set
            {
                centerImage = value;

                InvalidateEx();
            }
        }

        /// <summary>
        /// Angolo di rotazione dell'immagine
        /// </summary>
        public int RotationAngle
        {
            get
            {
                return rotationAngle;
            }
            set
            {
                rotationAngle = value;

                InvalidateEx();
            }
        }

        /// <summary>
        /// Flip verticale
        /// </summary>
        public bool VerticalFlip
        {
            get
            {
                return verticalFlip;
            }
            set
            {
                verticalFlip = value;

                InvalidateEx();

            }
        }

        /// <summary>
        /// Flip orizzontale
        /// </summary>
        public bool HorizontalFlip
        {
            get
            {
                return horizontalFlip;
            }
            set
            {
                horizontalFlip = value;

                InvalidateEx();

            }
        }

        /// <summary>
        /// Opacità
        /// </summary>
        public float Opacity
        {
            get
            {
                return opacity;
            }
            set
            {
                opacity = value;

                this.InvalidateEx();
            }
        }

        /// <summary>
        /// Immagine renderizzata
        /// </summary>
        public Image Image
        {
            get
            {
                return image;
            }
            set
            {
                image = value;

                this.InvalidateEx();
            }
        }

        /// <summary>
        /// Tipo di interpolazione durante la renderizzazione dell'immagine
        /// </summary>
        public InterpolationMode InterpolationMode
        {
            get
            {
                return interpolationMode;
            }
            set
            {
                interpolationMode = value;
                
                this.InvalidateEx();
            }

        }

        /// <summary>
        /// Padding di renderizzazione dell'immagine all'interno del controllo
        /// </summary>
        public new Padding Padding
        {
            get
            {
                return base.Padding;
            }
            set
            {
                base.Padding = value;

                this.InvalidateEx();
            }
        }
        
        #endregion

        #region Private Method

        void InvalidateEx()
        {
            this.MakePaintImage();

            if (this.Parent != null) this.Parent.Invalidate(true);
            else
            this.Invalidate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            InvalidateEx();
        }

        void MakePaintImage()
        {
            if (paintImage != null)
            {
                paintImage.Dispose();
                paintImage = null;
            }

            if (image != null)
            {
                paintImage = (Image)image.Clone();

                //Ruoto l'immagine
                if ((verticalFlip) && (!horizontalFlip))
                {
                    paintImage.RotateFlip(RotateFlipType.RotateNoneFlipY);
                }
                if ((horizontalFlip) && (!verticalFlip))
                {
                    paintImage.RotateFlip(RotateFlipType.RotateNoneFlipX);
                }
                if ((horizontalFlip) && (verticalFlip))
                {
                    paintImage.RotateFlip(RotateFlipType.RotateNoneFlipXY);
                }

                
            }
        }

        #endregion

        #region Override Method

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMove(EventArgs e)
        {
            base.OnMove(e);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            

            Graphics presentationMedium = e.Graphics;

            presentationMedium.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            presentationMedium.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
            presentationMedium.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            presentationMedium.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
 

                Rectangle clipRect = Rectangle.Empty;

                //Renderizzo l'immagine
                if (paintImage != null)
                {
                    ////Abilito l'opacità
                    ColorMatrix matrix = new ColorMatrix();
                    matrix.Matrix33 = opacity; //opacity 0 = completely transparent, 1 = completely opaque

                    ImageAttributes attributes = new ImageAttributes();
                    attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                    //ColorMap map = new ColorMap();
                    //map.OldColor = Color.Black;
                    //map.NewColor = Color.Red;
                    //attributes.SetRemapTable(new ColorMap[] { map }, ColorAdjustType.Bitmap);

                    //Determino la dimensione dell'immagine e la posizione di destinazione
                    int srcX = 0;
                    int srcY = 0;
                    int srcWidth = (int)paintImage.PhysicalDimension.Width;
                    int srcHeight = (int)paintImage.PhysicalDimension.Height;

                    int avaiableX = Padding.Left;
                    int avaiableY = Padding.Top;
                    int avaiableWidth = this.ClientSize.Width - (Padding.Left + Padding.Right);
                    int avaiableHeight = this.ClientSize.Height - (Padding.Top + Padding.Bottom);

                    int destX = avaiableX;
                    int destY = avaiableY;
                    int destWidth = srcWidth;
                    int destHeight = srcHeight;

                    if ((!StretchImage) && (CenterImage))
                    {
                        destX += (this.Width - (int)paintImage.PhysicalDimension.Width) / 2;
                        destY += (this.Height - (int)paintImage.PhysicalDimension.Height) / 2;
                    }

                    if (StretchImage)
                    {
                        destWidth = avaiableWidth;
                        destHeight = avaiableHeight;
                    }

                    float centerX = destX + (destWidth / 2);
                    float centerY = destY + (destHeight / 2);

                    presentationMedium.TranslateTransform(centerX, centerY);
                    presentationMedium.RotateTransform(RotationAngle);
                    presentationMedium.TranslateTransform(-centerX, -centerY);

                    if (StretchImage) presentationMedium.InterpolationMode = interpolationMode;

                    presentationMedium.DrawImage(paintImage, new Rectangle(destX, destY, destWidth, destHeight),
                            srcX, srcY, Convert.ToInt32(srcWidth),
                            Convert.ToInt32(srcHeight),
                            GraphicsUnit.Pixel, attributes);

                    attributes.Dispose();

                }
                if (image == null)
                {
                    presentationMedium.FillRectangle(new SolidBrush(this.BackColor), clipRect);
                }
            

        }

        /// <summary>
        /// 
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle = cp.ExStyle | 0x20;
                return cp;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pevent"></param>
        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            //base.OnPaintBackground(pevent);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (paintImage != null)
            {
                paintImage.Dispose();
            }

            if (image != null)
            {
                image.Dispose();
            }
        }
        #endregion
    }
}
