namespace KPO_HW2.Exceptions;

public sealed class DuplicateException(
    string entityName,
    string fieldName,
    string value,
    Exception? innerException = null)
    : Exception($"{entityName} with {fieldName} '{value}' already exists.", innerException)
{
    public string EntityName { get; } = entityName;
    public string FieldName { get; } = fieldName;
    public string Value { get; } = value;
}
