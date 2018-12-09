using Alma.DataAccess.MongoMapping;
using Alma.Exemplo.Dominio.Entidades;
using MongoDB.Bson.Serialization;

namespace Alma.Exemplo.Dados.Mongo.Mapeamentos
{
    public class UsuarioMap : ClassMapping<Usuario>
    {
        protected override void Map(BsonClassMap<Usuario> map)
        {
            map.SetCollection("Usuarios");

            map.MapMember(x => x.PersonUId)
                .SetIsRequired(true);

            map.MapMember(x => x.Name)
                .SetIsRequired(true);

            map.MapMember(x => x.Sobrenome)
                .SetIsRequired(false);


            map.MapAsComponent(x => x.Email, cm => cm
                .SetElementName("Email")
                .SetIsRequired(true)
                .MapMember(y => y.Endereco)
            );

            map.MapMember(x => x.Endereco)
                .SetIsRequired(false);
            map.MapMember(x => x.Telefone)
                .SetIsRequired(false);
            map.MapMember(x => x.UltimoAcesso)
                .SetIsRequired(false);
            map.MapMember(x => x.DomainUser)
                .SetIsRequired(false);

            map.MapAsRef(x => x.UsuarioPai, cm => cm
                 .SetIsRequired(false));

            map.MapListAsRefs(x => x.Perfis, "_perfis", cm => cm
                  .SetIsRequired(false)
            );



        }
    }
}
