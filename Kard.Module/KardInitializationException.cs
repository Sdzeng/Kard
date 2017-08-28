using System;
using System.Collections.Generic;
using System.Text;

namespace Kard.Module
{
 

    [Serializable]
    public class KardInitializationException : KardException
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public KardInitializationException()
        {

        }

 

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">Exception message</param>
        public KardInitializationException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Inner exception</param>
        public KardInitializationException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
