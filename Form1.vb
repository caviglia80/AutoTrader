Public Class Form1
	Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
		If debugMode Then tBatch.Interval = 10000 Else tBatch.Interval = 20000
	End Sub





	Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

		'WeHaveBuysV2()





		'Dim SellList As List(Of SIMBOLO) = WeHaveSells()
		'If SellList.Count > 0 Then
		'	WriteLog(String.Concat(vbTab, "|", vbTab, "A VENDER: ", SellList.Count.ToString))
		'	If Not WebPost_TrySELL(SellList) Then
		'		WriteLog("Fatal error en venta.")
		'	End If
		'Else
		'	WriteLog(String.Concat(vbTab, "|* NADA PARA VENDER."))
		'End If

	End Sub









	'Public GlobalToBuy As New List(Of SIMBOLO)



	Private Sub tBatch_Tick(sender As Object, e As EventArgs) Handles tBatch.Tick
		tBatch.Stop()
		Try
			'Return
			LabelBTC()
			'OldVersion()

			'SI NO ESTA EN MANT...
			If Not WebGet_Maintenance() Then
				If Not WEBGet_AccountAPITrading_isBlocked() Then

					Dim Buylist As List(Of SIMBOLO) = WeHaveBuysV2()
					If Buylist.Count > 0 Then
						If Not WebPost_TryBUY(Buylist) Then
							WriteLog("Fatal error en Compra.")
						End If
					End If

					Dim SellList As List(Of SIMBOLO) = WeHaveSells()
					If SellList.Count > 0 Then
						'WriteLine("SELL:")
						WriteLog(String.Concat(vbTab, "|", vbTab, "A VENDER: ", SellList.Count.ToString))
						If Not WebPost_TrySELL(SellList) Then
							WriteLog("Fatal error en venta.")
						End If
					End If

				Else
					WriteLog("API TRADING BLOQUEADA (ESPERANDO 15 MIN)")
					System.Threading.Thread.Sleep(900000)
				End If
			Else
				WriteLog("SERVIDOR EN MANTENIMIENTO (ESPERANDO 5 MIN)")
				System.Threading.Thread.Sleep(300000)
			End If
		Catch ex As Exception
			MsgBox(ex.Message)
		End Try

		tBatch.Start()
	End Sub

	Private Sub OldVersion()
		Try

			'SI NO ESTA EN MANT...
			If Not WebGet_Maintenance() Then
				If Not WEBGet_AccountAPITrading_isBlocked() Then

					If True Then 'CAMBIO24HS_BTC > 1.5
						Dim Buylist As List(Of SIMBOLO) = WeHaveBuys()
						'WriteLine("BUY:")

						If False Then
							WriteLog("Compra Inteligente Activada.")

							If Buylist.Count > 0 Then
								For Each coin In Buylist
									TBuysTemp_NewLog(coin.Symbol, Now, coin.MarketPrice)
									WriteLog(String.Concat("Agregado a tBuysTemp: ", coin.Symbol, vbTab, Now, vbTab, "New Price: ", coin.MarketPrice))
								Next
							End If

							Dim NewBuylist As New List(Of SIMBOLO)

							Dim NewPrices As New List(Of SIMBOLO)
							NewPrices.AddRange(WEB_GetPriceList(TCoins_getListOfCoins()))

							Dim OldPrices As New List(Of SIMBOLO)
							OldPrices.AddRange(TBuysTemp_GetOldPrices())

							For Each coin In OldPrices
								Dim SimboloActualizado As SIMBOLO = NewPrices.Find(Function(x) x.Symbol.Equals(coin.Symbol))

								If SimboloActualizado.MarketPrice > coin.MarketPrice Then
									WriteLog(String.Concat("NO BAJA MAS, SI agrego a tBuys: ", coin.Symbol, vbTab, "Old Price: ", coin.MarketPrice, vbTab, "New Price: ", SimboloActualizado.MarketPrice))
									NewBuylist.Add(coin)
								Else
									WriteLog(String.Concat("SI BAJA MAS, NO agrego a tBuys: ", coin.Symbol, vbTab, "Old Price: ", coin.MarketPrice, vbTab, "New Price: ", SimboloActualizado.MarketPrice))
								End If
							Next

							WriteLog(String.Concat(vbTab, "|", vbTab, "A COMPRAR: ", NewBuylist.Count.ToString, vbTab, vbTab, "EN ESPERA: ", (OldPrices.Count - NewBuylist.Count).ToString))
							If Not WebPost_TryBUY(NewBuylist) Then
								WriteLog("Fatal error en Compra Inteligente.")
							End If

						Else
							WriteLog("Compra Inteligente Desactivada.")
							WriteLog(String.Concat(vbTab, "|", vbTab, "A COMPRAR: ", Buylist.Count.ToString))
							If Buylist.Count > 0 Then
								If Not WebPost_TryBUY(Buylist) Then
									WriteLog("Fatal error en Compra.")
								End If
							End If
						End If

					Else
						WriteLine(String.Concat("* BTC % CAMBIO BAJO, NO SE COMPRARA.. [ ", CAMBIO24HS_BTC.ToString.Replace(",", "."), " ]"))
					End If

					Dim SellList As List(Of SIMBOLO) = WeHaveSells()
					If SellList.Count > 0 Then
						'WriteLine("SELL:")
						WriteLog(String.Concat(vbTab, "|", vbTab, "A VENDER: ", SellList.Count.ToString))
						If Not WebPost_TrySELL(SellList) Then
							WriteLog("Fatal error en venta.")
						End If
					Else
						WriteLine("* NADA PARA VENDER.")
					End If
				Else
					WriteLog("API TRADING BLOQUEADA (ESPERANDO 15 MIN)")
					System.Threading.Thread.Sleep(900000)
				End If
			Else
				WriteLog("SERVIDOR EN MANTENIMIENTO (ESPERANDO 5 MIN)")
				System.Threading.Thread.Sleep(300000)
			End If
		Catch ex As Exception
			MsgBox(ex.Message)
		End Try
	End Sub

	Private Sub btnVerLog_Click(sender As Object, e As EventArgs) Handles btnVerLog.Click
		Try
			Process.Start(New ProcessStartInfo("C:\Program Files\Sublime Text\sublime_text.exe", tmpLog))
		Catch ex As Exception
			MsgBox(ex.Message)
		End Try
	End Sub

	Private Sub LabelBTC()
		Try
			'PRIMERO ACTUALIZO EL CAMBIO DE BTC LOS % ULTIMAS 24HS (para compra)
			CAMBIO24HS_BTC = WebGet_Cambio24hsBTC()

			lEstado.Text = String.Concat("CAMBIO BTC ", CAMBIO24HS_BTC.ToString, "%")

			If CAMBIO24HS_BTC <= 0.5 Then
				lEstado.BackColor = Color.Red
				lEstado.ForeColor = Color.White
			ElseIf CAMBIO24HS_BTC <= 1.5 Then
				lEstado.BackColor = Color.Gold
				lEstado.ForeColor = Color.White
			Else
				lEstado.BackColor = Color.Lime
				lEstado.ForeColor = Color.Black
			End If

		Catch ex As Exception
			MsgBox(ex.Message)
		End Try
	End Sub

	Private Sub btnBorrarBD_Click(sender As Object, e As EventArgs) Handles btnBorrarBD.Click
		Try
			BD_Restart()
		Catch ex As Exception
			MsgBox(ex.Message)
		End Try
	End Sub

	Private Sub btnActualizarBTC24_Click(sender As Object, e As EventArgs) Handles btnActualizarBTC24.Click
		Try
			CAMBIO24HS_BTC = WebGet_Cambio24hsBTC()
			LabelBTC()
		Catch ex As Exception
			MsgBox(ex.Message)
		End Try
	End Sub
