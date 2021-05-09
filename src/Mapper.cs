using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace WVN.SimpleMapper
{
    public class Mapper<TSource, TDest> where TDest : new()
    {
        protected Func<Type, PropertyInfo, PropertyInfo> PropertyMatcher;
        public Mapper(StringComparison stringComparison = StringComparison.CurrentCulture)
        {
            PropertyMatcher = new Func<Type, PropertyInfo, PropertyInfo>((sourceType, destinationProperty) =>
            {
                return Array.Find(sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance), p => p.Name.Equals(destinationProperty.Name, stringComparison) && p.PropertyType == destinationProperty.PropertyType);
            });
        }

        #region Copying Properties
        protected virtual void CopyMatchingProperties(TSource source, TDest dest)
        {
            foreach (var destProp in typeof(TDest).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.CanWrite))
            {
                var sourceProp = PropertyMatcher(typeof(TSource), destProp);

                if (sourceProp != null)
                {
                    destProp.SetValue(dest, sourceProp.GetValue(source, null), null);
                }
            }
        }
        #endregion

        #region Custom transformations
        protected readonly IList<Action<TSource, TDest>> Mappings = new List<Action<TSource, TDest>>();

        public virtual void AddMapping(Action<TSource, TDest> mapping)
            => Mappings.Add(mapping);
        #endregion

        #region Perform Mappings
        public virtual TDest MapObject(TSource source, TDest dest)
        {
            CopyMatchingProperties(source, dest);
            foreach (var action in Mappings)
            {
                action(source, dest);
            }
            return dest;
        }

        public virtual TDest CreateMappedObject(TSource source)
            => MapObject(source, new TDest());
        #endregion
    }
}
