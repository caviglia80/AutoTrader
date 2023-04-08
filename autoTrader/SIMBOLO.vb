


<Serializable>
Public Class SIMBOLO




	Private Property _id As String
	Public Property ID() As String
		Get
			Return _id
		End Get
		Set(ByVal value As String)
			_id = value
		End Set
	End Property

	Private Property _symbol As String
	Public Property Symbol() As String
		Get
			Return _symbol
		End Get
		Set(ByVal value As String)
			_symbol = value
		End Set
	End Property

	Private Property _percentage As Double
	Public Property Percentage() As Double
		Get
			Return _percentage
		End Get
		Set(ByVal value As Double)
			_percentage = value
		End Set
	End Property

	Private Property _quantity As Double
	Public Property Quantity() As Double
		Get
			Return _quantity
		End Get
		Set(ByVal value As Double)
			_quantity = value
		End Set
	End Property

	Private Property _marketPrice As Double
	Public Property MarketPrice() As Double
		Get
			Return _marketPrice
		End Get
		Set(ByVal value As Double)
			_marketPrice = value
		End Set
	End Property

	Private Property _usdt As Double
	Public Property USDT() As Double
		Get
			Return _usdt
		End Get
		Set(ByVal value As Double)
			_usdt = value
		End Set
	End Property

	Private Property _price As Double
	Public Property Price() As Double
		Get
			Return _price
		End Get
		Set(ByVal value As Double)
			_price = value
			'_marketPrice = value
		End Set
	End Property

	Private Property _lastPrice As Double
	Public Property lastPrice() As Double
		Get
			Return _lastPrice
		End Get
		Set(ByVal value As Double)
			_lastPrice = value
			'_marketPrice = value
		End Set
	End Property

	Private Property _lowPrice As Double
	Public Property lowPrice() As Double
		Get
			Return _lowPrice
		End Get
		Set(ByVal value As Double)
			_lowPrice = value
		End Set
	End Property

	Private Property _highPrice As Double
	Public Property highPrice() As Double
		Get
			Return _highPrice
		End Get
		Set(ByVal value As Double)
			_highPrice = value
		End Set
	End Property

	Private Property _stopLost As Double
	Public Property StopLost() As Double
		Get
			Return _stopLost
		End Get
		Set(ByVal value As Double)
			_stopLost = value
		End Set
	End Property

	Private Property _SL As Boolean
	Public Property SL() As Boolean
		Get
			Return _SL
		End Get
		Set(ByVal value As Boolean)
			_SL = value
		End Set
	End Property

	Private Property _Var As Double
	Public Property Var() As Double
		Get
			Return _Var
		End Get
		Set(ByVal value As Double)
			_Var = value
		End Set
	End Property

	Private Property _ReadyToBuy As Boolean
	Public Property ReadyToBuy() As Boolean
		Get
			Return _ReadyToBuy
		End Get
		Set(ByVal value As Boolean)
			_ReadyToBuy = value
		End Set
	End Property



























	Public Sub New()
		_id = ""
		_symbol = ""
		_percentage = 0
		_marketPrice = 0
		_stopLost = 0
		_SL = False
		_Var = 0
		_ReadyToBuy = False
		_lowPrice = 0
		_highPrice = 0
		_lastPrice = 0
	End Sub

	'Public Sub New(Symbol As String, MarketPrice As Double)
	'	_symbol = Symbol
	'	_marketPrice = MarketPrice
	'End Sub

	'Public Sub New(Symbol As String, MarketPrice As Double, percentage As Double)
	'	_symbol = Symbol
	'	_marketPrice = MarketPrice
	'	_percentage = percentage
	'End Sub

	Public Sub New(ID As String, Symbol As String, MarketPrice As String, USDT As String, qty As String)
		_id = ID
		_symbol = Symbol
		_marketPrice = MarketPrice
		_usdt = USDT
		_quantity = qty
	End Sub

	Public Sub New(ID As String, Symbol As String, MarketPrice As String, qty As String)
		_id = ID
		_symbol = Symbol
		_marketPrice = MarketPrice
		_quantity = qty
	End Sub

	Public Sub New(ID As String, Symbol As String, MarketPrice As String)
		_id = ID
		_symbol = Symbol
		_marketPrice = MarketPrice
	End Sub

End Class











