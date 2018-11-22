using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Makaretu.Collections
{
    /// <summary>
    ///   A peer/node in the distributed system.
    /// </summary>
    public interface IContact
    {
        /// <summary>
        ///   Unique identifier of the contact.
        /// </summary>
        /// <value>
        ///   Typically a hash of a unique identifier. 
        /// </value>
        byte[] Id { get; }
    }
}
