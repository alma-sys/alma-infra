using System;
using Alma.Dominio;

namespace Alma.Dados.Hooks
{
    public interface ISavedDataHook : IDataHook
    {
        Type targetType { get; }
        void Handle(object entitySaved);
    }
}
