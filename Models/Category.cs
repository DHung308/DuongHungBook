using System;
using System.Collections.Generic;

namespace DHungBooks.Models;

public partial class Category
{
    public int CaId { get; set; }

    public string? CaName { get; set; }

    public string? Description { get; set; }

    public int? ParentId { get; set; }

    public int? Levels { get; set; }

    public int? Ordering { get; set; }

    public bool Published { get; set; }

    public string? Thumb { get; set; }

    public string? Title { get; set; }

    public string? Alias { get; set; }

    public string MetaDesc { get; set; } = null!;

    public string? MetaKey { get; set; }

    public string? Cover { get; set; }

    public string? SchemaMarkup { get; set; }

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
