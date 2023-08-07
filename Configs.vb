Imports System.IO




Module Configs




	'GLOBAL_VARS
#Region "GLOBAL_VARS"

	Public Const debugMode As Boolean = True
	Public Const USDT_A_GASTAR_PER_COIN As Double = 100.0
	Public ReadOnly tmpLog As String = "C:\Users\cavig\OneDrive\Escritorio\autoTraderLog.dat" 'String.Concat(Path.GetTempPath(), "autoTraderLog")
	Public CAMBIO24HS_BTC As Double = 0
	Public SENSIBILIDAD_COMPRA As Integer = 2 'default 2, aumentar la presicion puede disminuir las compras

#End Region

	'ENDPOINTS_GET
#Region "ENDPOINTS_GET"

#End Region

	'ENDPOINTS_POST
#Region "ENDPOINTS_POST"

#End Region







	'PRICES LIST
	'WEB_GetPriceList(TCoins_getListOfCoins())







End Module


















