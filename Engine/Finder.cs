namespace The_World.Engine;

/// <summary>
/// Fuzzy lookup for the things players type. Nobody wants to type
/// "goblin_1" when "goblin" - or even "gob" - should obviously work.
/// </summary>
public static class Finder
{
    /// <summary>
    /// Resolve a query against a keyed dictionary, matching (in order of
    /// preference): exact key, exact display name, name prefix, key prefix,
    /// then substring. Case-insensitive throughout.
    /// </summary>
    /// <param name="name">Extracts a display name from a value (e.g. c => c.Name).</param>
    /// <returns>The matched entry, or null if nothing fits.</returns>
    public static KeyValuePair<string, T>? Resolve<T>(
        IReadOnlyDictionary<string, T> dict,
        string query,
        Func<T, string> name)
    {
        if (string.IsNullOrWhiteSpace(query))
            return null;
        var q = query.Trim();
        const StringComparison ci = StringComparison.OrdinalIgnoreCase;

        foreach (var kv in dict)
            if (kv.Key.Equals(q, ci) || kv.Key.Replace('_', ' ').Equals(q, ci))
                return kv;

        foreach (var kv in dict)
            if (name(kv.Value).Equals(q, ci))
                return kv;

        foreach (var kv in dict)
            if (name(kv.Value).StartsWith(q, ci) || kv.Key.StartsWith(q, ci))
                return kv;

        foreach (var kv in dict)
            if (name(kv.Value).Contains(q, ci))
                return kv;

        return null;
    }
}
