using AwesomeAssertions.Common;

namespace AwesomeAssertions.Equivalency;

/// <summary>
/// Represents the path of a field or property in an object graph.
/// </summary>
public record Pathway
{
    /// <summary>
    /// Represents a method that produces the display representation of a path from its combined path and name.
    /// </summary>
    /// <param name="pathAndName">The combined path and name of the field or property.</param>
    /// <returns>The display representation of the path.</returns>
    public delegate string GetDescription(string pathAndName);

    private readonly string path = string.Empty;
    private string name = string.Empty;
    private string pathAndName;

    private readonly GetDescription getDescription;

    /// <summary>
    /// Initializes a new instance of the <see cref="Pathway"/> class with the specified path and name and a factory
    /// to provide a description for the path and name.
    /// </summary>
    /// <param name="path">The path of the field or property without the name.</param>
    /// <param name="name">The name of the field or property without the path.</param>
    /// <param name="getDescription">A factory that provides the display representation for the combined path and name.</param>
    public Pathway(string path, string name, GetDescription getDescription)
    {
        Path = path;
        Name = name;
        this.getDescription = getDescription;
    }

    /// <summary>
    /// Creates an instance of <see cref="Pathway"/> with the specified parent and name and a factory
    /// to provide a description for the path and name.
    /// </summary>
    public Pathway(Pathway parent, string name, GetDescription getDescription)
    {
        Path = parent.PathAndName;
        Name = name;
        this.getDescription = getDescription;
    }

    /// <summary>
    /// Gets the path of the field or property without the name.
    /// </summary>
    public string Path
    {
        get => path;
        private init
        {
            path = value;
            pathAndName = null;
        }
    }

    /// <summary>
    /// Gets the name of the field or property without the path.
    /// </summary>
    public string Name
    {
        get => name;
        internal set
        {
            name = value;
            pathAndName = null;
        }
    }

    /// <summary>
    /// Gets the path and name of the field or property separated by dots.
    /// </summary>
    public string PathAndName => pathAndName ??= path.Combine(name);

    /// <summary>
    /// Gets the display representation of this path.
    /// </summary>
    public string Description => getDescription(PathAndName);

    /// <summary>
    /// Returns the display representation of this path.
    /// </summary>
    /// <returns>The value of <see cref="Description"/>.</returns>
    public override string ToString() => Description;
}
