using System;
using Server;
using Server.Currency;

namespace Server.Market
{
	public class MarketEntry
	{
		private bool _active;
		private Category _category;
		private int _cost;
		private string _description;
		private Serial _objectSerial;
		private Mobile _seller;
		private int _tableId;

		public bool Active { get { return _active; } set { _active = value; } }
		public Category Category { get { return _category; } set { _category = value; } }
		public int Cost { get { return _cost; } set { _cost = value; } }
		public string Description { get { return _description; } set { _description = value; } }
		public Serial ObjectSerial { get { return _objectSerial; } }
		public Mobile Seller { get { return _seller; } }
		public int TableId { get { return _tableId; } set { _tableId = value; } }

		public MarketEntry( Mobile seller )
		{
			_active = true;
			_category = Category.Misc;
			_cost = 0;
			_description = "";
			_objectSerial = Serial.MinusOne;
			_seller = seller;
			_tableId = -1;
		}

		public void ChangeCost( int newGold, int newVerite, int newValorite )
		{
			Cost = newGold;
			Cost += Currency.CurrencySystem.ConvertTo( CurrencyType.Verite, CurrencyType.Gold, newVerite );
			Cost += Currency.CurrencySystem.ConvertTo( CurrencyType.Valorite, CurrencyType.Gold, newValorite );
		}

		public void SetSellItem( IEntity entity )
		{
			_objectSerial = entity.Serial;
		}
	}
}