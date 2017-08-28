using System;
using System.Collections.Generic;
using System.Text;

namespace Kard.Module
{

    [Serializable]
    public class KardException : Exception
    {
        /// <summary>
        /// Creates a new <see cref="AbpException"/> object.
        /// </summary>
        public KardException()
        {

        }


        /// <summary>
        /// Creates a new <see cref="AbpException"/> object.
        /// </summary>
        /// <param name="message">Exception message</param>
        public KardException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// Creates a new <see cref="AbpException"/> object.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Inner exception</param>
        public KardException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
