using System;
using System.Linq;
using ABI_RC.Core.InteractionSystem;

namespace BTKUILib.UIObjects.Objects;

public class CustomEngineOnFunction
{
    //Max 8 parameters of type T
    //Must use the correct TriggerEvent function
    public string FunctionName { get; private set; }
    public string JSCode { get; private set; }
    public Parameter[] Parameters { get; private set; }

    public CustomEngineOnFunction(string functionName, string jsCode, params Parameter[] parameters)
    {
        FunctionName = functionName;
        JSCode = jsCode;
        Parameters = parameters;
    }

    public void TriggerEvent(params object[] parameters)
    {
        if (!UIUtils.IsQMReady()) return;

        if (parameters.Length == 0 && Parameters.Any(x=>x.Required))
            throw new Exception($"CustomEngineOnEvent {FunctionName} TriggerEvent was attempted with 0 parameters yet there are required parameters!");

        for (int i = 0; i < Parameters.Length; i++)
        {
            var funcParam = Parameters[i];

            if (funcParam.Required && parameters.Length < i + 1)
                throw new Exception($"CustomEngineOnEvent {FunctionName} TriggerEvent was attempted with a missing required parameter!");

            var parameter = parameters[i];

            if((parameter == null && !funcParam.Nullable) || (parameter!=null && parameter.GetType() != funcParam.ParameterType))
                throw new Exception($"CustomEngineOnEvent {FunctionName} TriggerEvent was attempted with parameter that is either null or not the expected type!");
        }

        //Param check complete, pass to JS
        switch (parameters.Length)
        {
            case 0:
                CVR_MenuManager.Instance.quickMenu.View.TriggerEvent(FunctionName);
                break;
            case 1:
                CVR_MenuManager.Instance.quickMenu.View.TriggerEvent(FunctionName, parameters[0]);
                break;
            case 2:
                CVR_MenuManager.Instance.quickMenu.View.TriggerEvent(FunctionName, parameters[0], parameters[1]);
                break;
            case 3:
                CVR_MenuManager.Instance.quickMenu.View.TriggerEvent(FunctionName, parameters[0], parameters[1], parameters[2]);
                break;
            case 4:
                CVR_MenuManager.Instance.quickMenu.View.TriggerEvent(FunctionName, parameters[0], parameters[1], parameters[2], parameters[3]);
                break;
            case 5:
                CVR_MenuManager.Instance.quickMenu.View.TriggerEvent(FunctionName, parameters[0], parameters[1], parameters[2], parameters[3], parameters[4]);
                break;
            case 6:
                CVR_MenuManager.Instance.quickMenu.View.TriggerEvent(FunctionName, parameters[0], parameters[1], parameters[2], parameters[3], parameters[4], parameters[5]);
                break;
            case 7:
                CVR_MenuManager.Instance.quickMenu.View.TriggerEvent(FunctionName, parameters[0], parameters[1], parameters[2], parameters[3], parameters[4], parameters[5], parameters[6]);
                break;
            case 8:
                CVR_MenuManager.Instance.quickMenu.View.TriggerEvent(FunctionName, parameters[0], parameters[1], parameters[2], parameters[3], parameters[4], parameters[5], parameters[6], parameters[7]);
                break;
            default:
                throw new Exception($"CustomEngineOnEvent {FunctionName} TriggerEvent was attempted with too many parameters! Maximum parameters is 8!");
        }
    }
}

public struct Parameter
{
    public string ParameterName { get; private set; }
    public Type ParameterType { get; private set; }
    public bool Required { get; private set; }
    public bool Nullable { get; private set; }

    public Parameter(string parameterName, Type parameterType, bool required, bool nullable)
    {
        ParameterName = parameterName;
        ParameterType = parameterType;
        Required = required;
        Nullable = nullable;
    }
}