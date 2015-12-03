<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class WarrantyQuery
    Inherits System.Windows.Forms.Form

    'Form 重写 Dispose，以清理组件列表。
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Windows 窗体设计器所必需的
    Private components As System.ComponentModel.IContainer

    '注意:  以下过程是 Windows 窗体设计器所必需的
    '可以使用 Windows 窗体设计器修改它。  
    '不要使用代码编辑器修改它。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Me.Btn_Query = New System.Windows.Forms.Button()
        Me.CB_Site = New System.Windows.Forms.ComboBox()
        Me.Btn_Exit = New System.Windows.Forms.Button()
        Me.DGV = New System.Windows.Forms.DataGridView()
        Me.Btn_AddAbove = New System.Windows.Forms.Button()
        Me.Btn_AddBelow = New System.Windows.Forms.Button()
        Me.Btn_Del = New System.Windows.Forms.Button()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.C_Raw = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.C_End = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.C_Start = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.C_Model = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.C_Serial = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.C_User = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.C_Name = New System.Windows.Forms.DataGridViewTextBoxColumn()
        CType(Me.DGV, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'Btn_Query
        '
        Me.Btn_Query.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Btn_Query.Location = New System.Drawing.Point(843, 47)
        Me.Btn_Query.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.Btn_Query.Name = "Btn_Query"
        Me.Btn_Query.Size = New System.Drawing.Size(93, 43)
        Me.Btn_Query.TabIndex = 2
        Me.Btn_Query.Text = "Query"
        Me.Btn_Query.UseVisualStyleBackColor = True
        '
        'CB_Site
        '
        Me.CB_Site.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.CB_Site.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CB_Site.FormattingEnabled = True
        Me.CB_Site.ItemHeight = 16
        Me.CB_Site.Location = New System.Drawing.Point(843, 16)
        Me.CB_Site.Name = "CB_Site"
        Me.CB_Site.Size = New System.Drawing.Size(93, 24)
        Me.CB_Site.TabIndex = 1
        '
        'Btn_Exit
        '
        Me.Btn_Exit.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Btn_Exit.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Btn_Exit.Location = New System.Drawing.Point(843, 359)
        Me.Btn_Exit.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.Btn_Exit.Name = "Btn_Exit"
        Me.Btn_Exit.Size = New System.Drawing.Size(93, 27)
        Me.Btn_Exit.TabIndex = 3
        Me.Btn_Exit.Text = "Exit"
        Me.Btn_Exit.UseVisualStyleBackColor = True
        '
        'DGV
        '
        Me.DGV.AllowUserToAddRows = False
        Me.DGV.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.DGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DGV.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.C_Name, Me.C_User, Me.C_Serial, Me.C_Model, Me.C_Start, Me.C_End, Me.C_Raw})
        Me.DGV.Location = New System.Drawing.Point(12, 16)
        Me.DGV.Name = "DGV"
        Me.DGV.RowTemplate.Height = 23
        Me.DGV.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect
        Me.DGV.Size = New System.Drawing.Size(825, 370)
        Me.DGV.TabIndex = 4
        '
        'Btn_AddAbove
        '
        Me.Btn_AddAbove.Location = New System.Drawing.Point(6, 22)
        Me.Btn_AddAbove.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.Btn_AddAbove.Name = "Btn_AddAbove"
        Me.Btn_AddAbove.Size = New System.Drawing.Size(80, 27)
        Me.Btn_AddAbove.TabIndex = 5
        Me.Btn_AddAbove.Text = "Add above"
        Me.Btn_AddAbove.UseVisualStyleBackColor = True
        '
        'Btn_AddBelow
        '
        Me.Btn_AddBelow.Location = New System.Drawing.Point(6, 57)
        Me.Btn_AddBelow.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.Btn_AddBelow.Name = "Btn_AddBelow"
        Me.Btn_AddBelow.Size = New System.Drawing.Size(80, 27)
        Me.Btn_AddBelow.TabIndex = 6
        Me.Btn_AddBelow.Text = "Add below"
        Me.Btn_AddBelow.UseVisualStyleBackColor = True
        '
        'Btn_Del
        '
        Me.Btn_Del.Location = New System.Drawing.Point(6, 92)
        Me.Btn_Del.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.Btn_Del.Name = "Btn_Del"
        Me.Btn_Del.Size = New System.Drawing.Size(80, 27)
        Me.Btn_Del.TabIndex = 7
        Me.Btn_Del.Text = "Delete"
        Me.Btn_Del.UseVisualStyleBackColor = True
        '
        'GroupBox1
        '
        Me.GroupBox1.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupBox1.Controls.Add(Me.Btn_AddAbove)
        Me.GroupBox1.Controls.Add(Me.Btn_Del)
        Me.GroupBox1.Controls.Add(Me.Btn_AddBelow)
        Me.GroupBox1.Location = New System.Drawing.Point(844, 97)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(92, 125)
        Me.GroupBox1.TabIndex = 8
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Edit"
        '
        'C_Raw
        '
        Me.C_Raw.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft
        DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.C_Raw.DefaultCellStyle = DataGridViewCellStyle1
        Me.C_Raw.HeaderText = "Raw"
        Me.C_Raw.Name = "C_Raw"
        Me.C_Raw.ReadOnly = True
        '
        'C_End
        '
        Me.C_End.HeaderText = "End Date"
        Me.C_End.Name = "C_End"
        Me.C_End.ReadOnly = True
        '
        'C_Start
        '
        Me.C_Start.HeaderText = "Start Date"
        Me.C_Start.Name = "C_Start"
        Me.C_Start.ReadOnly = True
        '
        'C_Model
        '
        Me.C_Model.HeaderText = "Model"
        Me.C_Model.Name = "C_Model"
        '
        'C_Serial
        '
        Me.C_Serial.HeaderText = "Serial"
        Me.C_Serial.Name = "C_Serial"
        '
        'C_User
        '
        Me.C_User.HeaderText = "Login"
        Me.C_User.Name = "C_User"
        Me.C_User.ReadOnly = True
        '
        'C_Name
        '
        Me.C_Name.HeaderText = "Name / IP"
        Me.C_Name.Name = "C_Name"
        Me.C_Name.ToolTipText = "If your account has permission to connect to a remote WMI, you may fill computer " & _
    "name or IP address here and the program will try to get Login, Serial and Model " & _
    "via LAN."
        '
        'WarrantyQuery
        '
        Me.AcceptButton = Me.Btn_Query
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.Btn_Exit
        Me.ClientSize = New System.Drawing.Size(948, 399)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.DGV)
        Me.Controls.Add(Me.Btn_Exit)
        Me.Controls.Add(Me.CB_Site)
        Me.Controls.Add(Me.Btn_Query)
        Me.Font = New System.Drawing.Font("微软雅黑", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.Name = "WarrantyQuery"
        Me.Text = "Warranty Query"
        CType(Me.DGV, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents Btn_Query As System.Windows.Forms.Button
    Friend WithEvents CB_Site As System.Windows.Forms.ComboBox
    Friend WithEvents Btn_Exit As System.Windows.Forms.Button
    Friend WithEvents DGV As System.Windows.Forms.DataGridView
    Friend WithEvents Btn_AddAbove As System.Windows.Forms.Button
    Friend WithEvents Btn_AddBelow As System.Windows.Forms.Button
    Friend WithEvents Btn_Del As System.Windows.Forms.Button
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents C_Name As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents C_User As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents C_Serial As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents C_Model As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents C_Start As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents C_End As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents C_Raw As System.Windows.Forms.DataGridViewTextBoxColumn

End Class
