// MySync © 2016-2017 Damian 'Erdroy' Korczowski


using System;

namespace MySync.Client.Utilities
{
    internal class MySyncException : Exception
    {
        // hide this constructor
        private MySyncException() { }

        /// <summary>
        /// Standard constructor.
        /// </summary>
        /// <param name="message">The message.</param>
        public MySyncException(string message)
        {
            Message = message;
        }
        
        /// <summary>
        /// The message.
        /// </summary>
        public override string Message { get; }
    }
}