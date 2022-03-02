using System.Collections.Generic;

namespace Alteracia.Patterns
{
    public abstract class Librarian<T>
    {
        public static Dictionary<string, T> List { get; } = new Dictionary<string, T>();

        public static void AddRecord(string id, T artifact, bool replace = false)
        {
            if (List.ContainsKey(id))
            {
                if (replace) List[id] = artifact;
                return;
            }

            List.Add(id, artifact);
        }
    }
}