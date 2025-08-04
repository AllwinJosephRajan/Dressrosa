using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dressrosa.Converter
{
    public abstract class BaseConverter<TSource, TDestination> : IConverter<TSource, TDestination> where TSource : class
                                                        where TDestination : class
    {
        public abstract TDestination Convert(TSource source);
        public abstract TSource Convert(TDestination source);

        public List<TDestination> Convert(List<TSource> source)
        {
            var dtos = new List<TDestination>();
            if (source != null)
            {
                foreach (var entity in source)
                {
                    dtos.Add(Convert(entity));
                }
            }
            return dtos;
        }
        public List<TSource> Convert(List<TDestination> source)
        {
            var entities = new List<TSource>();
            if (source != null)
            {
                foreach (var dto in source)
                {
                    entities.Add(Convert(dto));
                }
            }
            return entities;
        }
    }
}
