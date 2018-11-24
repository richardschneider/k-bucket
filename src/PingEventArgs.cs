﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Makaretu.Collections
{
    /// <summary>
    ///   The contacts that should be checked.
    /// </summary>
    /// <seealso cref="KBucket{T}.Ping"/>
    public class PingEventArgs<T> : EventArgs
         where T : IContact
    {
        /// <summary>
        ///   The contacts that should be checked.
        /// </summary>
        public IEnumerable<T> Checks;

        /// <summary>
        ///   A new contact that wants to be added.
        /// </summary>
        public T Contact;
    }
}
