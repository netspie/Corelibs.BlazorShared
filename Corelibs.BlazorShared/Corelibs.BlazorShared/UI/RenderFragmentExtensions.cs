using Common.Basic.Collections;
using Corelibs.BlazorShared.UI.Css;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System.Reflection;

namespace Corelibs.BlazorShared.UI
{
    public static class RenderFragmentExtensions
    {
        public static RenderFragment CreateComponent<T>(Action<RenderTreeBuilder> modify, params object[] arguments)
             where T : notnull, IComponent
        {
            var type = typeof(T);
            var parameterProperties = type.GetProperties().Where(p => p.GetCustomAttribute(typeof(ParameterAttribute)) != null).ToArray();

            return builder =>
            {
                builder.OpenComponent<T>(0);

                for (int i = 0; i < arguments.Length; i++)
                    builder.AddAttribute(0, parameterProperties[i].Name, arguments[i]);

                modify?.Invoke(builder);

                builder.CloseComponent();
            };
        }

        public static RenderFragment CreateComponent<T>(params object[] arguments)
             where T : notnull, IComponent
        {
            return CreateComponent<T>(builder => {}, arguments);
        }

        public static RenderFragment CreateComponent<T>(ReferenceCapture<T> reference, params object[] arguments)
             where T : class, IComponent
        {
            return CreateComponent<T>(builder => 
            {
                builder.AddComponentReferenceCapture(0, obj =>
                {
                    reference.Value = obj as T;
                });
            }, arguments);
        }

        public static RenderFragment CreateComponent<T>(params RFArg[] arguments)
             where T : notnull, IComponent
        {
            var type = typeof(T);

            return builder =>
            {
                builder.OpenComponent<T>(0);

                for (int i = 0; i < arguments.Length; i++)
                    builder.AddAttribute(0, arguments[i].Name, arguments[i].Value);

                builder.CloseComponent();
            };
        }

        public static RenderFragment CreateComponent<T>(RFArg argument1, params object[] arguments)
             where T : notnull, IComponent
        {
            var type = typeof(T);
            var parameterProperties = type.GetProperties().Where(p => p.GetCustomAttribute(typeof(ParameterAttribute)) != null).ToArray();

            return builder =>
            {
                builder.OpenComponent<T>(0);

                for (int i = 0; i < arguments.Length; i++)
                    builder.AddAttribute(0, parameterProperties[i].Name, arguments[i]);

                builder.AddAttribute(0, argument1.Name, argument1.Value);

                builder.CloseComponent();
            };
        }

        public static RenderTreeBuilder OpenElement(this RenderTreeBuilder builder, ref int sequence, string name)
        {
            builder.OpenElement(sequence++, name);
            return builder;
        }

        public static RenderTreeBuilder OpenComponent<T>(this RenderTreeBuilder builder, ref int sequence) where T : notnull, IComponent
        {
            builder.OpenComponent<T>(sequence++);
            return builder;
        }

        public static RenderTreeBuilder AddAttribute(this RenderTreeBuilder builder, ref int sequence, string name, MulticastDelegate? value)
        {
            builder.AddAttribute(sequence++, name, value);
            return builder;
        }

        public static RenderTreeBuilder AddCssAttribute(this RenderTreeBuilder builder, ref int sequence, string name, double value)
        {
            if (value is not 0)
                builder.AddAttribute(sequence++, name, new CssAttribute(value));

            return builder;
        }

        public static RenderTreeBuilder AddCssAttribute(this RenderTreeBuilder builder, ref int sequence, string name, string value)
        {
            if (!value.IsNullOrEmpty())
                builder.AddAttribute(sequence++, name, new CssAttribute(value));

            return builder;
        }

        public static RenderTreeBuilder AddClassAttribute(this RenderTreeBuilder builder, ref int sequence, string value)
        {
            if (!value.IsNullOrEmpty())
                builder.AddAttribute(sequence++, "class", value);

            return builder;
        }
    }

    public class RFArg
    {
        public RFArg(string name, object value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; } = "";
        public object Value { get; } = "";

        public static implicit operator RFArg((string name, object value) arg) => new RFArg(arg.name, arg.value);
    }

    public class ReferenceCapture<T>
    {
        public T Value { get; set; }
    }
}
