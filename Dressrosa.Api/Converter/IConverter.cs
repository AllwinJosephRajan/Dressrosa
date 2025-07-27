using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dressrosa.Api.Converter
{
    public interface IConverter<TSource, TDestination> where TSource : class
                                                        where TDestination : class
    {
        TDestination Convert(TSource source);

        List<TDestination> Convert(List<TSource> source);

        TSource Convert(TDestination source);

        List<TSource> Convert(List<TDestination> source);
    }
}
