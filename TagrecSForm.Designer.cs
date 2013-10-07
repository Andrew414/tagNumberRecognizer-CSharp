namespace Tagrec_S
{
    partial class TagrecSForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.gbxSavedImages = new System.Windows.Forms.GroupBox();
            this.cbxAutoSave = new System.Windows.Forms.CheckBox();
            this.btnSaveSelected = new System.Windows.Forms.Button();
            this.lstSavedNumbers = new System.Windows.Forms.ListView();
            this.ilsSavedImages = new System.Windows.Forms.ImageList(this.components);
            this.gbxCurrent = new System.Windows.Forms.GroupBox();
            this.btnStartStop = new System.Windows.Forms.Button();
            this.pbxCurrentImage = new System.Windows.Forms.PictureBox();
            this.tmrCapture = new System.Windows.Forms.Timer(this.components);
            this.sfdSaveSelected = new System.Windows.Forms.SaveFileDialog();
            this.gbxSavedImages.SuspendLayout();
            this.gbxCurrent.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbxCurrentImage)).BeginInit();
            this.SuspendLayout();
            // 
            // gbxSavedImages
            // 
            this.gbxSavedImages.Controls.Add(this.cbxAutoSave);
            this.gbxSavedImages.Controls.Add(this.btnSaveSelected);
            this.gbxSavedImages.Controls.Add(this.lstSavedNumbers);
            this.gbxSavedImages.Location = new System.Drawing.Point(12, 12);
            this.gbxSavedImages.Name = "gbxSavedImages";
            this.gbxSavedImages.Size = new System.Drawing.Size(214, 417);
            this.gbxSavedImages.TabIndex = 0;
            this.gbxSavedImages.TabStop = false;
            this.gbxSavedImages.Text = "Saved numbers";
            // 
            // cbxAutoSave
            // 
            this.cbxAutoSave.AutoSize = true;
            this.cbxAutoSave.Location = new System.Drawing.Point(8, 390);
            this.cbxAutoSave.Name = "cbxAutoSave";
            this.cbxAutoSave.Size = new System.Drawing.Size(107, 17);
            this.cbxAutoSave.TabIndex = 3;
            this.cbxAutoSave.Text = "Automatic saving";
            this.cbxAutoSave.UseVisualStyleBackColor = true;
            // 
            // btnSaveSelected
            // 
            this.btnSaveSelected.Location = new System.Drawing.Point(121, 386);
            this.btnSaveSelected.Name = "btnSaveSelected";
            this.btnSaveSelected.Size = new System.Drawing.Size(86, 23);
            this.btnSaveSelected.TabIndex = 2;
            this.btnSaveSelected.Text = "Save selected";
            this.btnSaveSelected.UseVisualStyleBackColor = true;
            // 
            // lstSavedNumbers
            // 
            this.lstSavedNumbers.LargeImageList = this.ilsSavedImages;
            this.lstSavedNumbers.Location = new System.Drawing.Point(8, 19);
            this.lstSavedNumbers.MultiSelect = false;
            this.lstSavedNumbers.Name = "lstSavedNumbers";
            this.lstSavedNumbers.Size = new System.Drawing.Size(200, 360);
            this.lstSavedNumbers.TabIndex = 1;
            this.lstSavedNumbers.TileSize = new System.Drawing.Size(165, 64);
            this.lstSavedNumbers.UseCompatibleStateImageBehavior = false;
            this.lstSavedNumbers.View = System.Windows.Forms.View.Tile;
            this.lstSavedNumbers.SelectedIndexChanged += new System.EventHandler(this.lstSavedNumbers_SelectedIndexChanged);
            this.lstSavedNumbers.MouseCaptureChanged += new System.EventHandler(this.lstSavedNumbers_MouseCaptureChanged);
            // 
            // ilsSavedImages
            // 
            this.ilsSavedImages.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.ilsSavedImages.ImageSize = new System.Drawing.Size(64, 48);
            this.ilsSavedImages.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // gbxCurrent
            // 
            this.gbxCurrent.Controls.Add(this.btnStartStop);
            this.gbxCurrent.Controls.Add(this.pbxCurrentImage);
            this.gbxCurrent.Location = new System.Drawing.Point(232, 12);
            this.gbxCurrent.Name = "gbxCurrent";
            this.gbxCurrent.Size = new System.Drawing.Size(493, 417);
            this.gbxCurrent.TabIndex = 0;
            this.gbxCurrent.TabStop = false;
            this.gbxCurrent.Text = "Current capture";
            // 
            // btnStartStop
            // 
            this.btnStartStop.Location = new System.Drawing.Point(389, 386);
            this.btnStartStop.Name = "btnStartStop";
            this.btnStartStop.Size = new System.Drawing.Size(97, 23);
            this.btnStartStop.TabIndex = 2;
            this.btnStartStop.Text = "Stop capturing";
            this.btnStartStop.UseVisualStyleBackColor = true;
            this.btnStartStop.Click += new System.EventHandler(this.btnStartStop_Click);
            // 
            // pbxCurrentImage
            // 
            this.pbxCurrentImage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pbxCurrentImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbxCurrentImage.Location = new System.Drawing.Point(6, 19);
            this.pbxCurrentImage.Name = "pbxCurrentImage";
            this.pbxCurrentImage.Size = new System.Drawing.Size(480, 360);
            this.pbxCurrentImage.TabIndex = 0;
            this.pbxCurrentImage.TabStop = false;
            // 
            // tmrCapture
            // 
            this.tmrCapture.Tick += new System.EventHandler(this.tmrCapture_Tick);
            // 
            // TagrecSForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(732, 436);
            this.Controls.Add(this.gbxCurrent);
            this.Controls.Add(this.gbxSavedImages);
            this.Name = "TagrecSForm";
            this.Text = "Tagrec S";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TagrecSForm_FormClosing);
            this.gbxSavedImages.ResumeLayout(false);
            this.gbxSavedImages.PerformLayout();
            this.gbxCurrent.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbxCurrentImage)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.GroupBox gbxSavedImages;
        public System.Windows.Forms.GroupBox gbxCurrent;
        public System.Windows.Forms.PictureBox pbxCurrentImage;
        public System.Windows.Forms.ListView lstSavedNumbers;
        public System.Windows.Forms.ImageList ilsSavedImages;
        public System.Windows.Forms.CheckBox cbxAutoSave;
        public System.Windows.Forms.Button btnSaveSelected;
        public System.Windows.Forms.Button btnStartStop;
        public System.Windows.Forms.Timer tmrCapture;
        public System.Windows.Forms.SaveFileDialog sfdSaveSelected;

    }
}

