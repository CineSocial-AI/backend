using CineSocial.Core.Domain.Common;

namespace CineSocial.Core.Domain.Entities;

public class PostMedia : BaseEntity
{
    public Guid PostId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public MediaType Type { get; set; }
    public long FileSize { get; set; }
    public string? MimeType { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }
    public int? Duration { get; set; }
    public int Order { get; set; }

    public virtual Post Post { get; set; } = null!;
}

public enum MediaType
{
    Image = 1,
    Video = 2,
    Audio = 3,
    Document = 4
}
