using System;
using System.Collections.Specialized;
using System.Diagnostics;

namespace SystemModule.CoreSocket;

/// <summary>
/// IgnoreCaseNameValueCollection
/// </summary>
[DebuggerTypeProxy(typeof(NameValueCollectionDebugView))]
public class IgnoreCaseNameValueCollection : NameValueCollection
{
    /// <summary>
    /// IgnoreCaseNameValueCollection
    /// </summary>
    public IgnoreCaseNameValueCollection() : base(StringComparer.OrdinalIgnoreCase)
    {
    }
}