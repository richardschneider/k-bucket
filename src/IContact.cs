using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Makaretu.Collections
{
    /// <summary>
    ///   A peer/node in the tree.
    /// </summary>
    public interface IContact
    {
        /// <summary>
        ///   Unique identifier.
        /// </summary>
        /// <value>
        ///   Typically a hash of a name. 
        /// </value>
        byte[] Id { get; }
    }
}