End Class


















'For Each asd In TCoins_getListOfCoins()
'	MsgBox(asd)
'Next

'For Each c In WEB_GetPriceList(TCoins_getListOfCoins())
'	MsgBox(c.symbol + "					" + c.MarketPrice.ToString)
'Next






'Dim aaaa As New List(Of String)
'aaaa.Add("ETCUSDT")
'aaaa.Add("BTCUSDT")
'aaaa.Add("ETHUSDT")
'aaaa.Add("RVNUSDT")


'MsgBox(TCoins_percOperation("iii", "SELL").ToString)
'MsgBox(TSellsTCoins_NewSell("iii", Now, 1, 2, 3, 4).ToString)
'MsgBox(TCoinsTBuys_isAvailableToBuy("c").ToString)
'TBuys_NewBuy("nueva", Now, "4001", "0")
'tBuys_NowSelled(10)
'tBuys_NewBuy("aaa", Now, "3", "4", "5")
'MsgBox(tCoins_isEnabled("ETCUSDT").ToString)
'MsgBox(_GET("/api/v3/exchangeInfo?symbol=ETCUSDT"))
'MsgBox(_POST("/api/v3/order/test", "ETCUSDT", "BUY", "1"))























































''Dim TestURL As String = "https://testnet.binance.vision"
'Dim RealURL As String = "https://api.binance.com"
'Const WalletEndPoint As String = "/api/v3/order/test"

'Function getWalletInfo() As String
'	Dim URL = New UriBuilder(RealURL & WalletEndPoint)
'	Dim TimeStamp As String = CLng((DateTime.UtcNow - #1970/01/01#).TotalMilliseconds).ToString
'	Dim HashKey As String = getHash(TimeStamp)
'	Dim queryString = Web.HttpUtility.ParseQueryString(String.Empty)
'	Dim client = New WebClient()
'	client.Headers.Add("X-MBX-APIKEY", akey)
'	queryString("recvWindow") = "5000"
'	queryString("timestamp") = TimeStamp
'	queryString("signature") = HashKey

'	queryString("symbol") = "ETCUSDT"
'	queryString("side") = "BUY"
'	queryString("type") = "MARKET"
'	'queryString("timeInForce") = "GTC"
'	queryString("quantity") = "1"




