
Imports System.IO
Imports System.Net
Imports System.Security.Cryptography
Imports SharpDX.Text


Module Web

	Private ReadOnly akey As String = "jBHyvd7j0H8yQlIWv8EHrogF0ki67KLZCyR3AwB89e3kXeWTgDRZLhYr2RrfMLit"
	Private ReadOnly skey As String = "tPrE6UFdWjE2qIGNOq9aM8oyGQpHWh1pJaiaH1NVr9LdguC8C2MP9CefsUCQxXfL"
	Private ReadOnly binanceUrl As String = "https://api.binance.com"




	Public Function _POST(ByVal endPoint As String, ByVal symbol As String, ByVal side As String, ByVal quantity As String) As String
		'_Post("/api/v3/order/test", "ETCUSDT", "BUY", "1") EXAMPLE
		Dim responseStr As String = ""
		Try
			Dim TotalParam As String = String.Concat("symbol=", symbol, "&side=", side, "&type=MARKET&quantity=", quantity.Replace(",", "."), "&recvWindow=50000&timestamp=", timeStamp1())
			Dim APIUrl As String = String.Concat(binanceUrl, endPoint, "?", TotalParam, "&signature=", Cifrar(TotalParam, skey))
			Dim Request As System.Net.WebRequest = DirectCast(System.Net.WebRequest.Create(APIUrl), System.Net.WebRequest)
			Request.Method = "POST"
			Request.Headers.Add("X-MBX-APIKEY", akey)
			'Request.ContentType = "application/x-www-form-urlencoded"
			Request.ContentType = "application/json"
			'Dim Response As System.Net.WebResponse = DirectCast(Request.GetResponse(), System.Net.WebResponse)
			'Dim Read = New System.IO.StreamReader(Response.GetResponseStream).ReadToEnd
			'Return Read

			Using Response As System.Net.WebResponse = DirectCast(Request.GetResponse(), System.Net.WebResponse)
				Using Read As New IO.StreamReader(Response.GetResponseStream)
					responseStr = Read.ReadToEnd
				End Using
			End Using

			Return responseStr
		Catch wex As WebException
			If wex.Response Is Nothing Then Return "SIN_RED"
			If wex.Response.ContentLength <> 0 Then
				Dim ReaderResult As String = ""
				Using stream = wex.Response.GetResponseStream()
					Using reader = New StreamReader(stream)
						ReaderResult = reader.ReadToEnd()
					End Using
				End Using
				Return String.Concat("_POST(): ", ReaderResult)
			End If
		Catch ex As Exception
			WriteLog("ERR: _POST()")
			MsgBox(ex.Message)
		End Try
		Return Nothing
	End Function

	Public Function _GET(endPoint As String) As String
		'_GET("/api/v3/exchangeInfo?symbol=ETCUSDT") EXAMPLE
		Try
			Dim responseStr As String = ""
			Dim req As WebRequest = DirectCast(WebRequest.Create(String.Concat(binanceUrl, endPoint)), WebRequest)
			req.Method = "GET"
			req.ContentType = "application/json"
			Using response As System.Net.WebResponse = DirectCast(req.GetResponse(), System.Net.WebResponse)
				Using Read As New IO.StreamReader(response.GetResponseStream)
					responseStr = Read.ReadToEnd
				End Using
			End Using

			Return responseStr
		Catch wex As WebException
			If wex.Response Is Nothing Then Return "SIN_RED"
			If wex.Response.ContentLength <> 0 Then
				Dim ReaderResult As String = ""
				Using stream = wex.Response.GetResponseStream()
					Using reader = New StreamReader(stream)
						ReaderResult = reader.ReadToEnd()
						'MsgBox(ReaderResult)
					End Using
				End Using
				Return String.Concat("_GET(): ", ReaderResult)
			End If
		Catch ex As Exception
			WriteLog("ERR: _GET()")
			MsgBox(ex.Message)
		End Try
		Return Nothing
	End Function

	Public Function _GET_HMAC(endPoint As String, Optional params As String = "") As String
		Dim responseStr As String = ""
		If Not params.Length = 0 Then params = String.Concat(params, "&")
		Try
			Dim TotalParam As String = String.Concat(params, "recvWindow=10000&timestamp=", timeStamp1())
			Dim APIUrl As String = String.Concat(binanceUrl, endPoint, "?", TotalParam, "&signature=", Cifrar(TotalParam, skey))
			Dim Request As System.Net.WebRequest = DirectCast(System.Net.WebRequest.Create(APIUrl), System.Net.WebRequest)
			Request.Method = "GET"
			Request.Headers.Add("X-MBX-APIKEY", akey)
			Request.ContentType = "application/json"
			'Dim Response As System.Net.WebResponse = DirectCast(Request.GetResponse(), System.Net.WebResponse)
			'Dim Read = New System.IO.StreamReader(Response.GetResponseStream).ReadToEnd
			'Return Read

			Using Response As System.Net.WebResponse = DirectCast(Request.GetResponse(), System.Net.WebResponse)
				Using Read As New IO.StreamReader(Response.GetResponseStream)
					responseStr = Read.ReadToEnd
				End Using
			End Using

			Return responseStr
		Catch wex As WebException
			If wex.Response Is Nothing Then Return "SIN_RED"
			If wex.Response.ContentLength <> 0 Then
				Dim ReaderResult As String = ""
				Using stream = wex.Response.GetResponseStream()
					Using reader = New StreamReader(stream)
						ReaderResult = reader.ReadToEnd()
						'MsgBox(ReaderResult)
					End Using
				End Using
				Return String.Concat("_GET_HMAC(): ", ReaderResult)
			End If
		Catch ex As Exception
			WriteLog("ERR: _GET_HMAC()")
			MsgBox(ex.Message)
		End Try
		Return Nothing
	End Function

	Private Function timeStamp1() As String
		Try
			'Dim TimeStamp As String = CLng((DateTime.UtcNow - #1970/01/01#).TotalMilliseconds).ToString
			'Return CLng(DateTime.UtcNow.Subtract(New DateTime(1970, 1, 1)).TotalMilliseconds).ToString
			Return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString
		Catch ex As Exception
			WriteLog("ERR: timeStamp1()")
			MsgBox(ex.Message)
		End Try
		Return Nothing
	End Function

	Private Function Cifrar(dato As String, Clave_Privada As String) As String
		Try
			Dim dato_bytes() As Byte = Encoding.UTF8.GetBytes(dato)
			Dim clave_bytes() As Byte = Encoding.UTF8.GetBytes(Clave_Privada)
			Dim hmac As HMACSHA256 = New HMACSHA256(clave_bytes)
			Dim codificado() As Byte = hmac.ComputeHash(dato_bytes)
			Dim respuesta As String = BitConverter.ToString(codificado)
			respuesta = respuesta.Replace("-", "").ToLower
			Return respuesta
		Catch ex As Exception
			WriteLog("ERR: Cifrar()")
			MsgBox(ex.Message)
		End Try
		Return Nothing
	End Function


End Module
