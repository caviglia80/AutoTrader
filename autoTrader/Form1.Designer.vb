<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
	Inherits System.Windows.Forms.Form

	'Form overrides dispose to clean up the component list.
	<System.Diagnostics.DebuggerNonUserCode()>
	Protected Overrides Sub Dispose(ByVal disposing As Boolean)
		Try
			If disposing AndAlso components IsNot Nothing Then
				components.Dispose()
			End If
		Finally
			MyBase.Dispose(disposing)
		End Try
	End Sub

	'Required by the Windows Form Designer
	Private components As System.ComponentModel.IContainer

	'NOTE: The following procedure is required by the Windows Form Designer
	'It can be modified using the Windows Form Designer.  
	'Do not modify it using the code editor.
	<System.Diagnostics.DebuggerStepThrough()>
	Private Sub InitializeComponent()
		Me.components = New System.ComponentModel.Container()
		Me.Button1 = New System.Windows.Forms.Button()
		Me.tBatch = New System.Windows.Forms.Timer(Me.components)
		Me.btnVerLog = New System.Windows.Forms.Button()
		Me.lEstado = New System.Windows.Forms.Label()
		Me.btnBorrarBD = New System.Windows.Forms.Button()
		Me.btnActualizarBTC24 = New System.Windows.Forms.Button()
		Me.SuspendLayout()
		'
		'Button1
		'
		Me.Button1.Location = New System.Drawing.Point(21, 219)
		Me.Button1.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
		Me.Button1.Name = "Button1"
		Me.Button1.Size = New System.Drawing.Size(144, 64)
		Me.Button1.TabIndex = 0
		Me.Button1.Text = "TESTS"
		Me.Button1.UseVisualStyleBackColor = True
		'
		'tBatch
		'
		Me.tBatch.Enabled = True
		Me.tBatch.Interval = 20000
		'
		'btnVerLog
		'
		Me.btnVerLog.Location = New System.Drawing.Point(12, 12)
		Me.btnVerLog.Name = "btnVerLog"
		Me.btnVerLog.Size = New System.Drawing.Size(75, 23)
		Me.btnVerLog.TabIndex = 1
		Me.btnVerLog.Text = "Ver Log"
		Me.btnVerLog.UseVisualStyleBackColor = True
		'
		'lEstado
		'
		Me.lEstado.AutoSize = True
		Me.lEstado.Location = New System.Drawing.Point(93, 98)
		Me.lEstado.Name = "lEstado"
		Me.lEstado.Size = New System.Drawing.Size(49, 15)
		Me.lEstado.TabIndex = 2
		Me.lEstado.Text = "ESTADO"
		'
		'btnBorrarBD
		'
		Me.btnBorrarBD.Location = New System.Drawing.Point(12, 41)
		Me.btnBorrarBD.Name = "btnBorrarBD"
		Me.btnBorrarBD.Size = New System.Drawing.Size(75, 23)
		Me.btnBorrarBD.TabIndex = 3
		Me.btnBorrarBD.Text = "Borrar BD"
		Me.btnBorrarBD.UseVisualStyleBackColor = True
		'
		'btnActualizarBTC24
		'
		Me.btnActualizarBTC24.Location = New System.Drawing.Point(12, 70)
		Me.btnActualizarBTC24.Name = "btnActualizarBTC24"
		Me.btnActualizarBTC24.Size = New System.Drawing.Size(75, 43)
		Me.btnActualizarBTC24.TabIndex = 4
		Me.btnActualizarBTC24.Text = "Actualizar BTC 24"
		Me.btnActualizarBTC24.UseVisualStyleBackColor = True
		'
		'Form1
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ClientSize = New System.Drawing.Size(364, 294)
		Me.Controls.Add(Me.btnActualizarBTC24)
		Me.Controls.Add(Me.btnBorrarBD)
		Me.Controls.Add(Me.lEstado)
		Me.Controls.Add(Me.btnVerLog)
		Me.Controls.Add(Me.Button1)
		Me.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
		Me.Name = "Form1"
		Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
		Me.Text = "Form1"
		Me.TopMost = True
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub

	Friend WithEvents Button1 As Button
	Friend WithEvents tBatch As Timer
	Friend WithEvents btnVerLog As Button
	Friend WithEvents lEstado As Label
	Friend WithEvents btnBorrarBD As Button
	Friend WithEvents btnActualizarBTC24 As Button
End Class
