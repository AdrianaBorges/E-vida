namespace Converte_Object_Json
{
    partial class Form1
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
            this.txtResultado = new System.Windows.Forms.TextBox();
            this.btnObjectJson = new System.Windows.Forms.Button();
            this.btnJsonObject = new System.Windows.Forms.Button();
            this.btnSair = new System.Windows.Forms.Button();
            this.txtResultado2 = new System.Windows.Forms.TextBox();
            this.txtObject = new System.Windows.Forms.TextBox();
            this.txtJson = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txtResultado
            // 
            this.txtResultado.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtResultado.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.txtResultado.Location = new System.Drawing.Point(298, 13);
            this.txtResultado.Multiline = true;
            this.txtResultado.Name = "txtResultado";
            this.txtResultado.Size = new System.Drawing.Size(580, 187);
            this.txtResultado.TabIndex = 0;
            // 
            // btnObjectJson
            // 
            this.btnObjectJson.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.btnObjectJson.Font = new System.Drawing.Font("Moire", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnObjectJson.Location = new System.Drawing.Point(12, 399);
            this.btnObjectJson.Name = "btnObjectJson";
            this.btnObjectJson.Size = new System.Drawing.Size(280, 47);
            this.btnObjectJson.TabIndex = 1;
            this.btnObjectJson.Text = "Converte Object para JSON";
            this.btnObjectJson.UseVisualStyleBackColor = false;
            this.btnObjectJson.Click += new System.EventHandler(this.btnObjectJson_Click);
            // 
            // btnJsonObject
            // 
            this.btnJsonObject.BackColor = System.Drawing.Color.Aqua;
            this.btnJsonObject.Font = new System.Drawing.Font("Moire", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnJsonObject.Location = new System.Drawing.Point(298, 398);
            this.btnJsonObject.Name = "btnJsonObject";
            this.btnJsonObject.Size = new System.Drawing.Size(269, 47);
            this.btnJsonObject.TabIndex = 1;
            this.btnJsonObject.Text = "Converte JSON para Object";
            this.btnJsonObject.UseVisualStyleBackColor = false;
            this.btnJsonObject.Click += new System.EventHandler(this.btnJsonObject_Click);
            // 
            // btnSair
            // 
            this.btnSair.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.btnSair.Font = new System.Drawing.Font("Moire", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSair.Location = new System.Drawing.Point(725, 399);
            this.btnSair.Name = "btnSair";
            this.btnSair.Size = new System.Drawing.Size(152, 47);
            this.btnSair.TabIndex = 2;
            this.btnSair.Text = "Sair";
            this.btnSair.UseVisualStyleBackColor = false;
            this.btnSair.Click += new System.EventHandler(this.btnSair_Click);
            // 
            // txtResultado2
            // 
            this.txtResultado2.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtResultado2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.txtResultado2.Location = new System.Drawing.Point(298, 209);
            this.txtResultado2.Multiline = true;
            this.txtResultado2.Name = "txtResultado2";
            this.txtResultado2.Size = new System.Drawing.Size(579, 184);
            this.txtResultado2.TabIndex = 0;
            // 
            // txtObject
            // 
            this.txtObject.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtObject.Location = new System.Drawing.Point(19, 14);
            this.txtObject.Multiline = true;
            this.txtObject.Name = "txtObject";
            this.txtObject.Size = new System.Drawing.Size(273, 185);
            this.txtObject.TabIndex = 3;
            this.txtObject.Text = " Produto produto = new Produto\r\n {\r\n    ProdutoID = 1001,\r\n    ProdutoNome = \"Sam" +
    "sung Galaxy III\",\r\n    Categoria = \"Celular\",\r\n    Preco = 699.00\r\n };";
            // 
            // txtJson
            // 
            this.txtJson.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtJson.Location = new System.Drawing.Point(19, 209);
            this.txtJson.Multiline = true;
            this.txtJson.Name = "txtJson";
            this.txtJson.Size = new System.Drawing.Size(273, 185);
            this.txtJson.TabIndex = 3;
            this.txtJson.Text = "{\r\n  \"alunos\": [\r\n    {\r\n      \"nome\": \"Jose Carlos\",\r\n      \"sobrenome\": \"Macora" +
    "tti\"\r\n    },\r\n    {\r\n      \"nome\": \"Paulo\",\r\n      \"sobrenome\": \"Silveira\"\r\n    " +
    "}\r\n  ]\r\n}";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(894, 457);
            this.Controls.Add(this.txtJson);
            this.Controls.Add(this.txtObject);
            this.Controls.Add(this.btnSair);
            this.Controls.Add(this.btnJsonObject);
            this.Controls.Add(this.btnObjectJson);
            this.Controls.Add(this.txtResultado2);
            this.Controls.Add(this.txtResultado);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Conversor JSON  < = >  Object";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtResultado;
        private System.Windows.Forms.Button btnObjectJson;
        private System.Windows.Forms.Button btnJsonObject;
        private System.Windows.Forms.Button btnSair;
        private System.Windows.Forms.TextBox txtResultado2;
        private System.Windows.Forms.TextBox txtObject;
        private System.Windows.Forms.TextBox txtJson;
    }
}

