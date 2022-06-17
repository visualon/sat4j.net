using Sharpen;

namespace Org.Sat4j
{
	/// <summary>That class is intented to manage internationalisation within the application.</summary>
	/// <author>leberre</author>
	public sealed class Messages
	{
		private const string BundleName = "org.sat4j.messages";

		private static readonly ResourceBundle ResourceBundle = ResourceBundle.GetBundle(BundleName);

		/// <summary>No instances should be used.</summary>
		/// <remarks>
		/// No instances should be used. Use Messages.getString(key) to get localized
		/// message for key.
		/// </remarks>
		private Messages()
			: base()
		{
		}

		//$NON-NLS-1$
		public static string GetString(string key)
		{
			// TODO Auto-generated method stub
			try
			{
				return ResourceBundle.GetString(key);
			}
			catch (MissingResourceException)
			{
				return '!' + key + '!';
			}
		}
	}
}
