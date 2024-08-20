namespace Ecierge.Uno.Navigation.Routing;

using System.Collections.Immutable;

using MoreLinq;

public abstract record RouteSegmentInstance()
{
    public abstract RouteSegment Segment { get; }
}
public record NameSegmentInstance(NameSegment NameSegment) : RouteSegmentInstance
{
    public override RouteSegment Segment => NameSegment;
}
public record DataSegmentInstance(DataSegment DataSegment, string primitive, Task<object>? data) : RouteSegmentInstance
{
    public override RouteSegment Segment => DataSegment;
}
public record DialogSegmentInstance() : RouteSegmentInstance
{
    public override RouteSegment Segment => throw new NotImplementedException("Dialog segments not implemented.");
}

public record Route(ImmutableArray<RouteSegmentInstance> Segments, INavigationData? Data = null, bool Refresh = false)
{
    public Route GoBack()
    {
        var lastSegment = Segments.LastOrDefault();
        if (lastSegment is null) return this;
        switch (lastSegment)
        {
            case NameSegmentInstance _:
                return new(Segments.RemoveAt(Segments.Length - 1), Data, Refresh);
            case DataSegmentInstance dataSegment:
                var data = Data!.Remove(dataSegment.DataSegment.Name);
                return new(Segments.RemoveAt(Segments.Length - 2), Data, Refresh);
            case DialogSegmentInstance _:
                throw new NotSupportedException("Dialog segments must not be last.");
            default:
                throw new NotSupportedException("Unknown segment type.");
        }
    }

    public Route Remove(int count)
    {
        var dataToRemove =
            Segments[^count..]
                .Where(x => x is DataSegmentInstance)
                .Select(x => ((DataSegmentInstance)x).DataSegment.Name);
        var data = Data!.RemoveRange(dataToRemove);
        return new(Segments[..^count], Data, Refresh);
    }

    public Route Add(NameSegment segment)
    {
        return new(Segments.Add(new NameSegmentInstance(segment)), Data, Refresh);
    }

    public Route Add(DataSegment segment, string primitive, Task<object>? data)
    {
        var segments = Segments.AddRange(
                new NameSegmentInstance(segment.ParentNameSegment),
                new DataSegmentInstance(segment, primitive, data)
            );
        return new(segments, Data, Refresh);
    }

    internal ImmutableArray<RouteSegmentInstance> NavigatableSegments
    {
        get
        {
            if (Segments.Length < 2) return Segments;

            var last = Segments.Last();
            return
                Segments.Pairwise((current, next) =>
                {
                    if (next is DataSegmentInstance) return default!;
                    else return current!;
                })
                .Where(s => s is not null)
                .Append(last)
                .ToImmutableArray();
        }
    }
}
