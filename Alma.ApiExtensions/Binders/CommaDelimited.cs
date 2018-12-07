using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Alma.ApiExtensions.Binders
{
    public class CommaDelimitedAttribute : ModelBinderAttribute
    {
        public CommaDelimitedAttribute() : base(typeof(CommaDelimitedModelBinder)) { }
    }


    class CommaDelimitedModelBinder : IModelBinder
    {
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            await Task.Run(() =>
            {
                var key = bindingContext.ModelName;
                var val = bindingContext.ValueProvider.GetValue(key);
                if (val != null)
                {
                    var s = string.Join(",", val.Values.ToArray());
                    var elementType = bindingContext.ModelType.IsArray ? bindingContext.ModelType.GetElementType() : bindingContext.ModelType.IsGenericType ? bindingContext.ModelType.GetGenericArguments()[0] : null;
                    if (!string.IsNullOrWhiteSpace(s))
                    {
                        var converter = TypeDescriptor.GetConverter(elementType);
                        var values = Array.ConvertAll(s.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries),
                            x => { return converter.ConvertFromString(x != null ? x.Trim() : x); });

                        var typedValues = Array.CreateInstance(elementType, values.Length);

                        values.CopyTo(typedValues, 0);

                        bindingContext.Model = typedValues;
                    }
                    else
                    {
                        // change this line to null if you prefer nulls to empty arrays 
                        bindingContext.Model = Array.CreateInstance(elementType, 0);
                    }
                }
            });
        }
    }
}
