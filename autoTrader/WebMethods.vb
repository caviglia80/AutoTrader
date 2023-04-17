Imports System.Net
Imports Newtonsoft.Json



Module WebMethods

	'OBTENGO LOS ULTIMOS PRECIOS DE LAS CRIPTOS
	Public Function WEB_GetPriceList(coinList As List(Of String)) As List(Of SIMBOLO)
		Try
			'If debugMode Then If WEB_ValidSymbols(coinList) Then Return Nothing

			'CONVIERTO LA LISTA DE COINS A JSON
			Dim coinsJson As String = JsonConvert.SerializeObject(coinList, Formatting.None)

			'OBTENGO LA LISTA DE PRECIOS EN JSON.
			Dim JsonResponse As String = _GET(String.Concat("/api/v3/ticker/price?symbols=", coinsJson))

			If IsError(JsonResponse) Then Return Nothing

			'DESEREALIZO JSON Y CONVIERTO DATOS EN LISTA DE LA CLASE Prices
			Return JsonConvert.DeserializeObject(Of List(Of SIMBOLO))(JsonResponse)

		Catch ex As Exception
			WriteLog(ex.Message & "/ ERR: WEB_GetPriceList()")
			MsgBox(ex.Message)
		End Try
		Return Nothing
	End Function

	'OBTENGO LOS PRECIOS DE LAS CRIPTOS LOW AND HIGH PRICES
	Public Function WEB_GetPriceList24h(coinList As List(Of String)) As List(Of SIMBOLO)
		Try
			'If debugMode Then If WEB_ValidSymbols(coinList) Then Return Nothing

			'CONVIERTO LA LISTA DE COINS A JSON
			Dim coinsJson As String = JsonConvert.SerializeObject(coinList, Formatting.None)

			'OBTENGO LA LISTA DE PRECIOS EN JSON.
			Dim JsonResponse As String = _GET(String.Concat("/api/v3/ticker/24hr?symbols=", coinsJson))

			If IsError(JsonResponse) Then Return Nothing

			'DESEREALIZO JSON Y CONVIERTO DATOS EN LISTA DE LA CLASE Prices
			Return JsonConvert.DeserializeObject(Of List(Of SIMBOLO))(JsonResponse)

		Catch ex As Exception
			WriteLog(ex.Message & "/ ERR: WEB_GetPriceList24h()")
			MsgBox(ex.Message)
		End Try
		Return Nothing
	End Function

	'COMPRUEBO QUE TODOS LOS SIMBOLOS SEAN VALIDOS
	Public Function WEB_ValidSymbols(coinList As List(Of String)) As Boolean
		Try
			Dim JsonResponse As String
			Dim count As Integer = 0

			For Each symbol In coinList
				JsonResponse = _GET(String.Concat("/api/v3/ticker/price?symbols=[""", symbol, """]"))
				If IsError(String.Concat(symbol, vbTab, vbTab, JsonResponse)) Then count += 1 Else WriteLine(String.Concat(symbol, " OK"))
				Threading.Thread.Sleep(200)
			Next
			If count > 0 Then Return False

		Catch ex As Exception
			WriteLog(ex.Message & "/ ERR: WEB_ValidSymbols()")
			MsgBox(ex.Message)
		End Try
		Return True
	End Function

	'OBTENGO ULTIMO PRECIO DE UNA CRIPTO
	Public Function WEBGet_Price(coin As String) As Double
		Try
			'OBTENGO PRECIOS EN JSON.
			Dim JsonResponse As String = _GET(String.Concat("/api/v3/ticker/24hr?symbol=", coin))
			If IsError(JsonResponse) Then Return Nothing

			'DESEREALIZO JSON Y CONVIERTO DATOS EN SIMBOLO
			Dim json As New Chilkat.JsonObject()
			json.Load(JsonResponse)
			Return json.StringOf("lastPrice").Replace(".", ",")
		Catch ex As Exception
			WriteLog(ex.Message & "/ ERR: WEBGet_Price()")
			MsgBox(ex.Message)
		End Try
		Return Nothing
	End Function

	'REALIZO LAS COMPRAS
	Public Function WebPost_TryBUY(coinList As List(Of SIMBOLO)) As Boolean
		Try
			For Each Coin In coinList

				Dim QuantityToBuy As String = calculateQuantityToBuy(Coin.Symbol, Coin.lastPrice, TCoins_getInversion(Coin.Symbol))
				Dim respPost As String = ""
				If Not debugMode Then respPost = _POST("/api/v3/order", Coin.Symbol, "BUY", QuantityToBuy)

				If IsError(respPost) Then
					WriteLog("Error en compra.")
					Continue For
				End If

				WriteLog(String.Concat(vbTab, "|  COMPRA REALIZADA(", Coin.Symbol, ")", vbTab, "BTC: ", CAMBIO24HS_BTC.ToString().Replace(",", "."), "%"))

				Dim json As New Chilkat.JsonObject()
				If debugMode Then json.Load(CargamosDatosDePrueba(Coin.Symbol, "BUY", QuantityToBuy, 0)) Else json.Load(respPost)
				Dim USDTtoBUY As String = json.StringOf("cummulativeQuoteQty")
				Dim Qty As String = json.StringOf("executedQty")

				Dim oldPrice As Double = TCoins_getLastPriceOperations(Coin.Symbol)
				Dim result As Double = ((Coin.lastPrice * 100) / oldPrice) - 100
				Dim comments As String = String.Concat("Umbral: ", TCoins_percOperation(Coin.Symbol, "BUY").ToString("0.00"), ", Current: ", result.ToString("0.0000"))

				TBuysTCoins_NewBuy(Coin, Now, USDTtoBUY, Qty, comments)
				TBuysTemp_Reset(Coin.Symbol)
			Next

			Return True
		Catch ex As Exception
			WriteLog(ex.Message & "/ ERR: WebPost_TryBUY()")
			MsgBox(ex.Message)
		End Try
		Return False
	End Function

	'REALIZO LAS VENTAS
	Public Function WebPost_TrySELL(coinList As List(Of SIMBOLO)) As Boolean
		Try
			For Each Coin In coinList
				Dim respPost As String = ""
				If Not debugMode Then respPost = _POST("/api/v3/order", Coin.Symbol, "SELL", CStr(Coin.Quantity))
				If IsError(respPost) Then
					WriteLog("Error en venta.")
					Continue For
				End If

				If Coin.SL Then
					WriteLog(String.Concat(vbTab, "|", vbTab, "VENTA STOP LOST (", Coin.Symbol, ")", vbTab, "SL: ", Coin.StopLost.ToString().Replace(",", "."), vbTab, "BTC: ", CAMBIO24HS_BTC.ToString().Replace(",", "."), "%"))
					Sonido(False)
				Else
					WriteLog(String.Concat(vbTab, "|", vbTab, "VENTA PROFIT (", Coin.Symbol, ") ", vbTab, "BTC: ", CAMBIO24HS_BTC.ToString().Replace(",", "."), "%"))
					Sonido(True)
				End If

				Dim json As New Chilkat.JsonObject()
				If debugMode Then json.Load(CargamosDatosDePrueba(Coin.Symbol, "SELL", Coin.Quantity, Coin.MarketPrice)) Else json.Load(respPost)

				'MsgBox(Coin.MarketPrice.ToString)
				'MsgBox(json.StringOf("cummulativeQuoteQty"))

				Dim GananciaBruta As Double = CDbl(json.StringOf("cummulativeQuoteQty").Replace(".", ","))
				Dim Qty As Double = CDbl(json.StringOf("executedQty").Replace(".", ","))
				'ACTUALIZO TBuys(LA MARCO COMO VENDIDA) 
				TBuys_NowSelled(Coin)

				'ACTUALIZO TSells(INSERTO VENTA) Y TCoins(ACTUALIZO OperationLastPrice, Profit_USDT).
				TSellsTCoins_NewSell(Coin, Now, GananciaBruta, Qty, (GananciaBruta - Coin.USDT))
			Next

			Return True
		Catch ex As Exception
			WriteLog(ex.Message & "/ ERR: WebPost_TrySELL()")
			MsgBox(ex.Message)
		End Try
		Return False
	End Function

	'PREGUNTO SI B ESTA EN MANTENIMIENTO
	Public Function WebGet_Maintenance() As Boolean
		Try
			Dim JsonResponse As String = _GET("/sapi/v1/system/status")
			If IsError(JsonResponse) Then Return Nothing

			Dim json As New Chilkat.JsonObject()
			json.Load(JsonResponse)
			Return If(CInt(json.StringOf("status")) = 0, False, True)
		Catch ex As Exception
			WriteLog(ex.Message & "/ ERR: WebGet_Maintenance()")
			MsgBox(ex.Message)
		End Try
		Return True
	End Function

	'COIN BLOQUEADA PARA TRADING?
	Public Function WEBGet_AccountAPITrading_isBlocked() As Boolean
		Try
			Dim JsonResponse As String = _GET_HMAC("/sapi/v1/account/apiTradingStatus")
			If IsError(JsonResponse) Then Return Nothing

			Dim json As New Chilkat.JsonObject()
			json.Load(JsonResponse)
			Return CBool(json.StringOf("data.isLocked"))
		Catch ex As Exception
			WriteLog(ex.Message & "/ ERR: WEBGet_AccountAPITrading_isBlocked()")
			MsgBox(ex.Message)
		End Try
		Return True
	End Function

	'PREGUNTO El Valor De Cambio24hs BTC
	Public Function WebGet_Cambio24hsBTC() As Double
		Try
			Dim JsonResponse As String = _GET("/api/v3/ticker/24hr?symbol=BTCUSDT")
			If IsError(JsonResponse) Then Return Nothing
			Dim json As New Chilkat.JsonObject()
			json.Load(JsonResponse)
			Return CDbl(json.StringOf("priceChangePercent").Replace(".", ","))
		Catch ex As Exception
			WriteLog(ex.Message & "/ ERR: WebGet_Cambio24hsBTC()")
			MsgBox(ex.Message)
		End Try
		Return True
	End Function

	Public Function Tendencia(symbol As String, Optional interval As String = "1d", Optional limit As Integer = 24) As String
		Try
			Dim url As String = "https://api.binance.com/api/v3/klines?symbol=" & symbol & "&interval=" & interval & "&limit=" & limit
			Dim wc As New WebClient()
			Dim json As String = wc.DownloadString(url)

			Dim data As List(Of Object()) = JsonConvert.DeserializeObject(Of List(Of Object()))(json)

			Dim closingPrices As List(Of Decimal) = data.Select(Function(x) Convert.ToDecimal(x(4))).ToList()
			Dim lastPrice As Decimal = closingPrices.Last()
			Dim movingAverage As Decimal = closingPrices.Average()

			If lastPrice > movingAverage Then
				Return String.Concat("ALZA(", interval, ",", limit.ToString(), ")")
			Else
				Return String.Concat("BAJA(", interval, ",", limit.ToString(), ")")
			End If
		Catch ex As Exception
			MsgBox(ex.Message)
			Return "Error en Tendencia"
		End Try
	End Function

	Public Function Tendencia_v2_ToBuy(symbol As String) As Boolean
		Try
			Dim interval As String = "1s"
			Dim limit As Integer = 4

			Dim url As String = "https://api.binance.com/api/v3/klines?symbol=" & symbol & "&interval=" & interval & "&limit=" & limit
			Dim wc As New WebClient()
			Dim json As String = wc.DownloadString(url)

			Dim data As List(Of Object()) = JsonConvert.DeserializeObject(Of List(Of Object()))(json)

			Dim closingPrices As List(Of Decimal) = data.Select(Function(x) Convert.ToDecimal(x(4))).ToList()
			Dim lastPrice As Decimal = closingPrices.Last()
			Dim movingAverage As Decimal = closingPrices.Average()

			If lastPrice > movingAverage Then
				Return True
			Else
				Return False
			End If
		Catch ex As Exception
			MsgBox(ex.Message)
			Return "Error en Tendencia"
		End Try
	End Function












End Module







