Imports System.Data.SQLite





Module DB



	Private strConnection As String = "Data Source=C:\GitHub\AutoTrader\autoTrader\DB\db.db; Integrated Security=true"


	'LEER TCoins(ESTA HABILITADA PARA COMPRAR/VENDER?) 
	Public Function TCoins_isEnabled(ByVal Coin As String, ByVal Side As String) As Boolean
		Try
			Dim result
			Using SQLiteConnection As New SQLiteConnection With {.ConnectionString = strConnection}
				SQLiteConnection.Open()

				If Side.Equals("BUY") Then
					Using cmd As New SQLiteCommand With {
					.Connection = SQLiteConnection,
					.CommandText = String.Concat("SELECT EnabledBuys FROM tCoins WHERE Coin =""", Coin, """;")}
						Dim SQLiteReader As SQLiteDataReader = cmd.ExecuteReader()
						SQLiteReader.Read()
						'OR SQLiteReader("Enabled")
						result = CBool(CInt(SQLiteReader(0)))
						SQLiteReader.Close()
					End Using
				Else
					Using cmd As New SQLiteCommand With {
					.Connection = SQLiteConnection,
					.CommandText = String.Concat("SELECT EnabledSells FROM tCoins WHERE Coin =""", Coin, """;")}
						Dim SQLiteReader As SQLiteDataReader = cmd.ExecuteReader()
						SQLiteReader.Read()
						result = CBool(CInt(SQLiteReader(0)))
						SQLiteReader.Close()
					End Using
				End If
			End Using
			Return result
		Catch ex As Exception
			WriteLog(ex.Message & "/ ERR: TCoins_isEnabled()")
			MsgBox(ex.Message)
		End Try
		Return Nothing
	End Function

	'LEER TCoins(LLEGO A LAS COMPRAS SIMULTANEAS MAXIMAS ? LO USO PARA VER SI PUEDO SEGUIR COMPRANDO) 
	Public Function TCoinsTBuys_isAvailableToBuy(Coin As String) As Boolean
		Try
			Using SQLiteConnection As New SQLiteConnection With {.ConnectionString = strConnection}
				SQLiteConnection.Open()

				Dim max As Integer = 0
				Dim curren As Integer = 0

				Using cmd As New SQLiteCommand With {
					.Connection = SQLiteConnection,
					.CommandText = String.Concat("SELECT MaxBuys FROM tCoins WHERE Coin =""", Coin, """;")}
					Dim SQLiteReader As SQLiteDataReader = cmd.ExecuteReader()
					SQLiteReader.Read()
					max = CInt(SQLiteReader(0))
					SQLiteReader.Close()
				End Using

				Using cmd As New SQLiteCommand With {
					.Connection = SQLiteConnection,
					.CommandText = String.Concat("SELECT COUNT(*) FROM tBuys WHERE Coin=""", Coin, """ AND Selled=0;")}
					Dim SQLiteReader As SQLiteDataReader = cmd.ExecuteReader()
					SQLiteReader.Read()
					curren = CInt(SQLiteReader(0))
					SQLiteReader.Close()
				End Using

				If (max - curren) <= 0 Then Return False Else Return True
			End Using
		Catch ex As Exception
			WriteLog(ex.Message & "/ ERR: TCoinsTBuys_isAvailableToBuy()")
			MsgBox(ex.Message)
		End Try
		Return Nothing
	End Function

	'LEER TCoins(OBTENER PORCENTAJE DE COMPRA O VENTA) 
	Public Function TCoins_percOperation(Coin As String, Side As String) As Double
		Try
			Dim result
			Using SQLiteConnection As New SQLiteConnection With {.ConnectionString = strConnection}
				SQLiteConnection.Open()
				If Side.Equals("BUY") Then
					Using cmd As New SQLiteCommand With {
					.Connection = SQLiteConnection,
					.CommandText = String.Concat("SELECT BuyPercentage FROM tCoins WHERE Coin=""", Coin, """;")}
						Dim SQLiteReader As SQLiteDataReader = cmd.ExecuteReader()
						SQLiteReader.Read()
						result = CDbl(SQLiteReader(0))
						SQLiteReader.Close()
					End Using
				Else
					Using cmd As New SQLiteCommand With {
					.Connection = SQLiteConnection,
					.CommandText = String.Concat("SELECT SellPercentage FROM tCoins WHERE Coin =""", Coin, """;")}
						Dim SQLiteReader As SQLiteDataReader = cmd.ExecuteReader()
						SQLiteReader.Read()
						result = CDbl(SQLiteReader(0))
						SQLiteReader.Close()
					End Using
				End If
			End Using
			Return result

		Catch ex As Exception
			WriteLog(ex.Message & "/ ERR: TCoins_percOperation()")
			MsgBox(ex.Message)
		End Try
		Return Nothing
	End Function

	'LEER TCoins(OBTENER STOP LOST) 
	Public Function TCoins_getStopLost(Coin As String) As Double
		Try
			Dim result
			Using SQLiteConnection As New SQLiteConnection With {.ConnectionString = strConnection}
				SQLiteConnection.Open()
				Using cmd As New SQLiteCommand With {
					.Connection = SQLiteConnection,
					.CommandText = String.Concat("SELECT StopLost FROM tCoins WHERE Coin=""", Coin, """;")}
					Dim SQLiteReader As SQLiteDataReader = cmd.ExecuteReader()
					SQLiteReader.Read()
					result = CDbl(SQLiteReader(0))
					SQLiteReader.Close()
				End Using
			End Using
			Return result

		Catch ex As Exception
			WriteLog(ex.Message & "/ ERR: TCoins_percOperation()")
			MsgBox(ex.Message)
		End Try
		Return Nothing
	End Function

	'LEER TCoins(GET OperationLastPrice)
	Public Function TCoins_getLastPriceOperations(Coin As String) As Double
		Try
			Dim result
			Using SQLiteConnection As New SQLiteConnection With {.ConnectionString = strConnection}
				SQLiteConnection.Open()

				Using cmd As New SQLiteCommand With {
					.Connection = SQLiteConnection,
					.CommandText = String.Concat("SELECT OperationLastPrice FROM tCoins WHERE Coin =""", Coin, """;")}

					Dim SQLiteReader As SQLiteDataReader = cmd.ExecuteReader()
					SQLiteReader.Read()

					'or SQLiteReader("OperationLastPrice")
					result = CDbl(SQLiteReader(0))

					SQLiteReader.Close()
				End Using
			End Using
			Return result
		Catch ex As Exception
			WriteLog(ex.Message & "/ ERR: TCoins_getLastPriceOperations()")
			MsgBox(ex.Message)
		End Try
		Return Nothing
	End Function

	'LEER TCoins(OBTENER LISTA DE COINS)
	Public Function TCoins_getListOfCoins() As List(Of String)
		Try
			Dim result As New List(Of String)
			Using SQLiteConnection As New SQLiteConnection With {.ConnectionString = strConnection}
				SQLiteConnection.Open()

				Using cmd As New SQLiteCommand With {
					.Connection = SQLiteConnection,
					.CommandText = String.Concat("SELECT Coin FROM tCoins;")}
					Dim SQLiteReader As SQLiteDataReader = cmd.ExecuteReader()

					While SQLiteReader.Read()
						result.Add(CStr(SQLiteReader(0)))
					End While

					SQLiteReader.Close()
				End Using
			End Using
			Return result
		Catch ex As Exception
			WriteLog(ex.Message & "/ ERR: TCoins_getListOfCoins()")
			MsgBox(ex.Message)
		End Try
		Return Nothing
	End Function

	'ACTUALIZO TBuys(NUEVA COMPRA) Y TCoins(REMPLAZO CON EL ULTIMO OperationLastPrice y LastOperationDate).
	Public Function TBuysTCoins_NewBuy(Coin As SIMBOLO, _Date As Date, USDT As String, Quantity As String, Optional Comments As String = "") As Boolean
		Try
			Using SQLiteConnection As New SQLiteConnection With {.ConnectionString = strConnection}
				SQLiteConnection.Open()

				Dim comment As String = String.Concat("BUY-> BTC: ", CAMBIO24HS_BTC.ToString("0.00"), "%, Sensibilidad: ", SENSIBILIDAD_COMPRA.ToString(), "%, ", Comments)
				Using cmd As New SQLiteCommand With {
					.Connection = SQLiteConnection,
					.CommandText = String.Concat("INSERT INTO tBuys (Coin, Date, USDT, Quantity, MarketPrice, Comments) VALUES (""", Coin.Symbol, """,""", _Date, """,""", USDT.Replace(",", "."), """,""", Quantity, """,""", Coin.lastPrice.ToString().Replace(",", "."), """,""", comment, """);")}
					cmd.ExecuteNonQuery()
				End Using

				Using cmd As New SQLiteCommand With {
					.Connection = SQLiteConnection,
					.CommandText = String.Concat("UPDATE tCoins SET OperationLastPrice =""", Coin.lastPrice.ToString().Replace(",", "."), """ WHERE Coin =""", Coin.Symbol, """;")}
					cmd.ExecuteNonQuery()
				End Using

				Using cmd As New SQLiteCommand With {
					.Connection = SQLiteConnection,
					.CommandText = String.Concat("UPDATE tCoins SET LastOperationDate =""", _Date, """ WHERE Coin =""", Coin.Symbol, """;")}
					cmd.ExecuteNonQuery()
				End Using

				Using cmd As New SQLiteCommand With {
					.Connection = SQLiteConnection,
					.CommandText = String.Concat("UPDATE tCoins SET Quantity = Quantity + """, Quantity, """ WHERE Coin =""", Coin.Symbol, """;")}
					cmd.ExecuteNonQuery()
				End Using
			End Using



			Tendencia("BTCUSDT", "1h")




			Return True
		Catch ex As Exception
			WriteLog(ex.Message & "/ ERR: TBuysTCoins_NewBuy()")
			MsgBox(ex.Message)
		End Try
		Return False
	End Function

	'ACTUALIZO TBuys(LA MARCO COMO VENDIDA) 
	Public Function TBuys_NowSelled(Coin As SIMBOLO) As Boolean
		Try
			Using SQLiteConnection As New SQLiteConnection With {.ConnectionString = strConnection}
				SQLiteConnection.Open()

				Using cmd As New SQLiteCommand With {
					.Connection = SQLiteConnection,
					.CommandText = String.Concat("UPDATE tBuys SET Selled=1 WHERE ID=", Coin.ID, ";")}
					cmd.ExecuteNonQuery()
				End Using

				Dim Comments As String = String.Concat(", SELL-> BTC: ", CAMBIO24HS_BTC.ToString("0.00"), "%, ")
				Using cmd As New SQLiteCommand With {
					.Connection = SQLiteConnection,
					.CommandText = String.Concat("UPDATE tBuys SET Comments=Comments||""", Comments, """ WHERE ID=", Coin.ID, ";")}
					cmd.ExecuteNonQuery()
				End Using

				Using cmd As New SQLiteCommand With {
					.Connection = SQLiteConnection,
					.CommandText = String.Concat("UPDATE tBuys SET SL=""", If(Coin.SL, "SI", "NO"), """ WHERE ID=", Coin.ID, ";")}
					cmd.ExecuteNonQuery()
				End Using
			End Using
			Return True
		Catch ex As Exception
			WriteLog(ex.Message & "/ ERR: TBuys_NowSelled()")
			MsgBox(ex.Message)
		End Try
		Return False
	End Function

	'ACTUALIZO TSells(INSERTO VENTA) Y TCoins(ACTUALIZO OperationLastPrice, Profit_USDT, Profit_Quantity).
	Public Function TSellsTCoins_NewSell(Coin As SIMBOLO, _Date As Date, ProfitUSDTbr As Double, Quantity As Double, ProfitUSDT As Double) As Boolean
		Try
			Using SQLiteConnection As New SQLiteConnection With {.ConnectionString = strConnection}
				SQLiteConnection.Open()

				Dim Comments As String = String.Concat("BTC: ", CAMBIO24HS_BTC.ToString("0.00"), "%, ")
				Using cmd As New SQLiteCommand With {
					.Connection = SQLiteConnection,
					.CommandText = String.Concat("INSERT INTO tSells (Coin, Date, ProfitUSDTbr, Quantity, ProfitUSDT, SL, Comments) VALUES (""", Coin.Symbol, """,""", _Date, """,""", CStr(ProfitUSDTbr).Replace(",", "."), """,""", CStr(Quantity).Replace(",", "."), """,""", CStr(ProfitUSDT).Replace(",", "."), """,""", If(Coin.SL, "SI", "NO"), """,""", Comments, """);")}
					cmd.ExecuteNonQuery()
				End Using

				Using cmd As New SQLiteCommand With {
					.Connection = SQLiteConnection,
					.CommandText = String.Concat("UPDATE tCoins SET OperationLastPrice =""", CStr(Coin.MarketPrice).Replace(",", "."), """ WHERE Coin =""", Coin.Symbol, """;")}
					cmd.ExecuteNonQuery()
				End Using

				Using cmd As New SQLiteCommand With {
					.Connection = SQLiteConnection,
					.CommandText = String.Concat("UPDATE tCoins SET LastOperationDate =""", _Date, """ WHERE Coin =""", Coin.Symbol, """;")}
					cmd.ExecuteNonQuery()
				End Using

				Using cmd As New SQLiteCommand With {
					.Connection = SQLiteConnection,
					.CommandText = String.Concat("UPDATE tCoins SET ProfitUSDT=ProfitUSDT+""", CStr(ProfitUSDT).Replace(",", "."), """ WHERE Coin =""", Coin.Symbol, """;")}
					cmd.ExecuteNonQuery()
				End Using

				Using cmd As New SQLiteCommand With {
					.Connection = SQLiteConnection,
					.CommandText = String.Concat("UPDATE tCoins SET Quantity=Quantity-""", CStr(Quantity).Replace(",", "."), """ WHERE Coin =""", Coin.Symbol, """;")}
					cmd.ExecuteNonQuery()
				End Using

				Using cmd As New SQLiteCommand With {
					.Connection = SQLiteConnection,
					.CommandText = String.Concat("UPDATE tCoins SET OperationLastPrice =""", CStr(Coin.MarketPrice).Replace(",", "."), """ WHERE Coin =""", Coin.Symbol, """;")}
					cmd.ExecuteNonQuery()
				End Using

			End Using
			Return True
		Catch ex As Exception
			WriteLog(ex.Message & "/ ERR: TSellsTCoins_NewSell()")
			MsgBox(ex.Message)
		End Try
		Return False
	End Function

	'LEER LAS FILAS DE tBuys (COIN, PRICE, QTY)(SELLED=0)
	Public Function TBuys_getList(Optional Selled As Boolean = False) As List(Of SIMBOLO)
		Try
			Dim result As New List(Of SIMBOLO)
			Using SQLiteConnection As New SQLiteConnection With {.ConnectionString = strConnection}
				SQLiteConnection.Open()

				If Selled Then
					Using cmd As New SQLiteCommand With {
					.Connection = SQLiteConnection,
					.CommandText = String.Concat("SELECT ID, Coin, MarketPrice, USDT, Quantity FROM tBuys WHERE Selled=1;")}
						Dim SQLiteReader As SQLiteDataReader = cmd.ExecuteReader()

						While SQLiteReader.Read()
							result.Add(New SIMBOLO(CStr(SQLiteReader(0)), CStr(SQLiteReader(1)), CStr(SQLiteReader(2)), CStr(SQLiteReader(3)), CStr(SQLiteReader(4))))
						End While

						SQLiteReader.Close()
					End Using
				Else
					Using cmd As New SQLiteCommand With {
					.Connection = SQLiteConnection,
					.CommandText = String.Concat("SELECT ID, Coin, MarketPrice, USDT, Quantity FROM tBuys WHERE Selled=0;")}
						Dim SQLiteReader As SQLiteDataReader = cmd.ExecuteReader()

						While SQLiteReader.Read()
							result.Add(New SIMBOLO(CStr(SQLiteReader(0)), CStr(SQLiteReader(1)), CStr(SQLiteReader(2)), CStr(SQLiteReader(3)), CStr(SQLiteReader(4))))
						End While

						SQLiteReader.Close()
					End Using
				End If
			End Using
			Return result
		Catch ex As Exception
			WriteLog(ex.Message & "/ ERR: TBuys_getList()")
			MsgBox(ex.Message)
		End Try
		Return Nothing
	End Function

	'LEER tBuys(GET MarketPrice)
	Public Function TBuys_getMarketPrice(Coin As String) As Double
		Try
			Dim result
			Using SQLiteConnection As New SQLiteConnection With {.ConnectionString = strConnection}
				SQLiteConnection.Open()

				Using cmd As New SQLiteCommand With {
					.Connection = SQLiteConnection,
					.CommandText = String.Concat("SELECT MarketPrice FROM tBuys WHERE Coin =""", Coin, """;")}

					Dim SQLiteReader As SQLiteDataReader = cmd.ExecuteReader()
					SQLiteReader.Read()
					result = CDbl(SQLiteReader(0))
					SQLiteReader.Close()
				End Using
			End Using
			Return result
		Catch ex As Exception
			WriteLog(ex.Message & "/ ERR: TBuys_getMarketPrice()")
			MsgBox(ex.Message)
		End Try
		Return Nothing
	End Function

	'PONER A 0 LA BD
	Public Sub BD_Restart()
		Try
			Using SQLiteConnection As New SQLiteConnection With {.ConnectionString = strConnection}
				SQLiteConnection.Open()

				Using cmd As New SQLiteCommand With {
					.Connection = SQLiteConnection,
					.CommandText = String.Concat("UPDATE tCoins SET OperationLastPrice=99999;")}
					cmd.ExecuteNonQuery()
				End Using

				Using cmd As New SQLiteCommand With {
					.Connection = SQLiteConnection,
					.CommandText = String.Concat("UPDATE tCoins SET ProfitUSDT=0.0;")}
					cmd.ExecuteNonQuery()
				End Using

				Using cmd As New SQLiteCommand With {
					.Connection = SQLiteConnection,
					.CommandText = String.Concat("UPDATE tCoins SET Quantity=0.0;")}
					cmd.ExecuteNonQuery()
				End Using

				Using cmd As New SQLiteCommand With {
					.Connection = SQLiteConnection,
					.CommandText = String.Concat("UPDATE tCoins SET LastOperationDate=0;")}
					cmd.ExecuteNonQuery()
				End Using

				Using cmd As New SQLiteCommand With {
					.Connection = SQLiteConnection,
					.CommandText = String.Concat("DELETE FROM tBuys;")}
					cmd.ExecuteNonQuery()
				End Using

				Using cmd As New SQLiteCommand With {
					.Connection = SQLiteConnection,
					.CommandText = String.Concat("DELETE FROM tSells;")}
					cmd.ExecuteNonQuery()
				End Using

				Using cmd As New SQLiteCommand With {
					.Connection = SQLiteConnection,
					.CommandText = String.Concat("DELETE FROM tBuysTemp;")}
					cmd.ExecuteNonQuery()
				End Using

			End Using

			MsgBox("BD Borrada")
			Form1.Dispose()
		Catch ex As Exception
			WriteLog(ex.Message & "/ ERR: BD_Restart()")
			MsgBox(ex.Message)
		End Try
	End Sub

	'ACTUALIZO Comments en Tbuys
	Public Function Table_Comments(Table As String, ID As String, Comment As String) As Boolean
		Try
			Using SQLiteConnection As New SQLiteConnection With {.ConnectionString = strConnection}
				SQLiteConnection.Open()
				Using cmd As New SQLiteCommand With {
					.Connection = SQLiteConnection,
					.CommandText = String.Concat("UPDATE """, Table, """ SET Comments=Comments||""", Comment, """ WHERE ID =""", ID, """;")}
					cmd.ExecuteNonQuery()
				End Using
			End Using
			Return True
		Catch ex As Exception
			WriteLog(ex.Message & "/ ERR: Tbuys_Comments()")
			MsgBox(ex.Message)
		End Try
		Return False
	End Function

	'LEER tBuys(obtener fraccion de completado)
	Public Function TBuys_GET_Completado(Coin As String) As String
		Try
			Dim current1 As Integer = 0
			Dim current2 As Integer = 0

			Using SQLiteConnection As New SQLiteConnection With {.ConnectionString = strConnection}
				SQLiteConnection.Open()

				Using cmd As New SQLiteCommand With {
					.Connection = SQLiteConnection,
					.CommandText = String.Concat("SELECT COUNT(*) FROM tBuys WHERE Coin=""", Coin, """ AND Selled=1;")}
					Dim SQLiteReader As SQLiteDataReader = cmd.ExecuteReader()
					SQLiteReader.Read()
					current1 = CInt(SQLiteReader(0))
					SQLiteReader.Close()
				End Using

				Using cmd As New SQLiteCommand With {
					.Connection = SQLiteConnection,
					.CommandText = String.Concat("SELECT COUNT(*) FROM tBuys WHERE Coin=""", Coin, """;")}
					Dim SQLiteReader As SQLiteDataReader = cmd.ExecuteReader()
					SQLiteReader.Read()
					current2 = CInt(SQLiteReader(0))
					SQLiteReader.Close()
				End Using
			End Using

			Return String.Concat(current1, "/", current2)
		Catch ex As Exception
			WriteLog(ex.Message & "/ ERR: TBuys_Completado()")
			MsgBox(ex.Message)
		End Try
		Return Nothing
	End Function

	'ACTUALIZO tCoins Completado
	Public Function TCoins_SET_Completado(Coin As String, Conteo As String) As Boolean
		Try
			Using SQLiteConnection As New SQLiteConnection With {.ConnectionString = strConnection}
				SQLiteConnection.Open()

				Using cmd As New SQLiteCommand With {
					.Connection = SQLiteConnection,
					.CommandText = String.Concat("UPDATE tCoins SET Completado = """, Conteo, """ WHERE Coin =""", Coin, """;")}
					cmd.ExecuteNonQuery()
				End Using
			End Using
			Return True
		Catch ex As Exception
			WriteLog(ex.Message & "/ ERR: TCoins_SET_Completado()")
			MsgBox(ex.Message)
		End Try
		Return False
	End Function

	'LEER TCoins(OBTENER INVERSION)
	Public Function TCoins_getInversion(Coin As String) As Double
		Try
			Dim result As Double = 0
			Using SQLiteConnection As New SQLiteConnection With {.ConnectionString = strConnection}
				SQLiteConnection.Open()

				Using cmd As New SQLiteCommand With {
					.Connection = SQLiteConnection,
					.CommandText = String.Concat("SELECT Inversion FROM tCoins WHERE Coin=""", Coin, """;")}
					Dim SQLiteReader As SQLiteDataReader = cmd.ExecuteReader()
					SQLiteReader.Read()
					result = CDbl(SQLiteReader(0))
					SQLiteReader.Close()
				End Using
			End Using
			Return result
		Catch ex As Exception
			WriteLog(ex.Message & "/ ERR: TCoins_getListOfCoins()")
			MsgBox(ex.Message)
		End Try
		Return Nothing
	End Function

	'ACTUALIZO tBuysTemp
	Public Function TBuysTemp_NewLog(Coin As SIMBOLO) As Boolean
		Try
			Using SQLiteConnection As New SQLiteConnection With {.ConnectionString = strConnection}
				SQLiteConnection.Open()

				Using cmd As New SQLiteCommand With {
					.Connection = SQLiteConnection,
					.CommandText = String.Concat("INSERT INTO tBuysTemp (Coin, MarketPrice, Date) VALUES (""", Coin.Symbol, """,""", Coin.MarketPrice.ToString().Replace(",", "."), """,""", Now, """);")}
					cmd.ExecuteNonQuery()
				End Using

				'Using cmd As New SQLiteCommand With {
				'	.Connection = SQLiteConnection,
				'	.CommandText = String.Concat("UPDATE tCoins SET OperationLastPrice =""", MarketPrice.Replace(",", "."), """ WHERE Coin =""", Coin, """;")}
				'	cmd.ExecuteNonQuery()
				'End Using

				'Using cmd As New SQLiteCommand With {
				'	.Connection = SQLiteConnection,
				'	.CommandText = String.Concat("UPDATE tCoins SET LastOperationDate =""", _Date, """ WHERE Coin =""", Coin, """;")}
				'	cmd.ExecuteNonQuery()
				'End Using
			End Using
			Return True
		Catch ex As Exception
			WriteLog(ex.Message & "/ ERR: TBuysTemp_NewLog()")
			MsgBox(ex.Message)
		End Try
		Return False
	End Function

	'LEER tBuysTemp
	Public Function TBuysTemp_GetOldPrices() As List(Of SIMBOLO)
		Try
			Dim result As New List(Of SIMBOLO)
			Using SQLiteConnection As New SQLiteConnection With {.ConnectionString = strConnection}
				SQLiteConnection.Open()

				Using cmd As New SQLiteCommand With {
					.Connection = SQLiteConnection,
					.CommandText = String.Concat("SELECT ID, Coin, MarketPrice FROM tBuysTemp;")}
					Dim SQLiteReader As SQLiteDataReader = cmd.ExecuteReader()

					While SQLiteReader.Read()
						result.Add(New SIMBOLO(CStr(SQLiteReader(0)), CStr(SQLiteReader(1)), CStr(SQLiteReader(2))))
					End While

					SQLiteReader.Close()
				End Using
			End Using
			Return result
		Catch ex As Exception
			WriteLog(ex.Message & "/ ERR: TBuysTemp_GetOldPrices()")
			MsgBox(ex.Message)
		End Try
		Return Nothing
	End Function

	'BORRAR tBuysTemp ID
	Public Sub TBuysTemp_BorrarID(ID As String)
		Try
			Using SQLiteConnection As New SQLiteConnection With {.ConnectionString = strConnection}
				SQLiteConnection.Open()

				Using cmd As New SQLiteCommand With {
					.Connection = SQLiteConnection,
					.CommandText = String.Concat("DELETE FROM tBuysTemp WHERE ID =", ID, ";")}
					cmd.ExecuteNonQuery()
				End Using

			End Using

		Catch ex As Exception
			WriteLog(ex.Message & "/ ERR: TBuysTemp_BorrarID()")
			MsgBox(ex.Message)
		End Try
	End Sub


















	'"SELECT * from MySQLiteTable LIMIT 1;"
	'Public Function getSQL(cmd As String) As String
	'	Try
	'		Dim SQLitecmd As New SQLiteCommand
	'		Dim SQLiteReader As SQLiteDataReader
	'		SQLiteConn.Open()

	'		SQLitecmd.Connection = SQLiteConn
	'		SQLitecmd.CommandText = cmd
	'		SQLiteReader = SQLitecmd.ExecuteReader()

	'		MsgBox(SQLiteReader("Coin").ToString)

	'		SQLiteReader.Close()
	'		SQLiteConn.Close()
	'	Catch ex As Exception
	'		MsgBox(ex.Message)
	'	End Try

	'End Function




	'Public Sub tCoinsReplace()
	'	Dim SQLitecmd As New SQLiteCommand
	'	SQLiteConn.Open()
	'	SQLitecmd.Connection = SQLiteConn
	'	SQLitecmd.CommandText = "SELECT INTO ""tCoins"" (""Coin"", ""Enabled"", ""BuyPercentage"", ""SellPercentage"", ""OperationLastPrice"", ""ProfitUSDT"", ""Quantity"") VALUES (""ETCUSDT3"", 0, 6, 6, 8, 9, 10);"
	'	SQLitecmd.ExecuteNonQuery()
	'	SQLiteConn.Close()
	'	SQLitecmd.Dispose()
	'End Sub

	'Public Sub tCoinsInsert()
	'	Dim SQLitecmd As New SQLiteCommand
	'	SQLiteConn.Open()
	'	SQLitecmd.Connection = SQLiteConn
	'	SQLitecmd.CommandText = "INSERT INTO ""tCoins"" (""Coin"", ""Enabled"", ""BuyPercentage"", ""SellPercentage"", ""OperationLastPrice"", ""ProfitUSDT"", ""Quantity"") VALUES (""ETCUSDT3"", 0, 6, 6, 8, 9, 10);"
	'	SQLitecmd.ExecuteNonQuery()
	'	SQLiteConn.Close()
	'	SQLitecmd.Dispose()
	'End Sub












End Module


















