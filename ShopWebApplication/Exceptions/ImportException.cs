namespace ShopWebApplication.Exceptions
{
	public class ImportException : Exception
	{
		public ImportException()
		{
		}

        public class InvalidPriceException : Exception
        {
            public InvalidPriceException(string message) : base(message) { }
        }

        public class InvalidSizeException : Exception
        {
            public InvalidSizeException(string message) : base(message) { }
        }
    }
}

