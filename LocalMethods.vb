Imports System.IO


Module LocalMethods


	'DEVUELVO LOS NOMBRES Y PRECIOS DE LAS MONEDAS A COMPRAR
	Public Function WeHaveBuys() As List(Of SIMBOLO)
		Try
			Dim readyToBuy As New List(Of SIMBOLO)

			For Each coin In WEB_GetPriceList(TCoins_getListOfCoins())

				Dim oldPrice As Double = TCoins_getLastPriceOperations(coin.Symbol)             'OBTENGO COIN LOCAL, ultimo precio registrado, YA SEA ULTIMA COMPRA O ULTIMA VENTA
				Dim newPrice As Double = coin.MarketPrice                                             'OBTENGO LA LISTA REMOTA DE PRECIOS
				Dim precentageAllowedBUY As Double = TCoins_percOperation(coin.Symbol, "BUY")   'OBTENGO EL PORCENTAGE CONFIGURADO DE LA COIN

				Dim result As Double = ((newPrice * 100) / oldPrice) - 100 'REGLA DE 3 SIMPLES PARA SABER EL PORCENTAJE DE CAMBIO

				'MsgBox(coin.symbol & "  SUBIO O BAJO UN % :  " & (result).ToString & "   " & precentageAllowedBUY.ToString)

				WriteLog(String.Concat(vbTab, "| ", coin.Symbol, vbTab, vbTab, "VAR: ", If(result.ToString("0.000").Contains("-"), result.ToString("0.000"), result.ToString(" 0.000")), "%", vbTab, "%UMBRAL: ", precentageAllowedBUY.ToString, vbTab, " Old Price:", vbTab, oldPrice.ToString, If(oldPrice.ToString.Length <= 3, vbTab, ""), If(oldPrice.ToString.Length >= 8, "", vbTab), vbTab, "New Price:", vbTab, newPrice.ToString).Replace(",", "."))

				'CONSULTO SI EL PORCENTAJE CALCULADO (result) ES MENOR QUE EL CONFIGURADO (precentageAllowedBUY), SE COMPRA (INVERSA EN VENTA)
				If result <= precentageAllowedBUY Then
					coin.Var = result
					'readyToBuy.Add(New SIMBOLO(coin.Symbol, coin.MarketPrice))
					readyToBuy.Add(coin)
				End If
			Next

			Return readyToBuy
		Catch ex As Exception
			WriteLog("ERR: GetVariations()")
			MsgBox(ex.Message)
		End Try
		Return Nothing
	End Function

	Public Function WeHaveBuysV2() As List(Of SIMBOLO)
		Try
			Dim readyToBuy As New List(Of SIMBOLO)

			For Each coin In WEB_GetPriceList24h(TCoins_getListOfCoins())

				'"highPrice" "20.79000000",
				'"lowPrice" "20.16000000",
				'"lastPrice" "20.37000000",

				Dim PrecioMedioEntreMinYMax As Double = coin.lowPrice + ((coin.highPrice - coin.lowPrice) / 2)

				Dim PriceAPiso As Double = Math.Abs(coin.lastPrice - coin.lowPrice) ' Distancia entre el precio actual y LowPrice
				Dim PriceATecho As Double = Math.Abs(coin.lastPrice - PrecioMedioEntreMinYMax) ' Distancia entre el número actual y el PrecioMedio

				'SI EL PRECIO ACTUAL ES MAS CERCANO AL SUELO QUE DEL TECHO, COMPRO
				If PriceAPiso * SENSIBILIDAD_COMPRA < PriceATecho Then
					'MsgBox("compro " & coin.Symbol & "  " & PriceAPiso & "  " & PriceATecho & "  " & PrecioMedioEntreMinYMax)

					Dim oldPrice As Double = TCoins_getLastPriceOperations(coin.Symbol)  'OBTENGO COIN LOCAL, ultimo precio registrado, YA SEA ULTIMA COMPRA O ULTIMA VENTA
					Dim result As Double = ((coin.lastPrice * 100) / oldPrice) - 100 'REGLA DE 3 SIMPLES PARA SABER EL PORCENTAJE DE CAMBIO
					Dim precentageAllowedBUY As Double = TCoins_percOperation(coin.Symbol, "BUY")   'OBTENGO EL PORCENTAGE CONFIGURADO DE LA COIN

					'CONSULTO SI EL PORCENTAJE CALCULADO (result) ES MENOR QUE EL CONFIGURADO (precentageAllowedBUY), SE COMPRA (INVERSA EN VENTA)
					If result <= precentageAllowedBUY Then
						readyToBuy.Add(coin)
						WriteLog(String.Concat(vbTab, "|* ", coin.Symbol, "  ", vbTab, vbTab, "PriceAPiso: ", PriceAPiso.ToString("0.0000"), vbTab, "PriceATecho: ", PriceATecho.ToString("0.0000"), vbTab, " lastPrice:", vbTab, coin.lastPrice.ToString))
					End If
				Else
					WriteLog(String.Concat(vbTab, "|  ", coin.Symbol, "  ", vbTab, vbTab, "PriceAPiso: ", PriceAPiso.ToString("0.0000"), vbTab, "PriceATecho: ", PriceATecho.ToString("0.0000"), vbTab, " lastPrice:", vbTab, coin.lastPrice.ToString))
				End If
			Next

			Return readyToBuy
		Catch ex As Exception
			WriteLog("ERR: WeHaveBuysV2()")
			MsgBox(ex.Message)
		End Try
		Return Nothing
	End Function

	Public Function QuantityNormalized(coin As String, Qty As String) As String
		Try
			'WriteLog("0")

			Qty = Qty.Replace(",", ".")
			Dim json As New Chilkat.JsonObject()

			Dim JsonResponse As String = _GET(String.Concat("/api/v3/exchangeInfo?symbol=", coin))
			If IsError(JsonResponse) Then Return Nothing
			json.Load(JsonResponse)

			Dim minQty As String = json.StringOf("symbols[0].filters[1].minQty")

			'WriteLog(Qty)

			Try
				'WriteLog("1")

				If CInt(minQty.Split("."c)(0).Trim) = 1 Then    'TIENE 1 DELANTE DE LA COMA
					Try
						Qty = Qty.Split("."c)(0)
						'WriteLog("1.2")

						Return Qty.Trim
					Catch ex As Exception
						WriteLog("ERR: QuantityNormalized() 4")
						WriteLog(JsonResponse)
						MsgBox(ex.Message)
					End Try

				Else                                            'TIENE 1 DETRAS DE LA COMA

					Try
						Dim indexDelUno As Integer = minQty.Split("."c)(1).Trim().IndexOf("1")
						'WriteLog("indexDelUno = " + indexDelUno.ToString)
						'WriteLog("2")

						'WriteLog(Qty)

						'WriteLog("3")

						Dim onlyDecimals_Normalized As String = Qty.Split("."c)(1).Substring(0, indexDelUno + 1)

						'WriteLog("4")

						Qty = String.Concat(Qty.Split("."c)(0), ".", onlyDecimals_Normalized)

						'WriteLog("5")

						'If coin.Equals("TRXUSDT") Then
						'	WriteLog("TESTTTTTTTTTTT")
						'	WriteLog(JsonResponse)
						'End If

						Return Qty.Trim
					Catch ex As Exception
						WriteLog("ERR: QuantityNormalized() 3")
						WriteLog(JsonResponse)
						MsgBox(ex.Message)
					End Try
				End If
			Catch ex As Exception
				WriteLog("ERR: QuantityNormalized() 2")
				WriteLog(JsonResponse)
				MsgBox(ex.Message)
			End Try
		Catch ex As Exception
			WriteLog("ERR: QuantityNormalized() 1")
			MsgBox(ex.Message)
		End Try
		Return Nothing
	End Function

	Public Function calculateQuantityToBuy(coin As String, priceCurrentUSDT As Double, toBuyUSDT As Double) As String
		Try
			Dim result As Double = toBuyUSDT / priceCurrentUSDT
			'MsgBox(toBuyUSDT & " " & priceCurrentUSDT & " " & result)
			Return QuantityNormalized(coin, result.ToString)
		Catch ex As Exception
			MsgBox(ex.Message)
		End Try
		WriteLog("ERR: calculateQuantityToBuy()")
		Return Nothing
	End Function

	'DEVUELVO LISTA DE SIMBOLOS PARA VENDER
	Public Function WeHaveSells() As List(Of SIMBOLO)
		Try
			Dim readyToSell As New List(Of SIMBOLO)
			Dim listToSell_tmp As New List(Of SIMBOLO)
			Dim onlyNames As New List(Of String)

			'LEER LAS FILAS NO VENDIDAS DE tBuys (COIN, PRICE)(SELLED=0)
			For Each C In TBuys_getList(False)
				'VEMOS SI EL SIMBOLO ESTA HABILITADO PARA VENDER
				If TCoins_isEnabled(C.Symbol, "SELL") Then
					listToSell_tmp.Add(C)
					If Not onlyNames.FindAll(Function(x) x.Contains(C.Symbol)).Count > 0 Then onlyNames.Add(C.Symbol)
				End If
			Next

			If listToSell_tmp.Count = 0 Then Return New List(Of SIMBOLO)

			Dim prices As List(Of SIMBOLO) = WEB_GetPriceList(onlyNames)

			'TENGO QUE RECORRER TODOS LOS PRECIOS, SUMARLES UN 5% EJEMPLO, AL PRECIO Y COMPARARLO CON EL PRECIO ACTUAL
			For Each coinBuyed In listToSell_tmp

				Dim coinRT As SIMBOLO = prices.Find(Function(x) x.Symbol.Contains(coinBuyed.Symbol))
				Dim precentageAllowedSELL As Double = TCoins_percOperation(coinBuyed.Symbol, "SELL")        'OBTENGO EL PORCENTAGE CONFIGURADO DE LA COIN PARA VENTA
				Dim oldPrice As Double = coinBuyed.MarketPrice

				'TBuys_getMarketPrice(currentCoin.Symbol)
				Dim newPrice As Double = coinRT.Price

				'MsgBox(coinRT.Price & " " & coinRT.lastPrice)

				'coinBuyed.MarketPrice = currentCoin.MarketPrice
				coinBuyed.StopLost = TCoins_getStopLost(coinRT.Symbol)

				'REGLA DE 3 SIMPLES PARA SABER EL PORCENTAJE DE CAMBIO
				Dim result As Double = ((newPrice * 100) / oldPrice) - 100

				'MsgBox(result & "      " & coinBuyed.symbol & "      " & coinBuyed.quantity & "      " & precentageAllowedSELL)
				WriteLog(String.Concat(vbTab, "| ", coinBuyed.Symbol, vbTab, vbTab, "VAR: ", If(result.ToString("0.000").Contains("-"), result.ToString("0.000"), result.ToString(" 0.000")), "%", vbTab, "%UMBRAL: ", precentageAllowedSELL.ToString, vbTab, "%SL: ", coinBuyed.StopLost.ToString, vbTab, " Old Price: ", oldPrice.ToString, If(oldPrice.ToString.Length <= 3, vbTab, ""), If(oldPrice.ToString.Length >= 8, "", vbTab), vbTab, "New Price: ", newPrice.ToString).Replace(",", "."))

				'CONSULTO SI EL PORCENTAJE CALCULADO (result) ES MAYOR QUE EL CONFIGURADO (precentageAllowedSELL), SE VENDE (INVERSA EN COMPRA)
				If result >= precentageAllowedSELL Then
					'MsgBox("esta esta para vender    " & coinBuyed.symbol & "    " & coinBuyed.MarketPrice)
					readyToSell.Add(coinBuyed)

					'ElseIf result < coinBuyed.StopLost Then
					'	coinBuyed.SL = True
					'	readyToSell.Add(coinBuyed)
				End If
			Next

			Return readyToSell
		Catch ex As Exception
			WriteLog("ERR: WeHaveSells()")
			MsgBox(ex.Message)
		End Try
		Return Nothing
	End Function

	Public Function IsError(strResponse As String) As Boolean
		Try
			'NOTIFICAR SOLO ERROR ESPECIFICO
			If strResponse.Contains("code") Or strResponse.Contains(":-2010") Or strResponse.Contains("SIN_RED") Then
				If strResponse.Contains("insufficient balance") Then
					WriteLog(strResponse)
				ElseIf strResponse.Contains("SIN_RED") Then
					WriteLog("ERR(RESPONSE): NO HAY RED, O RED INESTABLE")
					'System.Threading.Thread.Sleep(300000)
				Else
					WriteLog(String.Concat("ERROR DESCONOCIDO: ", strResponse))
				End If

				Return True
			End If
		Catch ex As Exception
			MsgBox(ex.Message)
		End Try
		Return False
	End Function

	Public Sub WriteLog(log As String)
		Try
			File.AppendAllText(tmpLog, String.Concat(CStr(Now), vbTab, If(debugMode, "| DEBUG |", "|"), vbTab, log, vbCrLf))
		Catch ex As Exception
			MsgBox(ex.Message)
		End Try
	End Sub

	Public Sub WriteLine(log As String)
		Try
			Dim line As String = String.Concat("Line: ", New StackTrace(True).GetFrame(1).GetFileLineNumber.ToString)
			File.AppendAllText(tmpLog, String.Concat(CStr(Now), vbTab, If(debugMode, "| DEBUG |", "|"), vbTab, line, vbTab, "|", vbTab, log, vbCrLf))
		Catch ex As Exception
			MsgBox(ex.Message)
		End Try
	End Sub

	Public Function CargamosDatosDePrueba(coin As String, side As String, Qty As Object, Price As Object) As String
		Try
			Dim JsonResponse As String = File.ReadAllText("C:\Users\cavig\OneDrive\Escritorio\PIP\AUTO TRADER\autoTrader\TEST\BUY_SELL.txt")
			'Dim USDTtest As String = CStr(USDT_A_GASTAR_PER_COIN - ((USDT_A_GASTAR_PER_COIN * 0.3) / 100))

			Dim USDTtest As String = CStr(USDT_A_GASTAR_PER_COIN)
			JsonResponse = JsonResponse.Replace("executedQty1", CStr(Qty).Replace(",", ".")) 'QTY

			If side = "BUY" Then
				JsonResponse = JsonResponse.Replace("cummulativeQuoteQty1", USDTtest) 'USDT
			Else
				USDTtest = WEBGet_Price(coin) * CDbl(Qty)
				JsonResponse = JsonResponse.Replace("cummulativeQuoteQty1", USDTtest) 'USDT
			End If

			Return JsonResponse
		Catch ex As Exception
			MsgBox(ex.Message)
		End Try
		Return Nothing
	End Function

	Public Sub Sonido(Optional Profit As Boolean = True)
		Try
			If Profit Then
				My.Computer.Audio.Play("C:\Users\cavig\OneDrive\Escritorio\PIP\AUTO TRADER\autoTrader\venta.wav", AudioPlayMode.Background)
			Else
				My.Computer.Audio.Play("C:\Users\cavig\OneDrive\Escritorio\PIP\AUTO TRADER\autoTrader\noProfit.wav", AudioPlayMode.Background)
			End If
		Catch ex As Exception
			MsgBox(ex.Message)
		End Try
	End Sub








End Module




















