using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace SmartTVRemoteControl.Native.HTTP
{
	public class Headers : Dictionary<string, string>
	{
		private readonly bool asIs;

		private readonly static Regex validator;

		public string HeaderBlock
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (KeyValuePair<string, string> keyValuePair in this)
				{
					stringBuilder.AppendFormat("{0}: {1}\r\n", keyValuePair.Key, keyValuePair.Value);
				}
				return stringBuilder.ToString();
			}
		}

		public string this[string key]
		{
			get
			{
				return base[this.Normalize(key)];
			}
			set
			{
				base[this.Normalize(key)] = value;
			}
		}

		static Headers()
		{
			Headers.validator = new Regex("^[a-z\\d][a-z\\d_.-]+$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
		}

		public Headers(bool asIs)
		{
			this.asIs = asIs;
		}

		public Headers() : this(false)
		{
		}

		public void Add(string key, string value)
		{
			base.Add(this.Normalize(key), value);
		}

		public bool Contains(KeyValuePair<string, string> item)
		{
			if (!this.ContainsKey(this.Normalize(item.Key)))
			{
				return false;
			}
			return this[item.Key] == item.Value;
		}

		public bool ContainsKey(string key)
		{
			return base.ContainsKey(this.Normalize(key));
		}

		private string Normalize(string header)
		{
			if (!this.asIs)
			{
				header = header.ToLower();
			}
			header = header.Trim();
			if (!Headers.validator.IsMatch(header))
			{
				throw new ArgumentException(string.Concat("Invalid header: ", header));
			}
			return header;
		}

		public bool Remove(string key)
		{
			return base.Remove(this.Normalize(key));
		}

		public bool TryGetValue(string key, out string value)
		{
			return base.TryGetValue(this.Normalize(key), out value);
		}
	}
}