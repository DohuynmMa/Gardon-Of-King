using Assets.Scripts.NetWork.Packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;

public interface UIHelper
{
    public string GetName();
    public static Dictionary<string, Action> GetAllActions<T>(T t) where T : UIHelper
    {
        Dictionary<string, Action> actions = new Dictionary<string, Action>();
        foreach (var method in t.GetType().GetMethods())
        {
            var attribute = method.GetCustomAttribute<ActionAttribute>();
            if (attribute == null) continue;
            var name = t.GetName() + "." + attribute.name;
            actions.Add(name, () => {
                method.Invoke(t, new object[0]);
            });
        }
        return actions;
    }
}
public interface HiddenUI { }

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public sealed class ActionAttribute : Attribute
{
    public string name { get; private set; }
    public ActionAttribute(string actionName)
    {
        name = actionName;
    }
}
