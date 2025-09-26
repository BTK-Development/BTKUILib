using System;
using System.Linq;
using System.Runtime.CompilerServices;
using ABI_RC.Core.InteractionSystem;
using Unity.Collections.LowLevel.Unsafe;

namespace BTKUILib.UIObjects.Objects;

/// <summary>
/// Custom engine on functions exist within Javascript, this can be used to run code that effects your custom elements
/// </summary>
public class CustomEngineOnFunction
{
    //Max 8 parameters of type T
    //Must use the correct TriggerEvent function
    internal readonly ABI_RC.Systems.UI.UILib.UIObjects.Objects.CustomEngineOnFunction InternalCEFunction;

    /// <summary>
    /// Function constructor, components of this cannot be modified after generation
    /// </summary>
    /// <param name="functionName">Function name, this must be unique</param>
    /// <param name="jsCode">Javascript code to be ran within Cohtml</param>
    /// <param name="parameters">Parameters that are sent with your function from C#, there is a max of 8 supported</param>
    public CustomEngineOnFunction(string functionName, string jsCode, params Parameter[] parameters)
    {
        InternalCEFunction = new ABI_RC.Systems.UI.UILib.UIObjects.Objects.CustomEngineOnFunction(functionName, jsCode, UnsafeUtility.As<Parameter[], ABI_RC.Systems.UI.UILib.UIObjects.Objects.Parameter[]>(ref parameters));
    }

    /// <summary>
    /// TriggerEvent calls your function from C# with the supplied parameters
    /// </summary>
    /// <param name="parameters">Parameters to be sent with your function</param>
    /// <exception cref="Exception">Exception thrown if you pass in to many parameters</exception>
    public void TriggerEvent(params object[] parameters)
    {
        InternalCEFunction.TriggerEvent(parameters);
    }
}

/// <summary>
/// Parameter struct, used to validate parameters being passed in
/// </summary>
public struct Parameter
{
    internal string ParameterName { get; private set; }
    internal Type ParameterType { get; private set; }
    internal bool Required { get; private set; }
    internal bool Nullable { get; private set; }

    /// <summary>
    /// Creates a parameter to be used with your custom function
    /// </summary>
    /// <param name="parameterName">Parameter name, make sure this matches your variable name used in JS</param>
    /// <param name="parameterType">Parameter type, this is used to validate the parameter against</param>
    /// <param name="required">Sets if this parameter is required</param>
    /// <param name="nullable">Sets if this parameter can be null</param>
    public Parameter(string parameterName, Type parameterType, bool required, bool nullable)
    {
        ParameterName = parameterName;
        ParameterType = parameterType;
        Required = required;
        Nullable = nullable;
    }
}