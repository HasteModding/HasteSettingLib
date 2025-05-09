using System.Globalization;
using System.Reflection;

namespace SettingsLib;

/// <summary>
/// A class purely to have a name.
/// This will temporarily exist as long as ISettingsSaveLoad requires a Type
/// as its key, instead of a string.
/// </summary>
public class NamedType : Type
{
    private string _partialName;
    private string _name;

    public NamedType(string partialName, string name)
    {
        _partialName = partialName;
        _name = name;
    }

    public override string Namespace => throw new NotImplementedException();

    public override string AssemblyQualifiedName => throw new NotImplementedException();

    public override string FullName => $"{_partialName}.{_name}";

    public override Assembly Assembly => throw new NotImplementedException();

    public override Module Module => throw new NotImplementedException();

    public override Type UnderlyingSystemType => throw new NotImplementedException();

    public override Guid GUID => throw new NotImplementedException();

    public override Type BaseType => throw new NotImplementedException();

    public override string Name => _name;

    public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr) =>
        throw new NotImplementedException();

    public override object[] GetCustomAttributes(bool inherit) =>
        throw new NotImplementedException();

    public override object[] GetCustomAttributes(Type attributeType, bool inherit) =>
        throw new NotImplementedException();

    public override Type GetElementType() => throw new NotImplementedException();

    public override EventInfo GetEvent(string name, BindingFlags bindingAttr) =>
        throw new NotImplementedException();

    public override EventInfo[] GetEvents(BindingFlags bindingAttr) =>
        throw new NotImplementedException();

    public override FieldInfo GetField(string name, BindingFlags bindingAttr) =>
        throw new NotImplementedException();

    public override FieldInfo[] GetFields(BindingFlags bindingAttr) =>
        throw new NotImplementedException();

    public override Type GetInterface(string name, bool ignoreCase) =>
        throw new NotImplementedException();

    public override Type[] GetInterfaces() => throw new NotImplementedException();

    public override MemberInfo[] GetMembers(BindingFlags bindingAttr) =>
        throw new NotImplementedException();

    public override MethodInfo[] GetMethods(BindingFlags bindingAttr) =>
        throw new NotImplementedException();

    public override Type GetNestedType(string name, BindingFlags bindingAttr) =>
        throw new NotImplementedException();

    public override Type[] GetNestedTypes(BindingFlags bindingAttr) =>
        throw new NotImplementedException();

    public override PropertyInfo[] GetProperties(BindingFlags bindingAttr) =>
        throw new NotImplementedException();

    public override object InvokeMember(
        string name,
        BindingFlags invokeAttr,
        Binder binder,
        object target,
        object[] args,
        ParameterModifier[] modifiers,
        CultureInfo culture,
        string[] namedParameters
    ) => throw new NotImplementedException();

    public override bool IsDefined(Type attributeType, bool inherit) =>
        throw new NotImplementedException();

    protected override TypeAttributes GetAttributeFlagsImpl() =>
        throw new NotImplementedException();

    protected override ConstructorInfo GetConstructorImpl(
        BindingFlags bindingAttr,
        Binder binder,
        CallingConventions callConvention,
        Type[] types,
        ParameterModifier[] modifiers
    ) => throw new NotImplementedException();

    protected override MethodInfo GetMethodImpl(
        string name,
        BindingFlags bindingAttr,
        Binder binder,
        CallingConventions callConvention,
        Type[] types,
        ParameterModifier[] modifiers
    ) => throw new NotImplementedException();

    protected override PropertyInfo GetPropertyImpl(
        string name,
        BindingFlags bindingAttr,
        Binder binder,
        Type returnType,
        Type[] types,
        ParameterModifier[] modifiers
    ) => throw new NotImplementedException();

    protected override bool HasElementTypeImpl() => throw new NotImplementedException();

    protected override bool IsArrayImpl() => throw new NotImplementedException();

    protected override bool IsByRefImpl() => throw new NotImplementedException();

    protected override bool IsCOMObjectImpl() => throw new NotImplementedException();

    protected override bool IsPointerImpl() => throw new NotImplementedException();

    protected override bool IsPrimitiveImpl() => throw new NotImplementedException();
}
