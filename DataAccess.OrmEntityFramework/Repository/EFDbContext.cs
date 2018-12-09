using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Alma.DataAccess.OrmEntityFramework
{
    class EFDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public EFDbContext(string nameOrConnectionString) : base()
        {

            //base.Configuration.LazyLoadingEnabled = false;
            //base.Configuration.ValidateOnSaveEnabled = true;
            //base.Database.Log = new Action<string>(str => System.Diagnostics.Trace.Write(str, nameOrConnectionString));
            //if (!Config.ExecutarMigracoes)
            //Database.SetInitializer<Contexto>(null);
        }

        public void Delete(object instance)
        {
            if (instance != null)
                base.Remove(instance);
            //Set(instance.GetType()).Remove(instance);
        }

        public void Flush()
        {
            SaveChanges();
        }

        public void Merge(object instance)
        {
            if (instance != null)
            {
                Entry(instance).State = EntityState.Modified;
                SaveChanges();
            }
        }

        public IQueryable<T> Query<T>() where T : class
        {
            return Set<T>().AsQueryable();
        }

        public void Save(object instance)
        {
            if (instance != null)
            {
                Entry(instance).State = EntityState.Added;
                SaveChanges();
            }
        }

        public new void Update(object instance)
        {
            if (instance != null)
            {
                Entry(instance).State = EntityState.Modified;
                SaveChanges();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            //foreach (var a in Config.AssembliesMapeadas.Values.SelectMany(x => x))
            //    modelBuilder.Configurations.AddFromAssembly(a);

            throw new NotImplementedException("This component is not yet implemented.");
        }


    }
}