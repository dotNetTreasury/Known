﻿namespace Known;

public class CodeTableAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Class)]
public class TableAttribute : Attribute
{
    public TableAttribute(string name)
    {
        Name = name;
    }

    public string Name { get; set; }
}

[AttributeUsage(AttributeTargets.Property)]
public class ColumnAttribute : Attribute
{
    public ColumnAttribute(string description, bool required)
    {
        Description = description;
        Required = required;
    }

    public ColumnAttribute(
        string description,
        string columnName = null,
        bool required = false,
        string minLength = null,
        string maxLength = null,
        string dateFormat = null)
    {
        Description = description;
        ColumnName = columnName;
        Required = required;
        MinLength = minLength;
        MaxLength = maxLength;
        DateFormat = dateFormat;
        IsGrid = true;
    }

    public string Description { get; }
    public string ColumnName { get; }
    public bool Required { get; }
    public string MinLength { get; }
    public string MaxLength { get; }
    public string DateFormat { get; }
    public bool IsGrid { get; set; }
    public PropertyInfo Property { get; set; }

    internal virtual void Validate(object value, Type type, List<string> errors)
    {
        var valueString = value == null ? "" : value.ToString().Trim();
        if (Required && string.IsNullOrEmpty(valueString))
        {
            errors.Add(Language.NotEmpty.Format(Description));
            return;
        }
        else if (!string.IsNullOrEmpty(valueString))
        {
            var length = GetByteLength(valueString);
            if (!string.IsNullOrEmpty(MinLength) && length < int.Parse(MinLength))
                errors.Add(Language.MinLength.Format(Description, MinLength));
            if (!string.IsNullOrEmpty(MaxLength) && length > int.Parse(MaxLength))
                errors.Add(Language.MaxLength.Format(Description, MaxLength));

            var typeName = type.FullName;
            if (typeName.Contains("System.Int32"))
            {
                if (!int.TryParse(value.ToString(), out int i))
                    errors.Add(Language.MustInteger.Format(Description));
                if (!string.IsNullOrEmpty(MinLength) && i < int.Parse(MinLength))
                    errors.Add(Language.MustMinLength.Format(Description, MinLength));
                if (!string.IsNullOrEmpty(MaxLength) && i > int.Parse(MaxLength))
                    errors.Add(Language.MustMaxLength.Format(Description, MaxLength));
            }

            if (typeName.Contains("System.Decimal"))
            {
                if (!decimal.TryParse(value.ToString(), out decimal d))
                    errors.Add(Language.MustNumber.Format(Description));
                if (!string.IsNullOrEmpty(MinLength) && d < decimal.Parse(MinLength))
                    errors.Add(Language.MustMinLength.Format(Description, MinLength));
                if (!string.IsNullOrEmpty(MaxLength) && d > decimal.Parse(MaxLength))
                    errors.Add(Language.MustMaxLength.Format(Description, MaxLength));
            }

            if (typeName.Contains("System.DateTime"))
            {
                if (string.IsNullOrEmpty(DateFormat))
                {
                    if (!DateTime.TryParse(value.ToString(), out _))
                        errors.Add(Language.MustDateTime.Format(Description));
                }
                else
                {
                    if (!DateTime.TryParseExact(valueString, DateFormat, null, DateTimeStyles.None, out _))
                        errors.Add(Language.MustDateFormat.Format(Description, DateFormat));
                }
            }
        }
    }

    private static int GetByteLength(string value)
    {
        if (string.IsNullOrEmpty(value))
            return 0;

        return Encoding.Default.GetBytes(value).Length;
    }
}