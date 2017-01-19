using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;

namespace Alma.Dados.OrmEntityFramework
{
    class Contexto : DbContext, IContexto
    {
        public Contexto(string nameOrConnectionString) : base(nameOrConnectionString)
        {
            base.Configuration.LazyLoadingEnabled = false;
            base.Configuration.ValidateOnSaveEnabled = true;
            base.Database.Log = new Action<string>(str => System.Diagnostics.Trace.Write(str, nameOrConnectionString));
            if (!Config.ExecutarMigracoes)
                Database.SetInitializer<Contexto>(null);
        }

        public void Delete(object instance)
        {
            if (instance != null)
                Set(instance.GetType()).Remove(instance);
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

        public void Update(object instance)
        {
            if (instance != null)
            {
                Entry(instance).State = EntityState.Modified;
                SaveChanges();
            }
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            foreach (var a in Config.AssembliesMapeadas.Values.SelectMany(x => x))
                modelBuilder.Configurations.AddFromAssembly(a);
        }


    }

    interface IContexto
    {
        void Flush();
        void Merge(object instance);
        void Delete(object instance);
        void Update(object instance);
        void Save(object instance);
        IQueryable<T> Query<T>() where T : class;
    }
}