'	URL.Query = queryString.ToString()
'	Dim result As String
'	Try
'		result = client.DownloadString(URL.ToString)
'		'Read_JSon(result)
'	Catch ex As Exception
'		result = ""
'		'Stop
'	End Try
'	Return result
'End Function

'Function getHash(timestamp As String) As String
'	Dim keyBytes As Byte() = Encoding.UTF8.GetBytes(skey)
'	Dim hmacsha256 = New HMACSHA256(keyBytes)
'	Dim sourceBytes As Byte() = Encoding.UTF8.GetBytes("timestamp=" & timestamp)
'	Dim hash As Byte() = hmacsha256.ComputeHash(sourceBytes)
'	Return BitConverter.ToString(hash).Replace("-", "").ToLower()
'End Function

'Public Function WebrequestWithPost(ByVal url As String, ByVal dataToPost As String) As String
'	Dim postDataAsByteArray As Byte() = Encoding.UTF8.GetBytes(dataToPost)
'	Dim returnValue As String = String.Empty
'	Try
'		Dim webRequest As HttpWebRequest = webRequest.CreateHttp(url)  'change to: dim webRequest as var = DirectCast(WebRequest.Create(url), HttpWebRequest) if you are your .NET Version is lower than 4.5
'		If (Not (webRequest) Is Nothing) Then
'			'webRequest.AllowAutoRedirect = False
'			webRequest.Method = "POST"
'			webRequest.ContentType = "application/json"
'			webRequest.ContentLength = postDataAsByteArray.Length
'			Dim requestDataStream As Stream = webRequest.GetRequestStream
'			requestDataStream.Write(postDataAsByteArray, 0, postDataAsByteArray.Length)
'			requestDataStream.Close()
'			Dim response As WebResponse = webRequest.GetResponse
'			Dim responseDataStream As Stream = response.GetResponseStream
'			If (Not (responseDataStream) Is Nothing) Then
'				Dim responseDataStreamReader As StreamReader = New StreamReader(responseDataStream)
'				returnValue = responseDataStreamReader.ReadToEnd
'				responseDataStreamReader.Close()
'				responseDataStream.Close()
'			End If
'			response.Close()
'			requestDataStream.Close()
'		End If
'	Catch ex As WebException
'		If (ex.Status = WebExceptionStatus.ProtocolError) Then
'			Dim response As HttpWebResponse = CType(ex.Response, HttpWebResponse)
'			'handle this your own way.
'			MsgBox("Webexception! Statuscode: {0}, Description: {1}", CType(response.StatusCode, Integer), response.StatusDescription)
'		End If
'	Catch ex As Exception
'		'handle this your own way, something serious happened here.
'		MsgBox(ex.Message)
'	End Try
'	Return returnValue
'End Function

'Public Function CreateSignature(queryString As String, secret As String) As String
'	Dim keyBytes As Byte() = Encoding.UTF8.GetBytes(secret)
'	Dim queryStringBytes As Byte() = Encoding.UTF8.GetBytes(queryString)
'	Dim hmacsha256 As HMACSHA256 = New HMACSHA256(keyBytes)
'	Dim bytes As Byte() = hmacsha256.ComputeHash(queryStringBytes)
'	Return BitConverter.ToString(bytes).Replace("-", "").ToLower()
'End Function

''api/v3/exchangeInfo?symbol=ETCUSDT
'Public Function _GET(endPoint As String) As String
'	Try
'		Dim req As WebRequest = DirectCast(WebRequest.Create(String.Concat("https://api.binance.com/", endPoint)), WebRequest)
'		req.Method = "GET"
'		'req.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.1; ru; rv:1.9.2.3) Gecko/20100401 Firefox/4.0 (.NET CLR 3.5.30729"
'		req.ContentType = "application/json"
'		Dim resp As WebResponse
'		resp = DirectCast(req.GetResponse, WebResponse)
'		Dim postreqreader As New StreamReader(resp.GetResponseStream())
'		Dim thepage As String = postreqreader.ReadToEnd
'		Return thepage
'	Catch ex As Exception
'		MsgBox(ex.Message)
'	End Try
'	Return Nothing
'End Function



'Public Shared Function ConvertirAFechaBinance(fecha As DateTime) As Long
'	Return CLng(fecha.ToUniversalTime.Subtract(New DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalMilliseconds)
'End Function

'	Public Static String getHmacSha256(String key, String data) {
'    Mac sha256;
'    String result = null;

'	Try {
'        Byte[] byteKey = key.getBytes("UTF-8");
'        sha256 = Mac.getInstance(HMAC_SHA256);
'        SecretKeySpec keySpec = New SecretKeySpec(byteKey, HMAC_SHA256);
'        sha256.init(keySpec);
'        Byte[] macData = sha256.doFinal(data.getBytes("UTF-8"));
'        result = bytesToHex(macData);
'    }
'    Catch(Exception e) {
'        e.printStackTrace();
'    }
'    Return result;
'}

