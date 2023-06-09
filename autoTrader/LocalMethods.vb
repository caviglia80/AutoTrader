﻿Imports System.IO
Module LocalMethods


	Public Function WeHaveBuysV2() As List(Of SIMBOLO)
		Try
			Dim readyToBuy As New List(Of SIMBOLO)

			For Each coin In WEB_GetPriceList24h(TCoins_getListOfCoins())

				If Not TCoins_isEnabled(coin.Symbol, "BUY") Then
					WriteLog(String.Concat("ADVERTENCIA: DESHABILITADA para la compra ", coin.Symbol))
					Continue For              'SI NO ESTA HABILITADA PARA COMPRAR, LA SALTEA
				End If

				If Not TCoinsTBuys_isAvailableToBuy(coin.Symbol) Then
					WriteLog(String.Concat("ADVERTENCIA: MAXIMO DE COMPRAS ALCANZADO ", coin.Symbol))
					Continue For         'SI LLEGO AL MAXIMO DE COMPRAS SIMULTANEAS, LA SALTEO
				End If

				Dim oldPrice As Double = TCoins_getLastPriceOperations(coin.Symbol)  'OBTENGO COIN LOCAL, ultimo precio registrado, YA SEA ULTIMA COMPRA O ULTIMA VENTA
				Dim precentageAllowedBUY As Double = TCoins_percOperation(coin.Symbol, "BUY")   'OBTENGO EL PORCENTAGE CONFIGURADO DE LA COIN

				Dim DistanciaEntreMinYPromedio As Double = (coin.highPrice - coin.lowPrice) / 2
				'Dim PrecioPromedio As Double = coin.lowPrice + DistanciaEntreMinYPromedio                   ' Precio medio entre min y max
				'Dim DistanciaAMin As Double = Math.Abs(coin.lastPrice - coin.lowPrice)                      ' Distancia entre CurrentPrice y LowPrice
				'Dim DistanciaAPromedio As Double = Math.Abs(coin.lastPrice - PrecioPromedio)                ' Distancia entre CurrentPrice y el PrecioPromedio
				Dim UmbralPermitido As Double = (SENSIBILIDAD_COMPRA * DistanciaEntreMinYPromedio) / 100    ' Edito el % y me devuelve el equivalente
				'Dim PorcentajeActual As Double = (coin.lastPrice * 100) / (coin.lowPrice + UmbralPermitido)


				'SI EL PRECIO ACTUAL ES MAS CERCANO AL LOWPRICE QUE DEL PROMEDIO, COMPRO
				'If DistanciaAMin * SENSIBILIDAD_COMPRA < DistanciaAPromedio Then
				If coin.lastPrice <= (coin.lowPrice + UmbralPermitido) Then

					Dim result As Double = ((coin.lastPrice * 100) / oldPrice) - 100 'REGLA DE 3 SIMPLES PARA SABER EL PORCENTAJE DE CAMBIO
					'CONSULTO SI EL PORCENTAJE CALCULADO (result) ES MENOR QUE EL CONFIGURADO (precentageAllowedBUY), SE COMPRA (INVERSA EN VENTA)
					If result <= precentageAllowedBUY Then
						readyToBuy.Add(coin)
						WriteLog(String.Concat(vbTab, "|** ", coin.Symbol, "  ", vbTab, vbTab, "Current: ", result.ToString("0.0000"), vbTab, "Umbral: ", precentageAllowedBUY.ToString("0.00"), vbTab, " OP: ", oldPrice.ToString("0.0000"), vbTab, " NP: ", coin.lastPrice.ToString("0.0000")))
					Else
						WriteLog(String.Concat(vbTab, "|*  ", coin.Symbol, "  ", vbTab, vbTab, "Current: ", result.ToString("0.0000"), vbTab, "Umbral: ", precentageAllowedBUY.ToString("0.00"), vbTab, " OP: ", oldPrice.ToString("0.0000"), vbTab, " NP: ", coin.lastPrice.ToString("0.0000")))
						'WriteLog(String.Concat(vbTab, "|   ", coin.Symbol, "  ", vbTab, vbTab, "TFalta: ", (coin.lastPrice - (coin.lowPrice + UmbralPermitido)).ToString("0.0000"), vbTab, " OP: ", oldPrice.ToString("0.0000"), vbTab, " NP: ", coin.lastPrice.ToString("0.0000")))
					End If
				Else
					WriteLog(String.Concat(vbTab, "|   ", coin.Symbol, "  ", vbTab, vbTab, "Sensibilidad: ", SENSIBILIDAD_COMPRA, "%  Falta: ", Math.Max(coin.lastPrice - (coin.lowPrice + UmbralPermitido), 0).ToString("0.00000000"), vbTab, " OP: ", oldPrice.ToString("0.00000000"), vbTab, " NP: ", coin.lastPrice.ToString("0.00000000")))
					'WriteLog(String.Concat(vbTab, "|   ", coin.Symbol, "  ", vbTab, vbTab, "TFalta: ", (coin.lastPrice - (coin.lowPrice + UmbralPermitido)).ToString("0.0000"), vbTab, " OP: ", oldPrice.ToString("0.0000"), vbTab, " NP: ", coin.lastPrice.ToString("0.0000")))
				End If
			Next

			Return readyToBuy
		Catch ex As Exception
			WriteLog(ex.Message & "/ ERR: WeHaveBuysV2()")
			MsgBox(ex.Message)
		End Try
		Return Nothing
	End Function

	'Public Function WeHaveBuysV2_Intelligent() As List(Of SIMBOLO)
	'	Try
	'		Dim readyToBuy As New List(Of SIMBOLO)

	'		For Each coin In WEB_GetPriceList24h(TCoins_getListOfCoins())

	'			If Not TCoins_isEnabled(coin.Symbol, "BUY") Then
	'				WriteLog(String.Concat("ADVERTENCIA: DESHABILITADA para la compra ", coin.Symbol))
	'				Continue For              'SI NO ESTA HABILITADA PARA COMPRAR, LA SALTEA
	'			End If

	'			If Not TCoinsTBuys_isAvailableToBuy(coin.Symbol) Then
	'				WriteLog(String.Concat("ADVERTENCIA: MAXIMO DE COMPRAS ALCANZADO ", coin.Symbol))
	'				Continue For         'SI LLEGO AL MAXIMO DE COMPRAS SIMULTANEAS, LA SALTEO
	'			End If

	'			Dim oldPrice As Double = TCoins_getLastPriceOperations(coin.Symbol)  'OBTENGO COIN LOCAL, ultimo precio registrado, YA SEA ULTIMA COMPRA O ULTIMA VENTA
	'			Dim precentageAllowedBUY As Double = TCoins_percOperation(coin.Symbol, "BUY")   'OBTENGO EL PORCENTAGE CONFIGURADO DE LA COIN

	'			Dim DistanciaEntreMinYPromedio As Double = (coin.highPrice - coin.lowPrice) / 2
	'			'Dim PrecioPromedio As Double = coin.lowPrice + DistanciaEntreMinYPromedio                   ' Precio medio entre min y max
	'			'Dim DistanciaAMin As Double = Math.Abs(coin.lastPrice - coin.lowPrice)                      ' Distancia entre CurrentPrice y LowPrice
	'			'Dim DistanciaAPromedio As Double = Math.Abs(coin.lastPrice - PrecioPromedio)                ' Distancia entre CurrentPrice y el PrecioPromedio
	'			Dim UmbralPermitido As Double = (SENSIBILIDAD_COMPRA * DistanciaEntreMinYPromedio) / 100    ' Edito el % y me devuelve el equivalente
	'			'Dim PorcentajeActual As Double = (coin.lastPrice * 100) / (coin.lowPrice + UmbralPermitido)


	'			'SI EL PRECIO ACTUAL ES MAS CERCANO AL LOWPRICE QUE DEL PROMEDIO, COMPRO
	'			'If DistanciaAMin * SENSIBILIDAD_COMPRA < DistanciaAPromedio Then
	'			If coin.lastPrice <= (coin.lowPrice + UmbralPermitido) Then

	'				Dim result As Double = ((coin.lastPrice * 100) / oldPrice) - 100 'REGLA DE 3 SIMPLES PARA SABER EL PORCENTAJE DE CAMBIO
	'				'CONSULTO SI EL PORCENTAJE CALCULADO (result) ES MENOR QUE EL CONFIGURADO (precentageAllowedBUY), SE COMPRA (INVERSA EN VENTA)
	'				If result <= precentageAllowedBUY Then
	'					readyToBuy.Add(coin)
	'					WriteLog(String.Concat(vbTab, "|** ", coin.Symbol, "  ", vbTab, vbTab, "Current: ", result.ToString("0.0000"), vbTab, "Umbral: ", precentageAllowedBUY.ToString("0.00"), vbTab, " OP: ", oldPrice.ToString("0.0000"), vbTab, " NP: ", coin.lastPrice.ToString("0.0000")))
	'				Else
	'					WriteLog(String.Concat(vbTab, "|*  ", coin.Symbol, "  ", vbTab, vbTab, "Current: ", result.ToString("0.0000"), vbTab, "Umbral: ", precentageAllowedBUY.ToString("0.00"), vbTab, " OP: ", oldPrice.ToString("0.0000"), vbTab, " NP: ", coin.lastPrice.ToString("0.0000")))
	'					'WriteLog(String.Concat(vbTab, "|   ", coin.Symbol, "  ", vbTab, vbTab, "TFalta: ", (coin.lastPrice - (coin.lowPrice + UmbralPermitido)).ToString("0.0000"), vbTab, " OP: ", oldPrice.ToString("0.0000"), vbTab, " NP: ", coin.lastPrice.ToString("0.0000")))
	'				End If
	'			Else
	'				WriteLog(String.Concat(vbTab, "|   ", coin.Symbol, "  ", vbTab, vbTab, "Sensibilidad: ", SENSIBILIDAD_COMPRA, "%  Falta: ", Math.Max(coin.lastPrice - (coin.lowPrice + UmbralPermitido), 0).ToString("0.00000000"), vbTab, " OP: ", oldPrice.ToString("0.00000000"), vbTab, " NP: ", coin.lastPrice.ToString("0.00000000")))
	'				'WriteLog(String.Concat(vbTab, "|   ", coin.Symbol, "  ", vbTab, vbTab, "TFalta: ", (coin.lastPrice - (coin.lowPrice + UmbralPermitido)).ToString("0.0000"), vbTab, " OP: ", oldPrice.ToString("0.0000"), vbTab, " NP: ", coin.lastPrice.ToString("0.0000")))
	'			End If
	'		Next

	'		Return readyToBuy
	'	Catch ex As Exception
	'		WriteLog(ex.Message & "/ ERR: WeHaveBuysV2()")
	'		MsgBox(ex.Message)
	'	End Try
	'	Return Nothing
	'End Function

	Public Function QuantityNormalized(coin As String, Qty As String) As String
		Try
			Qty = Qty.Replace(",", ".")
			Dim json As New Chilkat.JsonObject()
			Dim JsonResponse As String = _GET(String.Concat("/api/v3/exchangeInfo?symbol=", coin))
			If IsError(JsonResponse) Then Return Nothing
			json.Load(JsonResponse)
			Dim minQty As String = json.StringOf("symbols[0].filters[1].minQty")

			Try
				If CInt(minQty.Split("."c)(0).Trim) = 1 Then    'TIENE 1 DELANTE DE LA COMA
					Try
						Qty = Qty.Split("."c)(0)
						Return Qty.Trim
					Catch ex As Exception
						WriteLog(ex.Message & "/ ERR: QuantityNormalized() 4")
						WriteLog(JsonResponse)
						MsgBox(ex.Message)
					End Try
				Else                                            'TIENE 1 DETRAS DE LA COMA
					Try
						Dim indexDelUno As Integer = minQty.Split("."c)(1).Trim().IndexOf("1")
						Dim onlyDecimals_Normalized As String = Qty.Split("."c)(1).Substring(0, indexDelUno + 1)
						Qty = String.Concat(Qty.Split("."c)(0), ".", onlyDecimals_Normalized)
						Return Qty.Trim
					Catch ex As Exception
						WriteLog(ex.Message & "/ ERR: QuantityNormalized() 3")
						WriteLog(JsonResponse)
						MsgBox(ex.Message)
					End Try
				End If
			Catch ex As Exception
				WriteLog(ex.Message & "/ ERR: QuantityNormalized() 2")
				WriteLog(JsonResponse)
				MsgBox(ex.Message)
			End Try
		Catch ex As Exception
			WriteLog(ex.Message & "/ ERR: QuantityNormalized() 1")
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
			WriteLog(ex.Message & "/ ERR: calculateQuantityToBuy()")
		End Try
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
				Dim newPrice As Double = coinRT.Price

				coinBuyed.StopLost = TCoins_getStopLost(coinRT.Symbol)

				'REGLA DE 3 SIMPLES PARA SABER EL PORCENTAJE DE CAMBIO
				Dim result As Double = ((newPrice * 100) / oldPrice) - 100

				WriteLog(String.Concat(vbTab, "| ", coinBuyed.Symbol, vbTab, vbTab, "VAR: ", If(result.ToString("0.000").Contains("-"), result.ToString("0.000"), result.ToString(" 0.000")), "%", vbTab, "Umbral: ", precentageAllowedSELL.ToString, vbTab, "SL: ", coinBuyed.StopLost.ToString, vbTab, " OP: ", oldPrice.ToString, If(oldPrice.ToString.Length <= 3, vbTab, ""), If(oldPrice.ToString.Length >= 8, "", vbTab), vbTab, "NP: ", newPrice.ToString).Replace(",", "."))

				'CONSULTO SI EL PORCENTAJE CALCULADO (result) ES MAYOR QUE EL CONFIGURADO (precentageAllowedSELL), SE VENDE (INVERSA EN COMPRA)
				If result >= precentageAllowedSELL Then
					readyToSell.Add(coinBuyed)

				ElseIf result < coinBuyed.StopLost Then
					coinBuyed.SL = True
					readyToSell.Add(coinBuyed)
				End If
			Next

			Return readyToSell
		Catch ex As Exception
			WriteLog(ex.Message & "/ ERR: WeHaveSells()")
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
			Dim JsonResponse As String = File.ReadAllText("C:\GitHub\AutoTrader\autoTrader\TEST\BUY_SELL.txt")
			'Dim USDTtest As String = CStr(USDT_A_GASTAR_PER_COIN - ((USDT_A_GASTAR_PER_COIN * 0.3) / 100))

			Dim USDTtest As String = CStr(TCoins_getInversion(coin))
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
				My.Computer.Audio.Play("C:\GitHub\AutoTrader\autoTrader\venta.wav", AudioPlayMode.Background)
			Else
				My.Computer.Audio.Play("C:\GitHub\AutoTrader\autoTrader\noProfit.wav", AudioPlayMode.Background)
			End If
		Catch ex As Exception
			MsgBox(ex.Message)
		End Try
	End Sub

	Public Sub Metricas()
		Try
			For Each Coin As String In TCoins_getListOfCoins()
				TCoins_SET_Completado(Coin, TBuys_GET_Completado(Coin))
			Next
		Catch ex As Exception
			MsgBox(ex.Message)
		End Try
	End Sub








End Module




















