namespace AndOS.Common.Classes;

public abstract class ValueObject
{
    protected static bool equalOperator(ValueObject left, ValueObject right)
    {
        if (left is null ^ right is null)
        {
            return false;
        }

        return left?.Equals(right!) != false;
    }

    protected static bool notEqualOperator(ValueObject left, ValueObject right)
    {
        return !equalOperator(left, right);
    }

    protected abstract IEnumerable<object> getEqualityComponents();

    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != GetType())
        {
            return false;
        }

        ValueObject other = (ValueObject)obj;
        return getEqualityComponents().SequenceEqual(other.getEqualityComponents());
    }

    public override int GetHashCode()
    {
        HashCode hash = new();

        foreach (object component in getEqualityComponents())
        {
            hash.Add(component);
        }

        return hash.ToHashCode();
    }
}