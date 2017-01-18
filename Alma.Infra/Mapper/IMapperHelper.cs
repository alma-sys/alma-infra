using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alma.Infra.Mapper
{
    public interface IMapperHelper
    {
        void Config(IMapperConfigurationExpression cfg);
    }
}
