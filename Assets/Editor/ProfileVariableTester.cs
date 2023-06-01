using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;
using UnityEngine.AddressableAssets.Initialization;

public class ProfileVariables
{
    public int count;
    public List<string> names;
    public List<string> values;
    public List<string> editorValues;
    public List<string> runtimeValues;

    public ProfileVariables(List<string> _names)
    {
        count = _names.Count;
        names = _names;
        values = new(count);
        editorValues = new(count);
        runtimeValues = new(count);
    }

    public override string ToString()
    {
        var msg = string.Empty;

        for (var i = 0; i < count; ++i)
        {
            msg += $"{names[i]} = '{values[i]}' -> '{editorValues[i]}' -> '{runtimeValues[i]}'\n";
        }

        return msg;
    }
}

public class ProfileVariableTester : MonoBehaviour
{
    [MenuItem("LoadyProfiles/Test Profile Variable")]
    private static void TestProfileVariable()
    {
        var addressableAssetSettings = AddressableAssetSettingsDefaultObject.Settings;
        var profileSettings = addressableAssetSettings.profileSettings;
        var activeProfileId = addressableAssetSettings.activeProfileId;
        var variables = new ProfileVariables(profileSettings.GetVariableNames());

        for (var i = 0; i < variables.count; ++i)
        {
            var variableName = variables.names[i];
            var value = profileSettings.GetValueByName(activeProfileId, variableName);
            variables.values.Add(value);

            var editorValue = profileSettings.EvaluateString(activeProfileId, value);
            variables.editorValues.Add(editorValue);

            var runtimeValue = AddressablesRuntimeProperties.EvaluateString(editorValue);
            variables.runtimeValues.Add(runtimeValue);
        }

        Debug.Log(variables);
    }
}